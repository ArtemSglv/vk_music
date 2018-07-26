function Send() {
    $("tr").each(function (){
    console.log(this);
    $("td",this).each(function(){
        console.log(this);
    });
});
}