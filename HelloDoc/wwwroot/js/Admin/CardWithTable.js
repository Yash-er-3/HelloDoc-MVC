
$(document).ready(function () {

    //$('input[name="requestby"]').on('change', function () {
    //    var value = $(this).attr('id');
    //    console.log(value);
    //    if (value == 'requestbyAll') {
    //        table.columns(0).search('').draw();
    //    }
    //    else {
    //        table.columns(0).search(value).draw(); // Replace 0 with the index of the column you want to filter
    //    }
    //});

   

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
    console.log(target1);
    $('.dashboardtab').on('click', function (e) {
        e.preventDefault();
        $('.dashboardtab').removeClass('active');
        $(this).addClass('active');
        var target1 = $(this).data('bs-target');
        console.log(target1);
        localStorage.setItem('target1', target1);
        console.log(localStorage.getItem(target1));
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
    console.log(target1);
    $('.dashboardtab[data-bs-target="' + target1 + '"]').trigger('click');

    if (target1 == null) {
        $('.dashboardtab[data-bs-target="#s_new"]').trigger('click');
    }

   
});



