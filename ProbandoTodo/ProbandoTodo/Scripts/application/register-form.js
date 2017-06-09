$(document).ready(registerFormActions);

function registerFormActions() {    
	$('#RegisterForm').on('blur', '#UserName', function () {
		var textbox = $(this);

		if (textbox.val().length > 0) {
			$.ajax({
				url: "/User/CheckUserName",
				contentType: "text/html; charset=utf-8",
				data: "name=" + textbox.val(),
				success: function (data) {
					$('.user-legend').remove();
					switch (data) {
						case "True":
							textbox.parent()
                                .append('<label class="label-form-required user-legend" style="color: red !important;"><span class="glyphicon glyphicon-remove"></span> El nombre ya fue utilizado</label>');
							break;
						default:
							textbox.parent()
                                .append('<label class="label-form-required user-legend" style="color: green !important;"><span class="glyphicon glyphicon-ok"></span> El nombre está disponible</label>');
							break;
					}
				},
				error: function () {
					alert("Ocurrió un error, intentelo de nuevo.");
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