var panels = ["THEME", "AVATAR", "PERSONAL_MESSAGE", "PREVIEW"]; // Array con todos los paneles disponibles
var panelActive = { NAME: "THEME", POS: 0 }; // Indica el panel que se esta mostrando
var preview = { "theme": { "name": "Default", "image": "/Content/Images/profile_wizard/theme_default.png" }, "avatarImg": null, "personalMessage": { "phrase": null, "color": null } };

$(document).ready(function () {
    prevNextBtnState();

    // Marco con css la opcion de tema elegida (y se lo quito al que no esta marcado)
    $('#ThemeDialog').on('click', '.theme', function () {
        $('.theme').removeClass("theme-select");
        $('input[name="theme_select"]', this).prop("checked", true);
        $(this).addClass("theme-select");
        preview.theme.name = $('input[name="theme_select"]', this).data("theme");
        preview.theme.image = $(this).children('img').attr('src');
    });

    // Aumento la posicion del array de paneles y envio la orden de mostrar el que toca
    $('#NextBtn').on('click', function () {
        panelActive.POS = panelActive.POS + 1;
        panelActive.NAME = panels[panelActive.POS];
        changingPage(panelActive.NAME);
        prevNextBtnState();        
    });

    // Disminuyo la posicion del array de paneles y envio la orden de mostrar el que toca
    $('#PrevBtn').on('click', function () {
        panelActive.POS = panelActive.POS - 1;
        panelActive.NAME = panels[panelActive.POS];
        changingPage(panelActive.NAME);
    });

    if (window.localStorage.getItem("user-theme") !== null) {
        $('*[data-theme="' + window.localStorage.getItem("user-theme") + '"]').prop("checked", true);
    }
});

// Verifica en que panel estoy para asi deshabilitar el boton de 'atras' o 'siguiente' segun corresponda
function prevNextBtnState() {
    if (panelActive.POS === 0)
        $('#PrevBtn').addClass("disabled");
    else
        $('#PrevBtn').removeClass("disabled");

    if (panelActive.POS >= panels.length - 1) {
        $('#NextBtn').addClass("disabled");
        $('#EndBtn').show();
    }        
    else {
        $('#NextBtn').removeClass("disabled");
        $('#EndBtn').hide();
    }        

    //panelActive.NAME === "PREVIEW" && panelActive.POS === 3 ? $('#EndBtn').show() : $('#EndBtn').hide();
}

// Segun el nombre de panel que elija, lo envio a la funcion que se encarga de mostrarlo
function changingPage() {
    switch (panelActive.NAME) {
        case "AVATAR": loadPanel("AvatarDialog");                                    
            break;
        case "PERSONAL_MESSAGE": loadPanel("PersonalMessageDialog");
            break;
        //case "PASSWORD": loadPanel("PasswordDialog");
        //    break;
        case "PREVIEW": loadPanel("PreviewDialog");
            generatePreview();
            break;
        default: loadPanel("ThemeDialog");
            break;
    }
}

// Si el panel elegido ya existe solamente lo muestra (ocultando cualquier otro que se este mostrando)
// Si es la primera vez que lo cargo envio la peticion y lo cargo en el PanelContainer
function loadPanel(selectedDialog) {    
    if ($('#' + selectedDialog).length != 0) {
        $('#PanelContainer').children('div[id*="Dialog"]').addClass('hide');
        $('#' + selectedDialog).removeClass('hide');        
    }
    else {
        $.ajax({
            url: "/User/" + selectedDialog,
            method: "GET",
            success: function (data) {
                $('#PanelContainer').children('div[id*="Dialog"]').addClass('hide');
                $('#PanelContainer').append(data);                
            },
            complete: function() {
                if (selectedDialog == "PersonalMessageDialog") {
                    availableColors($('#AvailableColors').data('color'));
                    $('#PersonalPhrase').css("height", document.getElementById("PersonalPhrase").scrollHeight);
                    $('.available-chars').text(140 - $('#PersonalPhrase').val().length);
                    var initText = $('#PersonalPhrase').val();

                    $('#PersonalPhrase').on('keyup', function () {
                        $('#TestPhrase').text($(this).val());
                    });
                    $('#UndoPhrase').on('click', function () {
                        $('#PersonalPhrase').val(initText);
                        $('.available-chars').text(140 - $('#PersonalPhrase').val().length);
                        if ($('.available-chars').text() < 0) {
                            $('.available-chars').addClass('text-danger');
                        }
                        else {
                            $('.available-chars').removeClass('text-danger');
                        }
                        $('#TestPhrase').text($('#PersonalPhrase').val());
                    });
                }

                if (selectedDialog == "AvatarDialog") {
                    document.getElementById("UploadAvatar").onchange = function () {
                        var data = new FormData();
                        var files = $("#UploadAvatar").get(0).files;
                        if (files.length > 0) {
                            data.append("TempFile", files[0]);
                        }
                        $.ajax({
                            url: "/User/StoreTempAvatar",
                            type: "POST",
                            processData: false,
                            contentType: false,
                            data: data,
                            success: function (response) {
                                //code after success
                                $('#CurrAvatar').attr('src', response);
                                preview.avatarImg = response;
                            },
                            error: function (er) {
                                alert(er);
                            }
                        });
                    };

                    $('#ResetAvatar').on('click', function () {
                        $('#CurrAvatar').attr('src', $('.default-avatar').attr('src'));
                        preview.avatarImg = $('#CurrAvatar').attr('src');
                    });
                }                
            }
        });
    }
    
    prevNextBtnState();    
}

