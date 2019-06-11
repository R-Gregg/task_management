//Snowflake Falling Function
$(document).ready(function () {
  $('.quadrant').hover(function () {
    $(this).css({
      'transform': 'scale(1.2)', 'z-index': '4', 'box-shadow': 'rgba(0, 0, 0, 0.9) 10px 5px 30px'
    });
  }, function () {
    $(this).css({ 'transform': 'scale(1.0)', 'z-index': '0', 'box-shadow': 'rgba(0, 0, 0, 0) 5px 0px 30px' });
  });

  //$('.quadrant-center').hover(function () {
  //  $(this).css({
  //    'transform': 'scale(1.2)', 'box-shadow': 'rgba(0, 0, 0, 0.9) 10px 5px 30px'
  //  });
  //}, function () {
  //  $(this).css({ 'transform': 'scale(1.0)', 'box-shadow': 'rgba(0, 0, 0, 0) 5px 0px 30px' });
  //});
});

// Snow Falling
function fallingSnow() {
  var $snowflakes = $(),
    createSnowflakes = function () {
      var qt = 40;
      for (var i = 0; i < qt; ++i) {
        var $snowflake = $('<div class="snowflakes"></div>');
        $snowflake.css({
          'left': (Math.random() * $('.jumbotron').width()) + 'px',
          'top': (- Math.random() * $('.jumbotron').height()) + 'px'
        });
        // add this snowflake to the set of snowflakes
        $snowflakes = $snowflakes.add($snowflake);
      }
      $('.jumbotron').prepend($snowflakes);
    },

    runSnowStorm = function () {
      $snowflakes.each(function () {

        var singleAnimation = function ($flake) {
          $flake.animate({
            top: "700px",
            opacity: "0",
          }, Math.random() * -2500 + 5000, function () {
            // this particular snow flake has finished, restart again
            $flake.css({
              'left': (Math.random() * $('.jumbotron').width()) + 'px',
              'top': (- Math.random() * $('.jumbotron').height()) + 'px',
              'opacity': 1
            });
            singleAnimation($flake);
          });
        };
        singleAnimation($(this));
      });
    };

  createSnowflakes();
  runSnowStorm();
}
        //fallingSnow();