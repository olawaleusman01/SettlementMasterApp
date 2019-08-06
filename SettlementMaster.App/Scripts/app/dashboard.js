$(document).ready(function () {
  

    var col;
    var url = BaseUrl() + 'Report/';
    BindDashboard();

    function BindDashboard() {


        var $reqLogin = {
            url: url + 'DashboardList',

            data: null,
            type: "Get",
            contentType: "application/json"
        };
        //loaderSpin2(true);
        var ajax = new Ajax();
        ajax.send($reqLogin).done(function (response) {
            // loaderSpin2(false);
           // alert(JSON.stringify(response.data));
            $('#tranxcount').html(response.data.TranxCount);
            $('#balcount').html(response.data.BalEnquiryCount);
            $('#revcount').html(response.data.ReversalCount);
            $('#smscount').html(response.data.SmsLogCount);
            BindDoughnotGraph(response.data);
            BindGraph(response.data.TranxMorix);
           // alert(response.data.TranxMorix[0]);
            var data = response.data.TranxLog;
           // alert(data.length);
           
            
                $("#example1 tbody").empty();
                if (data != null) {
                    var rw = '';


                    // alert('am not null');
                    for (var i = 0; i < data.length; i++) {
                       // alert('loop');
                        rw += '<tr>'
                               // + '<td>' + (i + 1) + '</td>'
                                + '<td>' + data[i].refenceId + '</td>'
                                + '<td>' + data[i].stan + '</td>'
                                + '<td>' + data[i].TranCode + '</td>'
                                + '<td>' + data[i].tranx_date + '</td>'
                                + '<td>' + fix_chars2(data[i].tranAmt) + '</td>'
                                + '<td>' + fix_chars2(data[i].surcharge) + '</td>'
                                + '<td>' + data[i].draccountnumber + '</td>'
                                + '<td>' + data[i].fullname + '</td>'
                                + '<td>' + data[i].pan + '</td>'
                                + '<td>' + data[i].responseCode + '</td>'
                                + '<td>' + data[i].responseDesc + '</td>'
                                + '<td>' + data[i].Narration + '</td>'
                                + '</tr>';
                        // alert(rw);
                    };
                    $("#example1 tbody").html(rw);
                  
                }
            
            //table2.clear().draw();
            //var data = response.data;
            //for (var i = 0; i < data.length; i++) {
            //    //alert(data[i]);
            //    table2.row.add(data[i]).draw();
            //}
        }).fail(function (xhr, status, err) {

          //  loaderSpin2(false);
        });

    }
    function BindDoughnotGraph(data)
    {
 
        Morris.Donut({
            element: 'morris-donut-chart',
            data: [{
                label: "ATM Transactions",
                value: data.TranxCount
            }, {
                label: "ATM Reversals",
                value: data.ReversalCount
            }, {
                label: "ATM Balance Enquiry",
                value: data.BalEnquiryCount
            }],
            resize: true,
            colors: ['#5cb85c', '#d9534f', '#337ab7']
        });
    }
    function BindGraph(data) {
        var months = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];

        Morris.Line({
            element: 'morris-area-chart',
            data: data,
            //[{
            //    m: '2015-01', // <-- valid timestamp strings
            //    a: 0,
            //    b: 270,
            //    c: 150
            //}, {
            //    m: '2015-02',
            //    a: 54,
            //    b: 256,
            //    c: 150
            //}, {
            //    m: '2015-03',
            //    a: 243,
            //    b: 334,
            //    c: 150
            //}, {
            //    m: '2015-04',
            //    a: 206,
            //    b: 282
            //}, {
            //    m: '2015-05',
            //    a: 161,
            //    b: 58,
            //    c: 150
            //}, {
            //    m: '2015-06',
            //    a: 187,
            //    b: 0,
            //    c: 150
            //}, {
            //    m: '2015-07',
            //    a: 210,
            //    b: 0,
            //    c: 150
            //}, {
            //    m: '2015-08',
            //    a: 204,
            //    b: 0,
            //    c: 150
            //}, {
            //    m: '2015-09',
            //    a: 224,
            //    b: 0,
            //    c: 150
            //}, {
            //    m: '2015-10',
            //    a: 301,
            //    b: 0,
            //    c: 150
            //}, {
            //    m: '2015-11',
            //    a: 262,
            //    b: 0,
            //    c: 150
            //}, {
            //    m: '2015-12',
            //    a: 199,
            //    b: 0,
            //    c: 150
            //}, ],
            xkey: 'm',
            ykeys: ['a', 'b', 'c'],
            labels: ['Transactions', 'Reversals', 'Balance Enquiry'],
            xLabelFormat: function (x) { // <--- x.getMonth() returns valid index
                var month = months[x.getMonth()];
                return month;
            },
            dateFormat: function (x) {
                var month = months[new Date(x).getMonth()];
                // var year = x;
                return month;
            },
            lineColors: ['#5cb85c', '#d9534f', '#337ab7']
        });
       
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