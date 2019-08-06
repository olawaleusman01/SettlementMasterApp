$(document).ready(function () {

    var col;

    // var url = BaseUrl() + '/ItemType/';
    var url = BaseUrl() + 'Item/';
     var url2 = BaseUrl() + 'Settings/';
     BindCombo();
     BindGrid();
     BindCategory();
     BindInventoryStatistic();
    var table2 = $('.datatable').DataTable({
        ajax: url + "ItemList",
        columns: [
         {
             data: null,
             className: "center_column",
             //defaultContent: '<a class="btn btn-primary btn-xs editor_edit" data-key="' + data.ItbId + '"><i class="fa fa-edit"></i></a>' +
             //    '  <a class="btn btn-warning btn-xs editor_remove" data-key="' + data.ItbId + '"><i class="fa fa-trash-o"></i></a>',
             render: function (data, type, row) {
                 // Combine the first and last names into a single table field
                 var html = '<a class="btn btn-primary btn-xs editor_edit" data-key="' + data.ItbId + '"><i class="fa fa-edit"></i></a>';
                 return html;
             }
         },
            { data: "ItemName" },
            { data: "Quantity" },
            { data: "ItbId" },
            { data: "UnitCost" },
           
            { data: "CreateDate" },
            {
                data: null,
                className: "center_column",
                //defaultContent: '<a class="btn btn-primary btn-xs editor_edit" data-key="' + data.ItbId + '"><i class="fa fa-edit"></i></a>' +
                //    '  <a class="btn btn-warning btn-xs editor_remove" data-key="' + data.ItbId + '"><i class="fa fa-trash-o"></i></a>',
                render: function (data, type, row) {
                    // Combine the first and last names into a single table field
                    var html = '<a class="btn btn-primary btn-xs editor_edit" data-key="' + data.ItbId + '"><i class="fa fa-edit"></i></a>';
                    return html;
                }
            }
        ]
    });

    var validator = $("#formItem").validate({
        rules: {
            ItemName: "required",
            ItemType_ItbId: "required",
            PurchaseDate: "required",
            UnitMeasure_ItbId: "required",
            Location_ItbId: "required",
            CostPrice: {
                //required: true,
                number:true
            },
            SellingPriceMargin: {
               // required: true,
                number:true
              },
            Quantity: {
                 // required: true,
                  number: true
            },
            ReOrderLevel: {
                //required: true,
                number: true
            },
            
        },
        messages: {

            // equalTo: "Password must be Equal",
            //Name: {
            //    required: "Please Enter Last Name"
            //},


        },
        submitHandler: function () {
            //  e.preventDefault();
            // alert('success');
            var btn = $('#btnSave').val();
            var urlTemp;
            var postTemp;
            var event;
            if (btn == "1") {

                urlTemp = url + 'Create';
                postTemp = 'post';
                $('#ItbId').val(0);
                event = 'new';
            }
            else {

                urlTemp = url + 'Create';
                //postTemp = 'put';
                postTemp = 'post';
                event = 'modify';
            };
            //  alert(urlTemp);
            var $reqLogin = {
                url: urlTemp,

                data: $('#formItem').serialize(),
                type: postTemp,
                contentType: "application/x-www-form-urlencoded"
            };
            //alert($('#formItem').serialize());
            loaderSpin(true);
            disableButton(true);
            var ajax = new Ajax();
            ajax.send($reqLogin).done(function (response) {
                loaderSpin(false);
                disableButton(false);
                if (response.RespCode === 0) {
                    new Util().clearform('#formItem');

                    if (event == 'new') {
                       // addGridItem(response.data);
                        $('#myModal').modal('hide');
                        $('#ItbId').val(0);
                        BindGrid();
                        // alert('Record Created Successfully');
                        displayDialogNoty('Notification', 'Record Created Successfully');
                    }
                    else {
                        var btn = $('#btnSave').html('<i class="fa fa-save"></i> Save');
                        // updateGridItem(response.data);
                        BindGrid();
                        $('#myModal').modal('hide');
                        $('#ItbId').val(0);
                        // alert('Record Updated successfully');
                        displayDialogNoty('Notification', 'Record Updated Successfully');

                    }

                }
                else {
                    // alert(response.RespMessage)
                    displayModalNoty('alert-warning', response.RespMessage, true);

                }


            }).fail(function (xhr, status, err) {
                loaderSpin(false);
                disableButton(false);
                displayModalNoty('alert-warning', 'No network connectivity. Please check your network connections.', true);
                //$('#display-error').show();
                //$('#errorMessage').text("No network connectivity. Please check your network connections.");
                // errorMessage
                // showPopDialog("No network connectivity. Please check your network connections.", handlerError);
            });


        }
    });





    $("#listItem").on("click", "a.editor_edit", editDetailServer);

    //$("#example1").on("click", ".current", setAsCurrent);

    function editDetailServer() {
        loaderSpin2(true);
        //  disableButton(true);
        var editLink = $(this).attr('data-key');
        col = $(this).parent();
        // alert(editLink);
        $('#ItbId').val(editLink);

        // alert( $('#ItbId').val());
        var $reqLogin = {
            url: url + 'ViewDetail/' + editLink,

            data: null,
            type: "Get",
            contentType: "application/json"
        };
        // alert($reqLogin.url);
        //$('#ajax-loading').show();
        var ajax = new Ajax();
        ajax.send($reqLogin).done(function (response) {
            loaderSpin2(false);
            if (response.RespCode === 0) {

                $('#ItemName').val(response.model.ItemName);
               // $('#Quantity').val(response.model.LocationNameShort);
                $('#ReOrderLevel').val(response.model.ReOrderLevel);
                $('#CostPrice').val(response.model.CostPrice);
                $('#SellingPrice2').val(response.model.SellingPrice2);
                $('#SellingPriceMargin').val(response.model.SellingPriceMargin);
                $('#ItemType_ItbId').val(response.model.ItemType_ItbId);
                $('#PurchaseDateString').val(response.model.PurchaseDateString);
                $('#ItemDescription').val(response.model.ItemDescription);
                $('#StockOpenQuantity').val(response.model.StockOpenQuantity);
                $('#SKU').val(response.model.SKU);
                $('#ItbId').val(response.model.ItbId);
                $('#UnitMeasure_ItbId').val(response.model.UnitMeasure_ItbId);
                //  $('#UserName').attr('disabled','disabled');
                $('#CreatedBy').text(response.model.CreatedBy);
                $('#CreatedDate').text(response.model.DateString);
                $('#Status').val(response.model.Status);
                $('#pnlAudit').css('display', 'block');
                $('a.editor_reset').hide();
                // $('').val(response.ExamName);
                $('#btnSave').html('<i class="fa fa-edit"></i> Update');
                $('#btnSave').val(2);
                $('#divStatus').show();

                $('#myModal').modal({ backdrop: 'static', keyboard: false });
            }
            else {
                displayDialogNoty('Notification', response.RespMessage);
            }
        }).fail(function (xhr, status, err) {
            //Disable Button
            // loginbtn.hide();
            //hide login loader
            $('#ajax-loading').hide();
            $('#display-error').show();
            $('#errorMessage').text("No network connectivity. Please check your network connections.");
            // errorMessage
            // showPopDialog("No network connectivity. Please check your network connections.", handlerError);
        });

    }
   
    function addGridItem(model) {
        table2.row.add(model).draw();
    }
    function updateGridItem(model) {
        var rowIdx = table2
    .cell(col)
    .index().row;

        var d = table2.row(rowIdx).data();
        d.UnitDescription = model.UnitDescription;
        d.UnitShortCode = model.UnitShortCode;

        //d.AmountString = model.AmountString;
        //d.ApplyToAllStaff = model.ApplyToAllStaff;
        //d.RateType = model.RateType;
        d.Status = model.Status

        table2
     .row(rowIdx)
     .data(d)
     .draw();

    }

    $('a.editor_create').click(function () {
        // formClear('#FormTerm');
        new Util().clearform('#formItem');

        validator.resetForm();
        $('#divStatus').hide();
        $('#pnlAudit').css('display', 'none');
        $('#ItbId').val(0);
        $('#btnSave').val(1);
        $('#btnSave').html('<i class="fa fa-save"></i> Save');
        // $('#UserName').removeAttr('disabled', 'disabled');
        $('a.editor_reset').show();
        $('#myModal').modal({ backdrop: 'static', keyboard: false });
    });
    $('a.editor_reset').click(function () {
        // formClear('#FormTerm');
        new Util().clearform('#formItem');

        validator.resetForm();
        $('#pnlAudit').css('display', 'none');
        $('#ItbId').val(0);

    });

    function BindCombo() {

        //  Bind Country
        var $reqLogin = {
            url: url2 + 'GetItemType',
            data: null,
            type: "Get",
            contentType: "application/json"
        };

        var ajax = new Ajax();
        ajax.send($reqLogin).done(function (response) {

            var exist = false;
            if (response.data.length > 0) {
                $("#ItemType_ItbId").empty();

                $("#ItemType_ItbId").append("<option value=''> --Select Item Category-- </option>");
                //  $("#Kin_Country").append("<option value=''> --Select a Country-- </option>");
                // $("#formRef #Country").append("<option value=''> --Select a Country-- </option>");

                for (var i = 0; i < response.data.length; i++) {
                    $("#ItemType_ItbId").append("<option value='" + response.data[i].ItbId + "'>" +
                    response.data[i].Name + "</option>");

                }
                //$("#CountryCode").val('NG');

            }
            // return response.data;
        }).fail(function (xhr, status, err) {
            return null;

        });

        //  Bind Level
        var $reqLogin = {
            url: url2 + 'GetUnit',
            data: null,
            type: "Get",
            contentType: "application/json"
        };

        var ajax = new Ajax();
        ajax.send($reqLogin).done(function (response) {

            var exist = false;
            if (response.data.length > 0) {
                $("#UnitMeasure_ItbId").empty();


                $("#UnitMeasure_ItbId").append("<option value=''> --Select Unit-- </option>");

                for (var i = 0; i < response.data.length; i++) {
                    $("#UnitMeasure_ItbId").append("<option value='" + response.data[i].ItbId + "'>" +
                    response.data[i].UnitDescription + "</option>");
                }
            }
            // return response.data;
        }).fail(function (xhr, status, err) {
            return null;

        });

    

          var $reqLogin = {
            url: url2 + 'GetLocation',
            data: null,
            type: "Get",
            contentType: "application/json"
        };

        var ajax = new Ajax();
        ajax.send($reqLogin).done(function (response) {

            //var exist = false;
            if (response.data.length > 0) {
                $("#Location_ItbId").empty();
                $("#LocationFilter_ItbId").empty();

                $("#Location_ItbId").append("<option value=''> --Select a Location-- </option>");
                $("#LocationFilter_ItbId").append("<option value=''> --ALL-- </option>");
                for (var i = 0; i < response.data.length; i++) {
                    $("#Location_ItbId").append("<option value='" + response.data[i].ItbId + "'>" +
                    response.data[i].LocationNameFull + "</option>");
                    $("#LocationFilter_ItbId").append("<option value='" + response.data[i].ItbId + "'>" +
                   response.data[i].LocationNameFull + "</option>");
                }
            }
            // return response.data;
        }).fail(function (xhr, status, err) {
            return null;

        });


    }
     function BindGrid(location,category) {
          loaderSpin2(true);
         // alert('here');
          $("#listItem tbody").empty();
        var $reqLogin = {
            url: url + 'ItemList?id=' + location + '&cat=' + category,

            data: null,
            type: "Get",
            contentType: "application/json"
        };
      
        var ajax = new Ajax();
        ajax.send($reqLogin).done(function (response) {
            loaderSpin2(false);
            var data = response.data;
           // alert('here 2');
            var rw = '';
          
            if (response.RespCode === 0) {
                if (data !== undefined) {
                    if (data.length > 0) {
                        for (var i = 0; i < data.length; i++) {
                           // console.log(JSON.stringify(data[i]));
                            //rw += '<tr>'
                            //       // + '<td>' + (i + 1) + '</td>'
                            //        //+ '<td><a class="btn btn-primary btn-xs editor_edit" data-key="' + data[i].BatchId + '">' + '<i class="fa fa-edit"></i></a></td>'
                            //        //+ '<td><input class="" type="checkbox" name="chkSingle" value="' + data[i].BatchId + '"/></td>'
                            //        + '<td>' + data[i].PrimaryAcctNo + '</td>'
                            //        + '<td>' + data[i].FullName + '</td>'
                            //        + '<td>' + data[i].CardNo + '</td>'
                            //        + '<td>' + data[i].RequestType + '</td>'
                            //        + '<td>' + data[i].DateCreatedString + '</td>'
                            //        + '<td>' + data[i].UserId + '</td>'
                            //        + '<td>' + data[i].Status + '</td>'
                            //        + '</tr>';
                            rw += '<tr>' +
                                                '<td>' +
                                                    '<strong><a>' + data[i].ItemName + '</a></strong><br>' +
                                                    '<div style="display: inline-block;" class="item-detail">' +
                                                        '<div>SKU: ' + data[i].SKU + '</div>' +
                                                        '<div>Category: ' + data[i].ItemCategory + '</div>' +
                                                        '<div>Location: ' + data[i].LocationNameShort + '</div>' +
                                                    '</div>' +
                    '</td>' +
                    '<td class="list-center"> <span class="' + data[i].price_class + '">' + data[i].Quantity + '&nbsp;' + data[i].UnitShortCode + '</span></td>' +
                    '<td class="list-center" style="padding-left: 16px;"><a>' + data[i].ItbId + '</a></td>' +
                    '<td class="jettisonable list-right">' +
                        '<a>₦' + data[i].CostPrice + '</a>' +
                    '</td>' +
                    '<td class="jettisonable list-center">' + data[i].DateString + '</td>' +
                    '<td>' +
                                '<a class="editor_edit btn btn-default btn-xs" data-key="' + data[i].ItbId + '"><i class="fa fa-edit"></i> Edit</a>'
                        //'<ul class="nav nav-pills">' +
                        //    '<li class="dropdown options">' +
                        //        '<a class="dropdown-toggle" data-toggle="dropdown" href="#"><span>Options <span class="caret"></span></span></a>' +
                        //        '<ul class="dropdown-menu" role="menu">' +
                        //            '<li><a>View</a></li>' +
                        //            '<li><a>Edit</a></li>' +

                        //            '<li><a>Delete</a></li>' +
                        //            '<li><a>Archive</a></li>' +
                        //        '</ul>' +
                        //    '</li>' +
                        //'</ul>' +

                    '</td>' +
                    '</tr>';
                            // alert(rw);
                        };
                        $("#listItem tbody").html(rw);
                        //$("#listItem").fadeOut();
                        //$("#listItem").fadeIn();
                    }
                    else
                    {
                        rw = '<tr><td colspan="6">There is no record to display.</td></tr>'
                        $("#listItem tbody").html(rw);
                    }
                    
                }
                else {
                    rw = '<tr><td colspan="6">There is no record to display.</td></tr>'
                    $("#listItem tbody").html(rw);
                }
            }
            else {
                displayDialogNoty('Notification',response.RespMessage );
            }
        }).fail(function (xhr, status, err) {
           
        });

    }
     function BindCategory()
     {
         try
         {
            // loaderSpin2(true);
              //alert('here');
             $("#listCategory").empty();
             var $reqLogin = {
                 url: url2 + 'GetItemType',

                 data: null,
                 type: "Get",
                 contentType: "application/json"
             };

             var ajax = new Ajax();
             ajax.send($reqLogin).done(function (response) {
                 loaderSpin2(false);
                 var data = response.data;
                 // alert('here 2');
                 var rw = '';

                 if (response.RespCode === 0) {
                     if (data !== undefined) {
                         if (data.length > 0) {
                             for (var i = 0; i < data.length; i++) {
                                // alert(data[i].Description);
                                 rw += '<li>' +
                                       '<span class="' + data[i].cat_class + '">' + data[i].Quantity + '</span>' +
                                       '<span class="name"><span class="drag">&nbsp;</span>';
                                 if (data[i].Quantity > 0)
                                 {
                                     rw += '<span><a href="#" class="filter_cat" data-key="' + data[i].ItbId +  '" title="' + (data[i].Description || '') + '">' + data[i].Name + '</a></span>';

                                 }
                                 else {
                                    rw += '<span>' + data[i].Name + '</span>';
                                 }
                                       
                                       rw += '</li>';
                             
                                 // alert(rw);
                             };
                             $("#listCategory").html(rw);
                             //$("#listItem").fadeOut();
                             //$("#listItem").fadeIn();
                             $('#listCategory li .filter_cat').on('click', function (e) {
                                 e.preventDefault();
                                 var sel = $(this).data('key');
                                // alert(sel);
                                 //filter by Category
                             });
                         }
                         else {
                             rw = '<tr><td colspan="6">There is no record to display.</td></tr>'
                             $("#listCategory tbody").html(rw);
                         }

                     }
                     else {
                         rw = '<tr><td colspan="6">There is no record to display.</td></tr>'
                         $("#listCategory").html(rw);
                     }
                 }
                 else {
                     displayDialogNoty('Notification', response.RespMessage);
                 }
             }).fail(function (xhr, status, err) {

             });
         }
         catch(err)
         {

         }
     }
     function BindInventoryStatistic() {
         try {
             // loaderSpin2(true);
             //alert('here');
             $("#listStatistic").empty();
             var $reqLogin = {
                 url: url2 + 'GetItemStockCount',

                 data: null,
                 type: "Get",
                 contentType: "application/json"
             };

             var ajax = new Ajax();
             ajax.send($reqLogin).done(function (response) {
                 loaderSpin2(false);
                 var data = response.data;
                 // alert('here 2');
                 var rw = '';

                 if (response.RespCode === 0) {
                     if (data !== undefined) {
                         //if (data.length > 0) {
                             //for (var i = 0; i < data.length; i++) {
                                 // alert(data[i].Description);
                                 rw += '  <li>Total Quantity Items: <span style="float:right">' + data.ItemCount + '</span></li>';
                                 rw += '  <li>Total Quantity Items: <span style="float:right">' + data.StockCount + '</span></li>';

                                 // alert(rw);
                            // };
                                 $("#listStatistic").html(rw);
                             //$("#listItem").fadeOut();
                             //$("#listItem").fadeIn();
                          
                         //}
                         //else {
                         //    rw = '<tr><td colspan="6">There is no record to display.</td></tr>'
                         //    $("#listStatistic tbody").html(rw);
                         //}

                     }
                     //else {
                     //    rw = '<tr><td colspan="6">There is no record to display.</td></tr>'
                     //    $("#listCategory").html(rw);
                     //}
                 }
                 else {
                     displayDialogNoty('Notification', response.RespMessage);
                 }
             }).fail(function (xhr, status, err) {

             });
         }
         catch (err) {

         }
     }
     var cat_itbid = null;
     var loc_itbid = null;
     $('#LocationFilter_ItbId').on('change', function () {
         loc_itbid = $(this).val();
         
        // alert(sel);
         //filter by location
         BindGrid(loc_itbid, cat_itbid);
     });
    
});