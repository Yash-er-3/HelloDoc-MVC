window.onload = function () {
    $('.admin-layout-nav').removeClass('admin-layout-nav-active');
    $('#nav-provider-tab').addClass('admin-layout-nav-active');
}

var email;
var providerid;

$(document).ready(function () {
    loadview();
});
function loadview() {
    $.ajax({
        url: '/Provider/ProviderMenuPartial',
        success: function (response) {
            $('#providermenutable').html(response)
        }
    });
}


//both sign and upload

try {

    var actualBtn = document.getElementById('actual-btn');
    var fileChosen = document.getElementById('file-chosen');

    actualBtn.addEventListener('change', function () {

        var filesname = this.files[0].name;
        console.log(filesname)
        for (var i = 1; i < this.files.length; i++) {
            filesname = filesname + " + " + this.files[i].name;
        }
        fileChosen.style.fontSize = "15px";
        fileChosen.style.fontWeight = "bold"
        fileChosen.textContent = filesname;
        fileChosen.ariaPlaceholder = filesname;
        console.log("run")
    })


} catch (Exception) {

}


try {

    var actualBtnSign = document.getElementById('actual-btn-sign');
    var fileChosenSign = document.getElementById('file-chosen-sign');


    console.log("sign")
    actualBtnSign.addEventListener('change', function () {

        var filesname = this.files[0].name;
        console.log(filesname)
        for (var i = 1; i < this.files.length; i++) {
            filesname = filesname + " + " + this.files[i].name;
        }
        fileChosenSign.style.fontSize = "15px";
        fileChosenSign.style.fontWeight = "bold"
        fileChosenSign.textContent = filesname;
        fileChosenSign.ariaPlaceholder = filesname;
        console.log("run")
    })
} catch (Exception) {

}

//sign and photo ajax

$('#provider-sign').on('click', function () {
    debugger
    var fileInput = document.querySelector('#actual-btn-sign');
    var file = fileInput.files[0];
    var reader = new FileReader();
    reader.onloadend = function () {
        var base64String = reader.result;
        $.ajax({
            url: '/Provider/EditProviderSign',
            type: 'POST',
            data: { providerid, base64String },
            success: function (response) {
                $('#providermenudiv').html(response);

            }
        });
    };
    reader.readAsDataURL(file);
});


$('#provider-photo').on('click', function () {
    debugger
    var fileInput = document.querySelector('#actual-btn');
    var file = fileInput.files[0];
    var reader = new FileReader();
    reader.onloadend = function () {
        var base64String = reader.result;
        $.ajax({
            url: '/Provider/EditProviderPhoto',
            type: 'POST',
            data: { providerid, base64String },
            success: function (response) {
                $('#providermenudiv').html(response);

            }
        });
    };
    reader.readAsDataURL(file);
});

//niche na 5 button file upload na

var onboardinguploadvalue;
$('.fileuploadbtn').on('click', function () {
    $('#SelectFileToUpload').click();

    onboardinguploadvalue = $(this).val();
});


//Reset password and account info JS

$('#username-editprovideraccount').on('input', function () {
    let username = $('#username-editprovideraccount').val();
    var regextext = /^[a-zA-Z][a-zA-Z0-9 ]+$/i;
    $('#username-editprovideraccount-span').html("");
    if (username == "") {
        $('#username-editprovideraccount-span').html("please enter username");
    }
    else if (!regextext.test(username)) {
        $('#username-editprovideraccount-span').html("username not valid");

    }
});
$('#Aieditbtn-editprovideraccount').on('click', function () {
    $('#Aisavebtn-editprovideraccount').removeClass('d-none')
    $('.aidisabled').removeAttr('disabled', false);
    $(this).addClass('d-none')
});
$('#Aisavebtn-editprovideraccount').on('click', function () {
    let username = $('#username-editprovideraccount').val();
    var model = {
        providerid: providerid,
        UserName: username,
    }

    if ($('#username-editprovideraccount-span').html() == "") {
        $('#Aieditbtn-editprovideraccount').removeClass('d-none')
        $('.aidisabled').attr('disabled', true);
        $(this).addClass('d-none');

        $.ajax({
            url: '/Provider/UpdatePhysicianInfo',
            type: 'POST',
            data: model,
            success: function (response) {
                //$('#nav-profile').html(response);
            },
            error: function (xhr, status, error) {
                console.error(error);
            }
        });
    }
});


