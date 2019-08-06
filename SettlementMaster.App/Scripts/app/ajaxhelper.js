
function Ajax() {

    // var pref = new ApplicationPreference();
    // Utility to make Ajax Request
    this.send = function (reqObj) {
        var defer = $.Deferred();
        $.ajax({
            url: reqObj.url,
            dataType: "json",
            type: reqObj.type,
            contentType: reqObj.contentType,
            data: reqObj.data,
            cache: false,
            headers: reqObj.headers,
            beforeSend: function () {
                if (this.send != null)
                    this.send.abort()
            },
           // timeout: 30000,
            success: function (response) {
                defer.resolve(response);
            },
            error: function (xhr, status, err) {
                defer.reject(xhr, status, err);
            }
        });

        return defer.promise();
    };

 
}


