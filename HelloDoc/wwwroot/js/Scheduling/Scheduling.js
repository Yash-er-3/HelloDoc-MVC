window.onload = function () {
    $('.admin-layout-nav').removeClass('admin-layout-nav-active');
    $('#nav-provider-tab').addClass('admin-layout-nav-active');
}

var regionid;
var status;
var filterDate = new Date($('#currentDateValue').text());
var currentPartial = "";
var timezoneOffset = filterDate.getTimezoneOffset();
filterDate.setMinutes(filterDate.getMinutes() - timezoneOffset);

function loadSchedulingPartial(PartialName) {
    currentPartial = PartialName;
    localStorage.setItem('currentPartial', PartialName)
    localStorage.setItem('filterDate', filterDate.toISOString())

    $.ajax({
        url: '/Scheduling/LoadSchedulingPartial',
        data: { PartialName: PartialName, date: filterDate.toISOString(), 'regionid': regionid, status: status },
        success: function (data) {
            $(".calander").html(data);
        },
        error: function (e) {
            console.log(e);
        }
    });
}

$(document).ready(function () {

    loadSchedulingPartial('_DayWise');
    $('#prevDateBtn').on('click', function () {
        if (currentPartial == "_MonthWise") {
            var date = filterDate.setMonth(filterDate.getMonth() - 1);
            loadSchedulingPartial(currentPartial);
        }
        else if (currentPartial == "_DayWise") {
            var date = filterDate.setDate(filterDate.getDate() - 1);
            loadSchedulingPartial(currentPartial);
        }
        else if (currentPartial == "_WeekWise") {
            var date = filterDate.setDate(filterDate.getDate() - 7);
            loadSchedulingPartial(currentPartial);
        }
    });

    $('#nextDateBtn').on('click', function () {
        if (currentPartial == "_MonthWise") {
            var date = filterDate.setMonth(filterDate.getMonth() + 1);
            loadSchedulingPartial(currentPartial);
        }
        else if (currentPartial == "_DayWise") {
            var date = filterDate.setDate(filterDate.getDate() + 1);
            loadSchedulingPartial(currentPartial);
        }
        else if (currentPartial == "_WeekWise") {
            var date = filterDate.setDate(filterDate.getDate() + 7);
            loadSchedulingPartial(currentPartial);
        }
    });

    $('.physiciandata').on('change', function (e) {
        var regionid = $(this).val();
        console.log(regionid)
        $.ajax({
            url: '/Scheduling/filterregion',
            data: { "regionid": regionid },
            success: function (response) {
                //console.log(response);
                var physelect = $('#physelectschedule');
                console.log(physelect);
                physelect.empty();
                physelect.append($('<option>', {
                    value: "",
                    text: "Select Physician"
                }))
                $.each(response, function (index, item) {
                    console.log(item);
                    physelect.append(
                        $('<option>', {
                            value: item.physicianid,
                            text: item.firstname + item.lastname
                        }));
                });
            },
            error: function (xhr, status, error) {
                console.error(error);
            }
        });
    });


    $('.repeatchk').on('change', function () {
        if ($(this).prop('checked')) {
            $('.disable').each(function () {
                $(this).prop('disabled', false);
            });
        }
        else {
            $('.disable').each(function () {
                $(this).prop('disabled', true);
            });
        }

    });
});

$('#editbtnviewshiftmodel').click(function () {
    console.log("jdsbhjdbshj")
    $(this).addClass('d-none');
    $('#savebtnviewshiftmodel').removeClass('d-none')
});

$('.viewshiftbtn').on('click', function () {
    console.log("dhsgb")
    $("#viewShiftModal").modal();

    var shiftid = $('#shiftidviewmodel').val()

    $.ajax({
        url: '/Scheduling/ViewShiftOpen',
        data: { shiftdetailid: shiftid },
        success: function (response) {
            $('#viewshiftregion').val(response.regionname)

            $('#viewshiftphysicianname').empty();
            $('#viewshiftphysicianname').append(
                $('<option>', {
                    value: response.physicianname,
                    text: response.physicianname,
                }));
            $('#viewshiftshiftdate').val(response.shiftdateviewshift)
            $('#viewshiftstartdate').val(response.starttime)
            $('#viewshiftenddate').val(response.endtime)
        },
        error: function (xhr, textStatus, error) {
            console.log(error)
        }
    });

});

$('#editbtnviewshiftmodel').on('click', function () {

    $('#viewshiftstartdate').prop('disabled', false)
    $('#viewshiftenddate').prop('disabled', false)
    $('#editbtnviewshiftmodel').addClass('d-none');
    $('#savebtnviewshiftmodel').removeClass('d-none');
});
$('#viewShiftModal').on('hidden.bs.modal', function (e) {
    $(this).remove();
})

//  , '#returnviewshift', '#deleteviewshift'
$('#savebtnviewshiftmodel,#returnviewshift, #deleteviewshift').on('click', function () {

    console.log("run view shift")

    var eventvalue = $(this).val()


    var model = {
        starttime: $('#viewshiftstartdate').val(),
        endtime: $('#viewshiftenddate').val(),
        shiftdetailsid: $('#shiftidviewmodel').val(),
        eventvalue: eventvalue
    }



    $.ajax({
        url: '/Scheduling/viewShiftEdit',
        data: model,
        success: function (response) {

            $('#viewshiftstartdate').prop('disabled', true)
            $('#viewshiftenddate').prop('disabled', true)
            $('#editbtnviewshiftmodel').removeClass('d-none');
            $('#savebtnviewshiftmodel').addClass('d-none');
            loadSchedulingPartial(currentPartial);
            $('#viewShiftModal').modal('hide');
        },
        error: function (xhr, textStatus, error) {
            console.log(error)
        }
    });
});

