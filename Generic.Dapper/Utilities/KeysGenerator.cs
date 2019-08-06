using MfbNibssFundTransfer.Dapper.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TechClearingProject.Dapper.Data;

namespace TechClearingProject.Dapper.Utilities
{
    public class KeysGenerator
    {
        private readonly IDapperUserSettings _repoUser = new DapperUserSettings();
        private string RNGCharacterMask(int length)
        {
            int maxSize = length;
            //int minSize = 5 ;
            char[] chars = new char[62];
            string a;
            a = "1234567890";
            chars = a.ToCharArray();
            int size = maxSize;
            byte[] data = new byte[1];
            RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider();
            crypto.GetNonZeroBytes(data);
            size = maxSize;
            data = new byte[size];
            crypto.GetNonZeroBytes(data);
            StringBuilder result = new StringBuilder(size);
            foreach (byte b in data)
            { result.Append(chars[b % (chars.Length - 1)]); }
            return result.ToString();
        }

        //public string GenerateUserLoginIdNumber(int length, int bankId)
        //{
        //    string genLoginId = string.Empty;
        //    var allUserLoginId = _repoUser.GetUsers(null).Select(f => f.UserName);
        //    for (int i = 0; i < allUserLoginId.Count(); i++)
        //    {
        //         genLoginId = bankId + RNGCharacterMask(6);
        //        var exist = allUserLoginId.FirstOrDefault(g => g == genLoginId);
        //        if (exist == null)
        //        {
        //            return genLoginId;
        //        }
        //    }

        //    genLoginId = bankId + RNGCharacterMask(6);
        //    return genLoginId;

        //}

        public static string GeneratePasswordPin(int length)
        {
            Random rand = new Random();
            var pin = rand.Next(100000, 999999);
            return pin.ToString();
        }
    }
}
