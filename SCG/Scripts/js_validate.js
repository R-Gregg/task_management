// Labeling for Required Fields
$('.val_asterik').remove();
$('label').each(function (i, e) {
    var id = $(e).attr('for');
    var r = $('#' + id).attr('required');
    if (r === 'required') {
        $(e).prepend('<span class="val_asterik" style="display:inline-block; vertical-align:top; padding-right:5px; color:red;">*</span>');
    }
});

// Labeling for DatePicker
$('.dt_pckr').each(function (i, e) {
    var w = $(e).width();
    $(e).css({ 'display': 'inline-block', 'width': (w - 12) + 'px' }).after(' <i class="fa fa-calendar fa-fw fa-lg" aria-hidden="true"></i>');
    $('.fa-calendar').click(function () {
        $(this).prev('input').focus();
    });
});

// Strip to Number
function fStripToNumber(x) {
    rx = x.replace(/[^0-9.]/g, '');
    if (x.startsWith('-')) {
        rx = '-' + rx;
    }
    return Number(rx);
}

// Form - Phone Validation
$('[type="phone"]').change(function () {
    var p = $(this).val();
    p = fStripToNumber(p.replace(/\./g, ''));
    var arr = String(p).split("");
    arr = arr.reverse();
    var np = ''; // New Phone
    for (var i = 0; i < arr.length; i++) {
        if (i == 4) {
            np = arr[i] + '-' + np;
        } else if (i == 6) {
            np = ') ' + arr[i] + np;
        } else if (i == 9) {
            np = ' (' + arr[i] + np;
        } else {
            np = arr[i] + np;
        }
    }
    if (np != 0) {
        $(this).val(String(np).trim());
    } else {
        $(this).val('');
    }
});

// Form - Zip Validation
$('[type="zipcode"]').change(function () {
    var z = $(this).val();
    z = fStripToNumber(z);
    var arr = String(z).split("");
    var nz = ''; // New Zip
    for (var i = 0; i < arr.length; i++) {
        if (i == 5) {
            nz += '-' + arr[i];
        } else {
            nz += arr[i];
        }
    }
    if (nz != 0) {
        $(this).val(String(nz).trim());
    } else {
        $(this).val('');
    }
});

// Form - Currency Validation
function fValCurrency(c) {
    c = fStripToNumber(c);
    c = c.toFixed(2);
    return ('$' + c);
}
$('[type="currency"]').each(function (i, e) {
    var v = $(e).val();
    if (v != '') {
        var c = fValCurrency(v);
        $(e).val(c);
    }
});
$('[type="currency"]').change(function () {
    var v = $(this).val();
    if (v != '') {
        var c = fValCurrency(v);
        $(this).val(c);
    }
});
$('form').submit(function () {
    $(this).find('[type="currency"]').each(function (i, e) {
        var v = $(e).val();
        if (v != ''){
            var c = fStripToNumber(v);
            $(e).val(c);
        }
    });
});

// Form - Number Only Validation
$('[type="unit"]').change(function () { // Had to call it something different than 'number' because of bootstrap
    var n = $(this).val();
    n = fStripToNumber(n);
    $(this).val(n);
});

// Extra Stuff ================>

// Select Rel/Value
$('select').each(function (i, e) {
    var v = $(e).attr('rel');
    if (v != undefined) {
        $(e).val(v);
    }
});

// Remove "novalidate" 
$('form').removeProp('novalidate');
$('form').removeAttr('novalidate');
$('.text-danger, .field-validation-error').remove();