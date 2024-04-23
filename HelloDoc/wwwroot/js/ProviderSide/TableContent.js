

$('#dtBasicExample').DataTable({

    "lengthMenu": [[10, 20, -1], [10, 20, "All"]],
    "pageLength": 10,
    language: {
        oPaginate: {
            sNext: '<i class="bi bi-caret-right-fill text-info"></i>',
            sPrevious: '<i class="bi bi-caret-left-fill text-info"></i>'

        }
    }
});

$('.dataTables_filter').hide();
var table = $('#dtBasicExample').DataTable();
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

        try {
            console.log('accp')
            var headers = document.querySelectorAll('.accordion-header');

            headers.forEach((header) => {
                var requesttype = header.querySelector('.requesttype-accordion');
                var nameText = requesttype.textContent || requesttype.innerText;
                console.log("running")


                if (nameText.includes(value)) {
                    console.log("running")
                    header.style.display = ''; // Show the header
                } else {
                    console.log("running")
                    header.style.display = 'none'; // Hide the header
                }
            });
        }
        catch {
        }
    }


});

$('#RegionSearch').change(function () {
    var regionid = $(this).val();

    if (regionid == '1234') {
        table.columns(1).search('').draw();

    } else {
        table.columns(1).search(regionid).draw();

        try {

            var headers = document.querySelectorAll('.accordion-header');

            headers.forEach((header) => {
                const regionName = header.querySelector('.region-accordion');
                const nameText = regionName.textContent || regionName.innerText;

                if (nameText.includes(regionid)) {
                    header.style.display = ''; // Show the header
                } else {
                    header.style.display = 'none'; // Hide the header
                }
            });
        }
        catch {

        }
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

    $('.closecase').on('click', function (e) {
        console.log("closecase");

        var requestid = $(this).attr('value');
        console.log(requestid)

        $.ajax({
            url: '/Admin/CloseCase',
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

    $('.encounter').on('click', function (e) {
        console.log('encounter');

        var requestid = $(this).attr('value');
        $('.requestid').val(requestid);

    })




    $('.encounter-save').on('click', function (e) {
        var requestid = $('.requestid').val();
        var encountervalue = $('input[name="options-outlined"]:checked').attr('value');
        //if (encountervalue == "Consult") {
        //}
        console.log(encountervalue)
        console.log(requestid)

        $.ajax({
            url: '/ProviderSide/EncounterSubmit',
            data: { requestid: requestid, encountervalue: encountervalue },
            success: function (data) {
                $('#exampleModalEncounter').click();
                $('#nav-tabContent').html(data);
                //location.reload();
            },
            error: function (xhr, status, error) {
                console.log(error);
            }

        })
    })

    $('.housecallbtnclickp').on('click', function () {
        var requestid = $(this).val();

        $.ajax({
            url: '/ProviderSide/OnHouseOpenEncounter',
            data: { requestid: requestid },
            type: 'POST',
            success: function (data) {
                $('#nav-tabContent').html(data);
            },
            error: function (xhr, status, error) {
                console.log(error);
            }

        })
    })


    $('.encounterclick_concludep').on('click', function () {
        var requestid = $(this).val();
        console.log("download encounter model open js")
        $.ajax({
            url: '/ProviderSide/CheckFinalize',
            data: { requestid: requestid },
            success: function (result) {
                console.log(result)
                if (result) {
                    $.ajax({
                        url: '/ProviderSide/Encounter',
                        data: { requestid: requestid },
                        success: function (result) {
                            $('#PopUps').html(result);
                            var my = new bootstrap.Modal(document.getElementById('DownloadEncounterModal'));
                            my.show();
                        },
                        error: function (xhr, status, error) {
                            console.log(error);
                        }

                    })
                }
                else {
                    $.ajax({
                        url: '/ProviderSide/Encounter',
                        data: { requestid: requestid },
                        success: function (result) {
                            $('#nav-tabContent').html(result);
                        },
                        error: function (xhr, status, error) {
                            console.log(error);
                        }

                    })
                }
                console.log(result.IsAjax);
                $('#download_Modal').html(result);
                var my = new bootstrap.Modal(document.getElementById('DownloadEncounterModal'));
                my.show();
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


