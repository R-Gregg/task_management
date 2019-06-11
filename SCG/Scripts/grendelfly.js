// Old Navigation Function
function subNav(sel) {
  var selector = "#" + sel + " a";
  var nav = "#" + sel + "_nav";
  if ($(nav).is(":hidden")) {
    $(".department-nav:visible").hide();
    $(nav).fadeIn(200);
    var left = $(selector).offset().left;
    var center = left + $(selector).width() / 2;
    var centerArrow = left + $(selector).width() / 2 + 8;
    var centerUl = center - $(nav + " ul").width() / 2 - 15;
    $(nav + " .arrow-up").css("left", centerArrow);
    if (centerUl >= 0) {
      $(nav + " ul").css("left", centerUl);
    }
  }
}

function loadBody(c, a) {

}

// Load Project into Holder
function loadProject(i, section, params) {
  var timeout = setTimeout(function () {
    $("body").append('<div class="loading-background"></div><div class="loading">Loading<span>.</span><span>.</span><span>.</span></div>');
  }, 200);
  var p = params != ('' || undefined) ? "?" + params : '';
  $("#projects").load('/' + section + '/Index/' + i + p, function () {
    clearTimeout(timeout);
    $(".loading-background, .loading").remove();
    $("#projects").css("display", "block");
  });
}

// Goes Back to List of All Projects
function viewProjects() {
  window.location.assign('/Projects?c=1');
}

// Universal Modal Box Function
function loadModal(url, id) {
  $("#modalBox").load(url, function () {
    $("#open" + id).modal();
  });
}

// Universal Page Loader, Used for all pages that loads from Pages Database
function loadPage(url) {
  $("#ip_holder").load(url);
}

// Load MJL Engagements
function loadEngagements(type, year, sort) {
  var timeout = setTimeout(function () {
    $("body").append('<div class="loading-background"></div><div class="loading">Loading<span>.</span><span>.</span><span>.</span></div>');
  }, 200);
  var engType = type != ('' || undefined) ? type.replace(/\s/g, '%20').replace('&', '%26').replace('amp;', '') : 'Loan%20Review';
  var params = "?type=" + engType;
  params = year != ('' || undefined) ? params + "&year=" + year : params;
  params = sort != ('' || undefined) ? params + "&sort=" + sort : params;
  $("#engagement-holder").load('/Engagements/Index' + params, function () {
    clearTimeout(timeout);
    $(".loading-background, .loading").remove();
  });
  $('.tabs-nav').removeClass('active');
  $('.tabs-nav.eng:contains("' + type + '")').addClass('active');
}

// Load MJL File Review
function loadFileReview(type, status, year, sort) {
  var timeout = setTimeout(function () {
    $("body").append('<div class="loading-background"></div><div class="loading">Loading<span>.</span><span>.</span><span>.</span></div>');
  }, 200);
  var engType = type != ('' || undefined) ? type.replace(/\s/g, '%20').replace('&', '%26').replace('amp;', '') : 'In Progress';
  var params = "?type=" + engType;
  params = status != ('' || undefined) ? params + "&status=" + status.replace(/\s/g, '%20') : params;
  params = year != ('' || undefined) ? params + "&year=" + year : params;
  params = sort != ('' || undefined) ? params + "&sort=" + sort : params;
  $("#engagement-holder").load('/Engagements/FileReview' + params, function () {
    clearTimeout(timeout);
    $(".loading-background, .loading").remove();
  });
}

// Load MJL Other Actions (Calendar, Bank Summary, Consultant Summary)
function loadMjlAction(action, year, id, month) {
  var timeout = setTimeout(function () {
    $("body").append('<div class="loading-background"></div><div class="loading">Loading<span>.</span><span>.</span><span>.</span></div>');
  }, 200);
  var url = '/MJL/' + action;
  var params = "?year=" + year;
  params = id != ('' || undefined) ? params + "&id=" + id : params;
  params = month != ('' || undefined) ? params + "&month=" + month : params;
  $("#engagement-holder").load(url + params, function () {
    clearTimeout(timeout);
    $(".loading-background, .loading").remove();
  });

  if (action == "IndBankSummary") {
    $('.tabs-nav').removeClass('active');
    $('.tabs-nav.ex.bs').addClass('active');
  } else if (action == "IndConsultantSummary") {
    $('.tabs-nav').removeClass('active');
    $('.tabs-nav.ex.cs').addClass('active');
  }
}

// Reset Fixed Table Header
function reflow(sel) {
  var tranX = $(".floatThead-container").css('transform').split('(')[1].split(',')[4];
  $(".floatThead-container").css({ transform: 'translateX(' + tranX + ') translateY(130px)' });
}

function createRow(sel, url) {
  $(sel).closest('table').prepend();
}

