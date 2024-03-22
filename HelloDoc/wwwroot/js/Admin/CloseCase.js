$(document).ready(function () {


    $('.viewcasephone').on('input', function () {

        var phonenumber = $('.viewcasephone').val();

        if (phonenumber == "") {
            $('#closecase-phonenumber').html("*Required")
        }
        else {
            var regex = /^[1-9][0-9]{9}$/

            if (!regex.test(phonenumber)) {
                $('#closecase-phonenumber').html("*Enter valid phone number")
            }
            else {
                $('#closecase-phonenumber').html("")
            }
        }
    })

    $('#ViewCasePatientEmail').on('input', function () {
        var email = $('#ViewCasePatientEmail').val();

        if (email == "") {

            $('#closecase-email').html("  *Required")

        }
        else {

            ///^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]/

            var regex = /^\w+@[a-zA-Z_]+?\.[a-zA-Z]{2,3}$/;
            console.log("hello")
            if (!regex.test(email)) {
                $('#closecase-email').html("*enter valid email")
            }
            else {
                $('#closecase-email').html("")
            }

        }
    })

    $('#closecase-edit').click(function () {
        // Remove disabled attribute to make inputs editable
        // $('#name').prop('disabled', false);
        // $('#age').prop('disabled', false);
        $('.disable').removeAttr('disabled', false);

        $('#edit-close-closecase').addClass('d-none')
        //$('#save-cancel-closecase').addClass('d-flex')
        $('#save-cancel-closecase').removeClass('d-none');
        // Hide the edit button after clicking it

    });
    $('#closecase-cancel').click(function () {
        // Remove disabled attribute to make inputs editable
        // $('#name').prop('disabled', false);
        // $('#age').prop('disabled', false);
        $('.disable').attr('disabled', true);

        $('#edit-close-closecase').removeClass('d-none')
        //$('#save-cancel-closecase').addClass('d-flex')
        $('#save-cancel-closecase').addClass('d-none');
        // Hide the edit button after clicking it


        var requestid = $('.CloseCaseRequestid').val();


        $.ajax({
            url: '/Admin/EditCloseCase',
            type: 'POST',
            data: { requestid: requestid },
            success: function (response) {
                $('#nav-home').html(response);
            },
            error: function (xhr, status, error) {
                console.error(error);
            }
        });
    });

    $('#closecase-save').click(function () {

        if ($('#closecase-email').text() == "" &&
            $('#closecase-phonenumber').text() == "") {


            $('.disable').attr('disabled', true);
            $('#edit-close-closecase').removeClass('d-none')
            $('#save-cancel-closecase').addClass('d-none');

            console.log("checked for view case edit btn")
            var requestid = $('.requestid').val();
            var email = $('#ViewCasePatientEmail').val();
            var phoneNumber = $('.viewcasephone').val();
            var requestid = $('.CloseCaseRequestid').val();
            console.log(email)
            console.log(phoneNumber)
            var ViewModel = {
                requestId: requestid,
                Email: email,
                PhoneNumber: phoneNumber,
                requestId: requestid,
            };
            console.log(ViewModel)

            $.ajax({
                url: '/Admin/EditCloseCase',
                type: 'POST',
                data: ViewModel,
                dataType: 'json',
                success: function (response) {
                    $('#nav-home').html(response);
                },
                error: function (xhr, status, error) {
                    console.error(error);
                }
            });
        }

    })

    $('#closecase-CloseCase').click(function () {
        var requestid = $('.requestid').val();
        var email = $('#ViewCasePatientEmail').val();
        var phoneNumber = $('.viewcasephone').val();
        var requestid = $('.CloseCaseRequestid').val();
        console.log(email)
        console.log(phoneNumber)
        var ViewModel = {
            requestId: requestid,
            Email: email,
            PhoneNumber: phoneNumber,
            requestId: requestid,
        };
        console.log(ViewModel)

        $.ajax({
            url: '/Admin/CloseCase',
            type: 'POST',
            data: ViewModel,
            dataType: 'json',
            success: function (response) {
                //$('#nav-home').html(response);
            },
            error: function (xhr, status, error) {
                console.error(error);
            }
        });

    })
});
