$('.my-profile-load-page').click(function () {    console.log("hello this is profile");    $.ajax({        url: '/Admin/AdminProfile',        type: 'POST',        success: function (response) {            $('#nav-profile').html(response);        },        error: function (xhr, status, error) {            console.error(error);        }    });});$('#provider-tab').on('click', function () {    console.log("pro")
    $.ajax({
        url: '/Provider/ProviderMenu',
        type: 'GET',
        success: function (response) {            $('#nav-provider').html(response);        },        error: function (xhr, status, error) {            console.error(error);        }
    });
});