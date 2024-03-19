
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

$('.uploadbtn').on('click', function (e) {
    e.preventDefault();

    var formData = new FormData();
    for (var i = 0; i < actualBtn.files.length; i++) {
        formData.append('files', actualBtn.files[i]); // Append each selected file
    }
    formData.append('RequestsId', $('.RequestsId').val());
    console.log(formData);
    // Add any other data you need (e.g., RequestsId)

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


function deleteSelectedFiles() {
    var selectedFiles = document.querySelectorAll('input[name="checkdoc"]:checked');

    selectedFiles.forEach(function (checkbox) {

    });
}
$('#DeleteAllFiles').click(function () {
    $('.child:checked').each(function () {
        console.log("123456")
        var wiseid = $(this).attr('value')
        var reqid = $('.RequestsId').val();

        $.ajax({
            url: '../Admin/DeleteDoc', // Replace with your controller action URL
            type: 'POST',
            data: { id: wiseid, reqid: reqid },
            success: function (response) {
                $('#nav-home').html(response);

            },
            error: function (error) {
                console.error('Error uploading files:', error);
            }
        });
    });
});




$('.deleteadmin').on('click', function (e) {
    e.preventDefault();
    var wiseid = $(this).attr('value')
    var reqid = $('.RequestsId').val();
    $.ajax({
        url: '../Admin/DeleteDoc', // Replace with your controller action URL
        type: 'POST',
        data: { id: wiseid, reqid: reqid },
        success: function (response) {
            $('#nav-home').html(response);

        },
        error: function (error) {
            console.error('Error uploading files:', error);
        }
    });
});

$('#sendEmail').on('click', function (e) {

    e.preventDefault();
    var formData = new FormData();
    var reqid = $('.RequestsId').val();

    $('input[type=checkbox]:checked').each(function () {
        var wiseid = $(this).val();
        console.log(wiseid)
        formData.append('wiseFileId', wiseid)
    });
    formData.append('reqid', reqid)
    $.ajax({
        url: '/Admin/SendMail', // Replace with your controller action URL
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

})

$(document).ready(function () {

    $('#doc').click(function () {
        $('.child').prop('checked', $(this).prop('checked'));
    });

    // Handle individual checkbox clicks to update the leader checkbox state
    $('.child').click(function () {
        $('#doc').prop('checked', $('.child:checked').length === $('.child').length);
    });
});


// Function to handle download of selected files
function downloadSelectedFiles() {
    var selectedFiles = document.querySelectorAll('input[name="checkdoc"]:checked');
    var fileUrls = [];

    // Iterate through selected checkboxes and extract file URLs
    selectedFiles.forEach(function (checkbox) {
        var row = checkbox.closest('tr');
        var fileUrl = row.querySelector('a').getAttribute('href');
        fileUrls.push(fileUrl);
    });

    // Download each file
    fileUrls.forEach(function (url) {
        // Create an anchor element to trigger the download
        var link = document.createElement('a');
        link.href = url;
        link.download = '';
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
    });
}

try {
    // Event listener for download button
    document.getElementById('downloadSelected').addEventListener('click', function (event) {
        event.preventDefault();
        downloadSelectedFiles();
    });

} catch {

}



