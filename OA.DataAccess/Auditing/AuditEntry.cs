using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;
using OA.Domin.Auditing;
using System;
using System.Collections.Generic;
using System.Text;

namespace OA.DataAccess.Auditing
{
    public class AuditEntry
    {
        public AuditEntry(EntityEntry entity)
        {
            this.Entry = entity;
        }

        public EntityEntry Entry { get; set; }

        public string TableName { get; set; }
        public string UserId { get; set; }
        public string Operation { get; set; }
        public Dictionary<string, object> OldValues { get; set; } = new Dictionary<string, object>();
        public Dictionary<string, object> NewValues { get; set; } = new Dictionary<string, object>();

        public List<PropertyEntry> TemporaryProperties { get; set; } = new List<PropertyEntry>();

        public bool HasTemporaryProperties => TemporaryProperties.Count > 0;

        public Audit GetAudit()
        {
            return new Audit
            {
                TableName = TableName,
                UserId = UserId,
                Date = DateTime.Now,
                Operation = Operation,
                OldValues = OldValues.Count == 0? null : JsonConvert.SerializeObject(OldValues),
                NewValues = NewValues.Count == 0? null : JsonConvert.SerializeObject(NewValues),
            };
        }
    }
}
