$("#createAdminAccountForm").submit(function (event) {
    debugger;
    event.preventDefault();
    if ($("#createAdminAccountForm").valid()) {
        var formData = new FormData(this);
        $('input[type="checkbox"]:checked').each(function () {
            formData.append("selectedregion", $(this).val());
        });
        $.ajax({
            url: '/Admin/CreateAdminAccount',
            type: 'POST',
            data: formData,
            processData: false,
            contentType: false,
            success: function (response) {
                window.location.href = "https://localhost:44300/";
            },
            error: function (xhr, textStatus, errorThrown) {
                console.log('Error while updating physician info:', errorThrown);
            }
        });
    }
});

$('.emailadminprofileblur').on('blur', function () {

    var email = $('.emailadminprofileblur').val();
    console.log(email);
    $.ajax({
        url: '/Admin/ValidateEmail',
        type: 'POST',
        data: { email },
        success: function (response) {
        },
        error: function (xhr, textStatus, errorThrown) {
            $('.emailadminprofileblur').val("");

            Swal.fire("Email already Exist!");
        }
    })
})