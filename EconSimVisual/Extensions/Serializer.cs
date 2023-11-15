namespace EconSimVisual.Extensions
{
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Xml.Serialization;

    internal class Serializer
    {
        public static void XmlSerialize(object obj, string path)
        {
            using (var writer = new StreamWriter(path))
                new XmlSerializer(obj.GetType()).Serialize(writer, obj);
        }

        public static T XmlDeserialize<T>(string path)
        {
            using (var reader = new StreamReader(path))
                return (T)new XmlSerializer(typeof(T)).Deserialize(reader);
        }

        public static void BinarySerialize(object obj, string path)
        {
            using (var stream = File.Create(path))
                new BinaryFormatter().Serialize(stream, obj);
        }

        public static T BinaryDeserialize<T>(string path)
        {
            using (var stream = File.OpenRead(path))
                return (T)new BinaryFormatter().Deserialize(stream);
        }
    }
}
