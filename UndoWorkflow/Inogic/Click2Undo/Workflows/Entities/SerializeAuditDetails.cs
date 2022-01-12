using System.Runtime.Serialization;

namespace Inogic.Click2Undo.Workflows.Entities
{
    [DataContract]
    public class SerializeAuditDetails
    {
        [DataMember] public AuditRecords[] AuditRecords { get; set; }

        [DataMember] public string TotalAuditRecordscount { get; set; }

        [DataMember] public string PagingCookie { get; set; }

        [DataMember] public string RecordsPerPage { get; set; }

        [DataMember] public FieldInfo[] fieldInfo { get; set; }
    }
}