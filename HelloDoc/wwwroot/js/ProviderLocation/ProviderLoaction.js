﻿var locationdata;
window.onload = function () {
    $('.admin-layout-nav').removeClass('admin-layout-nav-active');
    $('#nav-provider-location-tab').addClass('admin-layout-nav-active');



}
$.ajax({
    url: '/ProviderLocation/GetLocationS',
    method: 'GET',
    async: false,

    success: function (response) {
        console.log(locationdata);
        locationdata = response
        console.log(locationdata);
        //response.map(function (res) {
        //    L.marker([res.latitude, res.longitude]).addTo(map)
        //        .bindPopup(res.physicianname)
        //})
    }
})

var map = L.map('map').setView([22.2, 77.1], 5);

L.tileLayer('https://tile.openstreetmap.org/{z}/{x}/{y}.png', {
    maxZoom: 19,
    attribution: '&copy; <a href="http://www.openstreetmap.org/copyright">OpenStreetMap</a>'
}).addTo(map);


//$.ajax({
//    url: '/ProviderLocation/GetLocation',
//    success: function (response) {
//        console.log(response);
//        response.map(function (res) {
//            L.marker([res.latitude, res.longitude]).addTo(map)
//                .bindPopup(res.physicianname)
//        })
//    }
//});
//var marker = L.marker([51.5, -0.19]).addTo(map);


for (var i = 0; i < locationdata.length; i++) {
    var popupContent;
    var iconHtml = '<div class="d-flex" style="width: 30px; height: 30px; border-radius: 50%; overflow: hidden; border: 4px solid #008000;">' +
        '<img src="' + locationdata[i].photo + '" style="width: 100%; height: auto;" />' +
        '</div>' +
        '<div style="width: 0; height: 0; border-left: 10px solid transparent; border-right: 10px solid transparent; border-top: 15px solid #008000; margin-left : 5px ;margin-top: -4px;"></div>';

    var customIcon = L.divIcon({
        className: 'custom-icon',
        html: iconHtml,
        iconSize: [30, 45], // size of the icon
        iconAnchor: [15, 45], // point of the icon which will correspond to marker's location
    });
    if (locationdata[i].photo != null) {
        popupContent = '<img class="openeditphy" data-id="' + locationdata[i].physicianid + '" width = "60%" src="' + locationdata[i].photo + '" />' +
            '<p>Physician: ' + locationdata[i].name + '</p>';
    }
    else {
        popupContent = '<img class="openeditphy" data-id="' + locationdata[i].physicianid + '" width = "60%" src="/images/profile-icon.png" />' +
            '<p>Physician: ' + locationdata[i].name + '</p>';
    }
    //var popupContent = '<img class="openeditphy" data-id="' + locationdata[i].Physicianid + '" width = "60%" src="' + locationdata[i].Photo + '" />' +
    //    '<p>Physician: ' + locationdata[i].Name + '</p>';
    var marker = L.marker([locationdata[i].lat, locationdata[i].long], { icon: customIcon }).addTo(map)
        .bindPopup(popupContent);

    marker.on('popupopen', function (e) {
        $('.openeditphy').on('click', function () {
            var physicianid = $(this).data('id');
            console.log(physicianid);
            $.ajax({
                url: '/Provider/EditProviderAccount',
                type: 'GET',
                data: { providerid: physicianid },
                success: function (result) {
                    $('#providerLocationMainDiv').html(result);
                },
                error: function (xhr, status, error) {
                    console.log(error+"dghshd")
                }
            });
        });
    });
}