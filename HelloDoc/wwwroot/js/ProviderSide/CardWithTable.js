
$(document).ready(function () {

    $('.getregion').change(function () {
        var regionId = $(this).find(":selected").attr('itemid'); // This will get the id of the selected region
        $.ajax({
            url: '/Admin/GetPhysicianByRegionId', // Replace with your server script URL
            type: 'GET',
            data: { regionId: regionId },
            success: function (data) {

                var secondDropdown = $('.physiciandrop'); // Replace with your second dropdown selector
                secondDropdown.empty(); // Clear existing options

                secondDropdown.append($('<option>', {
                    hidden: "hidden",
                    value: "",
                    text: "Select Physician"
                }))
                $.each(data, function (index, item) {
                    secondDropdown.append($('<option>', {
                        value: item.firstname + item.lastname, // Replace with the actual value from your data
                        text: item.firstname + item.lastname // Replace with the actual text from your data
                    }));
                });
            }
        });
    });



    $.ajax({
        url: '/ProviderSide/NewState',
        success: function (response) {
            $('#status-tabContent').html('NewState');
            $('#dtBasicExample').html(response);

        },
        error: function (xhr, status, error) {
            console.error(error);
        }
    });



    var target1 = localStorage.getItem(target1);

    $('.dashboardtab').on('click', function (e) {
        e.preventDefault();
        $('.dashboardtab').removeClass('active');
        $(this).addClass('active');
        var target1 = $(this).data('bs-target');
        console.log(target1);
        localStorage.setItem('target1', target1);
        var url;
        switch (target1) {
            case '#s_new':
                $('#state').html("(New)")
                url = '/ProviderSide/NewState';
                break;
            case '#s_pending':
                $('#state').html("(Pending)")
                url = '/ProviderSide/PendingState';
                break;
            case '#s_active':
                $('#state').html("(Active)")
                url = '/ProviderSide/ActiveState';
                break;
            case '#s_conclude':
                $('#state').html("(Conclude)")
                url = '/ProviderSide/ConcludeState';
                break;
           
            default:
                url = '/ProviderSide/Admindashboard';
        }


        $.ajax({
            url: url,

            success: function (response) {
                $('#status-tabContent').html(response);
            },
            error: function (xhr, status, error) {
                console.error(error);
            }
        });

    });

    $('.dashboardtab[data-bs-target="' + target1 + '"]').trigger('click');

    if (target1 == null) {
        $('.dashboardtab[data-bs-target="#s_new"]').trigger('click');
    }

    //for search in accordion


});



$(document).ready(function () {

    $('#sendlink-submit').on('click', function () {
        var name = $('#sendlink-name-modal').val();
        var email = $('#sendlink-email-modal').val();
        var phonenumber = $('#sendlink-phonenumber-modal').val();

        if (name == "" && email == "" && phonenumber == "") {

            $('#sendlink-name').html("  *Required")
            $('#sendlink-email').html("  *Required")
            $('#sendlink-phonenumber').html("  *Required")
        }

        //else {
        //    $('#sendlink-submit').closest('form').submit()
        //}


        if ($('#sendlink-name').text() == "" &&
            $('#sendlink-email').text() == "" &&
            $('#sendlink-phonenumber').text() == "") {

            console.log("bdjc")

            $('#sendlink-submit').closest('form').submit()

        }

    });


    $('#sendlink-name-modal').on('input', function () {

        var name = $('#sendlink-name-modal').val();

        console.log(name)
        if (name == "") {

            $('#sendlink-name').html("  *Required")

        }
        else {
            const regex = /^[a-zA-Z]+$/i


            if (!regex.test(name)) {
                $('#sendlink-name').html("*not valid")

            } else {

                $('#sendlink-name').html("")
            }


        }
    });
    $('#sendlink-email-modal').on('input', function () {

        var email = $('#sendlink-email-modal').val();
        console.log(email)

        if (email == "") {

            $('#sendlink-email').html("  *Required")

        }
        else {

            ///^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]/

            var regex = /^\w+@[a-zA-Z_]+?\.[a-zA-Z]{2,3}$/;
            console.log("hello")
            if (!regex.test(email)) {
                $('#sendlink-email').html("*Enter valid email")
            }
            else {
                $('#sendlink-email').html("")
            }

        }
    });
    $('#sendlink-phonenumber-modal').on('input', function (e) {

        var phonenumber = $('#sendlink-phonenumber-modal').val();
        console.log(phonenumber)

        //if (e.which < 48 || e.which > 57) {
        //    // Prevent the default action (typing the character)
        //    e.preventDefault();
        //}

        if (phonenumber == "") {

            $('#sendlink-phonenumber').html("  *Required")

        }
        else {



            var regex = /^[1-9][0-9]{9}$/

            if (!regex.test(phonenumber)) {
                $('#sendlink-phonenumber').html("*Enter valid mobile number")
            }
            else {
                $('#sendlink-phonenumber').html("")
            }
        }
    });



});

function filterAccordionHeaders() {
    console.log("serach run")
    const input = document.getElementById('searchInput');
    const filter = input.value.toUpperCase();
    const headers = document.querySelectorAll('.accordion-header');


    headers.forEach((header) => {
        const patientName = header.querySelector('.patient-name');
        const nameText = patientName.textContent || patientName.innerText;

        if (nameText.toUpperCase().includes(filter)) {
            header.style.display = ''; // Show the header
        } else {
            header.style.display = 'none'; // Hide the header
        }
    });
}

// Attach the filter function to the input field
document.getElementById('searchInput').addEventListener('keyup', filterAccordionHeaders);




//validation for modal

