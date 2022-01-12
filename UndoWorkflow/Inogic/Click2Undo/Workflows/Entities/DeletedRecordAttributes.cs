using System.Runtime.Serialization;

namespace Inogic.Click2Undo.Workflows.Entities
{
    [DataContract]
    public class DeletedRecordAttributes
    {
        [DataMember] public string AuditDetailId { get; set; }

        [DataMember] public string RecordName { get; set; }

        [DataMember] public string DeletedBy { get; set; }

        [DataMember] public string DeletedOn { get; set; }

        [DataMember] public string FormattedDeletedOn { get; set; }
    }
}