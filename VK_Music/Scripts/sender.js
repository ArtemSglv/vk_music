function Send(action_url) {
    var arr = new Array();
    $("tr").each(function (){  
        var tr=$(this);        console.log(tr.find('input[type=checkbox]').prop('checked'));
        if(tr.find('input[type=checkbox]').prop('checked'))
            {
                console.log(tr.find('img').attr('src'));
                arr.push(tr.find('img').attr('src'));
            }     
});
    $.ajax({
        url:action_url,
        data:{ list: arr},
        type:post,
        success:function(data){ alert(data)});
}