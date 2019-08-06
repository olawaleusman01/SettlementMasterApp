
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Generic.Data
{
    public partial class MyContext : DbContext, IMyContext
    {

        
        static MyContext()
        {
            Database.SetInitializer<MyContext>(null);
        }
        public MyContext() : base("SettlementMasterEntities") { }
      
        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
        //    modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        //    modelBuilder.Configurations
        //    .Add(new EmployeeConfiguration())
        //    .Add(new DepartmentConfiguration());
        //}
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
          
            throw new UnintentionalCodeFirstException();
        }

        public DbSet<SM_AUDIT> DBAudits { get; set; }
        public override int SaveChanges()
        {
            throw new InvalidOperationException("User ID must be provided");
        }
        public int SaveChanges(string userId,string authId)
        {
            //try
            //{

            // Get all Added/Deleted/Modified entities (not Unmodified or Detached)
            foreach (var ent in this.ChangeTracker.Entries().Where(p => p.State == EntityState.Deleted || p.State == EntityState.Modified))
            {
                // For each changed record, get the audit record entries and add them
                foreach (SM_AUDIT x in GetAuditRecordsForChange(ent, userId,authId))
                {
                    this.DBAudits.Add(x);
                }
            }

            // var gh = base.SaveChanges();
            // return gh;
            // Call the original SaveChanges(), which will save both the changes made and the audit records
            return base.SaveChanges();




            //}
            //catch (Exception ex)
            //{
            //}

            //return 0;
        }

        private List<SM_AUDIT> GetAuditRecordsForChange(DbEntityEntry dbEntry, string userId, string authId)
        {
            List<SM_AUDIT> result = new List<SM_AUDIT>();
            try
            {
                DateTime changeTime = DateTime.Now;

                // Get the Table() attribute, if one exists
                TableAttribute tableAttr = dbEntry.Entity.GetType().GetCustomAttributes(typeof(TableAttribute), false).FirstOrDefault() as TableAttribute;
                var ipAddress = getip();
                // TableAttribute tableAttr = dbEntry.Entity.GetType().GetCustomAttributes(typeof(TableAttribute), true).FirstOrDefault() as TableAttribute;

                // Get table name (if it has a Table attribute, use that, otherwise get the pluralized name)
                //string tableName = tableAttr != null ? tableAttr.Name : dbEntry.Entity.GetType().Name;
                var tableName = ObjectContext.GetObjectType(dbEntry.Entity.GetType()).Name;
                // Get primary key value (If you have more than one key column, this will need to be adjusted)
                var keyNames = dbEntry.Entity.GetType().GetProperties().Where(p => p.GetCustomAttributes(typeof(KeyAttribute), false).Count() > 0).ToList();

                string keyName = keyNames.Count > 0 ? keyNames[0].Name : null; //dbEntry.Entity.GetType().GetProperties().Single(p => p.GetCustomAttributes(typeof(KeyAttribute), false).Count() > 0).Name;

                if (keyName != null)
                {
                    if (dbEntry.State == EntityState.Added)
                    {
                        // For Inserts, just add the whole record
                        // If the entity implements IDescribableEntity, use the description from Describe(), otherwise use ToString()

                        foreach (string propertyName in dbEntry.CurrentValues.PropertyNames)
                        {
                            result.Add(new SM_AUDIT()
                            {
                                //AUDITLOGID = Guid.NewGuid(),
                                USERID = userId,
                                EVENTDATE = changeTime,
                                EVENTTYPE = "A", // Deleted
                                TABLENAME = tableName,
                                RECORDID = dbEntry.OriginalValues.GetValue<object>(keyName).ToString(),
                                COLUMNNAME = propertyName,
                                NEWVALUE = dbEntry.CurrentValues.GetValue<object>(propertyName) == null ? null : dbEntry.CurrentValues.GetValue<object>(propertyName).ToString(),
                                IPADDRESS = ipAddress,

                            }
                                    );
                        }
                    }
                    else if (dbEntry.State == EntityState.Deleted)
                    {
                        //  Same with deletes, do the whole record, and use either the description from Describe() or ToString()
                        result.Add(new SM_AUDIT()
                        {
                            //AuditlogId = Guid.NewGuid(),
                            USERID = userId,
                            EVENTDATE = changeTime,
                            EVENTTYPE = "D", // Deleted
                            TABLENAME = tableName,
                            RECORDID = dbEntry.OriginalValues.GetValue<object>(keyName).ToString(),
                            COLUMNNAME = "*ALL",
                            NEWVALUE = "yes" ,// (dbEntry.OriginalValues.ToObject() is IDescribableEntity) ? (dbEntry.OriginalValues.ToObject() as IDescribableEntity).Describe() : dbEntry.OriginalValues.ToObject().ToString()
                            IPADDRESS = ipAddress,
                        }
                            );
                    }
                    else if (dbEntry.State == EntityState.Modified)
                    {
                        foreach (string propertyName in dbEntry.OriginalValues.PropertyNames)
                        {
                            //  For updates, we only want to capture the columns that actually changed
                            //if (!object.Equals(dbEntry.OriginalValues.GetValue<object>(propertyName), dbEntry.CurrentValues.GetValue<object>(propertyName)))
                            //{
                            var gf = dbEntry.OriginalValues.GetValue<object>(propertyName) == null ? null : dbEntry.OriginalValues.GetValue<object>(propertyName).ToString();
                            var ga = dbEntry.CurrentValues.GetValue<object>(propertyName) == null ? null : dbEntry.CurrentValues.GetValue<object>(propertyName).ToString();
                            if (gf != ga && !ExcemptList().Contains(propertyName))
                            {
                                result.Add(new SM_AUDIT()
                                {
                                    //AuditlogId = Guid.NewGuid(),
                                    USERID = userId,
                                    EVENTDATE = changeTime,
                                    EVENTTYPE = "M", // Deleted
                                    TABLENAME = tableName,
                                    RECORDID = dbEntry.OriginalValues.GetValue<object>(keyName).ToString(),
                                    COLUMNNAME = propertyName,
                                    NEWVALUE = ga, // dbEntry.CurrentValues.GetValue<object>(propertyName) == null ? null : dbEntry.CurrentValues.GetValue<object>(propertyName).ToString(),
                                    ORIGINALVALUE = gf,
                                    IPADDRESS = ipAddress,

                                }
                                    );
                            }
                            // }
                        }
                    }
                }
                //  Otherwise, don't do anything, we don't care about Unchanged or Detached entities


            }
            catch (Exception ex)
            {
            }

            return result;
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
        List<string> ExcemptList()
        {
            var lst = new List<string>();
            lst.Add("LAST_MODIFIED_DATE");
            lst.Add("LAST_MODIFIED_UID");
            lst.Add("LAST_MODIFIED_AUTHID");
            return lst;
        }
    }
}
