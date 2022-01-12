using System.Runtime.Serialization;

namespace Inogic.Click2Undo.Workflows.Entities
{
    [DataContract]
    public class FieldInfo
    {
        [DataMember] public string FieldLogicalName { get; set; }

        [DataMember] public string FieldDisplayName { get; set; }

        [DataMember] public string AttributeMask { get; set; }
    }
}