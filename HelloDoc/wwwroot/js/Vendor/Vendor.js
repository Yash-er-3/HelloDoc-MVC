
var search = ""; //for search
var professionid = 0; // for dropdown search profession
var vendorid = 0; //for delete btn vendor
$(document).ready(function () {
    loadview("", 0);
});
    function loadview(search, professionid) {
        $.ajax({

            url: '/Vendor/VendorFilter',
            data: { search: search, professionid: professionid },

            success: function (response) {

                $('#vendortablediv').html(response)
            }
        });
    }


$('#professionnamefilter').on('change', function () {

    professionid = $(this).val();

    $.ajax({
        url: '/Vendor/VendorFilter',
        data: { professionid: professionid },

        success: function (response) {
            loadview(search, professionid);
            $('#vendortablediv').html(response)
        }
    })
});

$('#searchfiltervendor').on('input', function () {

    search = $(this).val();

    $.ajax({
        url: '/Vendor/VendorFilter',
        data: { search: search },

        success: function (response) {
            loadview(search, professionid);
            $('#vendortablediv').html(response)
        }
    })
});

$('#addbusinessvendor').on('click', function () {

    $.ajax({
        url: '/Vendor/AddVendorAccount',
        success: function (response) {
            $('#vendormaindiv').html(response)
        }
    })
});

