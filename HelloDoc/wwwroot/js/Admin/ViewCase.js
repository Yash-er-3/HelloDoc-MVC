
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

    $('#savebtn').click(function () {
        console.log("checked for view case edit btn")
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

