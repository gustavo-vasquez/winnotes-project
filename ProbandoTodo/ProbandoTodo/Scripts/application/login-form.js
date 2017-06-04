function loginComplete(result) {
    $('body > .modal-backdrop').remove();

    switch (typeof result) {
        case "string":
            $('#LoginDialog').modal("show");
            $('#LoginForm > .modal-body').prepend('<p class="text-danger"><span class="glyphicon glyphicon-exclamation-sign"></span> Datos no válidos.</p>');
            break;
        case "object":
            window.location.href = result.url;
            break;
        default: alert("Ha ocurrido un error inesperado. Inténtelo de nuevo.");
    }
}