
$(document).ready(function () {
    $('#editButton').click(function () {
        // Remove disabled attribute to make inputs editable
        // $('#name').prop('disabled', false);
        // $('#age').prop('disabled', false);
        $('.disable').removeAttr('disabled', false);


        $('select').removeAttr('disabled', false);
        $(this).hide();
        $('#savebtn').removeAttr('hidden', false);
        $('#savebtn').show();
        // Hide the edit button after clicking it

    });



    $('.viewcasephone').on('input', function () {

        var phonenumber = $('.viewcasephone').val();

        if (phonenumber == "") {
            $('#viewcase-phone-validate').html("*Required")
        }
        else {
            var regex = /^[1-9][0-9]{9}$/

            if (!regex.test(phonenumber)) {
                $('#viewcase-phone-validate').html("*Enter valid phone number")
            }
            else {
                $('#viewcase-phone-validate').html("")
            }
        }
    })

    $('#ViewCasePatientEmail').on('input', function () {
        var email = $('#ViewCasePatientEmail').val();

        if (email == "") {

            $('#viewcase-email-validate').html("  *Required")

        }
        else {

            ///^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]/

            var regex = /^\w+@[a-zA-Z_]+?\.[a-zA-Z]{2,3}$/;
            console.log("hello")
            if (!regex.test(email)) {
                $('#viewcase-email-validate').html("*Enter valid email")
            }
            else {
                $('#viewcase-email-validate').html("")
            }

        }
    })

    $('#savebtn').click(function () {

        if ($('#viewcase-phone-validate').text() == "" &&
            $('#viewcase-email-validate').text() == "") {


            var requestid = $('.requestid').val();
            var email = $('#ViewCasePatientEmail').val();
            var phoneNumber = $('.viewcasephone').val();
            var confirmationNumber = $('.viewcaseconfirmationnumber').val();
            console.log(email)
            console.log(phoneNumber)
            var ViewModel = {
                requestId: requestid,
                Email: email,
                PhoneNumber: phoneNumber,
                ConfirmationNumber: confirmationNumber,
            };
            console.log(ViewModel)
            $('input[type="email"]').attr('disabled', true);
            $('input[type="tel"]').attr('disabled', true);
            $(this).hide();
            $('#savebtn').removeAttr('hidden', false);

            $('#editButton').show();
            $.ajax({
                url: '/Admin/Edit',
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

});


$(document).ready(function () {
    $('.getname').on('click', function (e) {
        var name = $(this).attr('value');
        var requestid = $(this).attr('id');
        console.log(requestid);
        $('.requestid').val(requestid);
        $('.patientname').html(name);
    });
});


$(document).ready(function () {
    $('.viewNotes').on('click', function (e) {
        console.log("hello")
        var requestid = $(this).attr('value');
        console.log(requestid);
        $.ajax({
            url: '/Admin/ViewNotes',
            type: 'GET',
            data: { id: requestid },
            success: function (response) {
                $('#nav-home').html(response);
            },
            error: function (xhr, status, error) {
                console.error(error);
            }
        });
    });
});


$('.acceptRequest').on('click', function () {
    console.log("accept")
    var requestid = $(this).val();
    Swal.fire({
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, Accept it!"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: '/ProviderSide/AcceptRequest',
                data: { requestid: requestid },
                success: function (response) {
                    $('#adminLayoutMainDiv').html(response)
                }
            })
            Swal.fire({
                title: "Accepted!",
                text: "Request Accepted Successfully.",
                icon: "success"
            });
        }
    });
});

$('.declineRequest').click(function () {
    var requestid = $(this).val();
    Swal.fire({
        title: "Are you sure?",
        text: "You want to Decline Request?\nYou won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, Decline it!"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: '/ProviderSide/DeclineRequest',
                data: { requestid: requestid },
                success: function (response) {
                    $('#adminLayoutMainDiv').html(response)
                    Swal.fire({
                        title: "Declined!",
                        text: "Request Declined Successfully.",
                        icon: "success"
                    });
                }
            })
        }
    });
});
