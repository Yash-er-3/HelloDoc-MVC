window.onload = function () {
    $('.admin-layout-nav').removeClass('admin-layout-nav-active');
    $('#nav-record-tab').addClass('admin-layout-nav-active');
}

$('.ViewCasePatientRecord').click(function () {
    let requestid = $(this).val();
    console.log("hdhbhsb")
    $.ajax({
        url: '/Admin/ViewCase',
        data: { 'id': requestid },
        success: function (data) {
            $('#patientRecordMainDiv').html(data);
        },
    });

})
$('.DocumentsPatientRecord').click(function () {
    let requestid = $(this).val();
    console.log("hdhbhsb")
    $.ajax({
        url: '/Admin/ViewUpload',
        data: { 'requestid': requestid },
        success: function (data) {
            $('#patientRecordMainDiv').html(data);
        },
    });

})
$('#viewNotesBackbtn').click(function () {
    location.reload();
});