$('#Airesetpasswordbtn-editprovideraccount').on('click', function () {
    $('#password-editprovideraccount').removeAttr('disabled', false);
    $(this).addClass('d-none');
    $('#Aiupdatebtn-editprovideraccount').removeClass('d-none');
});
$('#Aiupdatebtn-editprovideraccount').on('click', function () {
    let password = $('#password-editprovideraccount').val();
    var model = {
        providerid: providerid,
        password: password

    };
    var regexstrongpassword = /[A-Za-z\d@$!%*?&]{8,}/
    $('#resetpassword-editprovideraccount-span').html("");
    if (password == "") {

        $('#resetpassword-editprovideraccount-span').html("please enter password");
    }
    else if (!regexstrongpassword.test(password)) {
        alert("Please Enter Strong Password that contain\nAt least one uppercase letter\nAt least one lowercase letter\nAt least one digit\nAt least one special character\nAt least 8 characters in length")
    }
    else {
        $.ajax({
            url: '/Provider/UpdatePhysicianInfo',
            type: 'POST',
            data: model,
            success: function (response) {
                //$('#nav-profile').html(response);
                $('#password-editprovideraccount').attr('disabled', true);
                $('#Aiupdatebtn-editprovideraccount').addClass('d-none');
                $('#Airesetpasswordbtn-editprovideraccount').removeClass('d-none');
            },
            error: function (xhr, status, error) {
                console.error(error);
            }
        });
    }
});


//validation for physician information

$('#editbtn-editprovider').on('click', function () {

    $('.adisabled').removeAttr('disabled', false);
    $('#savebtn-editprovider').removeAttr('hidden', false);
    $('#savebtn-editprovider').show();
    $(this).hide();
})


$('#firstname-editprovider').on('input', function () {

    var firstname = $('#firstname-editprovider').val();

    if (firstname == "") {
        $('#providerinfo-firstname').html("*Required");

    }
    else {
        const regex = /^[a-zA-Z]+$/i

        if (!regex.test(firstname)) {
            $('#providerinfo-firstname').html("*not valid");

        } else {
            $('#providerinfo-firstname').html("");


        }
    }
})
$('#lastname-editprovider').on('input', function () {

    var lastname = $('#lastname-editprovider').val();

    if (lastname == "") {
        $('#providerinfo-lastname').html("*Required");

    }
    else {
        const regex = /^[a-zA-Z]+$/i

        if (!regex.test(lastname)) {
            $('#providerinfo-lastname').html("*not valid");

        } else {
            $('#providerinfo-lastname').html("");


        }
    }
});

$('#email-editprovider').on('input', function () {

    var email = $('#email-editprovider').val();

    if (email == "") {
        $('#providerinfo-email').html("*Required");
    }
    else {
        var regex = /^\w+@[a-zA-Z_]+?\.[a-zA-Z]{2,3}$/;

        if (!regex.test(email)) {
            $('#providerinfo-email').html("*enter valid email");

        } else {
            $('#providerinfo-email').html("");

        }
    }
})

$('#phonenumber-editprovider').on('input', function () {

    var phonenumber = $('#phonenumber-editprovider').val();

    console.log(phonenumber)

    if (phonenumber == "") {
        $('#providerinfo-phonenumber').html("*Required");
    }
    else {
        var regex = /^[1-9][0-9]{9}$/

        if (!regex.test(phonenumber)) {
            $('#providerinfo-phonenumber').html("*enter valid number");

        } else {
            $('#providerinfo-phonenumber').html("");

        }
    }
})

$('#medicallicence-editprovider').on('input', function () {

    var medicallicence = $('#medicallicence-editprovider').val();

    if (medicallicence == "") {
        $('#providerinfo-medicallicence').html("*Required");

    }
    else {
        const regex = /^[a-zA-Z0-9]+$/i

        if (!regex.test(medicallicence)) {
            $('#providerinfo-medicallicence').html("*not valid");

        } else {
            $('#providerinfo-medicallicence').html("");

        }
    }


});
$('#npinumber-editprovider').on('input', function () {

    var npinumber = $('#npinumber-editprovider').val();

    if (npinumber == "") {
        $('#providerinfo-npinumber').html("*Required");

    }
    else {
        const regex = /^[a-zA-Z0-9]+$/i

        if (!regex.test(npinumber)) {
            $('#providerinfo-npinumber').html("*not valid");

        } else {
            $('#providerinfo-npinumber').html("");

        }
    }
});

$('#syncemail-editprovider').on('input', function () {

    var syncemail = $('#syncemail-editprovider').val();

    if (syncemail == "") {
        $('#providerinfo-syncemail').html("*Required");

    }
    else {
        var regex = /^\w+@[a-zA-Z_]+?\.[a-zA-Z]{2,3}$/;


        if (!regex.test(syncemail)) {
            $('#providerinfo-syncemail').html("*not valid");

        } else {
            $('#providerinfo-syncemail').html("");

        }
    }


})

