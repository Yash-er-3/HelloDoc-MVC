window.onload = function () {
    $('.admin-layout-nav').removeClass('admin-layout-nav-active');
    $('#nav-record-tab').addClass('admin-layout-nav-active');
}
$('.unblockbtn_blockhistory').on('click', function () {

    var requestid = $(this).val();

    $.ajax({
        url: '/Records/UnblockbtnBlockHistory',
        data: { requestid: requestid },
        success: function (response) {
            $('#blockhistorystable').html(response)
        }
    });
});

$('.recordstab').DataTable({

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
