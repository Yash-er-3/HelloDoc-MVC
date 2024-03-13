


// Initialize your DataTable
var table = $('#dtBasicExample').DataTable({

    dom: 'lrtip',
    "lengthMenu": [[5, 10, -1], [5, 10, "All"]],
    "pageLength": 5,
    language: {
        oPaginate: {
            sNext: '<i class="bi bi-caret-right-fill text-info"></i>',
            sPrevious: '<i class="bi bi-caret-left-fill text-info"></i>'

        }
    }
}); 
$('input[type="search"]').on('keyup', function () {
    table.search(this.value).draw();
});
$('input[name="requestby"]').on('change', function () {
    var value = $(this).attr('id');
    console.log(value);
    if (value == 'requestbyAll') {
        table.columns(0).search('').draw();
    }
    else {
        table.columns(0).search(value).draw(); // Replace 0 with the index of the column you want to filter
    }
});

$('#RegionSearch').change(function () {
    var regionid = $(this).val();

    if (regionid == '1234') {
        table.columns(1).search('').draw();

    } else {
        table.columns(1).search(regionid).draw();
    }
})

$(document).ready(function () {
    $('.getname').on('click', function (e) {
        var name = $(this).attr('value');
        var requestid = $(this).attr('id');
        console.log(requestid);
        $('.requestid').val(requestid);
        $('.patientname').html(name);
    })
})

$(document).ready(function () {
    $('.Viewcase').on('click', function (e) {
        console.log("hello")
        var requestid = $(this).attr('value');
        console.log(requestid);
        $.ajax({
            url: '/Admin/ViewCase',
            type: 'GET',
            data: { id: requestid },
            success: function (response) {
                $('#nav-tabContent').html(response);
            },
            error: function (xhr, status, error) {
                console.error(error);
            }
        });
    });
});

$(document).ready(function () {
    $('.viewNotes').on('click', function (e) {
        console.log("hello")
        var requestid = $(this).attr('value');
        console.log(requestid);
        $.ajax({
            url: '/Admin/ViewNotes',
            type: 'GET',
            data: { id: requestid },
            success: function (response) {
                $('#nav-home').html(response);
            },
            error: function (xhr, status, error) {
                console.error(error);
            }
        });
    });

    $('.viewupload').on('click', function (e) {
        console.log("viewupload");
        var requestid = $(this).attr('value');
        console.log(requestid);
        $.ajax({
            url: '/Admin/ViewUpload',
            type: 'GET',
            data: { requestid: requestid },
            success: function (response) {
                $('#nav-home').html(response);
            },
            error: function (xhr, status, error) {
                console.error(error);
            }
        });
    })

    $('.orders').on('click', function (e) {
        console.log("orders");

        var requestid = $(this).attr('value');
        console.log(requestid)

        $.ajax({
            url: '/Admin/Orders',
            type: 'GET',
            data: { requestid: requestid },
            success: function (response) {
                $('#nav-home').html(response);
            },
            error: function (xhr, status, error) {
                console.log(error);
            }
        })
    })

    $('.data-agreement').on('click', function (e) {
        console.log("send agree gayu");

        var requestid = $(this).attr('value');
        console.log(requestid);

        $.ajax({
            url: '/Admin/GetAgreementData',
            type: 'GET',
            data: { requestid: requestid },

            success: function (data) {
                $('#agreement-phone').val(data.phonenumber);
                $('#agreement-email').val(data.email);
                var requesttype = data.requesttype;
                console.log(requesttype);
                console.log(data.requesttype);
                var color = "green-dot";
                var text = "";
                switch (requesttype) {
                    case 1:
                        {
                            color = "green-dot";
                            text = "Patient";
                            break;
                        }
                    case 2:
                        {
                            color = "orange-dot";
                            text = "Family/Friend";
                            break;
                        }
                    case 3:
                        {
                            color = "pink-dot";
                            text = "Business Partner";
                            break;
                        }
                    case 4:
                        {
                            color = "cyan-dot";
                            text = "Concierge";
                            break;
                        }
                    default:
                        {
                            color = "purple-dot";
                            text = "vip";
                            break;
                        }
                }
                $('#requesttype-agreement').html(text);
                $('#requesttype-dot').addClass(color);
            }
        })
    })
});


