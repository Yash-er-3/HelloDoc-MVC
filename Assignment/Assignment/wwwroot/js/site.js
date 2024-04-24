
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

$('#searchtable').on('keyup', function () {

    var text = $('#searchtable').val();

    $.ajax({
        url: '/Home/SearchTableStudent',
        data : text,
        success: function (response) {
            $('#studentdetail_table').html(response)
        }
    })
})