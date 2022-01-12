using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Inogic.Click2Undo.Workflows.Helper
{
    public static class PluginExtensions
    {
        public static string Serialize<T>(this T obj)
        {
            DataContractJsonSerializer dataContractJsonSerializer = new DataContractJsonSerializer(obj.GetType(),
                new DataContractJsonSerializerSettings
                {
                    UseSimpleDictionaryFormat = true
                });
            using MemoryStream memoryStream = new MemoryStream();
            dataContractJsonSerializer.WriteObject(memoryStream, obj);
            memoryStream.Position = 0L;
            using StreamReader streamReader = new StreamReader(memoryStream);
            return streamReader.ReadToEnd();
        }

        public static T Deserialize<T>(this string str) where T : class
        {
            DataContractJsonSerializer dataContractJsonSerializer = new DataContractJsonSerializer(typeof(T),
                new DataContractJsonSerializerSettings
                {
                    UseSimpleDictionaryFormat = true
                });
            using MemoryStream stream = new MemoryStream(Encoding.Unicode.GetBytes(str));
            return (T)dataContractJsonSerializer.ReadObject(stream);
        }
    }
}