$('#savebtn-editprovider').on('click', function () {
    debugger
    var firstname = $('#firstname-editprovider').val();
    var lastname = $('#lastname-editprovider').val();
    var email = $('#email-editprovider').val();
    var phonenumber = $('#phonenumber-editprovider').val();
    var medicallicence = $('#medicallicence-editprovider').val();
    var npinumber = $('#npinumber-editprovider').val();
    var syncemail = $('#syncemail-editprovider').val();
    var selectedregion = [];


    if (
        $('#providerinfo-firstname').text() == "" &&
        $('#providerinfo-lastname').text() == "" &&
        $('#providerinfo-email').text() == "" &&
        $('#providerinfo-phonenumber').text() == "" &&
        $('#providerinfo-medicallicence').text() == "" &&
        $('#providerinfo-npinumber').text() == "" &&
        $('#providerinfo-syncemail').text() == "") {

        $('input[type="checkbox"]:checked').each(function () {
            selectedregion.push($(this).val());
        });

        var model = {
            firstname: firstname,
            lastname: lastname,
            phonenumber: phonenumber,
            email: email,
            selectedregion: selectedregion,
            MedicalLicence: medicallicence,
            NPINumber: npinumber,
            SynchronizeEmail: syncemail,
            providerid: providerid
        }

        $('.adisabled').attr('disabled', true);
        $('#editbtn-editprovider').show();
        $(this).hide();

        $.ajax({
            url: '/Provider/UpdatePhysicianInfo',
            type: 'POST',
            data: model,
            success: function (response) {

            },

            error: function (xhr, status, error) {
                console.error(error);
            }
        });


    }
});
//mailing billing validation

$('#editbtn-mailing-editprovider').on('click', function () {

    $('.mdisabled').removeAttr('disabled', false);
    $('#savebtn-mailing-editprovider').removeAttr('hidden', false);
    $('#savebtn-mailing-editprovider').show();
    $(this).hide();
})

$('#savebtn-mailing-editprovider').on('click', function () {
    var addr1 = $('#addr1-editprovider').val();
    var addr2 = $('#addr2-editprovider').val();
    var city = $('#city-editprovider').val();
    var zip = $('#zip-editprovider').val();
    var altphone = $('#altphone-editprovider').val();

    if (
        $('#providerinfo-addr1').text() == "" &&
        $('#providerinfo-addr2').text() == "" &&
        $('#providerinfo-city').text() == "" &&
        $('#providerinfo-zip').text() == "" &&
        $('#providerinfo-altphone').text() == ""
    ) {

        var model = {
            address1: addr1,
            address2: addr2,
            city: city,
            zip: zip,
            alterphonenumber: altphone,
            providerid: providerid
        }

        $('.mdisabled').attr('disabled', true);
        $('#editbtn-mailing-editprovider').show();
        $(this).hide();

        $.ajax({
            url: '/Provider/UpdatePhysicianInfo',
            type: 'POST',
            data: model,
            success: function (response) {
            },

            error: function (xhr, status, error) {
                console.error(error);
            }
        });
    }
});

$('#addr1-editprovider').on('input', function () {

    var addr1 = $('#addr1-editprovider').val();

    if (addr1 == "") {
        $('#providerinfo-addr1').html("*Required");

    }
    else {
        const regex = /^[^\s\W,-/][\w\s,-/]*$/i

        if (!regex.test(addr1)) {
            $('#providerinfo-addr1').html("*not valid");

        } else {
            $('#providerinfo-addr1').html("");


        }
    }
})
$('#addr2-editprovider').on('input', function () {

    var addr2 = $('#addr2-editprovider').val();

    if (addr2 == "") {
        $('#providerinfo-addr2').html("*Required");

    }
    else {
        const regex = /^[^\s\W,-/][\w\s,-/]*$/i

        if (!regex.test(addr2)) {
            $('#providerinfo-addr2').html("*not valid");

        } else {
            $('#providerinfo-addr2').html("");


        }
    }
})

$('#city-editprovider').on('input', function () {

    var city = $('#city-editprovider').val();

    if (city == "") {
        $('#providerinfo-city').html("*Required");

    }
    else {
        const regex = /^[^\s\W][\w\s]*$/i


        if (!regex.test(city)) {
            $('#providerinfo-city').html("*not valid");

        } else {
            $('#providerinfo-city').html("");


        }
    }
})
$('#zip-editprovider').on('input', function () {

    var zip = $('#zip-editprovider').val();

    if (zip == "") {
        $('#providerinfo-zip').html("*Required");

    }
    else {
        const regex = /^[1-9][0-9]{5}$/

        if (!regex.test(zip)) {
            $('#providerinfo-zip').html("*not valid");

        } else {
            $('#providerinfo-zip').html("");

        }
    }
})

