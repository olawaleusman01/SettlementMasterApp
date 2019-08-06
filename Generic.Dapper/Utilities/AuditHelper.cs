using Generic.Dapper.Repository;
using Dapper;
using System.Data;
using Generic.Data;
using System.Reflection;
using System;
using Generic.Data.Utilities;

namespace Generic.Dapper.Utilities
{
    public static class AuditHelper 
    {
        public static int PostAudit2(SM_AUDIT obj)
        {
            var p = new DynamicParameters();
            p.Add("COLUMNNAME", obj.COLUMNNAME, DbType.String);
            p.Add("AUTHID", obj.AUTHID, DbType.String);
            p.Add("EVENTDATE", obj.EVENTDATE, DbType.DateTime);
            p.Add("EVENTTYPE", obj.EVENTTYPE, DbType.String);
            p.Add("INSTITUTION_ITBID", obj.INSTITUTION_ITBID, DbType.Int32);
            p.Add("NEWVALUE", obj.NEWVALUE, DbType.String);
            p.Add("ORIGINALVALUE", obj.ORIGINALVALUE, DbType.String);
            p.Add("RECORDID", obj.RECORDID, DbType.String);
            p.Add("TABLENAME", obj.TABLENAME, DbType.String);
            p.Add("USERID", obj.USERID, DbType.String);
            p.Add("IPADDRESS", SmartObj.getip(), DbType.String);
            using (var con = new RepoBase().OpenConnection(null))
            {
                var TT = con.Execute("POST_AUDITTRAIL", p, commandType:CommandType.StoredProcedure);
                return TT;
            }
        }
        public static int PostAudit(object oldRecord, object newRecord,string postType,string userId,string authId,DateTime eventDate,string key,string tableName,int instId)
        {
            var cnt = 0;
            FieldInfo[] fieldsA;
            FieldInfo[] fieldsB; // newRecord.GetType().GetFields();
            if (postType == "I")
            {
                if(newRecord == null)
                {
                    return 0;
                }
                //var gh = newRecord.GetType().GetFields();
                 fieldsB = newRecord.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                using (var con = new RepoBase().OpenConnection(null))
                {

                    for (int i = 0; i < fieldsB.Length; i++)
                    {
                        var val = fieldsB[i].GetValue(newRecord);
                        var name = fieldsB[i].Name;
                        var p = new DynamicParameters();
                        p.Add("COLUMNNAME", name, DbType.String);
                        p.Add("AUTHID", authId, DbType.String);
                        p.Add("EVENTDATE", eventDate, DbType.DateTime);
                        p.Add("EVENTTYPE", postType, DbType.String);
                        p.Add("INSTITUTION_ITBID", instId, DbType.Int32);
                        p.Add("NEWVALUE", val, DbType.String);
                        p.Add("ORIGINALVALUE", null, DbType.String);
                        p.Add("RECORDID", key, DbType.String);
                        p.Add("TABLENAME", tableName, DbType.String);
                        p.Add("USERID", userId, DbType.String);
                        p.Add("IPADDRESS", SmartObj.getip(), DbType.String);

                        cnt = con.Execute("POST_AUDITTRAIL", p, commandType: CommandType.StoredProcedure);
                      
                    }
                }
            }
            else if (postType == "M")
            {
                fieldsA =  oldRecord.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                fieldsB = newRecord.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                using (var con = new RepoBase().OpenConnection(null))
                {
                    for (int i = 0; i < fieldsA.Length; i++)
                    {
                        object a = 1;
                        object b = 1;
                        if (a.Equals(b))
                        {
                            object c = 2;
                        }
                        // var b = fieldsB[i].GetValue(newRecord);

                        var newVal = fieldsB[i].GetValue(newRecord);
                        var oldVal = fieldsA[i].GetValue(oldRecord);
                        var name = fieldsB[i].Name;
                        if ((oldVal != null && newVal != null && !oldVal.Equals(newVal)) || (oldVal == null && newVal != null) || (oldVal != null && newVal == null))
                        {
                            //var tt1 = newVal.GetType();
                            //var tt2 = newVal.GetType();
                            var p = new DynamicParameters();
                            p.Add("COLUMNNAME", name, DbType.String);
                            p.Add("AUTHID", authId, DbType.String);
                            p.Add("EVENTDATE", eventDate, DbType.DateTime);
                            p.Add("EVENTTYPE", postType, DbType.String);
                            p.Add("INSTITUTION_ITBID", instId, DbType.Int32);
                            p.Add("NEWVALUE", newVal, DbType.String);
                            p.Add("ORIGINALVALUE", oldVal, DbType.String);
                            p.Add("RECORDID", key, DbType.String);
                            p.Add("TABLENAME", tableName, DbType.String);
                            p.Add("USERID", userId, DbType.String);
                            p.Add("IPADDRESS", SmartObj.getip(), DbType.String);

                            cnt = con.Execute("POST_AUDITTRAIL", p, commandType: CommandType.StoredProcedure);
                        }
                    }
                }
            }
            return cnt;
        }
    }
}