function availableColors(color) {
    //var availableColors = ["black", "blue", "blueviolet", "brown", "coral", "gold", "crimson", "darkgoldenrod", "darkturquoise", "deeppink", "deepskyblue", "fuchsia", "hotpink", "lightskyblue", "limegreen", "seagreen"];

    //$.each(availableColors, function (index, value) {
    //    var newCircleColor = $('<label></label>');
    //    $('#AvailableColors').append(newCircleColor.addClass('color-box1').css('background-color', value).attr('title', value));

    //    if (color != null && color === value)
    //        $('#AvailableColors :last').append('<span class="glyphicon glyphicon-ok"></span>');
    //});

    $.each($('label[class=color-box-wizard]'), function () {
        $(this).css('background-color', $(this).attr('id'));
    });

    $('#TestPhrase').css('color', color);

    preview.personalMessage.phrase = $('#PersonalPhrase').val();
    preview.personalMessage.color = color;

    $('#PersonalPhrase').on('change', function () {
        preview.personalMessage.phrase = $(this).val();
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
    });

    $('.color-box-wizard').on('click', function () {
        $('.color-box-wizard').text('');
        $(this).append('&#10004;');
        $('#TestPhrase').css('color', $(this).css('background-color'));
        preview.personalMessage.color = $(this).attr('id');
    });
}

$('#EndBtn').on('click', function () {
    var answer = confirm("¿Está seguro?");

    if (answer) {
        $.ajax({
            url: "/User/WizardComplete",
            type: "POST",
            data: JSON.stringify(preview),
            contentType: "application/json; charset=utf-8",
            //dataType: 'json',
            success: function (response) {
                $('.wizardWrapper').children().remove();
                $('.wizardWrapper').append('<div class="panel-body"><h2 class="text-center">' + response + '</h2></div>');
                window.scrollTo(0, 0);
                window.localStorage.setItem("user-theme", preview.theme.name);
            },
            error: function (err) {
                alert(err);
            }
        });
    }
    //event.preventDefault();
    //var wizard_formData = new FormData();
    //wizard_formData.append("theme", JSON.stringify(preview.theme));
    //wizard_formData.append("avatarImg", preview.avatarImg);
    //wizard_formData.append("personalMessage", JSON.stringify(preview.personalMessage));
    //for(var key in preview) {
    //    wizard_formData.append(key,preview[key]);
    //}    

    //var request = new XMLHttpRequest();
    //request.open("POST", "/User/WizardComplete");
    //request.send(wizard_formData);    
});

function generatePreview() {
    $('#PreviewTheme').children('h2').text(preview.theme.name);
    $('#PreviewTheme').children('img').attr('src', preview.theme.image);
    preview.avatarImg != null ? $('#PreviewAvatar').attr('src', preview.avatarImg) : $('#PreviewAvatar').attr('src', $('#CurrAvatar').attr('src'));
    $('#PreviewPhrase').text(preview.personalMessage.phrase);
    $('#PreviewPhrase').css('color', preview.personalMessage.color);
}