$('#altphone-editprovider').on('input', function () {

    var altphone = $('#altphone-editprovider').val();

    if (altphone == "") {
        $('#providerinfo-altphone').html("*Required");
    }
    else {
        var regex = /^[1-9][0-9]{9}$/

        if (!regex.test(altphone)) {
            $('#providerinfo-altphone').html("*enter valid number");

        } else {
            $('#providerinfo-altphone').html("");

        }
    }
})



//provider profile edit and validation


$('#editbtn-ppdisable-editprovider').on('click', function () {

    $('.ppdisable').removeAttr('disabled', false);
    $('#savebtn-ppdisable-editprovider').removeAttr('hidden', false);
    $('#savebtn-ppdisable-editprovider').show();
    $(this).hide();
});


$('#savebtn-ppdisable-editprovider').on('click', function () {
    var businessname = $('#ppdisable-businessname').val();
    var businesswebsite = $('#ppdisable-businesswebsite').val();
    var adminnotes = $('#ppdisable-adminnotes').val();


    if (
        $('#ppdisable-adminnotes-span').text() == "" &&
        $('#ppdisable-businesswebsite-span').text() == "" &&
        $('#ppdisable-businessname-span').text() == ""
    ) {

        var model = {
            BusinessName: businessname,
            BusinessWebsite: businesswebsite,
            AdminNotes: businesswebsite,
            providerid: providerid

        }

        $('.ppdisable').attr('disabled', true);
        $('#editbtn-ppdisable-editprovider').show();
        $(this).hide();

        $.ajax({
            url: '/Provider/UpdatePhysicianInfo',
            type: 'POST',
            data: model,
            success: function (response) {
            },

            error: function (xhr, status, error) {
                console.error(error);
            }
        });
    }
});

$('#ppdisable-businessname').on('input', function () {

    var businessname = $('#ppdisable-businessname').val();

    if (businessname == "") {
        $('#ppdisable-businessname-span').html("*Required");

    }
    else {
        const regex = /^[^\s\W,-/][\w\s,-/]*$/i

        if (!regex.test(businessname)) {
            $('#ppdisable-businessname-span').html("*not valid");

        } else {
            $('#ppdisable-businessname-span').html("");
        }
    }
})

$('#ppdisable-businesswebsite').on('input', function () {

    var businesswebsite = $('#ppdisable-businesswebsite').val();

    if (businesswebsite == "") {
        $('#ppdisable-businesswebsite-span').html("*Required");

    }
    else {
        const regex = /^(https?:\/\/)?([a-zA-Z0-9]+\.)+[a-zA-Z]{2,}(\/[a-zA-Z0-9\-\._~:\/\?#\[\]@!\$&'\(\)\*\+,;=]*)?$/

        if (!regex.test(businesswebsite)) {
            $('#ppdisable-businesswebsite-span').html("*not valid");

        } else {
            $('#ppdisable-businesswebsite-span').html("");
        }
    }
})


$('#ppdisable-adminnotes').on('input', function () {

    var adminnotes = $('#ppdisable-adminnotes').val();

    if (adminnotes == "") {
        $('#ppdisable-adminnotes-span').html("*Required");

    }
    else {
        const regex = /^[^\s\W,-/][\w\s,-/.]*$/i

        if (!regex.test(adminnotes)) {
            $('#ppdisable-adminnotes-span').html("*not valid");

        } else {
            $('#ppdisable-adminnotes-span').html("");
        }
    }
})


//for onboarding

$('#SelectFileToUpload').on('change', function () {
    var fileInput = document.getElementById('SelectFileToUpload');
    var file = fileInput.files[0]; // Get the selected file

    var formData = new FormData();
    formData.append('file', file); // Append the file
    formData.append('providerid', providerid); // Append other data
    formData.append('onboardinguploadvalue', onboardinguploadvalue); // Append other data
    let x = file.name;
    let extention = x.split('.').pop();
    if (extention != "pdf") {
        alert("please upload pdf file")
    }
    else {

        $.ajax({
            url: '/Provider/uploadFile', // Replace with your server endpoint
            type: 'POST',
            data: formData,
            processData: false,
            contentType: false,
            success: function (response) {
                $('#providermenudiv').html(response);
            },
            error: function (xhr, status, error) {
                console.error(error);
            }
        });
    }
});

 //creating provider account
$('#createproviderbtn').on('click', function () {

    $.ajax({
        url: '/Provider/CreateProviderAccount', // Replace with your server endpoint
        success: function (response) {
            $('#providermenudiv').html(response);
        },
        error: function (xhr, status, error) {
            console.error(error);
        }    
    });
});