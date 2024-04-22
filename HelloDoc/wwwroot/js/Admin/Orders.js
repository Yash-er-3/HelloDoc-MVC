
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
                value: "",
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

$('.prescription').on('input', function () {

    var prescriptiondetails = $('.prescription').val();
    if (prescriptiondetails == "") {
        $('#order-prescription').html("*Required")
    }
    else {

        const regex = /^[^\s\W,-/][\w\s,-/]*$/i

        if (!regex.test(prescriptiondetails)) {
            $('#order-prescription').html("*not valid")

        } else {
            $('#order-prescription').html("")


        }

    }
})



//var prescriptiondetails = $('.prescription').val();

//if (prescriptiondetails == "") {
//    $('#order-prescription').html("*Required")

//}

if ($('#order-prescription').text() == "") {
    $('.ordersubmit').on('click', function () {

        var orderfirstdrop = $('.firstdropdown').val();
        var orderseconddrop = $('.secondDropdown').val();
        var prescriptiondetails = $('.prescription').val();

        var allvalidation = true;

        $('#firstdroporder-span').html("");
        $('#seconddroporder-span').html("");

        if (prescriptiondetails == "") {
            $('#order-prescription').html("*Required")
        }

        if (FirstDropDownValidation(orderfirstdrop)) {
            $('#firstdroporder-span').html("*Please Select Profession");
            allvalidation = false;
        }
        if (SecondDropDownValidation(orderseconddrop)) {
            if (FirstDropDownValidation(orderfirstdrop)) {
                $('#seconddroporder-span').html("*Please Select Profession First");

            }
            else {

                $('#seconddroporder-span').html("*Please Select Business");
            }
            allvalidation = false;
        }

        if (allvalidation && $('#order-prescription').text()=="") {
            var requestid = $('#requestid').val();
            console.log(requestid)
            var vendorid = $('.secondDropdown').val();
            var RefillNumber = $('.RefillNum').val();
            var presription = $('.prescription').val();
            

            $.ajax({
                url: '/Admin/Orders',
                type: 'POST',
                data: {
                    requestid: requestid, vendorid: vendorid,
                    RefillNumber: RefillNumber, presription: presription
                },
                success: function (data) {
                    window.location.reload();
                },
                error: function (xhr, status, error) {
                    console.log(error);
                }
            });

        }

    });
}



function FirstDropDownValidation(firstdropvalue) {
    console.log(firstdropvalue)
    if (firstdropvalue == "") {
        return true;
    }
    return false;
};
function SecondDropDownValidation(seconddropvalue) {
    console.log(seconddropvalue)
    if (seconddropvalue == "") {
        return true;
    }
    return false;
};



