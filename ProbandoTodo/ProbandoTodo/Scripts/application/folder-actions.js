$('#CreateFolder').on('click', function (event) {
    $.ajax({
        url: "/Folder/FolderPartial",
        success: function (data) {
            folderActions(data);
        }
    });
});



// ************************ FUNCIONES PARA CARPETA *****************************

function folderActions(data) {
    $('#MainDialog').html(data);
    $("#FolderDialog").css({
        'position': 'absolute',
        'left': $('#CreateFolder').offset().left,
        'top': $('#CreateFolder').offset().top + 50,
        'display': 'none'
    }).slideDown('slow');

    $('#NewFolderForm').on('click', '.close', function () {
        $("#FolderDialog").slideUp();
    });

    $('form').removeData('validator').removeData('unobtrusiveValidation');
    $.validator.unobtrusive.parse('form');
}

//function folderCreated(isCreated) {
//    var newAlert;

//    if (isCreated !== 'n') {
//        newAlert = $('<div></div>')
//            .addClass('folder-result')
//            .append('<div class="alert alert-info">¡Carpeta creada exitosamente!</div>');
//        $('footer').append(newAlert);
//        $("#FolderDialog").slideUp('slow');
//        $('.folder-result').fadeIn('slow').delay(3000).fadeOut('slow');
//        setTimeout(function () {
//            $('footer > div').last().remove();
//        }, 5000);
//    }
//    else {
//        newAlert = $('<div></div>')
//            .addClass('folder-result')
//            .append('<div class="alert alert-danger">Ha ocurrido un error. Inténtelo de nuevo.</div>');
//        $('footer').append(newAlert);
//        $("#FolderDialog").slideUp('slow');
//        $('.folder-result').fadeIn('slow').delay(3000).fadeOut('slow');
//        setTimeout(function () {
//            $('footer > div').last().remove();
//        }, 5000);
//    }
//}

