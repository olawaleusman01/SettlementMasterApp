$(function() {
    window.history.forward(-1);
    window.onload = function () {
        window.history.forward();
    };

    window.onunload = function () {
        null;
    };
    $('#side-menu').metisMenu();

});
$(function () {
    $(document).ajaxError(function (e, xhr) {
       // alert(xhr.status);
        if (xhr.status == 401) {
            // var resp = xhr.location;
            //  alert(resp);
            window.location = BaseUrl() + 'Account/Login';
        }
    })
});
//Loads the correct sidebar on window load,
//collapses the sidebar on window resize.
// Sets the min-height of #page-wrapper to window size
$(function() {
    $(window).bind("load resize", function() {
        topOffset = 50;
        width = (this.window.innerWidth > 0) ? this.window.innerWidth : this.screen.width;
        if (width < 768) {
            $('div.navbar-collapse').addClass('collapse');
            topOffset = 100; // 2-row-menu
        } else {
            $('div.navbar-collapse').removeClass('collapse');
        }

        height = ((this.window.innerHeight > 0) ? this.window.innerHeight : this.screen.height) - 1;
        height = height - topOffset;
        if (height < 1) height = 1;
        if (height > topOffset) {
            $("#page-wrapper").css("min-height", (height) + "px");
        }
    });

    var url = window.location;
    var element = $('ul.nav a').filter(function() {
        return this.href == url || url.href.indexOf(this.href) == 0;
    }).addClass('active').parent().parent().addClass('in').parent();
    if (element.is('li')) {
        element.addClass('active');
    }
    $('#logout').on('click', function () {
        if (window.confirm('Are you sure you want to Log Out from this Application?')) {
            var $reqLogin = {
                url: BaseUrl() + 'Account/LogOff',

                data: null,
                type: 'POST',
                contentType: "application/x-www-form-urlencoded"
            };
            // alert($('#formUsers').serialize());
            // loaderSpin(true);
            // disableButton(true);
            var ajax = new Ajax();
            ajax.send($reqLogin).done(function (response) {
                //loaderSpin(false);
                //disableButton(false);
                if (response.RespCode === 0) {
                    window.location.href = BaseUrl() + 'Account/Login';
                }


            }).fail(function (xhr, status, err) {
                //loaderSpin(false);
                //disableButton(false);
                displayModalNoty('alert-warning', 'No network connectivity. Please check your network connections.', true);

            });
        }
    });
    $('.datepicker').datepicker({ autoclose: true, format: 'dd-mm-yyyy' });
     $('.datepicker_default').datepicker({ autoclose: true, format: 'dd-mm-yyyy',defaultDate: new Date() });
    $('.select2').select2({
        theme: "classic"
    });
    $('.select2N').select2({
        //theme: "classic"
    });
    $(".datepickeryear").datepicker({
        autoclose: true,
        format: " yyyy",
        viewMode: "years",
        minViewMode: "years"
    });
    $(".datepickermonthyear").datepicker({
        autoclose: true,
        format: "mm-yyyy",
        viewMode: "months",
        minViewMode: "months"
    });
    //Bootstrap tooltip

    $("body").tooltip({ selector: '[data-toggle="tooltip"]', container: "body" });
    //END Bootstrap tooltip

    //Bootstrap Popover

    $("[data-toggle=popover]").popover();
    $(".popover-dismiss").popover({ trigger: 'focus' });
    //END Bootstrap Popover
   // $('.datepicker').datepicker({ dateFormat: 'yy-mm-dd' });
});
//$("#dialog-message").dialog({
//    modal: true,
//    buttons: {
//        Ok: function () {
//            $(this).dialog("close");
//        }
//    }
//});

