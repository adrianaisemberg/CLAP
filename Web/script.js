$(function(){
    $("#left a.menu_item").click(function(){
        $(".content:visible").hide();
        var href = $(this).attr("href");
        $(href).fadeIn(200);
        $(".selected").removeClass("selected");
        $(this).addClass("selected");
        
        // expand sub
        var visible_sub_menu = $(".sub_menu:visible");
        if (visible_sub_menu.attr("menu_id") != href)
        {
            visible_sub_menu.fadeOut(200);
        }
        
        $(".sub_menu[menu_id='" + href + "']").fadeIn(400);
        
        return false;
    });
    
    $("#home").show();
});
