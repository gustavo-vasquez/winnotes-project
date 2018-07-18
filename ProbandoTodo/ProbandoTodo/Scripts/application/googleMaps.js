var myCenter = new google.maps.LatLng(47.6425674, -122.139108);

function initialize() {
    var mapProp = {
        center: myCenter,
        zoom: 5,
        mapTypeId: google.maps.MapTypeId.ROADMAP
    };
    var map = new google.maps.Map(document.getElementById("divGoogleMap"), mapProp);


    var marker = new google.maps.Marker({
        position: myCenter,
        animation: google.maps.Animation.BOUNCE
    });

    marker.setMap(map);

    var infowindow = new google.maps.InfoWindow({
        content: "Microsoft Visitor Center"
    });

    //infowindow.open(map, marker);

    google.maps.event.addListener(marker, 'click', function () {
        infowindow.open(map, marker);
    });
}

google.maps.event.addDomListener(window, 'load', initialize);