function loaderSpin(visible) {
    // alert('loader');
    var $loader = $('.loader');
    if (visible === true) {
        $loader.show();
    }
    else {
        $loader.hide();
        //setTimeout(function () {
        //    $loader.hide();
        //}, 10);
    }
}
function loaderSpin2(visible) {
    // alert('loader');
    var $loader = $('.loader2');
    if (visible === true) {
        $loader.show();
    }
    else {
        $loader.hide();
        //setTimeout(function () {
        //    $loader.hide();
        //}, 10);
    }
}
function BaseUrl() {
   // alert( $('base').attr('href'));
    return $('base').attr('href');
    // return '/CliqBEChannel';
}
function displayNoty(css, msg,sticky) {
    // alert('loader normal noty');
    //var html = '<button type="button" class="close" data-dismiss="alert" aria-label="Close">' +
    //                        '<span aria-hidden="true">×</span>' +
    //                    '</button>';
    var html = msg;
    var $noty = $('.noty');
    var $notyMsg = $('.notyMsg');

    $noty.addClass(css);
    $notyMsg.text(html);
    if (sticky) {
        $noty.fadeIn('slow');
    }
    else {
        $noty.fadeToggle('slow');
        setTimeout(function () {
            $noty.fadeToggle('slow');
        }, 10000);
    }
}
function displayModalNoty(css, msg,sticky) {
    var html = msg;
    var $noty = $('.notyModal');
    var $notyMsg = $('.notyModalMsg');

    $noty.addClass(css);
    $notyMsg.text(html);
    if (sticky) {
        $noty.fadeIn('slow');
    }
    else {
        $noty.fadeToggle('slow');
        setTimeout(function () {
            $noty.fadeToggle('slow');
        }, 10000);
    }
}
function displayDialogNoty(title, msg) {
    //  alert('loader modal noty');
    //var html = '<button type="button" class="close" data-dismiss="alert" aria-label="Close">' +
    //                        '<span aria-hidden="true">×</span>' +
    //                    '</button>';
    var html = msg;
    var $noty = $('#dialog-message');
    var $notyMsg = $('.notyDialogMsg');

    $noty.attr('title', title);
    $noty.html(html);
    $("#dialog-message").dialog({
        modal: true,
        buttons: {
            Ok: function () {
                $(this).dialog("close");
            }
        }
    });
}
function disableButton(disable) {
    // alert('loader');
    var $btn_disable = $('.btn_disable');
    if (disable === true) {
        $btn_disable.attr('disabled', 'disabled');
    }
    else {
        $btn_disable.removeAttr('disabled');
        //setTimeout(function () {
        //    $loader.hide();
        //}, 10);
    }
}
function disableButton2(disable,id) {
    // alert('loader');
    var $btn_disable = $('#' + id);
    if (disable === true) {
        $btn_disable.attr('disabled', 'disabled');
    }
    else {
        $btn_disable.removeAttr('disabled');
        //setTimeout(function () {
        //    $loader.hide();
        //}, 10);
    }
}
function showPageLoader(shown) {
    var left = parseInt(($(window).width() - 50) / 2);
    var top = parseInt(($(window).height() - 96) / 2);
    $(".loader").css({
        "top": top + "px",
        "left": left + "px"
    });
    var $loader = $("#page-loader");
    if (shown) {
        //show loader
        $loader.show();
    } else {
        //hide loader
        setTimeout(function () {
            $loader.hide();
        }, 10);

    }
}
function addProccessDiv(msg, s) {

    $(".processing").remove();
    if (s != null) bg = ' background-color: green;'; else bg = '';
    var div = '<div class="processing" style="display: none; z-index: 99999999;' + bg + '">' + spinner + msg + '</div>';
    $("body").prepend(div);
    var bodyWidth = $(window).width();
    var divWidth = $(".processing").width();
    var left = parseInt((bodyWidth - divWidth) / 2);
    $(".processing").css("left", left + "px").fadeIn();
}
var spinner = '<img src="http://www.asslnigeria.com/tcn/images/mc_popup_loading.gif" width="16" height="16" class="spinner margin-right-10" style="display:inline-block; margin-right:10px">';
function startProcessing(msg) {
    msg = msg ? msg : 'Processing...';
    addProccessDiv(msg, null);
}
function endProcessing(msg, s) {
    $(".processing").remove().remove();
    if (msg) {
        addProccessDiv(msg, s);
        var interval = setInterval(function () {
            $(".processing").fadeOut("slow").remove();
            clearInterval(interval);
        }, 5000);
    } else {
        $(".processing").fadeOut("slow").remove();
    }
    return;
}

