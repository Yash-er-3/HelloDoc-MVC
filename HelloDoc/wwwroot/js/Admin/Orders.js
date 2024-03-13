
/*$(document).ready(function () {*/
    $('.firstdropdown').change(function () {
        var healthprofessionId = $(this).find(":selected").attr('itemid'); // This will get the id of the selected professional
        console.log(healthprofessionId);
        $.ajax({
            url: '/Admin/GetBusiness', // Replace with your server script URL
            type: 'GET',
            data: { healthprofessionId: healthprofessionId },
            success: function (data) {

                var secondDropdown = $('.business'); // Replace with your second dropdown selector
                secondDropdown.empty(); // Clear existing options
                secondDropdown.append($('<option>', {
                    hidden: "hidden",
                    value: "invalid",
                    text: "Business"
                }))
                $.each(data, function (index, item) {
                    secondDropdown.append($('<option>', {
                        value: item.vendorid, // Replace with the actual value from your data
                        text: item.vendorname // Replace with the actual text from your data
                    }));
                });
            }
        });
    });

    $('.secondDropdown').on('change', function () {
        $.ajax({
            url: '/Admin/GetVendorDetail',
            data: { vendorid: $(this).val() },
            success: function (data) {
                console.log(data)
                $('input[name="BusinessContact"]').val(data.businessContact);
                $('input[name="Email"]').val(data.email);
                $('input[name="FaxNumber"]').val(data.faxNumber);
            }
        });
    })

    //$.ajax({
    //    url: '/Admin/ViewNotes',
    //    type: 'POST',
    //    dataType: 'json',
    //    data: model,
    //    success: function (response) {

    //    },
    //    error: function (xhr, status, error) {
    //        console.error(error);
    //    }
    //});

    $('.ordersubmit').on('click', function () {
        debugger;

        var requestid = $('#requestid').val();
        console.log(requestid)
        var vendorid = $('.secondDropdown').val();
        var RefillNumber = $('.RefillNum').val();
        var presription = $('.prescription').val();
        //var OrderModal = {
        //    requestid : requestid,
        //    vendorid : vendorid
        //}


        $.ajax({
            url: '/Admin/Orders',
            type: 'POST',
            data: {
                requestid: requestid, vendorid: vendorid,
                RefillNumber: RefillNumber, presription: presription
            },
            success: function (data) {
                $('#status-tabContent').html(data);
            },
            error: function (xhr, status, error) {
                console.log(error);
            }
        });
    });

//});
