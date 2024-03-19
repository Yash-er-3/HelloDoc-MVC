
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
                    value: "invalid",
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
        url: '/Admin/NewState',
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
                url = '/Admin/NewState';
                break;
            case '#s_pending':
                $('#state').html("(Pending)")
                url = '/Admin/PendingState';
                break;
            case '#s_active':
                $('#state').html("(Active)")
                url = '/Admin/ActiveState';
                break;
            case '#s_conclude':
                $('#state').html("(Conclude)")
                url = '/Admin/ConcludeState';
                break;
            case '#s_toclose':
                $('#state').html("(ToClose)")
                url = '/Admin/ToCloseState';
                break;
            case '#s_unpaid':
                $('#state').html("(Unpaid)")
                url = '/Admin/UnpaidState';
                break;

            default:
                url = '/Admin/Admindashboard';
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

