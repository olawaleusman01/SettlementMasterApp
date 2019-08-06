
function Util() {

    // var pref = new ApplicationPreference();
    // Utility to make Ajax Request
    //this.send = function (reqObj) {
    //    var defer = $.Deferred();
    //    $.ajax({
    //        url: reqObj.url,
    //        dataType: "json",
    //        type: reqObj.type,
    //        contentType: reqObj.contentType,
    //        data: reqObj.data,
    //        cache: false,
    //        headers: reqObj.headers,
    //       // timeout: 30000,
    //        success: function (response) {
    //            defer.resolve(response);
    //        },
    //        error: function (xhr, status, err) {
    //            defer.reject(xhr, status, err);
    //        }
    //    });

    //    return defer.promise();
    //};

  this.clearform =  function(ele) {
       // alert("i got here");
        $(ele).find(':input').each
        (function () {
            switch (this.type) {
                case 'text':
                case 'email':
                case 'textarea':
                case 'single-text':
                case 'tel':
                case 'number':
                    $(this).val('');
                case 'radiobutton':
                case 'checkbox':
                    $(this).val = false;
                    break;
            }

            var tag = this.tagName.toLowerCase();
            if (tag == 'select') {
                this.selectedIndex = 0;
                // alert("succe");
            };



        });

    };
}


