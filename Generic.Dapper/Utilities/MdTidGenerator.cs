
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Configuration;
using System.Data.Common;
using System.Data;
using Generic.Dapper.Repository;
using Generic.Dapper.Model;

namespace Generic.Dapper.Utility
{
    public class MidTidGenerator
    {
        static int cnt = 0;
        public static string GenMid(string prefix, string last_mid)
        {
            try
            {
                if (string.IsNullOrEmpty(last_mid))
                {
                    return "";
                }
                cnt++;

                var alphabetList = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
                // var prefix = string.Concat(type,cbnCode,stateCode);
                var mid = "";
                var otherNo = "";
                decimal dc = 0;
                //  var numberList = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
                if (last_mid.Length == 15)
                {
                    // prefix = last_mid.Substring(0, 5);
                    otherNo = last_mid.Substring(6, 9);
                    if (decimal.TryParse(otherNo, out dc))
                    {
                        // last mid does not contain alphabet
                        dc += 1;
                        if (dc == 999999999)
                        {

                            mid = string.Concat(prefix, "A", "00000001");  // all combination excedded 
                            if (!MidExistDb(mid))
                            {
                                return mid;
                            }
                            else
                            {
                                return GenMid(prefix, mid);
                            }
                        }
                        else
                        {
                            mid = string.Concat(prefix, PadWithZero(dc.ToString(), 9));

                            if (!MidExistDb(mid))
                            {
                                return mid;
                            }
                            else
                            {
                                return GenMid(prefix, mid);
                            }

                        }
                    }
                    else
                    {

                        List<int> alphIdx = new List<int>();
                        foreach (var d in otherNo.ToList())
                        {
                            if (alphabetList.ToList().Contains(d.ToString()))
                            {
                                var idx = alphabetList.ToList().IndexOf(d.ToString());  // get all the index of alphabets before number
                                alphIdx.Add(idx);
                            }
                            else
                            {
                                break; // if you encounter non alphabet exit loop
                            }
                        }
                        int alphabetCnt = alphIdx.Count();
                        int numberCnt = 9 - alphabetCnt;
                        var numberVal = otherNo.Substring(alphabetCnt);
                        var alphVal = otherNo.Substring(0, alphabetCnt);

                        if (decimal.TryParse(numberVal, out dc))
                        {
                            var newNum = dc + 1;
                            if (newNum.ToString().Length == numberVal.Length)
                            {
                                // its ok. concatente the new value with prefix + alphabet
                                mid = string.Concat(prefix, alphVal, newNum);
                                // check if available in database
                                if (!MidExistDb(mid))
                                {
                                    return mid;
                                }
                                else
                                {
                                    return GenMid(prefix, mid);
                                }
                            }
                            else
                            {
                                // the number combination have been excedded, combine with alphabet
                                // var lst
                                //foreach (var d in alphIdx)
                                // {
                                //     if (d ==)
                                // }

                                // 10 

                                for (int i = alphIdx.Count() - 1; i >= 0; i--)
                                {
                                    var gh = alphIdx[i];
                                    if (gh != 25)
                                    {
                                        // then the last alphabet is not "Z"
                                        var newalphIdx = alphIdx[i] + 1;
                                        var alphValNew = string.Concat(alphVal.Substring(0, alphVal.Length - 1), alphabetList[newalphIdx]);
                                        mid = string.Concat(prefix, alphValNew, PadWithZero("1", 8 - i));
                                        if (!MidExistDb(mid))
                                        {
                                            return mid;
                                        }
                                        else
                                        {
                                            return GenMid(prefix, mid);
                                        }


                                    }
                                    //if (i == 0)
                                    //{
                                    // thne all aphabets are letter "Z"...increase alphabet line by 1 and pad 1 with zeros for the remaining length;
                                    // var cnta = alphIdx.Count() == 8 ? alphIdx.Count() : alphIdx.Count() + 1;
                                    var cnta = alphIdx.Count() + 1;
                                    var gA = "";
                                    if (cnta >= 9)
                                    {
                                        gA = string.Concat(GenerateA(cnta), PadWithZero("", 8 - alphIdx.Count()));
                                    }
                                    else
                                    {
                                        gA = string.Concat(GenerateA(cnta), PadWithZero("1", 8 - alphIdx.Count()));
                                    }
                                    mid = string.Concat(prefix, gA);
                                    if (!MidExistDb(mid))
                                    {
                                        return mid;
                                    }
                                    else
                                    {
                                        return GenMid(prefix, mid);
                                    }
                                    //}
                                }
                            }
                        }
                        else
                        {
                            //characters are alpha-numeric
                            if (alphIdx.Count() == 9)
                            {
                                //characters are alphabet
                                for (int i = alphIdx.Count() - 1; i >= 0; i--)
                                {
                                    var gh = alphIdx[i];
                                    if (gh != 25)
                                    {
                                        // then the last alphabet is not "Z"
                                        var newalphIdx = alphIdx[i] + 1;
                                        var alphValNew = string.Concat(alphVal.Substring(0, alphVal.Length - 1), alphabetList[newalphIdx]);
                                        mid = string.Concat(prefix, alphValNew);
                                        if (!MidExistDb(mid))
                                        {
                                            return mid;
                                        }
                                        else
                                        {
                                            return GenMid(prefix, mid);
                                        }
                                    }
                                    if (i == 8)
                                    {
                                        //last character is a Z
                                        for (int j = 0; j <= alphIdx.Count() - 1; i++)
                                        {
                                            var tt = alphIdx[j];
                                            if (tt != 25)
                                            {
                                                // then the last alphabet is not "Z"
                                                var newalphIdx = alphIdx[j] + 1;
                                                var alphValNew = string.Concat(alphVal.Substring(0, alphVal.Length - 1), alphabetList[newalphIdx]);
                                                mid = string.Concat(prefix, alphValNew);
                                                if (!MidExistDb(mid))
                                                {
                                                    return mid;
                                                }
                                                else
                                                {
                                                    return GenMid(prefix, mid);
                                                }
                                            }
                                        }
                                    }
                                    // thne all aphabets are letter "Z"...increase alphabet line by 1 and pad 1 with zeros for the remaining length;
                                    var gA = string.Concat(GenerateA(alphIdx.Count() + 1), PadWithZero("1", 8 - alphIdx.Count()));
                                    mid = string.Concat(prefix, gA);
                                    if (!MidExistDb(mid))
                                    {
                                        return mid;
                                    }
                                    else
                                    {
                                        return GenMid(prefix, mid);
                                    }
                                }

                                //}
                            }
                            else
                            {
                                //characters are alphanumeirc 
                            }
                        }
                    }
                }
                else
                {
                    return "";
                }
                return GenMid(prefix, mid);
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        public static string GenTid(string prefix, string last_mid)
        {
            var alphabetList = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
            // var prefix = string.Concat(type, cbnCode);
            if (string.IsNullOrEmpty(last_mid))
            {
                return "";
            }
        var tid = "";
            var otherNo = "";
            decimal dc = 0;
            //  var numberList = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
            if (last_mid.Length == 8)
            {
                // prefix = last_mid.Substring(0, 5);
                otherNo = last_mid.Substring(4, 4);
                if (decimal.TryParse(otherNo, out dc))
                {
                    // last mid does not contain alphabet
                    dc += 1;
                    if (dc == 9999)
                    {

                        tid = string.Concat(prefix, "A", "001");  // all combination excedded 
                        if (!TidExistDb(tid))
                        {
                            return tid;
                        }
                        else
                        {
                            return GenTid(prefix, tid);
                        }
                    }
                    else
                    {
                        tid = string.Concat(prefix, PadWithZero(dc.ToString(), 4));

                        if (!TidExistDb(tid))
                        {
                            return tid;
                        }
                        else
                        {
                            return GenTid(prefix, tid);
                        }

                    }
                }
                else
                {

                    List<int> alphIdx = new List<int>();
                    foreach (var d in otherNo.ToList())
                    {
                        if (alphabetList.ToList().Contains(d.ToString()))
                        {
                            var idx = alphabetList.ToList().IndexOf(d.ToString());  // get all the index of alphabets before number
                            alphIdx.Add(idx);
                        }
                        else
                        {
                            break; // if you encounter non alphabet exit loop
                        }
                    }
                    int alphabetCnt = alphIdx.Count();
                    int numberCnt = 4 - alphabetCnt;
                    var numberVal = otherNo.Substring(alphabetCnt);
                    var alphVal = otherNo.Substring(0, alphabetCnt);

                    if (decimal.TryParse(numberVal, out dc))
                    {
                        var newNum = dc + 1;
                        if (newNum.ToString().Length == numberVal.Length)
                        {
                            // its ok. concatente the new value with prefix + alphabet
                            tid = string.Concat(prefix, alphVal, newNum);
                            // check if available in database
                            if (!TidExistDb(tid))
                            {
                                return tid;
                            }
                            else
                            {
                                return GenTid(prefix, tid);
                            }
                        }
                        else
                        {
                            // the number combination have been excedded, combine with alphabet
                            // var lst
                            //foreach (var d in alphIdx)
                            // {
                            //     if (d ==)
                            // }

                            // 10 
                            for (int i = alphIdx.Count() - 1; i >= 0; i--)
                            {
                                if (alphIdx[i] != 25)
                                {
                                    // then the last alphabet is not "Z"
                                    var newalphIdx = alphIdx[i] + 1;
                                    var alphValNew = string.Concat(alphVal.Substring(0, alphVal.Length - 1), alphabetList[newalphIdx]);
                                    tid = string.Concat(prefix, alphValNew, PadWithZero("1", 3 - i));
                                    if (!TidExistDb(tid))
                                    {
                                        return tid;
                                    }
                                    else
                                    {
                                        return GenTid(prefix, tid);
                                    }


                                }
                                if (i == 0)
                                {
                                    // thne all aphabets are letter "Z"...increase alphabet line by 1 and pad 1 with zeros for the remaining length;
                                    var gA = string.Concat(GenerateA(alphIdx.Count() + 1), PadWithZero("1", 3 - alphIdx.Count()));
                                    tid = string.Concat(prefix, gA);
                                    if (!TidExistDb(tid))
                                    {
                                        return tid;
                                    }
                                    else
                                    {
                                        return GenTid(prefix, tid);
                                    }
                                }
                            }


                        }
                    }
                    else
                    {
                        return GenTid(prefix, prefix + "3999");
                    }
                }
            }
            else
            {
                return "";
            }
            return GenTid(prefix, tid);
        }
        private static bool MidExistDb(string mid)
        {
            return false;
        }
        private static  bool MidExistDbA(string mid)
        {
            using (var con = new RepoBase().OpenConnection(null))
            {
                var p = new DynamicParameters();
            //    p.Add(":P_INSTITUTION_ITBID", inst_itbid, OracleDbType.Int32);
                p.Add("@P_GEN_MID", mid, DbType.String);

                // p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
                string qry = "SELECT FUNC_MID_EXIST(:P_GEN_MID) RespCode FROM DUAL";  //@"GET_MID_ACCT";

                var rec =  con.Query<OutPutObj>(qry, p, commandType: CommandType.Text).FirstOrDefault();
                if (rec != null)
                {
                    if (rec.RespCode == 0)
                    {
                        // mid does not exist before
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
           
            return true;
        }

        private static bool TidExistDb(string tid)
        {
            using (var con = new RepoBase().OpenConnection(null))
            {
                var p = new DynamicParameters();
                //    p.Add(":P_INSTITUTION_ITBID", inst_itbid, OracleDbType.Int32);
                p.Add("@P_GEN_TID", tid, DbType.String);

                // p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
                string qry = "SELECT FUNC_TID_EXIST(:P_GEN_TID) RespCode FROM DUAL";  //@"GET_MID_ACCT";

                var rec = con.Query<OutPutObj>(qry, p, commandType: CommandType.Text).FirstOrDefault();
                if (rec != null)
                {
                    if (rec.RespCode == 0)
                    {
                        // mid does not exist before
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }

            return true;
        }

        private static string GenerateA(int v)
        {
            var As = "";
            for (int i = 0; i < v; i++)
            {
                As += "A";
            }
            return As;
        }

        private static string PadWithZero(string v, int length)
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

        /// <SUMMARY>Computes the Levenshtein Edit Distance between two enumerables.</SUMMARY>

        /// <TYPEPARAM name=”T”>The type of the items in the enumerables.</TYPEPARAM>

        /// <PARAM name=”x”>The first enumerable.</PARAM>

        /// <PARAM name=”y”>The second enumerable.</PARAM>

        /// <RETURNS>The edit distance.</RETURNS>

        public static int EditDistance<T>(IEnumerable<T> x, IEnumerable<T> y)

            where T : IEquatable<T>

        {

            // Validate parameters

            if (x == null) throw new ArgumentNullException("x");

            if (y == null) throw new ArgumentNullException("y");

            // Convert the parameters into IList instances

            // in order to obtain indexing capabilities

            IList<T> first = x as IList<T> ?? new List<T>(x);

            IList<T> second = y as IList<T> ?? new List<T>(y);


            // Get the length of both.  If either is 0, return

            // the length of the other, since that number of insertions

            // would be required.

            int n = first.Count, m = second.Count;

            if (n == 0) return m;

            if (m == 0) return n;


            // Rather than maintain an entire matrix (which would require O(n*m) space),

            // just store the current row and the next row, each of which has a length m+1,

            // so just O(m) space. Initialize the current row.

            int curRow = 0, nextRow = 1;

            int[][] rows = new int[][] { new int[m + 1], new int[m + 1] };

            for (int j = 0; j <= m; ++j) rows[curRow][j] = j;


            // For each virtual row (since we only have physical storage for two)

            for (int i = 1; i <= n; ++i)

            {

                // Fill in the values in the row

                rows[nextRow][0] = i;

                for (int j = 1; j <= m; ++j)

                {

                    int dist1 = rows[curRow][j] + 1;

                    int dist2 = rows[nextRow][j - 1] + 1;

                    int dist3 = rows[curRow][j - 1] +

                        (first[i - 1].Equals(second[j - 1]) ? 0 : 1);


                    rows[nextRow][j] = Math.Min(dist1, Math.Min(dist2, dist3));

                }


                // Swap the current and next rows

                if (curRow == 0)

                {

                    curRow = 1;

                    nextRow = 0;

                }

                else

                {

                    curRow = 0;

                    nextRow = 1;

                }

            }


            // Return the computed edit distance

            return rows[curRow][m];

        }
    }

    public class StringSift2
    {
        private int maxOffset;

        public StringSift2() : this(5) { }

        public StringSift2(int maxOffset)
        {
            this.maxOffset = maxOffset;
        }

        public float Distance(string s1, string s2)
        {
            if (String.IsNullOrEmpty(s1))
                return
                String.IsNullOrEmpty(s2) ? 0 : s2.Length;
            if (String.IsNullOrEmpty(s2))
                return s1.Length;
            int c = 0;
            int offset1 = 0;
            int offset2 = 0;
            int dist = 0;
            while ((c + offset1 < s1.Length)
            && (c + offset2 < s2.Length))
            {
                if (s1[c + offset1] != s2[c + offset2])
                {
                    offset1 = 0;
                    offset2 = 0;
                    for (int i = 0; i < maxOffset; i++)
                    {
                        if ((c + i < s1.Length)
                        && (s1[c + i] == s2[c]))
                        {
                            if (i > 0)
                            {
                                dist++;
                                offset1 = i;
                            }
                            goto ender;
                        }
                        if ((c + i < s2.Length)
                        && (s1[c] == s2[c + i]))
                        {
                            if (i > 0)
                            {
                                dist++;
                                offset2 = i;
                            }
                            goto ender;
                        }
                    }
                    dist++;
                }
                ender:
                c++;
            }
            return dist + (s1.Length - offset1
            + s2.Length - offset2) / 2 - c;
        }

        public float Similarity(string s1, string s2)
        {
            float dis = Distance(s1, s2);
            int maxLen = Math.Max(s1.Length, s2.Length);
            if (maxLen == 0) return 1;
            else
                return 1 - dis / maxLen;
        }
    }
}
