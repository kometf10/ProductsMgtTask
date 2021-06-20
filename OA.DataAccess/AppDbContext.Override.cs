using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyModel;
using OA.DataAccess.Auditing;
using OA.Domin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OA.DataAccess
{
    public partial class AppDbContext
    {

        private readonly static bool FullAuditOn = true;
        private readonly static bool SafeDelete = true;
        private readonly static List<string> BasicAuditValues = new List<string> { nameof(BaseEntity.CreatedAt), nameof(BaseEntity.CreatedBy), nameof(BaseEntity.LastModefiedAt), nameof(BaseEntity.LastModifiedBy) };

        public void SetGlobalQuery<T>(ModelBuilder modelBuilder) where T : BaseEntity
        {
            modelBuilder.Entity<T>().HasKey(e => e.Id);
            modelBuilder.Entity<T>().HasQueryFilter(e => !e.IsDeleted);
        }

        public static MethodInfo SetGlobalQueryMethod = typeof(AppDbContext).GetMethods(BindingFlags.Public | BindingFlags.Instance)
                                                                            .Single(t => t.IsGenericMethod && t.Name == "SetGlobalQuery");

        private static IList<Type> EntityTypeCache;

        public static IList<Type> GetEntityTypes()
        {
            if (EntityTypeCache != null)
                return EntityTypeCache.ToList();
            else
            {
                EntityTypeCache = (from a in GetReferencingAssemblies()
                                   from t in a.DefinedTypes
                                   where t.BaseType == typeof(BaseEntity)
                                   select t.AsType()).ToList();

                return EntityTypeCache;
            }
        }

        private static IEnumerable<Assembly> GetReferencingAssemblies()
        {
            var assemblies = new List<Assembly>();
            var dependencies = DependencyContext.Default.RuntimeLibraries;

            foreach(var library in dependencies)
            {
                try
                {
                    var assembly = Assembly.Load(new AssemblyName(library.Name));
                    assemblies.Add(assembly);
                }
                catch (FileNotFoundException) { }
               
            }
            return assemblies;
        }

        public string GetLoggedUser()
        {
            var httpContext = httpContextAccessor.HttpContext;
            if(httpContext != null)
            {
                if(httpContext.User != null)
                {
                    var user = httpContext.User.FindFirst("Id");
                    if (user != null)
                        return user.Value;
                }
            }
            return null;
        }


        private void BasicAudit()
        {
            var entries = ChangeTracker.Entries();
            foreach (var entry in entries)
                if (entry.Entity is BaseEntity trackable)
                {
                    var now = DateTime.UtcNow;
                    var userId = GetLoggedUser();
                    var createdAt = entry.OriginalValues.GetValue<DateTime?>("CreatedAt");
                    var createdBy = entry.OriginalValues.GetValue<string>("CreatedBy");
                    switch (entry.State)
                    {
                        case EntityState.Modified:
                            trackable.LastModefiedAt = now;
                            trackable.LastModifiedBy = userId;
                            trackable.CreatedAt = createdAt;
                            trackable.CreatedBy = createdBy;
                            break;
                        case EntityState.Added:
                            trackable.CreatedAt = now;
                            trackable.CreatedBy = userId;
                            break;
                        case EntityState.Deleted:
                            if (SafeDelete)
                            {
                                entry.State = EntityState.Modified;
                                trackable.LastModefiedAt = now;
                                trackable.LastModifiedBy = userId;
                                trackable.CreatedAt = createdAt;
                                trackable.CreatedBy = createdBy;
                                trackable.IsDeleted = true;
                            }
                            break;
                    }
                }
        }

        private List<AuditEntry> FullAuditBefore()
        {
            var auditEntries =  new List<AuditEntry>();

            var entries = ChangeTracker.Entries();
            var userId = GetLoggedUser();
            foreach(var entry in entries)
            {
                if (!(entry.Entity is BaseEntity) || entry.State == EntityState.Unchanged || entry.State == EntityState.Detached)
                    continue;

                var auditEntry = new AuditEntry(entry);
                auditEntry.TableName = entry.Entity.GetType().Name;
                auditEntry.UserId = userId;
                auditEntry.Operation = Enum.GetName(typeof(EntityState), entry.State);

                //In Case of Safe Delete Is On
                bool isDeleted = false;
                if (entry.State == EntityState.Modified)
                {                    
                    isDeleted = entry.CurrentValues.GetValue<bool>("IsDeleted");
                    if (isDeleted)
                    {                        
                        auditEntry.Operation = "Deleted";
                    }
                }

                var properties = entry.Properties;
                foreach(var property in properties)
                {
                    string propertyName = property.Metadata.Name;

                    //Ignore basic audit values
                    if (BasicAuditValues.Contains(propertyName))
                        continue;
                    
                    if (property.IsTemporary)
                    {
                        auditEntry.TemporaryProperties.Add(property);
                        continue;
                    }
                    switch (entry.State)
                    {
                        case EntityState.Added:
                            auditEntry.NewValues[propertyName] = property.CurrentValue;
                            break;
                        
                        //Not Hitted While Safe Delete Is Active 
                        case EntityState.Deleted:
                            auditEntry.OldValues[propertyName] = property.OriginalValue;
                            break;

                        case EntityState.Modified:
                            auditEntry.OldValues[propertyName] = entry.GetDatabaseValues()?.GetValue<object>(propertyName);
                            if(!isDeleted)
                                auditEntry.NewValues[propertyName] = property.CurrentValue;
                            
                            break;
                    }
                }

                auditEntries.Add(auditEntry);
            }

            foreach(var auditEntry in auditEntries.Where(_ => !_.HasTemporaryProperties))
            {
                Audits.Add(auditEntry.GetAudit());
            }

            return auditEntries.Where(_ => _.HasTemporaryProperties).ToList();
        }

        private Task FullAuditAfter(List<AuditEntry> auditEntries)
        {
            if (auditEntries == null || auditEntries.Count == 0)
                return Task.CompletedTask;

            foreach(var auditEntry in auditEntries)
            {
                foreach (var prop in auditEntry.TemporaryProperties)
                {
                    auditEntry.NewValues[prop.Metadata.Name] = prop.CurrentValue;                    
                }

                Audits.Add(auditEntry.GetAudit());
            }

            return SaveChangesAsync();
        }

        private List<AuditEntry> OnBeforeSaving()
        {
            BasicAudit();

            if (FullAuditOn)
            {
                var auditEntities = FullAuditBefore();
                return auditEntities;
            }
            return null;
        }

        private void OnAfterSaving(List<AuditEntry> auditEntities)
        {
            if(FullAuditOn)
                FullAuditAfter(auditEntities);
        }


        public override int SaveChanges()
        {
            var auditEntites = OnBeforeSaving();

            var result = base.SaveChanges();

            OnAfterSaving(auditEntites);

            return result;
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var auditEntity = OnBeforeSaving();
            var result = await base.SaveChangesAsync(cancellationToken);
            OnAfterSaving(auditEntity);

            return result;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach(Type type in GetEntityTypes())
            {
                var method = SetGlobalQueryMethod.MakeGenericMethod(type);
                method.Invoke(this, new object[] { modelBuilder });
            }

            //Set Delete Behavior If Safe Delete not Applyed
            //var relationships = modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys());
            //foreach (var relationship in relationships)            
            //    relationship.DeleteBehavior = DeleteBehavior.Cascade;
            

            base.OnModelCreating(modelBuilder);
        }

    }
}
