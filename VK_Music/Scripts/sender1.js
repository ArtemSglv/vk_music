function Send(action_url) {
    var arr = new Array();
    /*$("tr").each(function () {
        var tr = $(this);

        if (tr.find('input[type=checkbox]').prop('checked')) {
            //arr.push(tr.find('span').attr('id'));
        }
    });*/

    $('input[type=checkbox]').each(function () {
        if ($(this).prop('checked')) {
            arr.push($(this).attr('name'));
        }
    });

    $.ajax({
        url: action_url,
        data: { list: arr },
        type: 'post',
        success: function (data) {
            window.location.reload();
        }
    });
}