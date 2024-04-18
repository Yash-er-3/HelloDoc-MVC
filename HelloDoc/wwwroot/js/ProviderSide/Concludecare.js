
$('#notes_physician').on('blur', function () {
    var reqid = $('#rid').attr('value');
    var notes = $('#notes_physician').val();
    var viewModel = {
        requestid: reqid,
        BlockNotes: notes
    };
    console.log(viewModel)
    console.log(viewModel)
    console.log(viewModel)
    $.ajax({
        url: '/ProviderSide/PhysicianNotes', // replace with your URL
        type: 'POST',
        data: viewModel,
        success: function (data) {
            console.log('Success:', data);
        },
        error: function () {
            // Handle error
            console.log('Error occurred');
        }
    });

});
$('.uploadbtnconclude').click(function () {
    $('#actual-btn').click();
});

$('#actual-btn').change(function () {
    var formData = new FormData();
    for (var i = 0; i < actualBtn.files.length; i++) {
        formData.append('files', actualBtn.files[i]); // Append each selected file
    }
    formData.append('RequestsId', $('.RequestsId').val());
    console.log(formData);
    // Add any other data you need (e.g., RequestsId) gavsgvahgsvagvdsg
    $.ajax({
        url: '../Admin/UploadFiles', // Replace with your controller action URL
        type: 'POST',
        data: formData,
        processData: false,
        contentType: false,
        success: function (response) {
            $('#nav-home').html(response);
        },
        error: function (error) {
            console.error('Error uploading files:', error);
        }
    });


});
$('.ConcludeCareSubmitBtn').click(function () {
    let valuebtn = $(this).val();
    let requestid = $('.RequestsId').val();
    console.log(requestid)
    if (valuebtn == "notFinalize") {
        Swal.fire({
            icon: "error",
            title: "Oops...",
            text: "first finalize Encounter Form Then Try to conclude Care!",
        });
    }
    else {
        $.ajax({
            url: '/ProviderSide/ConcludeCareSubmit',

            data: { requestid: requestid },
            success: function () {
                location.reload();
            },
            error: function (error) {
                console.error('Error uploading files:', error);
            }
        });
    }
});



