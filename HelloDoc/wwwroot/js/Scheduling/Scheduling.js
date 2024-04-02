

var regionid;
var filterDate = new Date($('#currentDateValue').text());
var currentPartial = "";


function loadSchedulingPartial(PartialName) {
    currentPartial = PartialName;
    $.ajax({
        url: '/Scheduling/LoadSchedulingPartial',
        data: { PartialName: PartialName, date: filterDate.toISOString(), 'regionid': regionid },
        success: function (data) {
            $(".calander").html(data);
        },
        error: function (e) {
            console.log(e);
        }
    });
}

$(document).ready(function () {
    $('.region').on('change', function () {
        regionid = $(this).val();
        $.ajax({
            url: '/Scheduling/LoadSchedulingPartial',
            data: { PartialName: currentPartial, date: filterDate.toISOString(), 'regionid': regionid },
            success: function (data) {
                $(".calander").html(data);
            },
            error: function (e) {
                console.log(e);
            }
        });
    });
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
            var date = filterDate.setDate(filterDate.getDate() +7);
            loadSchedulingPartial(currentPartial);
        }
    });

    $('.physiciandata').on('change', function (e) {
        var regionid = $(this).val();
        debugger
        console.log(regionid)
        $.ajax({
            url: '/Scheduling/filterregion',
            data: { "regionid": regionid },
            success: function (response) {
                //console.log(response);
                var physelect = $('#physelect');
                //console.log(physelect);
                physelect.empty();
                physelect.append($('<option>', {
                    value: "",
                    text: "Select Physician"
                }))
                response.forEach(function (item) {
                    console.log(item);
                    physelect.append(
                        $('<option>', {
                            value: item.physicianid,
                            text: item.firstname
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
//$('#savebtnviewshiftmodel').on('click', function () {
//    console.log("fbhdshfgb");
//    event.preventDefault();
//    if ($("#providerProfileForm").valid()) {
//        var formData = new FormData(this);
//        $.ajax({
//            url: '/AdminProviders/UpdatePhyProfile',
//            type: 'POST',
//            data: formData,
//            processData: false,
//            contentType: false,
//            dataType: 'json',
//            success: function (response) {
//                if (response.success) {
//                    toastr.success(response.message);
//                    $("#confirmProfileBtns").addClass("d-none");
//                    $("#editProfileBtn").removeClass("d-none");
//                    $("#providerProfileForm :input").not("#editProfileBtn").prop("disabled", true);
//                }
//            },
//            error: function (xhr, textStatus, errorThrown) {
//                console.log('Error while updating physician info:', errorThrown);
//            }
//        });
//    }
//});

$('.viewshiftbtn').on('click',function () {
    console.log("dhsgb")
    $("#viewShiftModal").modal();

});