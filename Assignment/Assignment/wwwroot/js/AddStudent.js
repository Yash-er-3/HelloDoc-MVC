$('#submitbtn_addstudent').on('click', function () {


    var FirstName = $('#FirstName').val();
    var LastName = $('#LastName').val();
    var Email = $('#Email').val();
    var DOB = $('#DOB').val();
    var Gender = $('input[name="gender"]:checked').val();
    var Grade = $('#Grade').val();
    var Course = $('#Course').val();

  
    $.ajax({

        url: '/Home/AddStudent',
        type: 'POST',
        data: {
            FirstName: FirstName,
            LastName: LastName,
            Email: Email,
            DOB: DOB,
            Gender: Gender,
            Grade: Grade,
            Course: Course
        },
        success: function (response) {
            $('#studentdetail_table').html(response)
            location.reload()
        }
    })


})

$('#editbtn_addstudent').on('click', function () {

    var id = $('.editbtn_table').val();
    var FirstName = $('#FirstName').val();
    var LastName = $('#LastName').val();
    var Email = $('#Email').val();
    var DOB = $('#DOB').val();
    var Gender = $('input[name="gender"]:checked').val();
    var Grade = $('#Grade').val();
    var Course = $('#Course').val();

  
    $.ajax({

        url: '/Home/EditStudentSubmit',
        type: 'POST',
        data: {
            FirstName: FirstName,
            LastName: LastName,
            Email: Email,
            DOB: DOB,
            Gender: Gender,
            Grade: Grade,
            Course: Course,
            id : id
        },
        success: function (response) {
            $('#studentdetail_table').html(response)
            location.reload()
        }
    })


})

$('.editbtn_table').on('click', function () {

    var id = $(this).val();

    $.ajax({
        url: '/Home/EditStudent',
        data: { id: id },
        success: function (response) {
            $('#popups').html(response);
            var my = new bootstrap.Modal(document.getElementById('EditStudentModal'));
            my.show();


        }
    })
});

$('.deletebtn_table').on('click', function () {
    var id = $(this).val();

    $.ajax({
        url: '/Home/DeleteStudent',
        data: { id: id },
        success: function (response) {
            $('#studentdetail_table').html(response)
        }
    })
})

   
