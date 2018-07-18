$(document).ready(function () {
    //$("#txtSearch").autocomplete({
    //    source: "/Home/Suggestion"
    //});
    loadThemeSaved();    

    $('#themes').on('click', 'button', function () {
        setTheme($(this));
    });

    $(window).scroll(scrollFunction);    

    $('#registerLink').on('click', function (event) {
        event.preventDefault();

        $.ajax({
            url: "/User/Register",
            success: function (data) {
                $('#MainDialog').html(data);                
                $('#RegisterDialog').modal("show");
                registerFormActions();
                $("form").removeData("validator");
                $("form").removeData("unobtrusiveValidation");
                $.validator.unobtrusive.parse("form");
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
                $("form").removeData("validator");
                $("form").removeData("unobtrusiveValidation");
                $.validator.unobtrusive.parse("form");
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

    window.setTimeout(function () {
        checkIfEventsAreExpired();
        window.setInterval(checkIfEventsAreExpired, 60000);
    }, 60000 - (new Date().getSeconds())*1000);
});

function scrollFunction() {    
    if ($(window).scrollTop() > 20)
        $('#ToTop').fadeIn('fast');
    else
        $('#ToTop').fadeOut('fast');    
}

function goTop() {
    $('body,html').animate({
        scrollTop: 0
    }, 800);
    return false;
}

function checkIfEventsAreExpired() {
    if (window.sessionStorage.getItem("uid") !== null) {
        var model = { "EncryptedCookie": readCookie("UHICK") };

        $.ajax({
            url: "/Note/CheckExpiredEventsPartial",
            method: "POST",
            data: JSON.stringify(model),
            contentType: "application/json; charset=utf-8",
            //dataType: "html",
            success: function (data) {
                if (data.list) {
                    $('body').append(data.render);
                    $('.fired-alarm').show('puff');
                    $('.stop').on('click', stopAlarm);
                }                
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

$('.message-result').on('click', 'button', function () {
    $(this).parent().fadeOut(function () { $(this).remove() });
});

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

function errorHandler(xhr) {   
    $('body').append(xhr.responseText);
    $('.message-result').on('click', 'button', function () {
        $(this).parent().fadeOut(function () { $(this).remove() });
    });
    setTimeout(function () {
        $('.message-result').fadeOut(function () { $(this).remove() });
    }, 6000);
}

function setTheme($anchor) {
    var themeBefore = localStorage.getItem('user-theme');
    var themeNow = $anchor.data('color');    
    
    if (!themeBefore)
        $('link[href*="bootstrap.flatly.css"]').remove();
    else
        $('link[href*="bootstrap.' + themeBefore + '.css"]').remove();

    $('link[href*="site.css"]').remove();
    localStorage.setItem('user-theme', themeNow);

    var lnk = document.createElement('link');
    lnk.type = 'text/css';

    switch (themeNow) {
        case "cerulean":
            lnk.href = '/Content/bootstrap.cerulean.css';
            break;
        case "slate":
            lnk.href = '/Content/bootstrap.slate.css';
            break;
        case "flatly":
            lnk.href = '/Content/bootstrap.flatly.css';
            break;
        case "superhero":
            lnk.href = '/Content/bootstrap.superhero.css';
            break;
        case "united":
            lnk.href = '/Content/bootstrap.united.css';
            break;
        case "yeti":
            lnk.href = '/Content/bootstrap.yeti.css';
            break;        
    }

    lnk.rel = 'stylesheet';
    document.getElementsByTagName('head')[0].appendChild(lnk);

    var lnk2 = document.createElement('link');
    lnk2.type = 'text/css';
    lnk2.href = '/Content/site.css';
    lnk2.rel = 'stylesheet';
    document.getElementsByTagName('head')[0].appendChild(lnk2);    
}

function loadThemeSaved() {    
    var lnk = document.createElement('link');
    lnk.type = 'text/css';

    switch (localStorage.getItem('user-theme')) {        
        case "cerulean":
            lnk.href = '/Content/bootstrap.cerulean.css';
            break;
        case "slate":
            lnk.href = '/Content/bootstrap.slate.css';
            break;        
        case "superhero":
            lnk.href = '/Content/bootstrap.superhero.css';
            break;
        case "united":
            lnk.href = '/Content/bootstrap.united.css';
            break;
        case "yeti":
            lnk.href = '/Content/bootstrap.yeti.css';
            break;
        default: return false;
    } 

    lnk.rel = 'stylesheet';
    document.getElementsByTagName('head')[0].appendChild(lnk);

    $('link[href*="site.css"]').remove();
    var lnk2 = document.createElement('link');
    lnk2.type = 'text/css';
    lnk2.href = '/Content/site.css';
    lnk2.rel = 'stylesheet';
    document.getElementsByTagName('head')[0].appendChild(lnk2);    
}