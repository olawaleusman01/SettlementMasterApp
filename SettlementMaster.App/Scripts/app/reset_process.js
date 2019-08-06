     $(document).ready(function () {
         $('#btnReset').click(function () {
           
            $.ajax({
                type: "POST",
                url: '@Url.Action("updProcess", "ResetProcess")',
                data: null
                },
                dataType: 'json',
                success: function (result) {
                    if (result) {
                        console.log("Ok");
                    } else {
                        console.log("error");
                    }
                },
                error: function () {
                    console.log('something went wrong - debug it!');
                }
            })

         $('#btnReset2').click(function () {

             $.ajax({
                 type: "POST",
                 url: '@Url.Action("updProcess2", "ResetProcess")',
                 data: null
             },
                 dataType: 'json',
                 success: function (result) {
                     if (result) {
                         console.log("Ok");
                     } else {
                         console.log("error");
                     }
                 },
                 error: function () {
                     console.log('something went wrong - debug it!');
                 }
            })

         $('#btnReset3').click(function () {

             $.ajax({
                 type: "POST",
                 url: '@Url.Action("updProcess3", "ResetProcess")',
                 data: null
             },
                 dataType: 'json',
                 success: function (result) {
                     if (result) {
                         console.log("Ok");
                     } else {
                         console.log("error");
                     }
                 },
                 error: function () {
                     console.log('something went wrong - debug it!');
                 }
            })

     });


    
 