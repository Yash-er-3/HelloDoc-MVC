var regionid;
var status;
//var filterDate = new Date(Date.parse(localStorage.getItem("filterDate")));
var filterDate = new Date($('#currentDateValue').text());
console.log(filterDate)
var timezoneOffset = filterDate.getTimezoneOffset();
filterDate.setMinutes(filterDate.getMinutes() - timezoneOffset);
//var filterDate;
//if (localStorage.getItem("filterDate") == "")
//    filterDate = new Date($('#currentDateValue').text());
//else
//    filterDate = localStorage.getItem("filterDate");
//var timezoneOffset = filterDate.getTimezoneOffset();
//filterDate.setMinutes(filterDate.getMinutes() - timezoneOffset);

var currentPartial = "";
window.onload = function () {
    $('.admin-layout-nav').removeClass('admin-layout-nav-active');
    $('#nav-partner-tab').addClass('admin-layout-nav-active');
}
function loadSchedulingPartial() {
    currentPartial = "_MonthWise";
    localStorage.setItem("filterDate", filterDate.toISOString())
    $.ajax({
        url: '/Scheduling/LoadSchedulingPartial',
        data: { PartialName: currentPartial, date: filterDate.toISOString(), 'regionid': regionid, status: status },
        success: function (data) {
            localStorage.setItem("currentPartial", currentPartial);
            $(".calander").html(data);
        },
        error: function (e) {
            console.log(e);
        }
    });
}

