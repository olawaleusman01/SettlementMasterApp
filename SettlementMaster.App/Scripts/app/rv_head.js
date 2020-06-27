$(document).ready(function () {
    /// GetRoles(1);

    var col;
    //var table2 = $('.datatable').DataTable(
    //);
    // var cnt = 0;
    var menuId = $('#menuId').val();
    //alert(menuId);
    var gCode = $('#GROUPCODE').val();
    //alert(gCode);
    var key = $('#ITBID').val();
    //alert(key);
    var url = BaseUrl() + 'RevenueHead/';
    var table2 = $('.datatable').DataTable({
        ajax: url + "RvGroupList",
        columns: [
            {
                data: null,
                className: "center_column",
                //defaultContent: '<a class="btn btn-primary btn-xs editor_edit" data-key="' + data.RoleId + '"><i class="fa fa-edit"></i></a>' +
                //    '  <a class="btn btn-warning btn-xs editor_remove" data-key="' + data.RoleId + '"><i class="fa fa-trash-o"></i></a>',
                render: function (data, type, row) {
                    // Combine the first and last names into a single table field

                    var encodedUrl = url + 'Add/' + data.ITBID + '?m=' + encodeURIComponent(menuId);
                    //alert(encodedUrl);
                    var html = '<a class="btn btn-primary btn-xs editor_edit" href="' + encodedUrl + '"><i class="fa fa-edit"></i></a>';
                    return html;
                }
            },
            { data: "GROUPCODE" },
            { data: "GROUPNAME" },
            { data: "MERCHANTID" },
            { data: "MERCHANTNAME" },
            { data: "DEPOSIT_ACCOUNTNO" },
            //{ data: "DEPOSIT_ACCTNAME" },
            { data: "STATUS" },
        ]
    });
    if (key > 0) {
        var table3 = $('#tableRvHead1').DataTable({
            ajax: url + "RvHeadList/" + gCode,
            columns: [
                {
                    data: null,
                    className: "center_column",
                    //defaultContent: '<a class="btn btn-primary btn-xs editor_edit" data-key="' + data.RoleId + '"><i class="fa fa-edit"></i></a>' +
                    //    '  <a class="btn btn-warning btn-xs editor_remove" data-key="' + data.RoleId + '"><i class="fa fa-trash-o"></i></a>',
                    render: function (data, type, row) {
                        // Combine the first and last names into a single table field

                        //var encodedUrl = url + 'Add/' + data.ITBID + '?m=' + encodeURIComponent(menuId);
                        //alert(encodedUrl);
                        var html = '<a class="btn btn-primary btn-xs editor_edit" data-key="' + data.ITBID + '"><i class="fa fa-edit"></i></a>';
                        return html;
                    }
                },
               
                { data: "CODE" },
                { data: "DESCRIPTION" },
                //{ data: "PAYMENTITEMID" },
                { data: "BANKACCOUNT" },
                { data: "ACCT_NAME" },
                { data: "BANK_NAME" },
                { data: "FREQUENCY_DESC" },
                //{ data: "STATUS" },
            ]
        });
        var table4 = $('#tableDebitAcct').DataTable({
            ajax: url + "RvDrAcctList/" + gCode,
            columns: [
               
                { data: "MERCHANTID" },
                { data: "RVGROUPCODE" },
                { data: "AGENT_CODE" },
                { data: "DEPOSIT_BANKCODE" },
                { data: "BANKNAME" },
                { data: "DR_ACCOUNTNO" },
                { data: "DEPOSIT_ACCTNAME" },
                //{ data: "STATUS" },
            ]
        });
    }
    validateForm();
    function validateForm() {

        var validator = $("#formRvGroup").validate({
            rules: {
               // GROUPCODE: "required",
                GROUPNAME: "required",
                MERCHANTID: "required",
                SET_DAYS: {
                    required:true,
                    number:true,
                },
                STATUS: "required"
            },
            messages: {

                // equalTo: "Password must be Equal",
                Name: {
                    required: "Please Enter Last Name"
                },
            },
            submitHandler: function () {
                //console.log($('#formRvGroup').serialize());
                //return;
                loaderSpin2(true);
                form.submit();
                //  e.preventDefault();
                // alert('success');
                //    var btn = $('#btnSave').val();
                //    var urlTemp;
                //    var postTemp;
                //    var event;
                //    if (btn == "1") {

                //        urlTemp = url + 'Add';
                //        postTemp = 'post';
                //        $('#ITBID').val(0);
                //        event = 'new';
                //    }
                //    else {
                //        urlTemp = url + 'Add';
                //        postTemp = 'post';
                //        event = 'modify';
                //    };
                //    //  alert(urlTemp);
                //    var $reqLogin = {
                //        url: urlTemp,

                //        data: $('#formParty').serialize(),
                //        type: postTemp,
                //        contentType: "application/x-www-form-urlencoded"
                //    };
                //    console.log($('#formParty').serialize());
                //    return;
                //    loaderSpin2(true);
                //    disableButton(true);
                //    var ajax = new Ajax();
                //    ajax.send($reqLogin).done(function (response) {
                //        loaderSpin2(false);
                //        disableButton(false);
                //        if (response.RespCode === 0) {
                //            //new Util().clearform('#formParty');

                //            //$('a.editor_return').click();
                //            displayDialogNoty('Notification', response.RespMessage);
                //        }
                //        else {
                //            // alert(response.RespMessage)
                //            // $('a.editor_return').click();
                //            displayDialogNoty('Notification', response.RespMessage);

                //        }


                //    }).fail(function (xhr, status, err) {
                //        loaderSpin2(false);
                //        disableButton(false);
                //        displayDialogNoty('Notification', 'No network connectivity. Please check your network connections.', true);
                //    });


            }
        });

    }
    $(document).on('click', '#tableRvHead1 a.editor_edit', function (e) {
        e.preventDefault();
        $('#notyMsc').hide();
        var menuId = $('#m').val();
        // alert(menuId);
        var editLink = $(this).attr('data-key');
        // alert(editLink);
        var urlP = url + 'EditRvHeadLocal/' + editLink;

        //console.log(urlP);
        loaderSpin2(true);
        $('#myModalAcct').load(
            urlP, function () {
                loaderSpin2(false);
                $('#myModalAcct').modal({ backdrop: 'static', keyboard: false });
                validateAcctForm();
            });
    });
    $(document).on('click', '#btnAddAcct', function () {
        $('#notyMsc').hide();
        //var menuId = $('#m').val();
        var mid = $('#MERCHANTID').val();
        if (mid === '')
        {
            displayDialogNoty('Notification', 'Please Select a Merchant');
            return;
        }
        var urlP = url + 'AddRvHead/' + mid;
         //console.log(urlP);
        loaderSpin2(true);
        $('#myModalAcct').load(
            urlP, function () {
                loaderSpin2(false);
                $('#myModalAcct').unbind().modal({ backdrop: 'static', keyboard: false });
                validateAcctForm();

            });

    });
    $(document).on('click', '#tableAcct a#edit', function (e) {
        e.preventDefault();
        $('#notyMsc').hide();
        var menuId = $('#m').val();
        // alert(menuId);
        var editLink = $(this).attr('data-key');
        // alert(editLink);
        var urlP = url + 'EditRvHead/' + editLink + '?m=' + encodeURIComponent(menuId);

        //console.log(urlP);
        loaderSpin2(true);
        $('#myModalAcct').load(
            urlP, function () {
                loaderSpin2(false);
                $('#myModalAcct').modal({ backdrop: 'static', keyboard: false });
                validateAcctForm();
            });
    });
    $(document).on('click', '#tableAcct a#delete', function (e) {
        e.preventDefault();
        if (confirm('Are you sure you want to delete record?')) {
            //$('#notyMsc').hide();
            var menuId = $('#m').val();
            // alert(menuId);
            var editLink = $(this).attr('data-key');
            // alert(editLink);
            var urlP = url + 'DeleteAcct/' + editLink + '?m=' + encodeURIComponent(menuId);
            //var acq = $('#CBN_CODE').val();

            //console.log(urlP);
            loaderSpin2(true);
            $.get(
                urlP, function (response) {
                    loaderSpin2(false);
                    if (response.RespCode == 0) {
                        //console.log(response.data_msc);
                        $('#divAcct').html(response.data_html);
                    }
                    else {
                        displayDialogNoty('Notification', response.RespMessage);
                    }
                });
        }
    });
    $(document).on('click', '#tableAcct a#undo', function (e) {
        e.preventDefault();
        //if (confirm('Are you sure you want to delete record?')) {
        //$('#notyMsc').hide();
        var menuId = $('#m').val();
        // alert(menuId);
        var editLink = $(this).attr('data-key');
        // alert(editLink);
        var urlP = url + 'UndoAcct/' + editLink + '?m=' + encodeURIComponent(menuId);
        //var acq = $('#CBN_CODE').val();

        //console.log(urlP);
        loaderSpin2(true);
        $.get(
            urlP, function (response) {
                loaderSpin2(false);
                if (response.RespCode == 0) {
                    //console.log(response.data_msc);
                    $('#divAcct').html(response.data_html);
                }
                else {
                    displayDialogNoty('Notification', response.RespMessage);
                }
            });
        //}
    });
    function validateAcctForm() {

        var validator = $("#formRvCode").validate({
            rules: {
                CODE: "required",
                DESCRIPTION: "required",
                PAYMENTITEMID: {
                    required: true,
                    number: true
                },
                //ACCOUNT_ID: "required",
            },
            messages: {

                // equalTo: "Password must be Equal",
                Name: {
                    required: "Please Enter Last Name"
                },


            },
            submitHandler: function () {
                //  e.preventDefault();
                // alert('success');
                //var btn = $('#btnSave').val();
                var urlTemp;
                var postTemp;
                var event;
                urlTemp = url + 'AddRvHead';
                postTemp = 'post';
                //if (btn == "1") {

                //    urlTemp = url + 'Create';
                //    postTemp = 'post';
                //    $('#RoleId').val(0);
                //    event = 'new';
                //}
                //else {

                //    urlTemp = url + 'Create';
                //    //postTemp = 'put';
                //    postTemp = 'post';
                //    event = 'modify';
                //};
                //  alert(urlTemp);

                console.log($('#formRvCode').serialize());
                var $reqLogin = {
                    url: urlTemp,

                    data: $('#formRvCode').serialize(),
                    type: postTemp,
                    contentType: "application/x-www-form-urlencoded"
                };
                $('#MID').val($('#MERCHANTID').val());
                //console.log($('#formRvCode').serialize());
                //;
                loaderSpin(true);
                disableButton2(true, 'btnAddAcctGrid');
                var ajax = new Ajax();
                ajax.send($reqLogin).done(function (response) {
                    loaderSpin(false);
                    disableButton2(false, 'btnAddAcctGrid');
                    if (response.RespCode === 0) {
                        $('#myModalAcct').modal('hide');
                        $('#divAcct').html(response.data_html);
                        //console.log(JSON.stringify(response.data));
                        //console.log(JSON.stringify(response.data_msc));
                        
                    }
                    else {
                        $('#notyMsc').show();
                        $('#notyMscMsg').html(response.RespMessage);
                        // displayModalNoty('Notification', response.RespMessage);
                    }
                }).fail(function (xhr, status, err) {
                    loaderSpin(false);
                    disableButton2(false, 'btnAddAcctGrid');
                    displayModalNoty('Notification', 'No network connectivity. Please check your network connections.', true);
                });
            }
        });

    }

    $(document).on('change', '#PARTYTYPE_CODE', function () {
        var selected = $(this).val();;

        var urlP = url + 'GetIfCodeRequired/' + selected;
        //console.log(urlP)
        loaderSpin2(true);
        $.get(
            urlP, function (response) {
                loaderSpin2(false);
                if (response.RespCode == 0) {
                    //console.log(response.CodeRequired);
                    if (response.CodeRequired) {
                        $('#pCode').css('display', 'block');
                        $('#PARTYCODEREQUIRED').val('Y');
                    }
                    else {
                        $('#pCode').css('display', 'none');
                        $('#PARTYCODEREQUIRED').val('N');
                    }
                }
                else {
                    //displayDialogNoty('Notification', response.RespMessage);
                }
            });

    });
    $(document).on('change', '#MERCHANTID', function () {
        var selected = $(this).val();
        var urlP = url + 'GetMerchantAcct/' + selected;
        $("#ACCOUNT_ID").empty();

        $("#ACCOUNT_ID").append("<option value=''> --Select One-- </option>");
        console.log(urlP)
        loaderSpin2(true);
        $.get(
            urlP, function (response) {
                loaderSpin2(false);
                if (response.RespCode == 0) {
                    for (var i = 0; i < response.data.length; i++) {
                        $("#ACCOUNT_ID").append("<option value='" + response.data[i].ITBID + "'>" +
                        response.data[i].DEPOSIT_ACCOUNTNO + '-' + response.data[i].DEPOSIT_ACCTNAME + "</option>");

                    }
                }
                else {
                    //displayDialogNoty('Notification', response.RespMessage);
                }
            });
        $("#ACCOUNT_ID").val('').trigger('change');
    });

    //$(document).on('change', "#SET_INTL", function () {
    //    //alert('checked');
    //    if ($("#SET_INTL").is(":checked")) {
    //        $('.intlmsc').css('display', 'block');
    //    }
    //    else {
    //        $('.intlmsc').css('display', 'none');
    //    }
    //});
    $(document).on('change', '#SETTLEMENT_FREQUENCY', function () {
        var selected = $(this).val();
        var urlP = url + 'GetFrequency/' + selected;
        console.log(urlP)
        if (selected != '') {
            loaderSpin2(true);
            $.get(
                urlP, function (response) {
                    loaderSpin2(false);
                    if (response.RespCode == 0) {
                        if (response.data.CUSTOM) {
                            $('#divCustom').show();
                        }
                    }
                });
        }
        $('#divCustom').hide();
    });



    $(document).on('click', '#tblParty #btnAddRow', function () {

        var table = $('#tblParty'); //get the table
        //append the row to the table.

        var urlP = BaseUrl() + 'RevenueHead/' + 'AddSharingPartyRowView';
        //alert(urlP);
        if ($("#tblParty tfoot tr").length > 0) {
            alert('A new form row is already opened.');
            return;
        }
        loaderSpin(true);
        $.get(
            urlP, function (response) {
                loaderSpin(false);
                if (response.RespCode === 0) {
                    $("#tblParty tfoot").empty();
                    $("#tblParty tfoot").append(response.data_html);
                    $('.select2').select2({
                        theme: "classic",
                        width: '100%'
                    });
                }
                else {
                    displayDialogNoty('Notification', response.RespMessage);
                }
            });

    })
    $(document).on('click', '#tblParty #btnCancelPSharing', function () {
        var row = $(this).closest("tr");
        // var name = $("TD", row).eq(0).html();
        //Get the reference of the Table.
        var table = $("#tblParty")[0];
        //Delete the Table row using it's Index.
        table.deleteRow(row[0].rowIndex);

    });
    $(document).on("click", "#tblParty #btnAddPSharing", function () {
        //Reference the Name and Country TextBoxes.
        var $drpPartyId = $("#PartyId");
        var $txtPartyValue = $("#PartyValue");
        var $drpPartyAcctId = $("#PartyAccountId");

        ////Get the reference of the Table's TBODY element.
        //var tBody = $("#tblParty > TBODY")[0];

        ////Add Row.
        //var row = tBody.insertRow(-1);

        ////Add Name cell.
        //var cell = $(row.insertCell(-1));
        //cell.html(txtName.val());

        ////Add Country cell.
        //cell = $(row.insertCell(-1));
        //cell.html(txtCountry.val());

        ////Add Button cell.
        //cell = $(row.insertCell(-1));
        //var btnRemove = $("<input />");
        //btnRemove.attr("type", "button");
        //btnRemove.attr("onclick", "Remove(this);");
        //btnRemove.val("Remove");
        //cell.append(btnRemove);

        ////Clear the TextBoxes.
        //txtName.val("");
        //txtCountry.val("");
        var i = parseInt($("#last_count").val());
        //alert(i);
        var partyName = $("#PartyId option:selected").text();
        var partyId  = $("#PartyId").val();
        var acctId = $("#PartyAccountId").val();
        var acctName = $("#PartyAccountId option:selected").text();
        var partyValue = $("#PartyValue").val();

        if (partyId === '' || acctId === '' || partyValue === '') {
           
            alert('All fields are required.');
            return;
        }
        else if (isNaN(parseFloat(partyValue))) {
            alert('Party sharing value must be numeric');
            return;
        }
        var ptyExist = false;
        $("#tblParty tbody tr").each(function () {
            var row = $(this);
            var customer = {};
            extPtyId = row.find("td").eq(0).find($("[id$='PartyId']")).val();
            if (extPtyId == partyId) {
                alert('Party already exist.');
                ptyExist = true;
                return;
            }
        });
        if (ptyExist) {
            return;
        }
        var rowElement = '<tr>'
            + '<td> <label style="color:#333" id="lblPty">' + partyName + '</label><input type="hidden" id="ptyId" name="RevenueSharingPartys[' + i + '].PartyId"'
            + ' value="' + partyId + '" /> <input type="hidden" id="ptyId" name="RevenueSharingPartys[' + i + '].ItbId" value="" />'
            + '  <input type="hidden" name="RevenueSharingPartys.Index" value="' + i + '" /></td>'
            + '<td style="text-align:right"> <label style="color:#333" id="lblPtyValue">' + parseFloat(partyValue).toFixed(2) + '</label> <input type="hidden" id="ptyValue" name="RevenueSharingPartys[' + i + '].PartyValue" value="' + partyValue +'" /> </td>'
            + '<td> <label style="color:#333" id="lblPtyAccount">' + acctName + '</label> <input type="hidden" id="ptyAccountId" name="RevenueSharingPartys[' + i + '].PartyAccountId" value="' + acctId +'" /> </td>'
            + '<td>'
            //+ '<button type="button" class="btn btn-primary btn-xs" id="btnEditPSharing"><i class="fa fa-edit"></i></button>'
            + ' <button type="button" class="btn btn-danger btn-xs" id="btnDeletePSharing"><i class="fa fa-times"></i></button>'
            + '</td>'
            + '</tr>';
        
        $("#tblParty tbody").append(rowElement);
        $("#tblParty tfoot").empty();
        i += 1;

        $("#last_count").val(i);
    });

    $(document).on('click', '#tblParty #btnDeletePSharing', function () {
        var row = $(this).closest("tr");
        var partyName = $("td", row).eq(0).find('#lblPty').text();
        //Get the reference of the Table.
        if (confirm("Do you want to delete: " + partyName)) {
            var table = $("#tblParty")[0];
            //Delete the Table row using it's Index.
            table.deleteRow(row[0].rowIndex);
        }

    });

    $(document).on('change', '#tblParty #PartyId', function () {
        var urlP = url + 'GetPartyAccount/' + $(this).val();
        $.get(
            urlP, function (response) {
                //loaderSpin(false);
                if (response.RespCode === 0) {
                    $.each(response.data, function () {
                        $("#PartyAccountId").append($("<option     />").val(this.Code).text(this.Description));
                    });
                }
                else {
                    displayDialogNoty('Notification', response.RespMessage);
                }
            });
    });

    
});