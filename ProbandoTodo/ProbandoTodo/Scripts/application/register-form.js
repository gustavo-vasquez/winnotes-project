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

	$('input[type=radio][name=MailProvider]').on('change', function () {
	    switch ($(this).val()) {
	        case "gmail": $('.provider').text('@gmail.com');
	            break;
	        case "outlook": $('.provider').text('@outlook.com');
	            break;
	        case "yahoo": $('.provider').text('@yahoo.com');
	            break;
	    }
	})
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
        $('body').append('<div class="message-result"><p>' + response.responseJSON.message + '</p></div>');
    else
        $('body').append('<div class="message-result"><p>Se ha producido un error inesperado. Inténtelo nuevamente.</p></div>');
    setTimeout(function () {
        $('.message-result').remove();
    }, 6000);
}