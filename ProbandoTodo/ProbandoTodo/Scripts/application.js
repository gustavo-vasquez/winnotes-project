$(document).ready(function () {
    //$("#txtSearch").autocomplete({
    //    source: "/Home/Suggestion"
    //});

    $(window).scroll(scrollFunction);    

    $('#registerLink').on('click', function (event) {
        event.preventDefault();

        $.ajax({
            url: "/User/Register",
            success: function (data) {
                $('#MainDialog').html(data);                
                $('#RegisterDialog').modal("show");
                //$("form").removeData("validator");
                //$("form").removeData("unobtrusiveValidation");
                //$.validator.unobtrusive.parse("form");
            }
        });
    });

    $('#loginLink').on('click', function (event) {
        event.preventDefault();

        $.ajax({
            url: "/User/Login",            
            success: function (data) {
                $('#MainDialog').html(data);
                $('#LoginDialog').modal("show");                
            }
        });
    });        

    $('.navigation').on({
        mouseenter: function () {
            $(this).addClass('open');
        },
        mouseleave: function () {
            $(this).removeClass('open');
        },
        //click: function (event) {
        //    event.stopImmediatePropagation();
        //}
    });

    checkIfEventsAreExpired();
    setInterval(checkIfEventsAreExpired, (60 - new Date().getSeconds) * 1000);
});

function scrollFunction() {    
    if ($(window).scrollTop() > 20)
        $('#ToTop').fadeIn('fast');
    else
        $('#ToTop').fadeOut('fast');    
}

function topFunction() {    
    $('body,html').animate({
        scrollTop: 0
    }, 800);
    return false;
}

function checkIfEventsAreExpired() {
    var userCk = readCookie("UHICK");

    if(userCk != null && userCk != undefined) {
        var obj = { "uhick": userCk };
        $.ajax({
            url: "/Note/CheckExpiredEventsPartial",
            method: "POST",
            dataType: "html",
            data: JSON.stringify(obj),
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                $('.body-content').append(data);
                $('.fired-alarm').show('puff');
                $('.stop').on('click', stopAlarm);
            },
            error: function (response, error, errorThrown) {
                console.log(response);
                console.log(error);
                console.log(errorThrown);
            }
        });
    }
}

function stopAlarm() {
    $(this).closest('div[name=Alarm]').hide("puff", function () {
        $(this).remove();
    });    
}

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

// ****************** FUNCIÓN PARA OBTENER EL VALOR DE UNA COOKIE ********************************

function readCookie(name) {
    var nameEQ = name + "=";
    //Decode the cookie string, to handle cookies with special characters, e.g. '$' --> var decodedCookie = decodeURIComponent(document.cookie);
    var cookiesArray = document.cookie.split(';');

    for (var i = 0; i < cookiesArray.length; i++) {
        var cookie = cookiesArray[i];

        while (cookie.charAt(0) == ' ') {
            cookie = cookie.substring(1, cookie.length);
        }
        
        if (cookie.indexOf(nameEQ) == 0) {
            return cookie.substring(nameEQ.length, cookie.length);
        }
    }
    return null;
}

// *******************************************************************************

function addScripts(src) {
    $('body').append('<script src=' + src + '></script>');
}