function fix_chars2(textBox) {
    //textBox.value = textBox.value.replace(/[@'&_,%`"*#|<>;]/g, "");
    var strVal;
    var strVal1;
    var strVal2;
    var dot;
    var i;
    var strComma;
    strVal2 = "";
    strComma = "";
    strVal1 = "";
    strVal = textBox;
    dot = 0;
    for (i = 0; i < strVal.length; i++) {
        // alert(i);
        if (strVal.substring(i, i + 1).indexOf('.') > -1) {
            dot = dot + 1;
        }
        if ((strVal.substring(i, i + 1).indexOf('0') > -1) || (strVal.substring(i, i + 1).indexOf("1") > -1) || (strVal.substring(i, i + 1).indexOf("2") > -1) || (strVal.substring(i, i + 1).indexOf("3") > -1) || (strVal.substring(i, i + 1).indexOf("4") > -1) || (strVal.substring(i, i + 1).indexOf("5") > -1) || (strVal.substring(i, i + 1).indexOf("6") > -1) || (strVal.substring(i, i + 1).indexOf("7") > -1) || (strVal.substring(i, i + 1).indexOf("8") > -1) || (strVal.substring(i, i + 1).indexOf("9") > -1) || ((strVal.substring(i, i + 1).indexOf('.') > -1) && dot <= 1)) {
            strVal1 = strVal1 + strVal.substring(i, i + 1)
        }
    }
    if ((strVal1.indexOf('.') == 0)) {
        strVal1 = "0" + strVal1;
    }
    if (strVal1.indexOf('.') > 0) {
        if (((strVal1.length) - (strVal1.indexOf('.') + 1)) > 2) {
            strVal1 = strVal1.substring(0, strVal1.indexOf('.') + 3);
        }
    }

    strVal = "";
    if (strVal1.indexOf('.') != -1) {

        strVal = strVal1.substring(strVal1.indexOf('.'), strVal1.indexOf('.') + 3);
        strVal1 = strVal1.substring(0, strVal1.indexOf('.'));
    }
    //	    	        alert(strVal1.indexOf('.'));

    while (strVal1.length > 0) {
        if (strVal1.length > 3) {
            strVal2 = strVal1.substring(strVal1.length - 3, strVal1.length) + strComma + strVal2;
            strVal1 = strVal1.substring(0, strVal1.length - 3);
            strComma = ",";
        }
        else {
            strVal2 = strVal1 + strComma + strVal2;
            strVal1 = "";
        }
    }

    //	    	    if (strVal.length>0){
    //	    	    strVal= strVal;
    //	    	    }

    if (strVal2.indexOf('.') > 0) {
        strVal2 = strVal2.substring(0, strVal2.indexOf('.'));
        // alert(strVal2);
    }
    return strVal2 + strVal;
  //  textBox.value = strVal2 + strVal;
}


$('#displayMenu').click(function (e) {
    e.preventDefault();
    var key = $(this).val();
    if (key == 1) {
        toggleMenu(true);
        $(this).val('0');
    }
    else {
        toggleMenu(false);
        $(this).val('1');
    }
});
function toggleMenu(stat) {
    if (stat) {
        $('.sidebar').css('display', 'none');
        $('#page-wrapper').css('margin-left', '0px');
    }
    else {
        $('.sidebar').css('display', 'block');
        $('#page-wrapper').css('margin-left', '250px');
    }
}