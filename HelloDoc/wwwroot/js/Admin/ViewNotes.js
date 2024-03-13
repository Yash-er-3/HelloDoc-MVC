
$(document).ready(function () {
    $('#updatenotes').click(function () {
        console.log("hello notes")
        var notes = $('#note').val();
        var requestid = $('#reqid').attr('value');
        var model = {
            requestid: requestid,
            blocknotes: notes
        }
        console.log(model)
        $.ajax({
            url: '/Admin/ViewNotes',
            type: 'POST',
            dataType: 'json',
            data: model,
            success: function (response) {

            },
            error: function (xhr, status, error) {
                //console.log(error);
            }
        });
        $('#adminNote').html(notes)
    });

    $('#editButton').click(function () {
        // Remove disabled attribute to make inputs editable
        // $('#name').prop('disabled', false);
        // $('#age').prop('disabled', false);
        $('.disable').removeAttr('disabled', false);


        $('select').removeAttr('disabled', false);
        $(this).hide();
        $('#savebtn').removeAttr('hidden', false);
        // Hide the edit button after clicking it

    });



});