$(document).ready(function () {
    loadSchedulingPartial("_DayWise");
    $('#prevDateBtn').on('click', function () {
        if (currentPartial == "_MonthWise") {
            var date = filterDate.setMonth(filterDate.getMonth() - 1);
            loadSchedulingPartial(currentPartial);
        }
        //else if (currentPartial == "_DayWise") {
        //    var date = filterDate.setDate(filterDate.getDate() - 1);
        //    loadSchedulingPartial(currentPartial);
        //}
        //else if (currentPartial == "_WeekWise") {
        //    var date = filterDate.setDate(filterDate.getDate() - 7);
        //    loadSchedulingPartial(currentPartial);
        //}
    });

    $('#nextDateBtn').on('click', function () {
        if (currentPartial == "_MonthWise") {
            var date = filterDate.setMonth(filterDate.getMonth() + 1);
            loadSchedulingPartial(currentPartial);
        }
        //else if (currentPartial == "_DayWise") {
        //    var date = filterDate.setDate(filterDate.getDate() + 1);
        //    loadSchedulingPartial(currentPartial);
        //}
        //else if (currentPartial == "_WeekWise") {
        //    var date = filterDate.setDate(filterDate.getDate() + 7);
        //    loadSchedulingPartial(currentPartial);
        //}
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
                //console.log(physelect);
                physelect.empty();
                physelect.append($('<option>', {
                    value: "",
                    text: "Select Physician"
                }))
                $.each(response, function (index, item) {
                    console.log(item)
                    physelect.append($('<option>', {
                        value: item.physicianId,
                        text: item.firstName + item.lastName
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





//$('#regionDropDownScheduling').on('change', function () {
//    regionid = $(this).val()
//    loadSchedulingPartial(currentPartial);
//});
$('#endTimeAddShiftModel , #startTimeAddShiftModel').on('change', function () {
    let start = $('#startTimeAddShiftModel').val();
    let end = $('#endTimeAddShiftModel').val();
    console.log("sgvgvd" + start + end)
    if (start != "00:00" && end != "00:00:00.000") {
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
    }

});
$('#viewshiftstartdate , #viewshiftenddate').on('change', function () {
    let start = $('#viewshiftstartdate').val();
    let end = $('#viewshiftenddate').val();
    console.log("sgvgvd" + start + end)
    if (start != "00:00" && end != "00:00:00.000") {

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

$('#deletebtnviewshiftmodel').on('click', function () {
    let shiftdetailsid = $('#shiftdetailidviewmodel').val()
    console.log("sjhdihj" + shiftdetailsid)
    $.ajax({
        url: '/Scheduling/ViewShiftModelDeletebtn',
        type: 'POST',
        data: {
            shiftdetailsid: shiftdetailsid
        },

        success: function (response) {
            loadSchedulingPartial(currentPartial);
            $('#viewShiftModal').modal('hide');
            Swal.fire({
                position: "top-end",
                icon: "success",
                title: "Shift Deleted Successfully",
                showConfirmButton: false,
                timer: 1700
            });
        },
        error: function (xhr, textStatus, errorThrown) {
            console.log('Error:', errorThrown);
        }
    });

});

$('#returnbtnviewshiftmodel').on('click', function () {
    let shiftdetailsid = $('#shiftdetailidviewmodel').val()
    console.log("sjhdihj" + shiftdetailsid)
    $.ajax({
        url: '/Scheduling/ViewShiftModelReturnbtn',
        type: 'POST',
        data: {
            shiftdetailsid: shiftdetailsid
        },

        success: function (response) {
            loadSchedulingPartial(currentPartial);
            $('#viewShiftModal').modal('hide');
        },
        error: function (xhr, textStatus, errorThrown) {
            console.log('Error:', errorThrown);
        }
    });

});
$('#editbtnviewshiftmodel').click(function () {
    console.log("jdsbhjdbshj")
    $('#viewshiftstartdate').prop('disabled', false);
    $('#viewshiftenddate').prop('disabled', false);
    $('#editbtnviewshiftmodel').addClass('d-none');
    $('#savebtnviewshiftmodel').removeClass('d-none');
});
$("#viewShiftForm").submit(function (event) {
    event.preventDefault();
    if ($("#viewShiftForm").valid()) {
        var formData = new FormData(this);
        console.log("djhsbwdhb")

        $.ajax({
            url: '/Scheduling/ViewShiftModelSavebtn',
            type: 'POST',
            data: formData,
            processData: false,
            contentType: false,
            success: function (response) {
                loadSchedulingPartial(currentPartial);
        $('#viewshiftstartdate').prop('disabled', true);
        $('#viewshiftenddate').prop('disabled', true);
        $('#editbtnviewshiftmodel').removeClass('d-none');
        $('#savebtnviewshiftmodel').addClass('d-none');
                $('#viewShiftModal').modal('hide');

            },
            error: function (xhr, textStatus, errorThrown) {
                console.log('Error while updating physician info:', errorThrown);
            }
        });
    }
});


$('#pendingShiftbtn , #activeShiftbtn, #allShiftbtn').click(function () {
    status = $(this).val();
    loadSchedulingPartial(currentPartial);
    $('.shiftfilter').removeClass('border-bottom-active');
    $(this).addClass('border-bottom-active');
});
$('#calendar-icon-datepicker').click(function () {
    console.log("cal")
    $('#calendar-hidden').click();
})

$('#providerOnCallSchedulingbtn').click(function () {
    console.log(filterDate)
    $.ajax({
        url: '/Scheduling/ProviderOnCall',
        data: { PartialName: currentPartial, date: filterDate.toISOString(), 'regionid': regionid, status: status },

        success: function (response) {
            $('#shedulingMainDiv').html(response);
            console.log(filterDate)
        },
        error: function (xhr, textStatus, errorThrown) {
            console.log('Error', errorThrown);
        }
    });
});

$('#regionDropDownProviderOnCall').on('change', function () {
    console.log("dbvfhv")
    regionid = $(this).val()
    console.log(filterDate)
    $.ajax({
        url: '/Scheduling/ProviderOnCall',
        data: { PartialName: currentPartial, date: filterDate.toISOString(), 'regionid': regionid, status: status },

        success: function (response) {
            $('#shedulingMainDiv').html(response);
        },
        error: function (xhr, textStatus, errorThrown) {
            console.log('Error', errorThrown);
        }
    });
});




//var currentpage;
//var totalpages;
//var pagesize = 10;
//$('#shiftForReviewSchedulingbtn').click(function () {
//    $.ajax({
//        url: '/Scheduling/ShiftForReview',
//        data: { currentPartial: currentPartial, date: filterDate.toISOString(), 'regionid': regionid, 'pagesize': pagesize, 'currentpage': currentpage },

//        success: function (response) {
//            $('#shedulingMainDiv').html(response)


//        },
//        error: function (xhr, textStatus, errorThrown) {
//            console.log('Error', errorThrown);
//        }
//    });
//});