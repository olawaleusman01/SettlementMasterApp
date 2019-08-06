$(document).ready(function () {
    toggleMenu(true);

    var col;
    var url = BaseUrl() + 'Report/';
    var table2 = $('.datatable').DataTable({
        "pageLength": 100,
        "ordering": false,
        "lengthMenu": [[100, 200, 500, 1000, 5000, -1], [100, 200, 500, 1000, 5000, "All"]],
        ajax: null, // url + "TranxLogList",
        columns: [
            {data:null},
           {
               data: null,
               className: "center_column",
               // defaultContent: '<input type="checkbox" id="chkAll" name="chkAll" />',
               render: function (data, type, row) {
                   // Combine the first and last names into a single table field
                   var html = '<div style="white-space:nowrap">' + data.trandate + '<div>';
                   return html;
               }
           },
       {
           data: null,
           className: "center_column",
           // defaultContent: '<input type="checkbox" id="chkAll" name="chkAll" />',
           render: function (data, type, row) {
               // Combine the first and last names into a single table field
               var html = '<div style="white-space:nowrap">' + data.valuedate + '<div>';
               return html;
           }
       }, 
            { data: "mirroraccount2" },
              {
                  data: null,
                  className: "center_column",
                 // defaultContent: '<input type="checkbox" id="chkAll" name="chkAll" />',
                  render: function (data, type, row) {
                      // Combine the first and last names into a single table field
                      var html = '<div style="white-space:nowrap">' + data.mirror2acctname + '<div>';
                      return html;
                  }
              },
            //{ data: "fullname" },
            { data: "narration" },
             { data: "tellerno" },
                {
                    data: null,
                    className: "center_column",
                    //defaultContent: '<a class="btn btn-primary btn-xs editor_edit" data-key="' + data.ItbId + '"><i class="fa fa-edit"></i></a>' +
                    //    '  <a class="btn btn-warning btn-xs editor_remove" data-key="' + data.ItbId + '"><i class="fa fa-trash-o"></i></a>',
                    render: function (data, type, row) {
                        // Combine the first and last names into a single table field
                        var html = fix_chars2("'" + data.debit + "'");
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
                           var html = fix_chars2("'" + data.credit + "'");
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
                                var html = fix_chars2("'" +data.bkbalance + "'");
                                return html;
                            }
                        },
            //{ data: "debit" },
            //{ data: "credit" },
            //{ data: "closebalance" },

        ],
        "footerCallback": function (row, data, start, end, display) {
            // alert('suc');
            var api = this.api(), data;

            // Remove the formatting to get integer data for summation
            var intVal = function (i) {
                // alert(JSON.stringify(i));
                return typeof i === 'string' ? i.replace(/[\$,]/g, '') * 1 : typeof i === 'number' ? i : 0;
            };
            // alert(intVal);

            // Total over all pages
            totalDebit = api
                .column(7)
                .data()
                .reduce(function (a, b) {
                    // alert(JSON.stringify(b.tranAmt));
                    return intVal(a) + intVal(b.debit);
                }, 0);
            totalCredit = api
              .column(8)
              .data()
              .reduce(function (a, b) {
                  // alert(JSON.stringify(b.tranAmt));
                  return intVal(a) + intVal(b.credit);
              }, 0);

            // Total over this page
            //pageTotal = api
            //    .column(6, { page: 'current' })
            //    .data()
            //    .reduce(function (a, b) {
            //        return intVal(a) + intVal(b.tranAmt);
            //    }, 0);
            // Update footer
            $(api.column(7).footer()).html(
                 '₦' + fix_chars2("'" + totalDebit + "'")
            );
            $(api.column(8).footer()).html(
             '₦' + fix_chars2("'" + totalCredit + "'")
        );
        }
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
        var acctNo = $('#txtq').val();
        if (fromDate === '') {
            displayDialogNoty('Notification', 'From Date is required.');
            return;
        }
        if (toDate === '') {
            displayDialogNoty('Notification', 'TO Date is required.');
            return;
        }
        if (acctNo === '') {
            displayDialogNoty('Notification', 'Account No is required.');
            return;
        }
        GetCardEnquiryGroup(fromDate, toDate, acctNo);

    });

    function GetCardEnquiryGroup(fromDate, toDate, acctNo) {


        var $reqLogin = {
            url: url + 'RptStatementList?FromDate=' + fromDate + '&ToDate=' + toDate + '&q=' + acctNo,

            data: null,
            type: "Get",
            contentType: "application/json"
        };
        loaderSpin2(true);
        var ajax = new Ajax();
        ajax.send($reqLogin).done(function (response) {
            loaderSpin2(false);

             table2.clear().draw();
            //$("#example1 tbody").empty();
            var data = response.data;
            for (var i = 0; i < data.length; i++) {
                //alert(data[i]);
                 table2.row.add(data[i]).draw();

                //rw += '<tr title="' + data[i].ResponseMsg + '">'
                //        + '<td>' + (i + 1) + '</td>'
                //        + '<td>' + data[i].FirstName + '</td>'
                //        + '<td>' + data[i].LastName + '</td>'
                //        + '<td>' + data[i].DateString + '</td>'
                //        + '<td>' + data[i].CardNo + '</td>'
                //        + '<td>' + data[i].CustomerId + '</td>'
                //        + '<td>' + data[i].PrimaryAcctNo + '</td>'
                //        + '<td>' + data[i].AccountType + '</td>'
                //        + '<td>' + data[i].PhoneNo + '</td>'
                //      //  + '<td>' + data[i].LinkedAccountNo + '</td>'
                //        + '<td>' + data[i].Address + '</td>'
                //        + '<td>' + data[i].City + '</td>'
                //        + '<td>' + data[i].Region + '</td>'
                //        + '<td>' + data[i].BranchCode + '</td>'
                //        + '</tr>';
            }
           // $("#example1 tbody").html(rw);
        }).fail(function (xhr, status, err) {

            loaderSpin2(false);
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