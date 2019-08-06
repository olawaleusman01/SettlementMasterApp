$(document).ready(function () {
    //alert(fix_chars2(1000.00));
    toggleMenu(true);
   // var ret = location.pathname.substring(location.pathname.lastIndexOf("/") + 1);
       var url = BaseUrl() + 'Report/';
   var qryStr = getParameterByName('FilterBy');
    if (qryStr)
    {
       // alert(qryStr);
        $('#FilterBy').val(qryStr);
        GetCardEnquiryGroup('', '', qryStr);
    }
    var cnt = 0;
    var col;

    var table2 = $('.datatable').DataTable({
        "pageLength": 100,
        "ordering": false,
        "lengthMenu": [[100, 200, 500,1000,5000, -1], [100, 200, 500,1000,5000, "All"]],
        ajax:null, // url + "TranxLogList",
        columns: [
            {data:null},
            { data: "refenceId" },
            { data: "stan" },
            { data: "TranCode" },
            { data: "tranx_date" },
              
           // { data: "tranAmt" },
            { data: "draccountnumber" },
            { data: "fullname" },
            { data: "pan" },
         
             {
                 data: null,
                 className: "center_column",
                 //defaultContent: '<a class="btn btn-primary btn-xs editor_edit" data-key="' + data.ItbId + '"><i class="fa fa-edit"></i></a>' +
                 //    '  <a class="btn btn-warning btn-xs editor_remove" data-key="' + data.ItbId + '"><i class="fa fa-trash-o"></i></a>',
                 render: function (data, type, row) {
                     // Combine the first and last names into a single table field
                     var html = fix_chars2(data.tranAmt);
                     return html;
                 }
             },
                {
                    data: null,
                    className: "center_column",
                    //defaultContent: '<a class="btn btn-primary btn-xs editor_edit" data-key="' + data.ItbId + '"><i class="fa fa-edit"></i></a>' +
                    //    '  <a class="btn btn-warning btn-xs editor_remove" data-key="' + data.ItbId + '"><i class="fa fa-trash-o"></i></a>',
                    render: function (data, type, row) {
                        // Combine the first and last names into a single table field
                        var html = fix_chars2(data.surcharge);
                        return html;
                    }
                },
             
            { data: "responseCode" },
            { data: "responseDesc" },
            { data: "Narration" },
           
        ],
      
    
    });

    table2.on('order.dt search.dt', function () {
        table2.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
            cell.innerHTML = i + 1;
        });
    }).draw();
    $('#btnSearch').on('click', function (e) {
        e.preventDefault();
        var fromDate = $('#FromDate').val();
        var toDate = $('#ToDate').val();
        var tranxType = $('#FilterBy').val();
        if (fromDate === '') {
            displayDialogNoty('Notification', 'From Date is required.');
            return;
        }
        if (toDate === '') {
            displayDialogNoty('Notification', 'TO Date is required.');
            return;
        }
        GetCardEnquiryGroup(fromDate, toDate,tranxType);
     
    });

    function GetCardEnquiryGroup(fromDate, toDate,tranxType) {


        var $reqLogin = {
            url: url + 'TranxLogList?FromDate=' + fromDate + '&ToDate=' + toDate  + '&FilterBy=' + tranxType,

            data: null,
            type: "Get",
            contentType: "application/json"
        };
        console.log($reqLogin.url);
        loaderSpin2(true);
        var ajax = new Ajax();
        ajax.send($reqLogin).done(function (response) {
            loaderSpin2(false);
           
            table2.clear().draw();
            var data = response.data;
            for (var i = 0; i < data.length; i++) {
                //alert(data[i]);
                table2.row.add(data[i]).draw();
            }
        }).fail(function (xhr, status, err) {

            loaderSpin2(false);
        });

    }
    function getParameterByName(name, url) {
    if (!url) {
      url = window.location.href;
    }
    name = name.replace(/[\[\]]/g, "\\$&");
    var regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)"),
        results = regex.exec(url);
    if (!results) return null;
    if (!results[2]) return '';
    return decodeURIComponent(results[2].replace(/\+/g, " "));
}
    // var cnt = 0;
   
    //var table2 = $('.datatable').DataTable({
    //    ajax: url + "RoleList",
    //    columns: [

    //        { data: "Name" },
    //        { data: "Status" },

    //        {
    //            data: null,
    //            className: "center_column",
    //            //defaultContent: '<a class="btn btn-primary btn-xs editor_edit" data-key="' + data.ItbId + '"><i class="fa fa-edit"></i></a>' +
    //            //    '  <a class="btn btn-warning btn-xs editor_remove" data-key="' + data.ItbId + '"><i class="fa fa-trash-o"></i></a>',
    //            render: function (data, type, row) {
    //                // Combine the first and last names into a single table field
    //                var html = '<a class="btn btn-primary btn-xs editor_edit" data-key="' + data.ItbId + '"><i class="fa fa-edit"></i></a>' +
    //                '  <a class="btn btn-warning btn-xs editor_remove" href="' + url + 'RolePriviledge/' + data.ItbId + '"><i class="fa fa-check"></i> Assign Priviledge</a>';
    //                return html;
    //            }
    //        }
    //    ]
    //});


});