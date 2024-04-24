
$(document).ready(function () {
    loadsearchtable()
});
function loadsearchtable() {
    $.ajax({
        url: '/Home/SearchRecordsFilter',
        success: function (response) {
            $('#studentdetail_table').html(response)
        }
    });
}

$('#AddAndEditStudentBtn').on('click', function () {

    $.ajax({
        url: '/Home/openModal',
        success: function (response) {
            $('#popups').html(response);
            var my = new bootstrap.Modal(document.getElementById('AddAndEditStudentModal'));
            my.show();
        }
    })

   
})