// Universal Table Row Load after Edit/Create
function selectRow(sel, url) {
  $(sel).closest('tr').load(url);
}

// Universal Delete Function
function deleteObject(url, sel, message) {
  if (confirm(message)) {
    $.ajax({
      url: url,
      method: 'POST',
      success: function (result) {
        $(sel).hide();
      }
    })
  }
}

// Universal Modal Delete Function
function deleteItem(url, sel) {
  if (confirm('Are you sure you want to delete this?')) {
    $.ajax({
      url: url,
      method: 'POST',
      success: function (result) {
        $('#' + sel).modal('hide');
      }
    });
  }
}

//  Universal Numeric Strip
function fNum_Strip(num) {
  'use strict';

  var n = 0;
  if (num !== null && num !== undefined) {
    n = num.replace(/[^0-9.-]/g, '');
  }
  return n;

}

// Navigation Behaviors
$('.nav-1 li').mouseenter(function () {
  var i = $(this).attr('id');
  var o = $(this).attr('class');
  if (o == undefined) { o = ''; }
  if (o.indexOf('slt') == -1) {
    $('.nav-1 li').removeClass('tmp');
    $(this).addClass('tmp');
    $('.nav-2').hide();
    $('#' + i.replace('_nav', '') + '_subnav').show();
  }
});

$('#nav').mouseleave(function () {
  setTimeout(function () {
    var i = $('body').attr('class');
    var c = i.split(' ');
    $('.tmp').removeClass('tmp');
    $('.nav-2').hide();
    $('#' + c[1].replace('_area', '') + '_subnav').show();
  }, 1000);
});

// Home Check - Remove
$('#nav a').each(function (i, e) {
  var u = $(e).attr('href');
  if (u == 'Javascript:void(0);') {
    $(e).css({ 'opacity': '.4' });
  }
});

// Universal Full Screen Function
function goFullScreen(sel) {
  if ($(sel).hasClass('fa-arrows-alt')) {
    $(sel).removeClass('fa-arrows-alt').addClass('fa-columns');
    $('.window_expand').fadeOut(function () {
      $('#columns > div > div').css('padding', '30px 20px;');
    });
    $('body').addClass('fullscreen').css("padding-top", "0px");
  } else {
    $(sel).removeClass('fa-columns').addClass('fa-arrows-alt');
    $('.window_expand').fadeIn(function () {
      $('#columns > div > div').css('padding', '40px 20px;');
    });
    $('body').removeClass('fullscreen').css("padding-top", "50px");
  }
}

$('#full-screen').click(function () {
  goFullScreen($(this));
});

$(function () {
  'use strict';
  $(document).ready(function () {

    //  Navigation Button Function
    $('.f_nav').css({ 'cursor': 'pointer' });
    $('.f_nav').click(function () {
      var l = $(this).attr('url');
      window.location.assign(l);
    });

    //  Scroll Up Footer
    $('<div/>', {
      'id': 'js_scrl_ftr',
      'text': 'Scroll To Top',
      click: function () {
        $('html:not(:animated),body:not(:animated)').animate({ scrollTop: 0 }, 500);
      }
    }).appendTo('body');

    $('<i/>', {
      'class': 'fa fa-arrow-circle-o-up fa-fw fa-lg'
    }).appendTo('#js_scrl_ftr');

    $('#js_scrl_ftr').css({
      'position': 'fixed', 'bottom': '0',
      'width': '100%', 'padding': '10px 0',
      'font-size': '16px',
      'text-align': 'center', 'color': '#FFF', 'cursor': 'pointer',
      'background-color': '#000'
    }).hide();

    $(window).scroll(function () {
      var w = $(this).width();
      var o = $(this).scrollTop();
      if (w >= 1024 && o > 100) {
        $('#js_scrl_ftr').fadeIn();
      } else {
        $('#js_scrl_ftr').hide();
      }
    });

    //  Sub-Nav Scroll Effect
    $('.scroll a').click(function (e) {
      e.preventDefault();
      var l = $(this).attr('href');			// Link
      var a = l.split('#');					// Anchor Name
      var t = $('a[name="' + a[1] + '"]'); 	// Target
      var o = t.offset().top - 60; 				// Offset of Target
      $('html:not(:animated),body:not(:animated)').animate({ scrollTop: o }, 500);
    });

    //  Toggle/Slide Function
    $('.toggle-trg').css({ 'cursor': 'pointer' });
    $('.toggle-trg').click(function () {

      var trg = $(this).attr('rel');
      $('#' + trg).slideToggle();

    });

    //  IE8 Detection Though CSS / 2014.04.04
    var ie8_check = $('.ie-test').css('display');
    if (ie8_check !== 'block') { // No IE
    }

  }); // $(document).ready(function()
}); // $(function()