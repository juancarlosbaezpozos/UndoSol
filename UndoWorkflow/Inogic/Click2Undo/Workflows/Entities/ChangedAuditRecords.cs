using System.Runtime.Serialization;

namespace Inogic.Click2Undo.Workflows.Entities
{
    [DataContract]
    public class ChangedAuditRecords
    {
        [DataMember] public string AuditDetailId { get; set; }

        [DataMember] public string ChangedFieldName { get; set; }

        [DataMember] public string ChangedDate { get; set; }
    }
}