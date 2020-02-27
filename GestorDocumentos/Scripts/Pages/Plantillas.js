$(document).ready(function () {
    $('#FechaH, #FechaD').datepicker({
        autoclose: true,
        format: 'dd/mm/yyyy'
    });

    $("#chxt").change(function () {
        if ($(this).is(":checked")) {
            $(".chx").prop("checked", true);
        } else {
            $(".chx").prop("checked", false);
        }
    });

});
