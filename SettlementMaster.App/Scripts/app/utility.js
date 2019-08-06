

function GetRoles(roleid) {
  //  alert('here');
    var $reqLogin = {
        url: BaseUrl() + 'Roles/RoleList',

        data: null,
        type: "Get",
        contentType: "application/json"
    };

    var ajax = new Ajax();
    ajax.send($reqLogin).done(function (response) {
       // alert(response.data);
        var exist = false;
        if (response.data.length > 0) {
            $("#RoleId").empty();
            $("#RoleId").append("<option value=''> --Select a Role-- </option>");
            for (var i = 0; i < response.data.length; i++) {
                $("#RoleId").append("<option value='" + response.data[i].RoleId + "'>" +
                response.data[i].RoleName + "</option>");
                if (roleid == response.data[i].RoleId)
                {
                    exist = true;
                }
            }
            if (exist)
            {
                $("#RoleId").val(roleid);
            }
            else {
                $("#RoleId").val("");
            }

        }
       // return response.data;
    }).fail(function (xhr, status, err) {
        return null;
       
    });

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
        alert(strVal2);
    }

    //textBox.value = strVal2 + strVal;
    return strVal2 + strVal;
}