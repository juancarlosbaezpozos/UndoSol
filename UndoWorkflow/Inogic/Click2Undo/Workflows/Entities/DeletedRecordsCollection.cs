using System.Runtime.Serialization;

namespace Inogic.Click2Undo.Workflows.Entities
{
    [DataContract]
    public class DeletedRecordsCollection
    {
        [DataMember] public DeletedRecordAttributes DeletedRecordAttributes { get; set; }
    }
}