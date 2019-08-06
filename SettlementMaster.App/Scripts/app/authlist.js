$(document).ready(function () {
    /// GetRoles(1);

    var col;
    //var table2 = $('.datatable').DataTable(
    //);
    // var cnt = 0;

    var url = BaseUrl() + 'AuthList/';
    var table2 = $('.datatable').DataTable({
        ajax: url + "AuthQueue",
        columns: [
            
            {
                data: null,
                className: "center_column",
                //defaultContent: '<a class="btn btn-primary btn-xs editor_edit" data-key="' + data.RoleId + '"><i class="fa fa-edit"></i></a>' +
                //    '  <a class="btn btn-warning btn-xs editor_remove" data-key="' + data.RoleId + '"><i class="fa fa-trash-o"></i></a>',
                render: function (data, type, row) {
                    var urlPP = BaseUrl() + data.ReqUrl;
                    // Combine the first and last names into a single table field
                    var html = '<a  href="' + urlPP + '"> View Detail</a>';
                    return html;
                }
            },
            { data: "EVENTTYPE" },
            { data: "CREATED_BY" },
            { data: "MenuName" },
            { data: "Institution_Name" },
            { data: "DATESTRING" },
            { data: "BATCHID" },
            { data: "STATUS" },
        ]
    });
});