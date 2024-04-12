

$('.editvendor').on('click', function () {
    console.log("edit")
    var vendorid = $(this).val();

    $.ajax({
        url: '/Vendor/AddVendorAccount',
        data: { vendorid: vendorid },

        success: function (response) {
            $('#vendormaindiv').html(response)
        }
    })
});

$('.deletevendor').on('click', function () {
    var vendorid = $(this).val();

    $.ajax({
        url: '/Vendor/VendorFilter',
        data: { vendorid: vendorid },

        success: function (response) {
            loadview(search, professionid);
            $('#vendortablediv').html(response)
        }
    })
});

