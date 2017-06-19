$(document).ready(viewActions)

function viewActions() {
    //$("form").removeData("validator");
    //$("form").removeData("unobtrusiveValidation");
    //$.validator.unobtrusive.parse("form");

    // Se abre la barra con los botones al hacer click en el textbox
    $('#PersonalPhrase').on('focus', function () {
        $('.available-chars').text(140 - $(this).val().length);
        $(this).removeClass('phrase-color-effect');
        $(this).next().slideDown(1000);
    });

    // Avisa cuando se excede de los 140 caracteres y tocando ESC se esconden los botones
    $('#PersonalPhrase').on('keyup', function (e) {
        $('.available-chars').text(140 - $(this).val().length);
        $(this).css("height", document.getElementById("PersonalPhrase").scrollHeight);

        if ($('.available-chars').text() < 0) {
            $('.available-chars').addClass('text-danger');
        }
        else {
            $('.available-chars').removeClass('text-danger');
        }

        if (e.keyCode === 27) { // escape key maps to keycode '27'
            // <DO YOUR WORK HERE>            
            $(this).next().slideUp(1000);
            $(this).addClass('phrase-color-effect');
            $(this).blur();
        }
    });

    // Segun el valor del campo oculto que guarda el color dispara el evento click al circulo de color que corresponde
    switch ($('#PhraseColor').val()) {
        case "black": $('#pBlack').prev().trigger('click');
            break;
        case "red": $('#pRed').prev().trigger('click');
            break;
        case "blue": $('#pBlue').prev().trigger('click');
            break;
        case "green": $('#pGreen').prev().trigger('click');
            break;
        default: $('#pBlack').prev().trigger('click');
    }

    paintColorCircle();

    $('input[name=pColor]').on('change', function () {
        paintColorCircle();
    });
}

// ****************** FUNCIONES PARA LA PAGINA DE PERFIL *************************


// Le pone el tilde al circulo de color que corresponde
function paintColorCircle() {
    $.each($('input[name=pColor]'), function (index, value) {
        switch ($(this).is(':checked')) {
            case true: $(this).prev().html("&#10004;");
                $('#PersonalPhrase').css('color', $(this).prev().css("background-color"));
                break;
            case false: $(this).prev().html("");
                break;
        }
    });
}