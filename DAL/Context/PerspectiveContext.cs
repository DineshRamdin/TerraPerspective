using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DAL.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using DAL.Common;
using DAL.Models.Administration;


namespace DAL.Context
{
    public class PerspectiveContext : AuditContext<ApplicationUser, ApplicationRole, string>
    {
        public PerspectiveContext()
        {

        }

        private readonly string _username;

        public PerspectiveContext(DbContextOptions<PerspectiveContext> options, IHttpContextAccessor httpContextAccessor)
            : base(options)
        {
            var claimsPrincipal = httpContextAccessor.HttpContext?.User;
            _username = claimsPrincipal?.Claims?.SingleOrDefault(c => c.Type == "username")?.Value ?? "Unauthenticated user";
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                //optionsBuilder.UseLazyLoadingProxies();
                optionsBuilder.UseSqlServer(new ApplicationSettingsHandler().settings.ConnectionStrings.GetValueOrDefault(Databases.PerspectiveConnection), builder => builder.EnableRetryOnFailure());
                optionsBuilder.UseSqlServer(new ApplicationSettingsHandler().settings.ConnectionStrings.GetValueOrDefault(Databases.PerspectiveConnection), x => x.UseNetTopologySuite());
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<AuditEntry>().Property(ae => ae.Changes).HasConversion(value => JsonConvert.SerializeObject(value), serializedValue => JsonConvert.DeserializeObject<Dictionary<string, object>>(serializedValue));
            builder.Entity<AuditEntry>().Property(ae => ae.OriginalValues).HasConversion(value => JsonConvert.SerializeObject(value), serializedValue => JsonConvert.DeserializeObject<Dictionary<string, object>>(serializedValue));

            base.OnModelCreating(builder);
        }

