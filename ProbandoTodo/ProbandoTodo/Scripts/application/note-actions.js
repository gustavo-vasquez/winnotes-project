$(document).ready(note_actions);

function note_actions() {
    $.datepicker.regional['es'] = {
        closeText: 'Cerrar',
        prevText: 'Anterior',
        nextText: 'Siguiente',
        currentText: 'Hoy',
        monthNames: ['Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio', 'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'],
        monthNamesShort: ['Ene', 'Feb', 'Mar', 'Abr', 'May', 'Jun', 'Jul', 'Ago', 'Sep', 'Oct', 'Nov', 'Dic'],
        dayNames: ['Domingo', 'Lunes', 'Martes', 'Miércoles', 'Jueves', 'Viernes', 'Sábado'],
        dayNamesShort: ['Dom', 'Lun', 'Mar', 'Mié', 'Juv', 'Vie', 'Sáb'],
        dayNamesMin: ['Do', 'Lu', 'Ma', 'Mi', 'Ju', 'Vi', 'Sá'],
        weekHeader: 'Sm',
        dateFormat: 'dd/mm/yy',
        firstDay: 1,
        isRTL: false,
        showMonthAfterYear: false,
        yearSuffix: ''
    };

    $.datepicker.setDefaults($.datepicker.regional['es']);

    // ************************ ACCIONES PARA NOTAS *******************************

    $('#UserInfo').on('mouseenter mouseleave', 'a', function () {
        $(this).parent('p').toggleClass('hover-info');
    });

    // Calendario en la pantalla de crear nota
    $('#DatePicker').datepicker({        
        showOtherMonths: true,
        selectOtherMonths: true,
        changeMonth: true,
        changeYear: true,
        minDate: 0,
        altField: "#ExpirationDate",
        altFormat: "dd/mm/yy"
    });

    // Destacar una tarea al momento de crearla
    $('#Starred').on('change', function () {
        if ($(this).is(':checked')) {
            $(this).prev().removeClass('glyphicon-star-empty').addClass('glyphicon-star');
        }
        else {
            $(this).prev().removeClass('glyphicon-star').addClass('glyphicon-star-empty');
        }
    });    

    // ********************** ACCIONES PARA LISTADO DE NOTAS *******************************

    $('input[type="hidden"][name="Note.ExpirationDate"]').datepicker({
        showAnim: "drop",
        showOtherMonths: true,
        selectOtherMonths: true,
        changeMonth: true,
        changeYear: true,
        minDate: 0,
        onSelect: function (dateText, inst) {
            $this = $(this);
            $formNotes = $this.parent().prev().prev();

            if ($formNotes.hasClass('in-folder')) {
                myData = "currentDate=" + dateText + "&previousDate=" + $this.prev().text() + "&idNote=" + $formNotes.data('ion') + "&idFolder=" + $('#ThisFolder').data('iof') + "&inFolder=true"
            }
            else {
                myData = "currentDate=" + dateText + "&previousDate=" + $this.prev().text() + "&idNote=" + $formNotes.data('ion') + "&idFolder=" + $formNotes.data('iof') + "&inFolder=false"
            }

            $.ajax({
                url: "/Note/ChangeDateTimeEventPartial",
                method: "GET",
                data: myData,
                success: function (data) {
                    $('body').append(data);                    
                    $('.alarm-cancel').on('click', function () {
                        $this.val(inst.lastVal);
                        $('.alarm-overlay').remove();
                    });
                },
                error: function () {
                    alert("Ocurrió un error. Intentelo de nuevo.");
                    $this.val(inst.lastVal);
                }
            });
        }
    });

    $('.calendar-trigger').on('click', function () {        
        $(this).prev().datepicker('show');
    });

    // Forzar completado de una tarea
    $('.force-complete-task').on({
        mouseover: function () {
            $(this).attr('src', '/Content/Images/tick-mark-128.png');
        },
        mouseleave: function () {
            $(this).attr('src', '/Content/Images/alarm-clock-128.png');
        },
        click: function () {
            if (confirm("Ha elegido completar la tarea ahora, ¿Está seguro?\n\nNota: esto no puede deshacerse")) {
                doNoteRequest("complete-task", $(this).closest('.form-notes'));
            }
        }
    });

    // Destacar o quitar la estrella en una tarea
    $('.star-icon').on('click', function () {
        doNoteRequest("star", $(this).closest('.form-notes'));
    });

    $.each($('input[name="Note.Starred"]'), function (index, value) {
        if ($(value).is(':checked')) {
            $(this).next().removeClass('glyphicon-star-empty').addClass('glyphicon-star');
        }
    });
    
    $('.panel').on('mouseenter mouseleave', function () {
        $(this).toggleClass('task-background');
    });

    function doNoteRequest(action, $formNotes) {
        var urlPath;
        var myData;
        switch (action) {
            case "complete-task":
                urlPath = "/Note/ForceCompleteTask";
                break;
            case "star":
                urlPath = "/Note/StarTask";
                break;
            default: return false;
        }

        if ($formNotes.hasClass('in-folder')) {
            myData = "folderID=" + $('#ThisFolder').data('iof') + "&noteID=" + $formNotes.data('ion') + "&inFolder=true";
        }
        else {
            myData = "folderID=" + $formNotes.data('iof') + "&noteID=" + $formNotes.data('ion') + "&inFolder=false";
        }

        $.ajax({
            url: urlPath,
            method: "POST",
            data: myData,
            success: function (data) {
                $('#NotesInside').html(data);
                note_actions();
            },
            error: errorHandler            
        });
    }

    // Botón para cambiar carpeta en la vista de notas por carpeta
    $('.change-folder-button').on('click', function () {
        var $this = $(this);
        $.ajax({
            url: "/Folder/ChangeFolderPartial",
            method: "GET",
            data: "folderID=" + $('#ThisFolder').data('iof') + "&folderName=" + $('#EditFolder').data('folder-name') + "&noteID=" + $this.data('ion'),
            success: function (data) {
                showChangeFolderSection(data, $this);
            },
            error: errorHandler
        });
    });

    // Botón para cambiar carpeta en la vista de todas las notas
    $('.change-folder-btn').on('click', function () {
        var $this = $(this);
        $.ajax({
            url: "/Folder/ChangeFolderPartial",
            method: "GET",
            data: "folderID=" + $this.data('iof') + "&folderName=" + $this.data('nof') + "&noteID=" + $this.data('ion'),
            success: function (data) {
                showChangeFolderSection(data, $this);
            },
            error: errorHandler
        });
    });

    function showChangeFolderSection(data, $this) {
        $('#MainDialog').html(data);
        $("#ChangeFolderDialog").css({
            'position': 'absolute',
            'left': $this.offset().left + -230,
            'top': $this.offset().top,
            'display': 'none'
        }).show("slide", { direction: "right" }, 500);

        $('#ChangeFolderForm').on('click', '.close', function () {
            $("#ChangeFolderDialog").slideUp();
        });

        $('#btnChangeFolder').on('click', function () {
            $folder = $('#FolderSelected');            

            $.ajax({
                url: "/Folder/ChangeFolder",
                method: "POST",
                data: "FolderID=" + $folder.data('iof') + "&NoteID=" + $folder.data('ion') + "&FolderSelected=" + $folder.val(),
                success: function (data) {
                    $("#ChangeFolderDialog").slideUp();
                    $('#NotesInside').html(data);
                    note_actions();                    
                },
                error: errorHandler
            });
        });        
    }    
}

function changeDatetimeEvent(data) {
    $('.alarm-overlay').remove();
    $('#NotesInside').html(data);
    note_actions();
}

function ajaxError(response) {
    $('.alarm-overlay').remove();
    $('footer').append('<div class="message-result"><div class="alert alert-danger">' + response.statusText + ' (' + response.status + ')</div></div>');
    setTimeout(function () {
        $('.message-result').remove();
    }, 6000);
}