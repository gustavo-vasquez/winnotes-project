function loginComplete(result) {
    $('body > .modal-backdrop').remove();

    switch (typeof result) {
        case "string":
            $('#LoginDialog').modal("show");
            //$('#LoginForm > .modal-body').prepend('<p class="text-danger"><span class="glyphicon glyphicon-exclamation-sign"></span> Datos no válidos.</p>');
            break;
        case "object":
            window.location.href = result.url;
            break;
        default: alert("Ha ocurrido un error inesperado. Inténtelo de nuevo.");
    }
}

function loginFailed(response) {
    if (response.responseJSON != undefined)
        $('body').append('<div class="message-result"><p>' + response.responseJSON.message + '</p></div>');
    else
        $('body').append('<div class="message-result"><p>Se ha producido un error inesperado. Inténtelo nuevamente.</p></div>');            

    //$('.message-result').fadeIn();
    setTimeout(function () {
        $('.message-result').fadeOut('slow', function () { $(this).remove(); });
    }, 6000);
}