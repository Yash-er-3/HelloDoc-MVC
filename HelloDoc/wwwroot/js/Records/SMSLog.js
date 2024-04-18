window.onload = function () {
    $('.admin-layout-nav').removeClass('admin-layout-nav-active');
    $('#nav-record-tab').addClass('admin-layout-nav-active');
}
var Role;
var ReceiverName;
var Email;
var CreatedDate;
var SentDate;
function LoadData() {
    console.log("load datad email log")
    $.ajax({
        url: '/Records/GetSMSLogTable',
        data: { 'Role': Role, 'ReceiverName': ReceiverName, 'Email': Email, 'CreatedDate': CreatedDate, 'SentDate': SentDate },
        success: function (data) {
            $("#SMSLogTable").html(data)
        },
        error: function (xhr, status, error) {

            console.error(error);
        }
    });
}
LoadData()

$('#searchSMSLogbtn').click(function () {
    Role = $('#RoleFilterEmailLog').val();
    ReceiverName = $('#ReceiverNameEmailLog').val();
    Email = $('#EmailIdEmailLog').val();
    CreatedDate = $('#CreatedDateEmailLog').val();
    SentDate = $('#SentDateEmailId').val();
    LoadData();
});
$('#clearSMSLogbtn').click(function () {
    $('#RoleFilterEmailLog').val(0);
    $('#ReceiverNameEmailLog').val("");
    $('#EmailIdEmailLog').val("");
    $('#CreatedDateEmailLog').val("01-01-0001 00:00:00");
    $('#SentDateEmailId').val("01-01-0001 00:00:00");
    Role = 0;
    ReceiverName = "";
    Email = "";
    CreatedDate = "01-01-0001 00:00:00";
    SentDate = "01-01-0001 00:00:00";
    LoadData();
    //$("#EmailLogTable").load('@Url.Action("Records","GetEmailLogTable")', { 'Role': Role, 'ReceiverName': ReceiverName, 'Email': Email, 'CreatedDate': CreatedDate, 'SentDate': SentDate });

});

