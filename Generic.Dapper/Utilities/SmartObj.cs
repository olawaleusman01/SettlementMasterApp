using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Generic.Data.Utilities
{
    public static class SmartObj
    {
        //private static int DEFAULT_MIN_PASSWORD_LENGTH = 8;
        //private static int DEFAULT_MAX_PASSWORD_LENGTH = 10;

        // Define supported password characters divided into groups.
        // You can add (or remove) characters to (from) these groups.
        private static string PASSWORD_CHARS_LCASE = "abcdefgijkmnopqrstwxyz";
        private static string PASSWORD_CHARS_UCASE = "ABCDEFGHJKLMNPQRSTWXYZ";
        private static string PASSWORD_CHARS_NUMERIC = "23456789";
        private static string PASSWORD_CHARS_SPECIAL = "*$-+?_&=!%{}/";
        //public static void ClearWebPage(Control ctrl)
        //{
        //    if (ctrl != null)
        //    {
        //        foreach (Control c in ctrl.Controls)
        //        {
        //            //'If TypeOf c Is RadTextBox Then
        //            //'    DirectCast(c, RadTextBox).Text = Nothing
        //            //'End If

        //            if (c is TextBox)
        //            {
        //                ((TextBox)c).Text = string.Empty;
        //            }
        //            if (c is TextBox)
        //            {
        //                ((TextBox)c).Text = string.Empty;
        //            }

        //            // ''If TypeOf c Is RadNumericTextBox Then
        //            // ''    DirectCast(c, RadNumericTextBox).Text = Nothing
        //            // ''End If

        //            // ''If TypeOf c Is RadCalendar Then
        //            // ''    DirectCast(c, RadCalendar).SelectedDate = Nothing
        //            // ''End If

        //            if (c is Calendar)
        //            {
        //                //((Calendar)c).SelectedDate = null;
        //            }

        //            // ''If TypeOf c Is RadComboBox Then
        //            // ''    DirectCast(c, RadComboBox).SelectedValue = Nothing
        //            // ''End If

        //            if (c is DropDownList)
        //            {
        //                ((DropDownList)c).SelectedValue = string.Empty;
        //            }

        //            // ''If TypeOf c Is RadListBox Then
        //            // ''    DirectCast(c, RadListBox).SelectedValue = Nothing
        //            // ''End If

        //            //if (c is ListBox)
        //            //{
        //            //    ((ListBox)c).SelectedValue = null;
        //            //}


        //            //if (c is CheckBox)
        //            //{
        //            //    //((CheckBox)c).Checked = null;
        //            //}

        //            //  radClearWebPage(c);
        //        }
        //    }

        //}
        public static string FormatMoney(string Dmoney)
        {
            //  string ticno = "";
            string NamePath = Dmoney.ToString().Trim();
            int NameLength1 = 0;
            decimal amount = 0;
            decimal Nmoney = 0;

            if (!string.IsNullOrEmpty(NamePath))
            {
                string curval1 = "";
                string curval12 = null;
                NameLength1 = Strings.InStr(1, Strings.StrReverse(NamePath), "h");

                if (NameLength1 == 0)
                {
                    NameLength1 = Strings.InStr(1, Strings.StrReverse(NamePath), "t");
                }

                if (NameLength1 == 0)
                {
                    NameLength1 = Strings.InStr(1, Strings.StrReverse(NamePath), "m");
                }

                if (NameLength1 != 0)
                {
                    curval1 = Strings.Mid(Strings.StrReverse(NamePath), NameLength1, 1);
                    curval12 = Strings.Mid(Dmoney.ToString(), 1, Strings.Len(Dmoney.ToString()) - 1);
                    amount = Convert.ToDecimal(curval12);

                    if (curval1 == "h" | curval1 == "H")
                    {
                        Nmoney = amount * 100;
                    }
                    else if (curval1 == "t" | curval1 == "T")
                    {
                        Nmoney = amount * 1000;
                    }
                    else if (curval1 == "m" | curval1 == "M")
                    {
                        Nmoney = amount * 1000000;

                    }
                }
                else
                {
                    Nmoney = Convert.ToDecimal(Dmoney);
                }


            }
            Nmoney = Convert.ToDecimal(Nmoney);
            return Strings.Format(Convert.ToDecimal(Nmoney), "#,##0.00");
        }
        public static string GenOrderNo()
        {
            string refNo = "";
            Random rand = new Random();
            refNo = rand.Next(10000000, 99999999).ToString();
            return refNo; // string.Concat("O", refNo);
        }
        public static string GenRefNo()
        {
            string result = string.Empty;
            Random rand = new Random(999 + DateTime.Now.Millisecond);
            string rst2 = rand.Next(100, 999).ToString();
            string m = DateTime.Now.ToString("MMddyyhhmmss");
            result = rst2 + m;

            //result = rand.Next(100000, 999999).ToString();


            return result;
        }
        public static string GenRefNo2()
        {
            string result = string.Empty;
            //Random rand = new Random(999 + DateTime.Now.Millisecond);
            //string rst2 = rand.Next(100, 999).ToString();
            string m = DateTime.Now.ToString("MMddyyhhmmss");
            result = m;

            //result = rand.Next(100000, 999999).ToString();


            return result;
        }
        public static string GenUserId(string LastName, string FirstName)
        {
            string usId = "";
            if (!string.IsNullOrEmpty(LastName) && !string.IsNullOrEmpty(FirstName))
            {
                return string.Concat(FirstName, ".", LastName);
                //if (LastName.Length > 4 )
                //{
                //    usId += string.Concat(LastName.Substring(0, 4));

                //}
                //else
                //{
                //    usId += LastName;
                //}
                //if (FirstName.Length > 4)
                //{
                //    usId += string.Concat(FirstName.Substring(0, 4));

                //}
                //else
                //{
                //    usId += FirstName;
                //}
            }
            return usId;
        }

        public static string GenerateRandomPassword(int passLength)
        {
            return Generate(passLength, passLength);
            //string pass = "password1#";
            //return pass;
        }

        public static string Encrypt(string clearText)
        {
            string EncryptionKey = "MAKV2SPBNI99212";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }

        public static object CastNull(object value1)
        {
            if (value1 == null)
            {
                return "Null";
            }
            return "'" + value1 + "'";
        }
        public static object CastNullNumber(object value1)
        {
            if (value1 == null)
            {
                return "Null";
            }
            return value1;
        }
        public static string Decrypt(string cipherText)
        {
            string EncryptionKey = "MAKV2SPBNI99212";
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }

        public static string GetClass(string style)
        {
            switch (style)
            {
                case "success":
                    {
                        return "alert alert-success alert-dismissable";
                    }
                case "warning":
                    {
                        return "alert alert-warning alert-dismissable";
                    }
            }
            return "";
        }

        /// <summary>
        /// Generates a random password of the exact length.
        /// </summary>
        /// <param name="length">
        /// Exact password length.
        /// </param>
        /// <returns>
        /// Randomly generated password.
        /// </returns>
        public static string Generate(int length)
        {
            return Generate(length, length);
        }

        /// <summary>
        /// Generates a random password.
        /// </summary>
        /// <param name="minLength">
        /// Minimum password length.
        /// </param>
        /// <param name="maxLength">
        /// Maximum password length.
        /// </param>
        /// <returns>
        /// Randomly generated password.
        /// </returns>
        /// <remarks>
        /// The length of the generated password will be determined at
        /// random and it will fall with the range determined by the
        /// function parameters.
        /// </remarks>
        public static string Generate(int minLength,
                                      int maxLength)
        {
            // Make sure that input parameters are valid.
            if (minLength <= 0 || maxLength <= 0 || minLength > maxLength)
                return null;

            // Create a local array containing supported password characters
            // grouped by types. You can remove character groups from this
            // array, but doing so will weaken the password strength.
            char[][] charGroups = new char[][]
            {
            PASSWORD_CHARS_LCASE.ToCharArray(),
            PASSWORD_CHARS_UCASE.ToCharArray(),
            PASSWORD_CHARS_NUMERIC.ToCharArray(),
            PASSWORD_CHARS_SPECIAL.ToCharArray()
            };

            // Use this array to track the number of unused characters in each
            // character group.
            int[] charsLeftInGroup = new int[charGroups.Length];

            // Initially, all characters in each group are not used.
            for (int i = 0; i < charsLeftInGroup.Length; i++)
                charsLeftInGroup[i] = charGroups[i].Length;

            // Use this array to track (iterate through) unused character groups.
            int[] leftGroupsOrder = new int[charGroups.Length];

            // Initially, all character groups are not used.
            for (int i = 0; i < leftGroupsOrder.Length; i++)
                leftGroupsOrder[i] = i;

            // Because we cannot use the default randomizer, which is based on the
            // current time (it will produce the same "random" number within a
            // second), we will use a random number generator to seed the
            // randomizer.

            // Use a 4-byte array to fill it with random bytes and convert it then
            // to an integer value.
            byte[] randomBytes = new byte[4];

            // Generate 4 random bytes.
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            rng.GetBytes(randomBytes);

            // Convert 4 bytes into a 32-bit integer value.
            int seed = BitConverter.ToInt32(randomBytes, 0);

            // Now, this is real randomization.
            Random random = new Random(seed);

            // This array will hold password characters.
            char[] password = null;

            // Allocate appropriate memory for the password.
            if (minLength < maxLength)
                password = new char[random.Next(minLength, maxLength + 1)];
            else
                password = new char[minLength];

            // Index of the next character to be added to password.
            int nextCharIdx;

            // Index of the next character group to be processed.
            int nextGroupIdx;

            // Index which will be used to track not processed character groups.
            int nextLeftGroupsOrderIdx;

            // Index of the last non-processed character in a group.
            int lastCharIdx;

            // Index of the last non-processed group.
            int lastLeftGroupsOrderIdx = leftGroupsOrder.Length - 1;

            // Generate password characters one at a time.
            for (int i = 0; i < password.Length; i++)
            {
                // If only one character group remained unprocessed, process it;
                // otherwise, pick a random character group from the unprocessed
                // group list. To allow a special character to appear in the
                // first position, increment the second parameter of the Next
                // function call by one, i.e. lastLeftGroupsOrderIdx + 1.
                if (lastLeftGroupsOrderIdx == 0)
                    nextLeftGroupsOrderIdx = 0;
                else
                    nextLeftGroupsOrderIdx = random.Next(0,
                                                         lastLeftGroupsOrderIdx);

                // Get the actual index of the character group, from which we will
                // pick the next character.
                nextGroupIdx = leftGroupsOrder[nextLeftGroupsOrderIdx];

                // Get the index of the last unprocessed characters in this group.
                lastCharIdx = charsLeftInGroup[nextGroupIdx] - 1;

                // If only one unprocessed character is left, pick it; otherwise,
                // get a random character from the unused character list.
                if (lastCharIdx == 0)
                    nextCharIdx = 0;
                else
                    nextCharIdx = random.Next(0, lastCharIdx + 1);

                // Add this character to the password.
                password[i] = charGroups[nextGroupIdx][nextCharIdx];

                // If we processed the last character in this group, start over.
                if (lastCharIdx == 0)
                    charsLeftInGroup[nextGroupIdx] =
                                              charGroups[nextGroupIdx].Length;
                // There are more unprocessed characters left.
                else
                {
                    // Swap processed character with the last unprocessed character
                    // so that we don't pick it until we process all characters in
                    // this group.
                    if (lastCharIdx != nextCharIdx)
                    {
                        char temp = charGroups[nextGroupIdx][lastCharIdx];
                        charGroups[nextGroupIdx][lastCharIdx] =
                                    charGroups[nextGroupIdx][nextCharIdx];
                        charGroups[nextGroupIdx][nextCharIdx] = temp;
                    }
                    // Decrement the number of unprocessed characters in
                    // this group.
                    charsLeftInGroup[nextGroupIdx]--;
                }

                // If we processed the last group, start all over.
                if (lastLeftGroupsOrderIdx == 0)
                    lastLeftGroupsOrderIdx = leftGroupsOrder.Length - 1;
                // There are more unprocessed groups left.
                else
                {
                    // Swap processed group with the last unprocessed group
                    // so that we don't pick it until we process all groups.
                    if (lastLeftGroupsOrderIdx != nextLeftGroupsOrderIdx)
                    {
                        int temp = leftGroupsOrder[lastLeftGroupsOrderIdx];
                        leftGroupsOrder[lastLeftGroupsOrderIdx] =
                                    leftGroupsOrder[nextLeftGroupsOrderIdx];
                        leftGroupsOrder[nextLeftGroupsOrderIdx] = temp;
                    }
                    // Decrement the number of unprocessed groups.
                    lastLeftGroupsOrderIdx--;
                }
            }

            // Convert password characters into a string and return the result.
            return new string(password);
        }

        public static DateTime ConvertToAnotherZoneTime(DateTime dateLocalUtc, string zoneId = "W. Central Africa Standard Time")
        {
            //TimeZoneInfo cstZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
            //var dt = TimeZoneInfo.ConvertTimeFromUtc(date, cstZone);
            TimeZoneInfo wcaZone = TimeZoneInfo.FindSystemTimeZoneById(zoneId);
            var dtA = TimeZoneInfo.ConvertTimeFromUtc(dateLocalUtc, wcaZone);
            return dtA;
        }
        public static string ReplaceQuote(string value)
        {
            string val;
            if (value != null)
            {
                val = value.Replace("'", "''");
                return val;

            }
            return "";
        }

        public static string PadWithZero(string v, int length)
        {
            v = v ?? "";

            var zeros = "";
            if (v.Length > length)
            {
                return v;
            }

            for (int i = 0; i < length - v.Length; i++)
            {
                zeros += "0";
            }
            return zeros + v;

        }

        public static List<DropdownObj> GetCalculationBasis()
        {
            var rleBase = new List<DropdownObj>();
            rleBase.Add(new DropdownObj()
            {
                Code = "R",
                Description = "Rate",
            });
            rleBase.Add(new DropdownObj()
            {
                Code = "F",
                Description = "Flat",
            });
            return rleBase;
        }
        public static List<DropdownObj> GetStatus()
        {
            var rleBase = new List<DropdownObj>();
            rleBase.Add(new DropdownObj()
            {
                Code = "ACTIVE",
                Description = "ACTIVE",
            });
            rleBase.Add(new DropdownObj()
            {
                Code = "CLOSE",
                Description = "CLOSE",
            });
            return rleBase;
        }
        public static List<DropdownObj> GetStatusMerchant()
        {
            var rleBase = new List<DropdownObj>();
            rleBase.Add(new DropdownObj()
            {
                Code = "ACTIVE",
                Description = "ACTIVE",
            });
            rleBase.Add(new DropdownObj()
            {
                Code = "BLOCKED",
                Description = "BLOCKED",
            });
            return rleBase;
        }
        public static List<DropdownObj> GetStatus2()
        {
            var rleBase = new List<DropdownObj>();
            rleBase.Add(new DropdownObj()
            {
                Code = "ACTIVE",
                Description = "ACTIVE",
            });
            rleBase.Add(new DropdownObj()
            {
                Code = "LOCK",
                Description = "LOCK",
            });
            rleBase.Add(new DropdownObj()
            {
                Code = "DELETE",
                Description = "DELETE",
            });

            return rleBase;
        }
        public static List<DropdownObj> GetReportList()
        {
            var rleBase = new List<DropdownObj>();
            rleBase.Add(new DropdownObj()
            {
                Code = "1",
                Description = "SETTLEMENT DETAIL",
            });


            return rleBase;
        }
        public static string GetBrowser(HttpRequestBase request)
        {
            var browser = request.Browser;
            string s = "Browser Capabilities\n"
                + "Type = " + browser.Type + "\n"
                + "Name = " + browser.Browser + "\n"
                + "Version = " + browser.Version + "\n";
            //+ "Major Version = " + browser.MajorVersion + "\n"
            //+ "Minor Version = " + browser.MinorVersion + "\n";
            //+ "Platform = " + browser.Platform + "\n"
            //+ "Is Beta = " + browser.Beta + "\n"
            //+ "Is Crawler = " + browser.Crawler + "\n"
            //+ "Is AOL = " + browser.AOL + "\n"
            //+ "Is Win16 = " + browser.Win16 + "\n"
            //+ "Is Win32 = " + browser.Win32 + "\n"
            //+ "Supports Frames = " + browser.Frames + "\n"
            //+ "Supports Tables = " + browser.Tables + "\n"
            //+ "Supports Cookies = " + browser.Cookies + "\n"
            //+ "Supports VBScript = " + browser.VBScript + "\n"
            //+ "Supports JavaScript = " +
            //    browser.EcmaScriptVersion.ToString() + "\n"
            //+ "Supports Java Applets = " + browser.JavaApplets + "\n"
            //+ "Supports ActiveX Controls = " + browser.ActiveXControls
            //      + "\n"
            //+ "Supports JavaScript Version = " +
            //    browser["JavaScriptVersion"] + "\n";
            return s;
        }
        public static DataTable ToDataTable<T>(this IList<T> data)
        {
            PropertyDescriptorCollection properties =
                TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;
        }
        public static string GetStringFromList(List<string> val)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(@"<div style=""color:red;font-size:11px"">");
            foreach (var d in val)
            {
                sb.AppendLine(@"<i class=""fa-arrow-right fa""> </i> " + d + "<br/>");
            }
            sb.AppendLine("</div>");
            var l = sb.ToString();
            return l;
        }

        public static string GetNapsMessage(string respCode)
        {
            switch (respCode)
            {
                case "00":
                    {
                        return "Successful";
                    }
                case "01":
                    {
                        return "Duplicate Upload";
                    }
                case "02":
                    {
                        return "Mandatory Field Not Set";
                    }
                case "05":
                    {
                        return "Format Error";
                    }
                case "06":
                    {
                        return "In Progress";
                    }
                case "16":
                    {
                        return "Request Accepted";
                    }
                case "17":
                    {
                        return "Payment Schedule Not Found";
                    }
                default:
                    {
                        return "Unkown Code.";
                    }
            }
        }
        public static string GetNapsMessage2(string respMessage)
        {
            switch (respMessage)
            {
                case "SUCCESSFUL":
                case "COMPLETE":
                    {
                        return "00";
                    }
                case "PROCESSING":
                case "REQUESTED":
                    {
                        return "16";
                    }
                case "PENDING":
                    {
                        return "06";
                    }
                case "EXPIRED":
                    {
                        return "05";
                    }
                default:
                    {
                        return "Unkown Code.";
                    }
            }
        }
        public static List<string> NapsCode()
        {
            var lsta = new List<string>();
            lsta.Add("16");
            lsta.Add("00");
            return lsta;
        }

        public static string getip()
        {
            string IP = "";
            try
            {
                IP = HttpContext.Current.Request.UserHostAddress;
            }
            catch (Exception) { }
            return IP;
        }
    }


    public class DropdownObj
    {
        public string Code { get; set; }
        public string Description { get; set; }
    }
    public class ReportListObj
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public string reportClass { get; set; }
        public string reporttype { get; set; }
        public string subreporttype { get; set; }
    }
    public class MDropdownObj
    {
        public string MERCHANTID { get; set; }
        public string MERCHANTNAME { get; set; }
        public string DESCRIPTION { get { return string.Concat(MERCHANTID, "-", MERCHANTNAME); } }
    }
}