$('#schedulingregion').on('change', function () {
    regionid = $(this).val();

    loadSchedulingPartial(currentPartial);

});
$('#activeShiftbtn,#pendingShiftbtn,#allShiftbtn').on('click', function () {
    status = $(this).val();

    loadSchedulingPartial(currentPartial);

});

$('#endTimeAddShiftModel , #startTimeAddShiftModel').on('change', function () {
    debugger;
    let start = $('#startTimeAddShiftModel').val();
    let end = $('#endTimeAddShiftModel').val();
    console.log("sgvgvd" + start + end)
    const startdate = new Date(`1970-01-01T${start}`);
    const enddate = new Date(`1970-01-01T${end}`);
    let diff = enddate - startdate
    if (diff < 0) {
        Swal.fire({
            title: "Alert",
            text: "Selected End Time can not be earlier from start time ",
            icon: "warning",
        });
        $('#endTimeAddShiftModel').val("");

    }
    else {

        let diffMilliseconds = Math.abs(enddate - startdate);
        let minutes = Math.floor(diffMilliseconds / 60000);

        console.log(minutes)
        if (minutes < 120) {

            Swal.fire({
                title: "Alert",
                text: "you can add minimum 2 hour shift",
                icon: "warning",
            });
            $('#endTimeAddShiftModel').val("");
        }

    }

});
$('#viewshiftstartdate , #viewshiftenddate').on('change', function () {
    let start = $('#viewshiftstartdate').val();
    let end = $('#viewshiftenddate').val();



    if (start != "00:00" && end != "00:00:00") {

        console.log("sgvgvd" + start + end)
        let startdate = new Date(`1970-01-01T${start}`);
        let enddate = new Date(`1970-01-01T${end}`);
        let diff = enddate - startdate
        if (diff < 0) {
            Swal.fire({
                title: "Alert",
                text: "Selected End Time can not be earlier from start time ",
                icon: "warning",
            });
            $('#viewshiftenddate').val("");

        }
        else {

            let diffMilliseconds = Math.abs(enddate - startdate);
            let minutes = Math.floor(diffMilliseconds / 60000);

            console.log(minutes)
            if (minutes < 120) {

                Swal.fire({
                    title: "Alert",
                    text: "you can add minimum 2 hour shift",
                    icon: "warning",
                });
                $('#viewshiftenddate').val("");
            }

        }
    }
});

$('#addshiftstartdate , #addshiftenddate').on('change', function () {


    let start = $('#addshiftstartdate').val();
    let end = $('#addshiftenddate').val();

    if (start != "00:00" && end != "00:00:00") {

        console.log("sgvgvd" + start + end)
        let startdate = new Date(`1970-01-01T${start}`);
        let enddate = new Date(`1970-01-01T${end}`);
        let diff = enddate - startdate
        if (diff < 0) {
            Swal.fire({
                title: "Alert",
                text: "Selected End Time can not be earlier from start time ",
                icon: "warning",
            });
            $('#viewshiftenddate').val("");

        }
        else {

            let diffMilliseconds = Math.abs(enddate - startdate);
            let minutes = Math.floor(diffMilliseconds / 60000);

            console.log(minutes)
            if (minutes < 120) {

                Swal.fire({
                    title: "Alert",
                    text: "you can add minimum 2 hour shift",
                    icon: "warning",
                });
                $('#viewshiftenddate').val("");
            }

        }
    }
});

$('#calendar-icon-datepicker').click(function () {
    console.log("cal")
    $('#calendar-hidden').click();
})
$('.requestedshifts').click(function () {

    $.ajax({
        url: '/Scheduling/RequestedShifts',
        /*data: { currentPartial : currentPartial, filterDate : filterDate.toISOString(), regionid : regionid },*/
        success: function (response) {
            $('#scheduling-div').html(response)

            //var totalCount = response;
            //var pageSize = 10; // Customize your page size here

            //// Calculate the total number of pages
            //var totalPages = Math.ceil(totalCount / pageSize);

            //for (var i = 1; i <= totalPages; i++) {
            //    $('#paginationButtons').append('<button onclick="loadStaff(' + i + ')">Page ' + i + '</button>');
            //}

            //// Load initial staff data (e.g., page 1)
            //loadStaff(1);


        },
        error: function (status, xhr, error) {
            console.error(error)
        }
    });
})


$('#provideroncall').on('click', function () {

    $.ajax({
        url: '/Scheduling/ProviderOnCall',
        data: { PartialName: currentPartial, date: filterDate.toISOString(), 'regionid': regionid, status: status },
        success: function(data) {
            $("#scheduling-div").html(data);
        },
        error: function (e) {
            console.log(e);
        }
    })
})

$('#regionDropDownProviderOnCall').on('change', function () {
    console.log("dbvfhv")
    regionid = $(this).val()
    console.log(filterDate)
    $.ajax({
        url: '/Scheduling/ProviderOnCall',
        data: { PartialName: currentPartial, date: filterDate.toISOString(), 'regionid': regionid, status: status },

        success: function (response) {
            $('#scheduling-div').html(response);
        },
        error: function (xhr, textStatus, errorThrown) {
            console.log('Error', errorThrown);
        }
    });
});
