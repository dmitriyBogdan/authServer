$(function () {
    $('.delete').parent().click(function (e) {
        
        var answer = confirm("Are you sure?");
        if (answer == true) {
            return;
        } else {
            e.preventDefault();
        }
    });
})