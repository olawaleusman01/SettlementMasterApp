$(document).ready(function () {
    /// GetRoles(1);

    var col;
    //var table2 = $('.datatable').DataTable(
    //);
    // var cnt = 0;
    var menuId = $('#menuId').val();
    //alert(menuId);
    //alert(gCode);
    var key = $('#ITBID').val();
    var url = BaseUrl() + 'ApprovalDetail/';
    var example = $('.datatable').DataTable({
        ajax: url + "GetApprovalDetail",
        columns: [
            {
                data: null,
                className: "center_column",
                //defaultContent: '<a class="btn btn-primary btn-xs editor_edit" data-key="' + data.RoleId + '"><i class="fa fa-edit"></i></a>' +
                //    '  <a class="btn btn-warning btn-xs editor_remove" data-key="' + data.RoleId + '"><i class="fa fa-trash-o"></i></a>',
                render: function (data, type, row) {
                    // Combine the first and last names into a single table field

                    var encodedUrl = url + 'ApprovalDetail/' + data.ITBID + '?m=' + encodeURIComponent(menuId);
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
    //if (key > 0) {
    //    var table3 = $('#tableRvHead1').DataTable({
    //        ajax: url + "RvHeadList/" + gCode,
    //        columns: [
    //            {
    //                data: null,
    //                className: "center_column",
    //                //defaultContent: '<a class="btn btn-primary btn-xs editor_edit" data-key="' + data.RoleId + '"><i class="fa fa-edit"></i></a>' +
    //                //    '  <a class="btn btn-warning btn-xs editor_remove" data-key="' + data.RoleId + '"><i class="fa fa-trash-o"></i></a>',
    //                render: function (data, type, row) {
    //                    // Combine the first and last names into a single table field

    //                    //var encodedUrl = url + 'Add/' + data.ITBID + '?m=' + encodeURIComponent(menuId);
    //                    //alert(encodedUrl);
    //                    var html = '<a class="btn btn-primary btn-xs editor_edit" data-key="' + data.ITBID + '"><i class="fa fa-edit"></i></a>';
    //                    return html;
    //                }
    //            },
    //            { data: "CODE" },
    //            { data: "DESCRIPTION" },
    //            { data: "BANKACCOUNT" },
    //            { data: "ACCT_NAME" },
    //            { data: "BANK_NAME" },
    //            { data: "FREQUENCY_DESC" },
    //            //{ data: "STATUS" },
    //        ]
    //    });
    //    var table4 = $('#tableDebitAcct').DataTable({
    //        ajax: url + "RvDrAcctList/" + gCode,
    //        columns: [
               
    //            { data: "MERCHANTID" },
    //            { data: "RVGROUPCODE" },
    //            { data: "AGENT_CODE" },
    //            { data: "DEPOSIT_BANKCODE" },
    //            { data: "BANKNAME" },
    //            { data: "DR_ACCOUNTNO" },
    //            { data: "DEPOSIT_ACCTNAME" },
    //            //{ data: "STATUS" },
    //        ]
    //    });
    //}
   
});