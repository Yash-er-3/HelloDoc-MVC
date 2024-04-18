



$(document).ready(function () {
    loadsearchtable()
});
function loadsearchtable() {
    $.ajax({
        url: '/Records/SearchRecordsFilter',
        success: function (response) {
            $('#searchrecordstable').html(response)
        }
    });
}

$('#searchbtn_searchrecords,#exportdata_searchrecords').on('click', function () {

    var reqstatus = $('#requeststatusfilter_searchrecords').val();
    var patientname = $('#patient_name_searchrecords').val();
    var requesttype = $('#requesttypefilter_searchrecords').val();
    var fromdateofservice = $('#fromdos_searchrecords').val();
    var todateofservice = $('#todos_searchrecords').val();
    var physicianname = $('#provider_name_searchrecords').val();
    var email = $('#email_searchrecords').val();
    var phonenumber = $('#phone_searchrecords').val();
    var download = $(this).val();
    console.log(fromdateofservice)

    if (download == "download") {
        $.ajax({
            url: '/Records/DownloadSearchRecord',
            data: {
                reqstatus: reqstatus,
                patientname: patientname,
                requesttype: requesttype,
                fromdateofservice: fromdateofservice,
                todateofservice: todateofservice,
                physicianname: physicianname,
                email: email,
                phonenumber: phonenumber,
            },

            success: function (base64String) {
                var byteCharacters = atob(base64String);
                var byteNumbers = new Array(byteCharacters.length);
                for (var i = 0; i < byteCharacters.length; i++) {
                    byteNumbers[i] = byteCharacters.charCodeAt(i);
                }
                var byteArray = new Uint8Array(byteNumbers);
                var blob = new Blob([byteArray], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
                var link = document.createElement('a');
                link.href = window.URL.createObjectURL(blob);
                var date = new Date();
                var day = String(date.getDate()).padStart(2, '0');
                var month = String(date.getMonth() + 1).padStart(2, '0'); //January is 0!
                var year = date.getFullYear();
                var hours = date.getHours();
                var minutes = date.getMinutes();
                var fileName = 'All_Records_' + day + '_' + month + '_' + year + '_' + hours + '_' + minutes + '.xlsx';
                link.download = fileName;
                link.click();
            }
        })
    }
    else {

        $.ajax({
            url: '/Records/SearchRecordsFilter',
            data: {
                reqstatus: reqstatus,
                patientname: patientname,
                requesttype: requesttype,
                fromdateofservice: fromdateofservice,
                todateofservice: todateofservice,
                physicianname: physicianname,
                email: email,
                phonenumber: phonenumber,
                download: download
            },

            success: function (response) {
                $('#searchrecordstable').html(response)
            }
        })
    }

})

$('#clearbtn_searchrecords').on('click', function () {
    console.log("runjbjbjkbkj")

    $('#requeststatusfilter_searchrecords').val(0);
    $('#patient_name_searchrecords').val("");
    $('#requesttypefilter_searchrecords').val(0);
    $('#fromdos_searchrecords').val("");
    $('#todos_searchrecords').val("");
    $('#provider_name_searchrecords').val("");
    $('#email_searchrecords').val("");
    $('#phone_searchrecords').val("");


    $.ajax({
        url: '/Records/SearchRecordsFilter',


        success: function (response) {
            $('#searchrecordstable').html(response)
        }
    })
})

