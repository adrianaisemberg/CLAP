$(function(){
    $("#left").find("a.menu_item").click(function(){
        selectLink($(this));
    });
    
    var hash = window.location.hash;
    if (hash)
    {
        selectLink($("a[href='" + hash + "']"));
    }
    else
    {
        selectLink($("a[href='#what']"));
    }
    
    SyntaxHighlighter.defaults['toolbar'] = false;
    SyntaxHighlighter.all();
});

function selectLink(link){
    $(".content:visible").hide();
    var href = link.attr("href");
    var content = $(href);
    content.fadeIn(200);
    $(".selected").removeClass("selected");
    link.addClass("selected");
    
    // expand sub
    //
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

    var parent = content.parent();

    if (!parent.is(":visible"))
    {
        parent.fadeIn(200);
        
        // if this is a sub menu item - expand the menu
        //
        var subMenuItem = $("a.sub_menu_item[href='" + href + "']");
        
        if (!subMenuItem.is(":visible"))
        {
            subMenuItem.parent().fadeIn(400);
        }
    }
}

function fadeSubMenuIn(href){
    $(".sub_menu[menu_id='" + href + "']").fadeIn(400);
}
