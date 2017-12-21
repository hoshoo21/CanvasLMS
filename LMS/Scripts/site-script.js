$(document).ready(function () {

    $(document).on('click', '.cls-delete-confirm', function () {
        var obj = $(this);
        swal({
            title: Common.CheckIsNullAndReplace(obj.data('title'), "Are you sure?"),
            text: Common.CheckIsNullAndReplace(obj.data('message'), 'Confirm?'),
            type: "warning",
            showCancelButton: true,
            confirmButtonColor: "#DD6B55",
            confirmButtonText: "Yes",
            closeOnConfirm: false
        },
            function () { // IF YES
                {
                    window.location.href = obj.data('href');
                }
            });
    });

    $('.datepicker').datepicker({
        format: "dd/mm/yyyy",
        autoclose: true
    });

    $('.date-future').datepicker().on('changeDate', function (e) {
        ignoreReadonly: true
        $(this).datepicker('hide');
        if (!Common.CheckIsNull($(this).data('end-date-id'))) {
            var endID = $(this).closest('form').find('#' + $(this).data('end-date-id') + '');
            endID.val('');
            endID.datepicker('remove');
            endID.datepicker({
                startDate: $(this).val()
            });
        }
    });


})
