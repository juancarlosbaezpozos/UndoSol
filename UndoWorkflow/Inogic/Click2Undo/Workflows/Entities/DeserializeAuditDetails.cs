using System.Runtime.Serialization;

namespace Inogic.Click2Undo.Workflows.Entities
{
    [DataContract]
    public class DeserializeAuditDetails
    {
        [DataMember] public ChangedAuditRecords[] ChangedAuditRecords { get; set; }
    }
}