//  - - - - - - - - - - - - - - - - - - - - 
//  
//  JS Pop Up Window - POPUP.V2.R1.2017.08
//  [c] 2014 Grendelfly - A Division of SC&G Technology Solutions
//  Requires: JQuery(jquery.min.js)
//  
//  Target Class: .js_popup
//    
//  Elements w/in Target -
//      w = Width of Popup Window
//      h = Height of Popup Window
//      rel = Related Content to Pull into Window
//  Example: <a class="js_popup" w="360px" h="200px" rel="mycontent.html">Launch Window</a>
//  Example: <a class="js_popup" w="40%" h="80%" rel="mycontent.html">Launch Window</a>
//  
//  Functions -
//      Launch:  fPopup(r,w,h);
//      Distroy: fPopup_Kill();
//  
//  - - - - - - - - - - - - - - - - - - - - 

// Popup Window
function fPopup(r, w, h) {
    var dw = $(document).width();       // Document Width
    var dh = $(window).height();        // Document Height
    var cw = w.replace(/\D/g, '');      // Content Width - Stripped
    var ch = h.replace(/\D/g, '');      // Content Height - Stripped
    var ctm = 0;                        // Content Top Margin
    var clm = 0;                        // Content Left Margin

    // Get Content Left Margin
    if (w.indexOf('%') != -1) {
        w = (dw * (cw * 0.01));
        clm = (dw - w) / 2;
    } else {
        w = cw;
        clm = (dw - cw) / 2;
    }

    // Get Content Top Margin
    if (h.indexOf('%') != -1) {
        h = (dh * (ch * 0.01));
        ctm = (dh - h) / 2;
    } else {
        h = ch;
        ctm = (dh - ch) / 2;
    }
    $('body').append('<div class="js_popup_screen js_popup_kill"></div>');
    $('body').append('<div class="js_popup_content"></div>');
    $('.js_popup_content').append('<div></div>');
    $('.js_popup_content').append('<input type="button" class="js_popup_xbutton js_popup_kill" value="&times;">');
    $('.js_popup_screen').css({
        'position': 'fixed',
        'top': '0',
        'width': '100vw',
        'height': '100vw',
        'background-color': 'black',
        'opacity': '.35',
        'z-index': '9998'
    });
    $('.js_popup_content').css({
        'position': 'fixed',
        'top': (ctm - 20) + 'px',
        'left': (clm - 20) + 'px',
        'width': w +'px',
        'height': h,
        'padding': '20px',
        'background-color': 'white',
        'border-radius': '8px',
        'overflow':'hidden',
        'z-index': '9999'
    });
    $('.js_popup_xbutton').css({
        'position': 'absolute',
        'top': '10px',
        'right': '10px',
        'padding': '0px 6px 4px 6px',
        'font-size': '18px',
        'line-height': '18px'
    });
    if (r != undefined && r != '') {
        $('.js_popup_content > div').load(r, function () {
            var chd = $('.js_popup_content > div').height();
            $('.js_popup_content').animate({
                'height': (chd + 40) + 'px',
                'top': (((dh - (chd + 40)) * .5) - 20) + 'px'
            }, 150)
       });
    }
    AddEvents();
}

function fPopup_Kill() {
    $('.js_popup_screen,.js_popup_content').remove();
}

$('.js_popup').click(function () {
    var r = $(this).attr('rel');
    var w = $(this).attr('w');
    var h = $(this).attr('h');
    fPopup(r,w,h);
});

function AddEvents() {
$('.js_popup_kill').click(function () {
    fPopup_Kill();
    });
}
