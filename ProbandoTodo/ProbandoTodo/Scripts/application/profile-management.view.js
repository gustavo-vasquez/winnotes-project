$('#PersonalPhrase').on('focus', function () {
    $('.available-chars').text(140 - $(this).val().length);
    $(this).removeClass('phrase-color-effect');
    $(this).next().slideDown(1000);
});

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

$.each($('input[name=pColor]'), function (index, value) {
    switch ($(this).is(':checked')) {
        case true: $(this).prev().html("&#10004;");
            $('#PersonalPhrase').css('color', $(this).prev().css("background-color"));
            break;
        case false: $(this).prev().html("");
            break;
    }
});

$('input[name=pColor]').on('change', function () {
    $.each($('input[name=pColor]'), function (index, value) {
        switch ($(this).is(':checked')) {
            case true: $(this).prev().html("&#10004;");
                $('#PersonalPhrase').css('color', $(this).prev().css("background-color"));
                break;
            case false: $(this).prev().html("");
                break;
        }
    });
});

// ****************** FUNCIONES PARA LA PAGINA DE PERFIL *************************

function selectColorOfPhrase() {
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