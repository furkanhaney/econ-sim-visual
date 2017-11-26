namespace EconSimVisual.Extensions
{
    using System.IO;
    using System.Xml.Serialization;

    internal class Serializer
    {
        public static void Serialize(object obj, string path)
        {
            var serializer = new XmlSerializer(obj.GetType());
            using (var writer = new StreamWriter(path)) serializer.Serialize(writer, obj);
        }

        public static T Deserialize<T>(string path)
        {
            var serializer = new XmlSerializer(typeof(T));
            using (var reader = new StreamReader(path)) return (T)serializer.Deserialize(reader);
        }
    }
}
