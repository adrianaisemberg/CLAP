$(function(){
    $("#left a.menu_item").click(function(){
        $(".content:visible").hide();
        var href = $(this).attr("href");
        $(href).fadeIn(200);
        $(".selected").removeClass("selected");
        $(this).addClass("selected");
        
        // expand sub
        var visible_sub_menu = $(".sub_menu:visible");
        if (visible_sub_menu.length > 0 && visible_sub_menu.attr("menu_id") != href)
        {
            visible_sub_menu.fadeOut(200, function(){
                fadeSubMenuIn(href);
            });
        }
        else
        {
            fadeSubMenuIn(href);
        }
        
        return false;
    });
    
    $("#what").show();
    
    SyntaxHighlighter.defaults['toolbar'] = false;
    SyntaxHighlighter.all();
});

function fadeSubMenuIn(href){
    $(".sub_menu[menu_id='" + href + "']").fadeIn(400);
}
