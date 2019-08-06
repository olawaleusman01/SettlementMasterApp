
using System;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace Generic.Data
{
    public class GeneralSettings
    {
       
        public static string GetMd5Hash(string value)
        {
            var md5Hasher = MD5.Create();
            var data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(value));
            var sBuilder = new StringBuilder();
            for (var i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));

            }

            return sBuilder.ToString();
        }

        public static string GetRandomPasswordUsingGuid(int length)
        {
            string guidResult = Guid.NewGuid().ToString();
            guidResult = guidResult.Replace("-", string.Empty);

            if (length <= 0 || length > guidResult.Length)
            {
                throw new ArgumentException("Length must be between 1 and " + guidResult.Length); ;
            }

            return guidResult.Substring(0, length); ;
        }
        public string GenId()
        {
            string result = string.Empty;
            Random rand = new Random(999 + DateTime.Now.Millisecond);
            string rst2 = rand.Next(100, 999).ToString();
            string m = DateTime.Now.ToString("MMddyyhhmmss");
            result = rst2 + m;

            result = rand.Next(100000, 999999).ToString();


            return result;
        }

        public string GenRefNo()
        {
            string result = string.Empty;
            Random rand = new Random(999 + DateTime.Now.Millisecond);
            string rst2 = rand.Next(100, 999).ToString();
            string m = DateTime.Now.ToString("MMddyyhhmmss");
            result = rst2 + m;

            //result = rand.Next(100000, 999999).ToString();


            return result;
        }

        public enum StatusEnum
        {
            [StringStatus("A")]
            AUTHORISED = 1,
            [StringStatus("Un Authorized")]
            UNAUTHORISED = 2,
            [StringStatus("C")]
            CLOSED = 3
        }
        public enum StatusEnum2
        {

           [StringStatus("ACTIVE")]
            ACTIVE = 1,
           [StringStatus("INACTIVE")]
            INACTIVE = 2,
           [StringStatus("CLOSE")]
            CLOSED = 3,
           [StringStatus("OPEN")]
           OPEN = 4,
           [StringStatus("APPROVED")]
           APPROVED = 5,
           [StringStatus("REJECTED")]
           REJECTED = 6,
           [StringStatus("UN-APPROVED")]
           UNAPPROVED = 7,
          [StringStatus("LOADED")]
           LOADED = 7,
          [StringStatus("DELETED")]
           DELETED = 8
        }
        public enum RespCode
        {
            [StringStatus("0")]
            SUCCESSFUL = 0,
            [StringStatus("1")]
            FAILED = 1,
            [StringStatus("2")]
            INVALIDACCOUNT = 2,
            [StringStatus("3")]
            EODPROCESSINGRUNNING = 3

        }
        public enum LimitStatus
        {
            [StringStatus("1")]
            RoleLimit = 1,
            [StringStatus("2")]
            DailyLimit = 2,
            [StringStatus("3")]
            CummulativeLimit = 3
        }



        public string LimitStatusMessage(short limitStatusCode)
        {
            switch (limitStatusCode)
            {
                case 1:
                    return "Transaction is above user Limit and has been forwarded for Authorization";
                    break;
                case 2:
                    return "MFB Daily Transaction Limit Exceeded and has been forwarded for Authorization";
                    break;
                case 3:
                    return "Daily Transaction Limit Exceeded and has been forwarded for Authorization";
                    break;
                default:
                    return null;
            }
        }
        public enum RespMessage
        {
            SUCCESSFUL,
            FAILED,
            INVALIDACCOUNT,
            EODPROCESSINGRUNNING
        }
        public enum PostingStatus
        {
            [StringStatus("Posted")]
            POSTED,
            [StringStatus("Not Posted")]
            NOTPOSTED,
            [StringStatus("Pending")]
            AUTHORIZATION,
            [StringStatus("Generated")]
            GENERATED,
            [StringStatus("Not Generated")]
            NOTGENERATED,
            [StringStatus("Rejected")]
            REJECTED,
            [StringStatus("Loaded")]
            LOADED
        }

       
        public enum StaffStatusEnum
        {
            [StringStatus("ACTIVE")]
            A = 1,
            [StringStatus("BLOCKED")]
            B = 2,
            [StringStatus("DEACTIVATE")]
            C = 3,
            [StringStatus("ON LEAVE")]
            D = 4,
            [StringStatus("DISENGAGE")]
            X = 5
        }

        public static string StaffStatus(string statusdesc)
        {
            if (statusdesc == "A")
            {
                return StringEnum.GetStringvalue(StaffStatusEnum.A);
            }
            else if (statusdesc == "B")
            {
                return StringEnum.GetStringvalue(StaffStatusEnum.B);
            }
            else if (statusdesc == "C")
            {
                return StringEnum.GetStringvalue(StaffStatusEnum.C);
            }
            else if (statusdesc == "D")
            {
                return StringEnum.GetStringvalue(StaffStatusEnum.D);
            }
            else
            {
                return StringEnum.GetStringvalue(StaffStatusEnum.X);
            }

        }

        //public static void ClearWebPage(Control parent)
        //{
        //    foreach (Control ctrl in parent.Controls)
        //    {
        //        int 

        //        if (  typeof( is TextBox)
        //        {
        //        }
        //    }
        //        For Each c As Control In parent.Controls
        //            If TypeOf c Is RadTextBox Then
        //                DirectCast(c, RadTextBox).Text = Nothing
        //            End If
        //            If TypeOf c Is RadNumericTextBox Then
        //                DirectCast(c, RadNumericTextBox).Text = Nothing
        //            End If

        //            If TypeOf c Is TextBox Then
        //                DirectCast(c, TextBox).Text = Nothing
        //            End If

        //            If TypeOf c Is RadCalendar Then
        //                DirectCast(c, RadCalendar).SelectedDate = Nothing
        //            End If

        //            If TypeOf c Is Calendar Then
        //                DirectCast(c, Calendar).SelectedDate = Nothing
        //            End If

        //            If TypeOf c Is RadComboBox Then
        //                DirectCast(c, RadComboBox).SelectedValue = Nothing
        //            End If

        //            If TypeOf c Is DropDownList Then
        //                DirectCast(c, DropDownList).SelectedValue = Nothing
        //            End If

        //            If TypeOf c Is RadListBox Then
        //                DirectCast(c, RadListBox).SelectedValue = Nothing
        //            End If

        //            If TypeOf c Is ListBox Then
        //                DirectCast(c, ListBox).SelectedValue = Nothing
        //            End If


        //            If TypeOf c Is CheckBox Then
        //                DirectCast(c, CheckBox).Checked = Nothing
        //            End If

        //            radClearWebPage(c)
        //        Next

        //}


    }

    public class StringStatus : System.Attribute
    {

        private string _value;
        public StringStatus(String value)
        {
            _value = value;
        }
        public String value
        {
            get { return _value; }
        }



    }

    public static class StringEnum
    {

        public static string GetStringvalue(Enum value)
        {
            string output = null;
            Type type = value.GetType();
            FieldInfo f1 = type.GetField(value.ToString());
            StringStatus[] attrs = f1.GetCustomAttributes(typeof(StringStatus), false) as StringStatus[];
            if (attrs.Length > 0) output = attrs[0].value;
            return output;
        }


    }
}