        #region AUDITMethod
        public async override Task<int> SaveChangesAsync()
        {
            // Get audit entries
            IHttpContextAccessor httpContextAccessor = new HttpContextAccessor();
            string claimsPrincipal = null;
            Guid gidc = Guid.Empty;
            if (httpContextAccessor.HttpContext != null)
            {
                claimsPrincipal = httpContextAccessor.HttpContext.User.Identity.Name;
            }
            else
            {
                claimsPrincipal = this.Users.Where(x => x.Email.ToLower() == "admin@gmail.com").Select(x => x.Id).FirstOrDefault();
            }

            var UGuid = this.Users.Where(x => x.UserName == claimsPrincipal).Select(x => x.Id).FirstOrDefault();
            if (UGuid == null || UGuid == String.Empty)
            {
                try
                {
                    var checkAdded = this.ChangeTracker.Entries().Where(t => t.State == EntityState.Added).Select(t => t.Entity).ToArray();
                    foreach (var entity in checkAdded)
                    {
                        if (entity is BaseAuditModel)
                        {
                            var track = entity as BaseAuditModel;
                            if (track.CreatedBy != gidc)
                            {
                                UGuid = track.CreatedBy.ToString();
                            }

                        }
                    }
                    var modified = this.ChangeTracker.Entries().Where(t => t.State == EntityState.Modified).Select(t => t.Entity).ToArray();

                    foreach (var entity in modified)
                    {
                        if (entity is BaseAuditModel)
                        {
                            var track = entity as BaseAuditModel;
                            if (track.UpdatedBy != gidc)
                            {
                                UGuid = track.CreatedBy.ToString();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }
            }
            var auditEntries = OnBeforeSaveChanges(UGuid);
            if (UGuid != null && UGuid != String.Empty)
            {
                Guid Userguid = Guid.Parse(UGuid);
                this.ChangeTracker.DetectChanges();
                var added = this.ChangeTracker.Entries()
                            .Where(t => t.State == EntityState.Added)
                            .Select(t => t.Entity)
                            .ToArray();

                foreach (var entity in added)
                {
                    if (entity is BaseAuditModel)
                    {
                        var track = entity as BaseAuditModel;
                        track.CreatedDate = DateTime.Now;
                        track.CreatedBy = Userguid;
                    }
                }

                var modified = this.ChangeTracker.Entries()
                            .Where(t => t.State == EntityState.Modified)
                            .Select(t => t.Entity)
                            .ToArray();

                foreach (var entity in modified)
                {
                    if (entity is BaseAuditModel)
                    {
                        var track = entity as BaseAuditModel;
                        track.UpdatedDate = DateTime.Now;
                        track.UpdatedBy = Userguid;
                    }
                }
            }
            // Save current entity
            var result = await base.SaveChangesAsync();

            // Save audit entries
            OnAfterSaveChangesAsync(auditEntries.AUL);
            return result;
        }
        //public static string Sess()
        //{
        //    ISession session = HttpContext.Session;
        //    session.SetString("Username", "ffff");
        //}
        public override int SaveChanges()
        {
            IHttpContextAccessor httpContextAccessor = new HttpContextAccessor();
            string claimsPrincipal = null;
            if (httpContextAccessor.HttpContext != null)
            {
                claimsPrincipal = httpContextAccessor.HttpContext.User.Identity.Name;
            }
            else
            {
                claimsPrincipal = this.Users.Where(x => x.Email.ToLower() == "admin@gmail.com").Select(x => x.Id).FirstOrDefault();
            }
            Guid gidc = Guid.Empty;
            DateTime dtc = DateTime.MinValue;
            var UGuid = this.Users.Where(x => x.UserName == claimsPrincipal).Select(x => x.Id).FirstOrDefault();

            if (UGuid == null || UGuid == String.Empty)
            {
                try
                {
                    var checkAdded = this.ChangeTracker.Entries().Where(t => t.State == EntityState.Added).Select(t => t.Entity).ToArray();
                    foreach (var entity in checkAdded)
                    {
                        if (entity is BaseAuditModel)
                        {
                            var track = entity as BaseAuditModel;
                            if (track.CreatedBy != gidc)
                            {
                                UGuid = track.CreatedBy.ToString();
                            }

                        }
                    }
                    var modified = this.ChangeTracker.Entries().Where(t => t.State == EntityState.Modified).Select(t => t.Entity).ToArray();

                    foreach (var entity in modified)
                    {
                        if (entity is BaseAuditModel)
                        {
                            var track = entity as BaseAuditModel;
                            if (track.CreatedBy != gidc)
                            {
                                UGuid = track.CreatedBy.ToString();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }
            }
            // string
            if ((UGuid == null || UGuid == String.Empty || UGuid == "00000000-0000-0000-0000-000000000000") && claimsPrincipal != null)
            {
                UGuid = this.Users.Where(x => x.UserName == claimsPrincipal).Select(x => x.Id).FirstOrDefault();
            }
            // Get audit entries
            var auditEntries = OnBeforeSaveChanges(UGuid);
            if (UGuid != null && UGuid != String.Empty)
            {
                Guid Userguid = Guid.Parse(UGuid);
                this.ChangeTracker.DetectChanges();
                var added = this.ChangeTracker.Entries()
                            .Where(t => t.State == EntityState.Added)
                            .Select(t => t.Entity)
                            .ToArray();


                foreach (var entity in added)
                {
                    if (entity is BaseAuditModel)
                    {
                        var track = entity as BaseAuditModel;
                        if (track.CreatedDate == dtc)
                        {
                            track.CreatedDate = DateTime.Now;
                        }
                        if (track.CreatedBy == gidc)
                        {
                            track.CreatedBy = Userguid;
                        }

                    }
                }

                var modified = this.ChangeTracker.Entries()
                            .Where(t => t.State == EntityState.Modified)
                            .Select(t => t.Entity)
                            .ToArray();

                foreach (var entity in modified)
                {
                    if (entity is BaseAuditModel)
                    {
                        var track = entity as BaseAuditModel;
                        if (track.UpdatedDate == dtc)
                        {
                            track.UpdatedDate = DateTime.Now;
                        }
                        if (track.UpdatedBy == gidc)
                        {
                            track.UpdatedBy = Userguid;
                        }
                        track.UpdatedDate = DateTime.Now;
                        track.UpdatedBy = Userguid;
                        if (track.CreatedBy == null || track.CreatedBy == new Guid())
                        {
                            if (auditEntries.CreatedBy == null)
                            {
                                track.CreatedBy = new Guid();
                            }
                            else
                            {
                                track.CreatedBy = Guid.Parse(auditEntries.CreatedBy.ToString());
                            }
                        }
                        else
                        {
                            track.CreatedBy = track.CreatedBy;
                        }

                        if (track.CreatedDate == null || track.CreatedDate == DateTime.MinValue)
                        {
                            track.CreatedDate = Convert.ToDateTime(auditEntries.CreatedDate);
                        }
                        else
                        {
                            track.CreatedDate = track.CreatedDate;
                        }
                    }
                }

            }
            // Save current entity
            var result = base.SaveChanges();

            // Save audit entries
            OnAfterSaveChangesAsync(auditEntries.AUL);
            return result;
        }
        public Dictionary<string, object> newval { get; set; }
        public Dictionary<string, object> ori { get; set; }
        public class Auditdto
        {
            public List<AuditEntry> AUL { get; set; } = new List<AuditEntry>();
            public DateTime? CreatedDate { get; set; }
            public Guid? CreatedBy { get; set; }
        }
        private Auditdto OnBeforeSaveChanges(string UGuid)
        {
            ChangeTracker.DetectChanges();
            var entries = new List<AuditEntry>();
            Auditdto adto = new Auditdto();

            foreach (var entry in ChangeTracker.Entries())
            {
                // Prevent Circular Dependencies
                if (entry.State == EntityState.Detached || entry.State == EntityState.Unchanged || !(entry.Entity is IAuditable))
                {
                    continue;
                }
                else if (entry.State == EntityState.Added)
                {
                    newval = entry.Properties.Select(p => new { p.Metadata.Name, p.CurrentValue }).ToDictionary(i => i.Name, i => i.CurrentValue);
                    newval["CreatedBy"] = UGuid;
                    newval["CreatedDate"] = DateTime.Now;
                    var auditEntry = new AuditEntry
                    {
                        ActionType = "INSERT",
                        EntityId = entry.Properties.Single(p => p.Metadata.IsPrimaryKey()).CurrentValue.ToString(),
                        EntityName = entry.Metadata.ClrType.Name,
                        UserId = UGuid == null ? "System-Generated" : UGuid == string.Empty ? "System-Generated" : UGuid,
                        TimeStamp = DateTime.UtcNow,
                        OriginalValues = newval,
                        Changes = newval,
                        TempProperties = entry.Properties.Where(p => p.IsTemporary).ToList(),
                    };

                    entries.Add(auditEntry);
                }
                else if (entry.State == EntityState.Modified || entry.State == EntityState.Deleted)
                {
                    Dictionary<string, object> ori = new Dictionary<string, object>();
                    Dictionary<string, object> newval = new Dictionary<string, object>();
                    Dictionary<string, object> cori = new Dictionary<string, object>();
                    Dictionary<string, object> cnewval = new Dictionary<string, object>();
                    newval = entry.Properties.Select(p => new { p.Metadata.Name, p.CurrentValue }).ToDictionary(i => i.Name, i => i.CurrentValue);
                    //ori = entry.Properties.Select(p => new { p.Metadata.Name, p.OriginalValue }).ToDictionary(i => i.Name, i => i.OriginalValue);
                    newval["UpdatedBy"] = UGuid;
                    newval["UpdatedDate"] = DateTime.Now;
                    var modifiedEntities = ChangeTracker.Entries().Where(p => p.State == EntityState.Modified).ToList();
                    var now = DateTime.UtcNow;

                    foreach (var change in modifiedEntities)
                    {
                        var entityName = change.Entity.GetType().Name;

                        foreach (var prop in change.OriginalValues.Properties)
                        {
                            var value = change.GetDatabaseValues().GetValue<object>(prop.Name);
                            if (prop.Name == "CreatedBy")
                            {
                                adto.CreatedBy = Guid.Parse(value.ToString());
                            }
                            if (prop.Name == "CreatedDate")
                            {
                                adto.CreatedDate = Convert.ToDateTime(value.ToString());
                            }
                            if (ori.Where(x => x.Key == prop.Name).Any())
                            {
                                ori.Add(entityName + prop.Name, value);
                            }
                            else
                            {
                                ori.Add(prop.Name, value);
                            }

                        }
                    }
                    foreach (var x in newval)
                    {
                        string name = x.Key;
                        string newv = newval[x.Key] != null ? newval[x.Key].ToString() : "";
                        string oriv = string.Empty;
                        if (entry.State == EntityState.Deleted)
                        {
                            oriv = newv;
                        }
                        else
                        {
                            oriv = ori[x.Key] != null ? ori[x.Key].ToString() : "";
                        }

                        if (newv != oriv && x.Key != "CreatedBy" && x.Key != "CreatedDate")
                        {
                            cori.Add(x.Key, oriv);
                            cnewval.Add(x.Key, newv);
                        }
                    }
                    var auditEntry = new AuditEntry
                    {
                        ActionType = entry.State == EntityState.Deleted ? "DELETE" : "UPDATE",
                        EntityId = entry.Properties.Single(p => p.Metadata.IsPrimaryKey()).CurrentValue.ToString(),
                        EntityName = entry.Metadata.ClrType.Name,
                        UserId = UGuid == null ? "System-Generated" : UGuid == string.Empty ? "System-Generated" : UGuid,
                        TimeStamp = DateTime.UtcNow,
                        OriginalValues = cori,
                        Changes = cnewval,
                        TempProperties = entry.Properties.Where(p => p.IsTemporary).ToList(),
                    };

                    entries.Add(auditEntry);
                }

            }
            adto.AUL = entries;

            return adto;
        }

        private async void OnAfterSaveChangesAsync(List<AuditEntry> auditEntries)
        {
            if (auditEntries == null || auditEntries.Count == 0)
            {
                return;
            }
            PerspectiveContext fc = new PerspectiveContext();
            //return Task.CompletedTask;

            foreach (var entry in auditEntries)
            {
                foreach (var prop in entry.TempProperties)
                {
                    if (prop.Metadata.IsPrimaryKey())
                    {
                        entry.EntityId = prop.CurrentValue.ToString();
                        entry.Changes[prop.Metadata.Name] = prop.CurrentValue;
                        entry.OriginalValues[prop.Metadata.Name] = prop.OriginalValue;
                    }
                    else
                    {
                        entry.Changes[prop.Metadata.Name] = prop.CurrentValue;
                        entry.OriginalValues[prop.Metadata.Name] = prop.OriginalValue;
                    }
                }
            }

            fc.AuditEntries.AddRange(auditEntries);
            fc.SaveChangesAsync();
        }
        #endregion

        public DbSet<AuditEntry> AuditEntries { get; set; }
        public DbSet<SYS_User> SYS_User { get; set; }
        public DbSet<SYS_UserDetails> SYS_UserDetails { get; set; }
        public DbSet<SYS_Poster> SYS_Poster { get; set; }
        public DbSet<SYS_Carousel> SYS_Carousel { get; set; }
        public DbSet<SYS_CarouselPosterMapping> SYS_CarouselPosterMapping { get; set; }

        public DbSet<SYS_Room> SYS_Room { get; set; }
        public DbSet<SYS_Resource> SYS_Resource { get; set; }
        public DbSet<SYS_AccessLog> SYS_AccessLog { get; set; }
        public DbSet<SYS_Device> SYS_Device { get; set; }
        public DbSet<SYS_LookUpType> SYS_LookUpType { get; set; }
        public DbSet<SYS_LookUpValue> SYS_LookUpValue { get; set; }
        public DbSet<SYS_GeomertyData> SYS_GeomertyData { get; set; }
        public DbSet<SYS_Testing> SYS_Testing { get; set; }
        public DbSet<SYS_GlobalParam> SYS_GlobalParam { get; set; }
        public DbSet<SYS_Modules> SYS_Modules { get; set; }
        public DbSet<SYS_AccessRights> SYS_AccessRights { get; set; }
        public DbSet<SYS_AccessRightsDetails> SYS_AccessRightsDetails { get; set; }
        public DbSet<SYS_GroupMatrix> SYS_GroupMatrix { get; set; }
        public DbSet<SYS_GroupMatrixUser> SYS_GroupMatrixUser { get; set; }
        public DbSet<SYS_RowAccess> SYS_RowAccess { get; set; }
        public DbSet<SYS_Reports> SYS_Reports { get; set; }
        public DbSet<SYS_SystemIcon> SYS_SystemIcon { get; set; }

    }
}
