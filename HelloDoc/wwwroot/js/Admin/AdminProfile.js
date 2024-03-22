
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


    if ($('#admininfo-password').text() == "") {


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
    }

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

    if (email != confirmemail) {
        $('#confirmemail-validationspan').html("email and confirmation email not matched");
    } else {
        $('#confirmemail-validationspan').html("");

    }

    if (
        $('#admininfo-firstname').text() == "" &&
        $('#admininfo-lastname').text() == "" &&
        $('#admininfo-email').text() == "" &&
        $('#confirmemail-validationspan').text() == "" &&
        $('#admininfo-phonenumber').text() == "") {

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

    if (
        $('#admininfo-addr1').text() == "" &&
        $('#admininfo-addr2').text() == "" &&
        $('#admininfo-city').text() == "" &&
        $('#admininfo-zip').text() == "" &&
        $('#admininfo-altphone').text() == "") {


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
    }

});

//password adminprofile validation

$('#password-adminprofile').on('input', function () {

    var password = $('#password-adminprofile').val();

    console.log(password)

    if (password == "") {
        $('#admininfo-password').html("*Required");
    }
    else {
        $('#admininfo-password').html("");
    }
})

//validation for administrator info

$('#firstname-adminprofile').on('input', function () {

    var firstname = $('#firstname-adminprofile').val();

    if (firstname == "") {
        $('#admininfo-firstname').html("*Required");

    }
    else {
        const regex = /^[a-zA-Z]+$/i

        if (!regex.test(firstname)) {
            $('#admininfo-firstname').html("*not valid");

        } else {
            $('#admininfo-firstname').html("");


        }
    }
})

$('#lastname-adminprofile').on('input', function () {

    var lastname = $('#lastname-adminprofile').val();

    if (lastname == "") {
        $('#admininfo-lastname').html("*Required");
    }
    else {
        const regex = /^[a-zA-Z]+$/i

        if (!regex.test(lastname)) {
            $('#admininfo-lastname').html("*not valid");

        } else {
            $('#admininfo-lastname').html("");

        }
    }
})
$('#email-adminprofile').on('input', function () {

    var email = $('#email-adminprofile').val();

    if (email != confirmemail) {
        $('#confirmemail-validationspan').html("email and confirmation email not matched!")
    }

    if (email == "") {
        $('#admininfo-email').html("*Required");
    }
    else {
        var regex = /^\w+@[a-zA-Z_]+?\.[a-zA-Z]{2,3}$/;

        if (!regex.test(email)) {
            $('#admininfo-email').html("*enter valid email");

        } else {
            $('#admininfo-email').html("");

        }
    }
})
$('#confirmemail-adminprofile').on('input', function () {


    var confirmemail = $('#confirmemail-adminprofile').val();

    if (confirmemail == "") {
        $('#confirmemail-validationspan').html("*Required");
    }
    else {
        var regex = /^\w+@[a-zA-Z_]+?\.[a-zA-Z]{2,3}$/;

        if (!regex.test(confirmemail)) {
            $('#confirmemail-validationspan').html("*enter valid email");

        } else {
            $('#confirmemail-validationspan').html("");

        }
    }
})
$('#phonenumber-adminprofile').on('input', function () {

    var phonenumber = $('#phonenumber-adminprofile').val();

    console.log(phonenumber)

    if (phonenumber == "") {
        $('#admininfo-phonenumber').html("*Required");
    }
    else {
        var regex = /^[1-9][0-9]{9}$/

        if (!regex.test(phonenumber)) {
            $('#admininfo-phonenumber').html("*enter valid number");

        } else {
            $('#admininfo-phonenumber').html("");

        }
    }
})


//mailing billing validation

$('#addr1-adminprofile').on('input', function () {

    var addr1 = $('#addr1-adminprofile').val();

    if (addr1 == "") {
        $('#admininfo-addr1').html("*Required");

    }
    else {
        const regex = /^[^\s\W,-/][\w\s,-/]*$/i

        if (!regex.test(addr1)) {
            $('#admininfo-addr1').html("*not valid");

        } else {
            $('#admininfo-addr1').html("");


        }
    }
})
$('#addr2-adminprofile').on('input', function () {

    var addr2 = $('#addr2-adminprofile').val();

    if (addr2 == "") {
        $('#admininfo-addr2').html("*Required");

    }
    else {
        const regex = /^[^\s\W,-/][\w\s,-/]*$/i

        if (!regex.test(addr2)) {
            $('#admininfo-addr2').html("*not valid");

        } else {
            $('#admininfo-addr2').html("");


        }
    }
})

$('#city-adminprofile').on('input', function () {

    var city = $('#city-adminprofile').val();

    if (city == "") {
        $('#admininfo-city').html("*Required");

    }
    else {
        const regex = /^[^\s\W][\w\s]*$/i



        if (!regex.test(city)) {
            $('#admininfo-city').html("*not valid");

        } else {
            $('#admininfo-city').html("");


        }
    }
})
$('#zip-adminprofile').on('input', function () {

    var zip = $('#zip-adminprofile').val();

    if (zip == "") {
        $('#admininfo-zip').html("*Required");

    }
    else {
        const regex = /^[1-9][0-9]{5}$/

        if (!regex.test(zip)) {
            $('#admininfo-zip').html("*not valid");

        } else {
            $('#admininfo-zip').html("");

        }
    }
})

$('#altphone-adminprofile').on('input', function () {

    var altphone = $('#altphone-adminprofile').val();

    if (altphone == "") {
        $('#admininfo-altphone').html("*Required");
    }
    else {
        var regex = /^[1-9][0-9]{9}$/

        if (!regex.test(altphone)) {
            $('#admininfo-altphone').html("*enter valid number");

        } else {
            $('#admininfo-altphone').html("");

        }
    }
})
