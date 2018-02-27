$(document).ready(function () {
    var availableColors = ["black", "blue", "blueviolet", "brown", "coral", "crimson", "darkgoldenrod", "darkturquoise", "deeppink", "deepskyblue", "gold", "fuchsia", "hotpink", "lightskyblue", "limegreen", "seagreen"];

    $.each(availableColors, function (index, value) {
        var newCircleColor = $('<label></label>');
        $('#AvailableColors').append(newCircleColor.addClass('color-box1').css('background-color', value));
        //('<label for="pBlack" class="color-box1" style="background-color:' + value + '" title="' + value + '"></label>');
    });
});