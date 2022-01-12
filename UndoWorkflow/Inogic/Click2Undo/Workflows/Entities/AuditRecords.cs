using System.Runtime.Serialization;

namespace Inogic.Click2Undo.Workflows.Entities
{
    [DataContract]
    public class AuditRecords
    {
        [DataMember] public AuditDetailAttributes[] AuditDetailAttributes { get; set; }
    }
}