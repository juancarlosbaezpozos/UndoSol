using System.Runtime.Serialization;

namespace Inogic.Click2Undo.Workflows.Entities
{
    [DataContract]
    public class AuditDetailAttributes
    {
        [DataMember] public string AuditDetailId { get; set; }

        [DataMember] public string ChangedDate { get; set; }

        [DataMember] public string FormattedChangedDate { get; set; }

        [DataMember] public string ChangedBy { get; set; }

        [DataMember] public string ChangedFieldType { get; set; }

        [DataMember] public string ChangedFieldName { get; set; }

        [DataMember] public string ChangedFieldLogicalName { get; set; }

        [DataMember] public string attributemask { get; set; }

        [DataMember] public string NewValue { get; set; }

        [DataMember] public string OldValue { get; set; }
    }
}