var regexnote = /^[a-zA-Z0-9][a-zA-Z0-9 ]+$/i;
var regexemail = /^\w+@[a-zA-Z_]+?\.[a-zA-Z]{2,3}$/;
var regexphone = /^[1-9][0-9]{9}$/;
$('#CancelModalSubminbtn').click(function () {
    console.log("assign modal validation js")
    var note = $('#CancelModalNote').val();
    var firsrdrop = $('#CancelModalFirstDropDownSelect').val();
    var allvalidation = true;
    $('#CancelModalFirstDropDownSpan').html("");
    $('#CancelModalNoteSpan').html("");
    if (FirstDropDownValidation(firsrdrop)) {
        $('#CancelModalFirstDropDownSpan').html("Please Select Reason for cancellation");
        allvalidation = false;
    }

    if (note == "") {
        $('#CancelModalNoteSpan').html("Please Enter cancellation Note");
        allvalidation = false;
    }
    else if (!regexnote.test(note)) {
        $('#CancelModalNoteSpan').html("cancellation Note is not valid");
        allvalidation = false;
    }
    if (allvalidation) {
        $('#CancelModalSubminbtn').closest('form').submit()
    }
});

$('#AssignModalSubminbtn').click(function () {
    console.log("assign modal validation js")
    var note = $('#AssignModalNote').val();
    var Region = $('#AssignModalFirstDropDownSelect').val();
    var Physician = $('#AssignModalSecondDropDownSelect').val();
    var allvalidation = true;
    $('#AssignModalFirstDropDownSpan').html("");
    $('#AssignModalNoteSpan').html("");
    $('#AssignModalSecondDropDownSpan').html("");
    if (FirstDropDownValidation(Region)) {
        $('#AssignModalFirstDropDownSpan').html("Please Select Region");
        allvalidation = false;
    }
    if (SecondDropDownValidation(Physician)) {
        if (FirstDropDownValidation(Region)) {
            $('#AssignModalSecondDropDownSpan').html("Please Select Region first");

        }
        else {

            $('#AssignModalSecondDropDownSpan').html("Please Select Physician");
        }
        allvalidation = false;
    }
    if (note == "") {
        $('#AssignModalNoteSpan').html("Please Enter Assign Note");
        allvalidation = false;
    }
    else if (!regexnote.test(note)) {
        $('#AssignModalNoteSpan').html("Assign Note is not valid");
        allvalidation = false;
    }
    if (allvalidation) {
        $('#AssignModalSubminbtn').closest('form').submit()
    }
});


$('#BlockModalSubminbtn').click(function () {
    console.log("assign modal validation js")
    var note = $('#BlockModalNote').val();
    let allvalidation = true;
    $('#BlockModalNoteSpan').html(" ");
    $('#BlockModalNoteSpan').html("");
    if (note == "") {
        $('#BlockModalNoteSpan').html("Please Enter Block Note");
        allvalidation = false;
    }
    else if (!regexnote.test(note)) {
        $('#BlockModalNoteSpan').html("Block Note is not valid");
        allvalidation = false;
    }
    if (allvalidation) {
        $('#BlockModalSubminbtn').closest('form').submit()
    }
});




$('#TransferModalSubminbtn').click(function () {
    console.log("assign modal validation js")
    var note = $('#TransferModalNote').val();
    var Region = $('#TransferModalFirstDropDownSelect').val();
    var Physician = $('#TransferModalSecondDropDownSelect').val();
    var allvalidation = true;
    console.log(Physician)
    console.log(note)
    $('#TransferModalFirstDropDownSpan').html("");
    $('#TransferModalNoteSpan').html("");
    $('#TransferModalSecondDropDownSpan').html("");
    if (FirstDropDownValidation(Region)) {
        $('#TransferModalFirstDropDownSpan').html("Please Select Region");
        allvalidation = false;
    }
    if (SecondDropDownValidation(Physician)) {
        if (FirstDropDownValidation(Region)) {
            $('#TransferModalSecondDropDownSpan').html("Please Select Region First");
        }
        else {
            $('#TransferModalSecondDropDownSpan').html("Please Select Physician");

        }
        allvalidation = false;
    }
    if (note == "") {
        $('#TransferModalNoteSpan').html("Please Enter Assign Note");
        allvalidation = false;
    }
    else if (!regexnote.test(note)) {
        $('#TransferModalNoteSpan').html("Assign Note is not valid");
        allvalidation = false;
    }
    if (allvalidation) {
        $('#TransferModalSubminbtn').closest('form').submit();
    }
});

$('#SendAgreementModalSubminbtn').click(function () {
    var phone = $('#agreement-phone').val();
    var email = $('#agreement-email').val();
    let allvalidation = true;
    console.log(phone)
    $('#SendAgreementModalphoneSpan').html("");
    $('#SendAgreementModalemailSpan').html("");
    if (phone == "") {
        $('#SendAgreementModalphoneSpan').html("Please Enter Phone number");
        allvalidation = false;
    }
    else if (!regexphone.test(phone)) {
        $('#SendAgreementModalphoneSpan').html("Please enter 10 digit mobile number");
        allvalidation = false;
    }
    if (email == "") {
        $('#SendAgreementModalemailSpan').html("Please Enter Email");
        allvalidation = false;
    }
    else if (!regexemail.test(email)) {
        $('#SendAgreementModalemailSpan').html("Email is not valid");
        allvalidation = false;
    }


    if (allvalidation) {
        $('#SendAgreementModalSubminbtn').closest('form').submit()
    }
});



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




function ModalNotesValidation(note) {
    console.log(note)
    const regexPattern = /^[a-zA-Z0-9 ]+$/;
    if (note == "") {
        return true;
    }
    else if (!regexPattern.test(note)) {
        return true;
    }
    return false;
};
