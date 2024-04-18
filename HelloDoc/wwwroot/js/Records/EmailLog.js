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
        url: '/Records/GetEmailLogTable',
        data: { 'Role': Role, 'ReceiverName': ReceiverName, 'Email': Email, 'CreatedDate': CreatedDate, 'SentDate': SentDate },
        success: function (data) {
            $("#EmailLogTable").html(data)
        },
        error: function (xhr, status, error) {

            console.error(error);
        }
    });
}
LoadData()

$('#searchEmailLogbtn').click(function () {
    Role = $('#RoleFilterEmailLog').val();
    ReceiverName = $('#ReceiverNameEmailLog').val();
    Email = $('#EmailIdEmailLog').val();
    CreatedDate = $('#CreatedDateEmailLog').val();
    SentDate = $('#SentDateEmailId').val();
    LoadData();
});
$('#clearEmailLogbtn').click(function () {
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


$('#recordstab').DataTable({

    "lengthMenu": [[5, 10, -1], [5, 10, "All"]],
    "pageLength": 5,
    language: {
        oPaginate: {
            sNext: '<i class="bi bi-caret-right-fill text-info"></i>',
            sPrevious: '<i class="bi bi-caret-left-fill text-info"></i>'

        }
    }
});
$('.dataTables_filter').hide();