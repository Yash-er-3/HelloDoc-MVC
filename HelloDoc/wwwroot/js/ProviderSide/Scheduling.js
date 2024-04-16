

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
        url: '/MyScheduling/LoadSchedulingPartial',
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
            url: '/MyScheduling/filterregion',
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
        url: '/MyScheduling/ViewShiftOpen',
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


$('#viewShiftModal').on('hidden.bs.modal', function (e) {
    $(this).remove();
})

//  , '#returnviewshift', '#deleteviewshift'

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



