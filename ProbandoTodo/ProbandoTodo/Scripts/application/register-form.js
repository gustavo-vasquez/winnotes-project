$(document).ready(registerFormActions);

function registerFormActions() {
    console.log("register-form cargado");
	$('#RegisterForm').on('blur', '#UserName', function () {
		var $textbox = $(this);

		if ($textbox.val().length > 0) {
			$.ajax({
				url: "/User/CheckUserName",
				contentType: "text/html; charset=utf-8",
				data: "name=" + $textbox.val(),
				success: function (data) {
				    $('.user-legend').remove();
				    if (data == "True") {
				        $textbox.parent()
                                .append('<label class="label-form-required user-legend" style="color: red !important;"><span class="glyphicon glyphicon-remove"></span> El nombre ya fue utilizado</label>');
				    }
				    else {
				        $textbox.parent()
                                .append('<label class="label-form-required user-legend" style="color: green !important;"><span class="glyphicon glyphicon-ok"></span> El nombre está disponible</label>');
				    }					
				}
			});
		}
		$('.user-legend').remove();
	});	
}

function registerComplete(result) {    
    $('body > .modal-backdrop').remove();

    switch (typeof result) {
        case "string":
            $('#RegisterDialog').modal("show");
            $('#UserName').parent().append('<label class="label-form-required user-legend" style="color: red !important;"><span class="glyphicon glyphicon-remove"></span> El nombre ya fue utilizado</label>');
            registerFormActions();
            break;
        case "object":
            window.location.href = result.url;
            break;
        default: alert("Ha ocurrido un error inesperado. Inténtelo de nuevo.");
    }
}

function registerFailed(response) {    
    if (response.responseJSON != undefined)
        $('footer').append('<div class="message-result"><div class="alert alert-danger">' + response.responseJSON.message + '</div></div>');
    else
        $('footer').append('<div class="message-result"><div class="alert alert-danger">Se ha producido un error inesperado. Inténtelo nuevamente.</div></div>');
    setTimeout(function () {
        $('.message-result').remove();
    }, 6000);
}