//    Public Shared Sub radClearWebPage(parent As Control)
//        For Each c As Control In parent.Controls
//            If TypeOf c Is RadTextBox Then
//                DirectCast(c, RadTextBox).Text = Nothing
//            End If
//            If TypeOf c Is RadNumericTextBox Then
//                DirectCast(c, RadNumericTextBox).Text = Nothing
//            End If

//            If TypeOf c Is TextBox Then
//                DirectCast(c, TextBox).Text = Nothing
//            End If

//            If TypeOf c Is RadCalendar Then
//                DirectCast(c, RadCalendar).SelectedDate = Nothing
//            End If

//            If TypeOf c Is Calendar Then
//                DirectCast(c, Calendar).SelectedDate = Nothing
//            End If

//            If TypeOf c Is RadComboBox Then
//                DirectCast(c, RadComboBox).SelectedValue = Nothing
//            End If

//            If TypeOf c Is DropDownList Then
//                DirectCast(c, DropDownList).SelectedValue = Nothing
//            End If

//            If TypeOf c Is RadListBox Then
//                DirectCast(c, RadListBox).SelectedValue = Nothing
//            End If

//            If TypeOf c Is ListBox Then
//                DirectCast(c, ListBox).SelectedValue = Nothing
//            End If


//            If TypeOf c Is CheckBox Then
//                DirectCast(c, CheckBox).Checked = Nothing
//            End If

//            radClearWebPage(c)
//        Next

//    End Sub
//        Sub genID()
//            Try

//                Dim rnd As New Random((9999 + DateTime.Now.Millisecond))
//                Dim result As String = rnd.Next(10000, 99999).ToString()
//                Dim m As String = Format(Now(), "MMddyyhhmmss")
//                Me.lblbankTransId.Text =  result & m

//            Catch ex As Exception

//            End Try
//    End Sub
//}
