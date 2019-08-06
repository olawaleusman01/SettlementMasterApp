$(document).ready(function () {
    var url = BaseUrl() + 'NapsEnquiry/';
    var fromdate = $('#fromDate').val();
    var todate = $('#toDate').val();
    var batchid = $('#batchId').val();
    var table = $('#tblSummary').DataTable({
        "processing": true,
        "serverSide": true,
        "pageLength": 100,
        "bFilter": false,
        dom: 'Bfrtip',
        "info": true,
        "stateSave": true,
        "lengthMenu": [[10, 20, 50, -1], [10, 20, 50, "All"]],
        "ajax": {
            "url": url + "GetNapsEnquiry?fromdate=" + fromdate + '&todate=' + todate + '&batchid=' + batchid,
            "type": "GET",
        },
        "columns": [
             {
                 data: null,
                 className: "center_column",
                 //defaultContent: '<a class="btn btn-primary btn-xs editor_edit" data-key="' + data.RoleId + '"><i class="fa fa-edit"></i></a>' +
                 //    '  <a class="btn btn-warning btn-xs editor_remove" data-key="' + data.RoleId + '"><i class="fa fa-trash-o"></i></a>',
                 render: function (data, type, row) {
                     // var urlPP = BaseUrl() + data.ReqUrl;
                     // Combine the first and last names into a single table field
                     var html = '<a href="#" class="viewdetail"  data-key="' + data.BATCHID + '"> View Detail</a>';
                     return html;
                 }
             },
             { "data": "BATCHID", "orderable": false },
             { "data": "REQUESTTYPE", "orderable": false },
             { "data": "SETTDATE", "orderable": false },
             { "data": "TRAXCOUNT", "orderable": false },
             { "data": "USERID", "orderable": false },
             { "data": "DATECREATED", "orderable": false },
        ],
        //"order": [[0, "asc"]]
    });

    $('#btnSearch').on('click', function (e) {
        e.preventDefault();
        fromdate = $('#fromDate').val();
        todate = $('#toDate').val();
        batchid = $('#batchId').val();
        //console.log(table);
        var urlP = url + "GetNapsEnquiry?fromdate=" + fromdate + '&todate=' + todate + '&batchid=' + batchid;
        table.ajax.url(urlP).load();
    })

    $(document).on('click', '#tblSummary .viewdetail', function (e) {
        e.preventDefault();
        var editLink = $(this).attr('data-key');
       // alert(editLink);
        var urlString = url + 'GetNapsBatch/' + editLink;
        $('#hidBatchId').val(editLink);
        //console.log(urlString);
        loaderSpin2(true);
        $.get(urlString, function (data) {
            loaderSpin2(false);
            $('#divDetail').html(data.data_html).fadeIn();
            $('#divSummary').fadeOut();
            
        })
    })

    $(document).on('click', '.return', function (e) {
        e.preventDefault();
        $('#divDetail').fadeOut();
        $('#divSummary').fadeIn();
    });
    $(document).on('click', '#btnReprocess', function (e) {
        //alert('reprocess');
        e.preventDefault();
      
        var cnt = $('#tblNaps input[id*="chkSingle"]:checkbox:checked').size();
        if (cnt == 0) {
            displayDialogNoty('Notification', 'No Record is Selected for process');
            return;
        }
        var bid = $('#hidBatchId').val();
        var lst = new Array();
        $('#tblNaps input[id*="chkSingle"]:checkbox:checked').each(function (i, e) {

            lst.push($(this).attr('data-key'));
        });
        console.log(JSON.stringify({ selected_record: lst, BatchId: bid }));
        loaderSpin2(true);
        //return;
        $.ajax({
            type: "POST",
            url: url + 'PostReprocess',
            contentType: "application/json",
            data: JSON.stringify({ selected_record: lst, BatchId: bid }),// JSON.stringify(data),
            success: function (response) {
                loaderSpin2(false);
                // alert(response.RespMessage);
                if (response.RespCode == 0) {
                    $('#divDetail').html(response.data_html);
                    displayDialogNoty('Notification', response.RespMessage);
                }
                else {
                    //alert(response.RespMessage);
                    var msg = '<p>' + response.RespMessage + '</p>';
                    displayDialogNoty('Notification', msg);
                }
            },
            error: function (err) {
                //alert('Error Posting Record');
                console.log(err);
                loaderSpin2(false);
            }
        });
    });
});