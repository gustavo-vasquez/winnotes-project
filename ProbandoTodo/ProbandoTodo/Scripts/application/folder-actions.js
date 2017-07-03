$('#CreateFolder').on('click', function () {
    $.ajax({
        url: "/Folder/Create",
        method: "GET",
        success: function (data) {            
            folderActions(data, true);
        }
    });
});

$('#EditFolder').on('click', function () {   
    //var dateModel = {
    //    "FolderID": $('#ThisFolder').data('iof'),
    //    "Name": $('#EditFolder').data('folder-name'),
    //    "Details": $('#EditFolder').data('folder-details')
    //};
    $.ajax({
        url: "/Folder/Edit",
        method: "GET",
        data: "folderID=" + $('#ThisFolder').data('iof') + "&name=" + $('#EditFolder').data('folder-name') + "&details=" + $('#EditFolder').data('folder-details'),        
        success: function (data) {
            folderActions(data, false);
        }
    });
});

$('.trash-folder').on('click', function () {
    if (confirm("¿Está seguro que quiere borrar esta carpeta?\n\nNOTA: Se perderán todas las notas que contenga.\nNO PUEDE DESHACERSE")) {
        $.ajax({
            url: "/Folder/Remove",
            method: "POST",
            data: "folderID=" + $('#ThisFolder').data('iof'),
            success: function() {
                window.location.href = "/Folder/List";
            },
            error: function () {
                window.location.reload();
            }               
        });
    }    
});

// ************************ FUNCIONES PARA CARPETA *****************************

function folderActions(data, action) {
    $('#MainDialog').html(data);

    if (action) {
        $("#FolderDialog").css({
            'position': 'absolute',
            'left': $('#CreateFolder').offset().left,
            'top': $('#CreateFolder').offset().top + 50,
            'display': 'none'
        }).slideDown('fast');

        $('#NewFolderForm').on('click', '.close', function () {
            $("#FolderDialog").slideUp('fast');
        });
    }
    else {
        $("#FolderDialog").css({
            'position': 'absolute',
            'left': $('#EditFolder').offset().left - 100,
            'top': $('#EditFolder').offset().top + 30,
            'display': 'none'
        }).slideDown('fast');

        $('#EditFolderForm').on('click', '.close', function () {
            $("#FolderDialog").slideUp('fast');
        });
    }

    $('form').removeData('validator').removeData('unobtrusiveValidation');
    $.validator.unobtrusive.parse('form');
}

