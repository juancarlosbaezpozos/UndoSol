using System.Runtime.Serialization;

namespace Inogic.Click2Undo.Workflows.Entities
{
    [DataContract]
    public class SerializeDeletedRecords
    {
        [DataMember] public DeletedRecordsCollection[] DeletedRecordsCollection { get; set; }

        [DataMember] public string TotalDeletedRecordscount { get; set; }

        [DataMember] public string PagingCookie { get; set; }

        [DataMember] public string RecordsPerPage { get; set; }
    }
}