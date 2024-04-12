
$(document).ready(function () {
    loadblockhistorytable()
});
function loadblockhistorytable() {
    $.ajax({
        url: '/Records/BlockHistoryFilter',
        success: function (response) {
            $('#blockhistorystable').html(response)
        }
    });
}

$('#searchbtn_blockhistory').on('click', function () {

    var name = $('#name_blockhistory').val();
    var date = $('#date_blockhistory').val();
    var email = $('#email_blockhistory').val();
    var phonenumber = $('#phone_blockhistory').val();


    $.ajax({
        url: '/Records/BlockHistoryFilter',
        data: {
            name: name,
            date: date,
            email: email,
            phonenumber: phonenumber
        },

        success: function (response) {
            $('#blockhistorystable').html(response)
        }
    })
})

$('#clearbtn_blockhistory').on('click', function () {

    $('#name_blockhistory').val("");
    $('#date_blockhistory').val("");
    $('#email_blockhistory').val("");
    $('#phone_blockhistory').val("");

    $.ajax({
        url: '/Records/BlockHistoryFilter',
       

        success: function (response) {
            $('#blockhistorystable').html(response)
        }
    })
})

