function PrintSCR() {

    $(".mud-appbar-fixed-top").hide();
    $(".mud-drawer-fixed").hide();

    $(".showWhenPrint").show();
    $(".hideWhenPrint").hide();
    //var contentElement = document.getElementsByClassName("content")[0];
    //var originalPadding = contentElement.style.padding;
    //contentElement.style.padding = "5px";        
    window.print();
    /* contentElement.style.padding = originalPadding;  */
    $(".hideWhenPrint").show();
    $(".showWhenPrint").hide();

    $(".mud-appbar-fixed-top").show();
    $(".mud-drawer-fixed").show();
}
