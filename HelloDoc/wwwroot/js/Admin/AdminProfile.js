
var phoneInputField2 = document.querySelector("#phonenumber-adminprofile");
var phoneInput = window.intlTelInput(phoneInputField2, {
    utilsScript:
        "https://cdnjs.cloudflare.com/ajax/libs/intl-tel-input/17.0.8/js/utils.js",
});

var phoneInputField2 = document.querySelector("#altphone-adminprofile");
var phoneInput = window.intlTelInput(phoneInputField2, {
    utilsScript:
        "https://cdnjs.cloudflare.com/ajax/libs/intl-tel-input/17.0.8/js/utils.js",
});


$('#resetpassword-adminprofile').click(function () {
    console.log("reset pass")

    $('#password-adminprofile').removeAttr('disabled', false);

    $('#resetpassword-adminprofile').addClass('d-none');
    $('#savepassword-adminprofile').removeClass('d-none');

});

$('#savepassword-adminprofile').click(function () {
    $('#password-adminprofile').removeAttr('disabled', true);


    $('#resetpassword-adminprofile').removeClass('d-none');
    $('#savepassword-adminprofile').addClass('d-none');

    var password = $('#password-adminprofile').val();
    var email = $('#email-adminprofile').val();
    $.ajax({
        url: '/CredentialAdmin/ResetPasswordProfileAdmin',
        type: 'POST',
        data: { password: password, email: email },
        success: function (response) {
            $('#nav-profile').html(response);
        },
        error: function (xhr, status, error) {
            console.error(error);
        }
    });

})
$('#editbtn-administrator-adminprofile').click(function () {
    console.log("reset pass")
    var password = $('#password-adminprofile').val();
    var email = $('#email-adminprofile').val();
    $('.adisabled').removeAttr('disabled', false);

    $('#savebtn-administrator-adminprofile').removeAttr('hidden', false);
    $('#savebtn-administrator-adminprofile').show();
    $(this).hide();


    $('#phonenumber-adminprofile').on('change', function () {
        var regex = /^(?!.*[a-zA-Z])[+]?\d{10}$/

        console.log("regex")
        if (!regex.test(phone)) {
            $('#phonenumber-validationspan').html("Phone number not valid");
        }
    })

    

    // $.ajax({
    //     url: '/AdminCredential/ResetPassword',
    //     type: 'POST',
    //     data: { email: email, password: password },
    //     success: function (response) {
    //         $('#nav-profile').html(response);
    //     },
    //     error: function (xhr, status, error) {
    //         console.error(error);
    //     }
    // });
});
$('#savebtn-administrator-adminprofile').click(function () {
    console.log("reset pass")
    debugger
    var password = $('#password-adminprofile').val();
    var email = $('#email-adminprofile').val();
    var confirmemail = $('#confirmemail-adminprofile').val();
    var phone = $('#phonenumber-adminprofile').val();
    var firstname = $('#firstname-adminprofile').val();
    var lastname = $('#lastname-adminprofile').val();
    var selectedregion = [];


   

   
   

    $('input[type="checkbox"]:checked').each(function () {
        selectedregion.push($(this).val());
    });
    console.log(selectedregion)
    var model = {
        firstname: firstname,
        lastname: lastname,
        phonenumber: phone,
        email: email,
        selectedregion: selectedregion,
    }

    if (email != confirmemail) {
        $('#confirmemail-validationspan').html("Email and Confirm Email is not Match");
    }
    else {
        $('#confirmemail-validationspan').html("");
        $('.adisabled').attr('disabled', true);
        $('#editbtn-administrator-adminprofile').show();
        $(this).hide();

        $.ajax({
            url: '/Admin/UpdateAdministrationInfoAdminProfile',
            type: 'POST',
            //dataType: 'json',
            data: model,
            success: function (response) {
                $('#nav-profile').html(response);
            },


            error: function (xhr, status, error) {
                console.error(error);
            }
        });
    }
});
$('#editbtn-mailing-adminprofile').click(function () {
    console.log("reset pass")
    var password = $('#password-adminprofile').val();
    var email = $('#email-adminprofile').val();
    $('.mdisabled').removeAttr('disabled', false);

    $('#savebtn-mailing-adminprofile').removeAttr('hidden', false);
    $('#savebtn-mailing-adminprofile').show();
    $(this).hide();

    // $.ajax({
    //     url: '/AdminCredential/ResetPassword',
    //     type: 'POST',
    //     data: { email: email, password: password },
    //     success: function (response) {
    //         $('#nav-profile').html(response);
    //     },
    //     error: function (xhr, status, error) {
    //         console.error(error);
    //     }
    // });
});

$('#savebtn-mailing-adminprofile').click(function () {


    var addr1 = $('#addr1-adminprofile').val();
    var addr2 = $('#addr2-adminprofile').val();
    var city = $('#city-adminprofile').val();
    var state = $('#state-adminprofile').val();
    var zip = $('#zip-adminprofile').val();
    var altphone = $('#altphone-adminprofile').val();


    var MailingBillingEdit = {
        address1: addr1,
        address2: addr2,
        city: city,
        state: state,
        zip: zip,
        alterphonenumber: altphone
    }

    $('.mdisabled').attr('disabled', true);

    $('#editbtn-mailing-adminprofile').show();
    $(this).hide();

    $.ajax({
        url: '/Admin/MailingBillingEditProfileAdmin',
        type: 'POST',
        data: MailingBillingEdit,
        success: function (response) {
            $('#nav-profile').html(response);
        },
        error: function (xhr, status, error) {
            console.error(error);
        }
    });
});