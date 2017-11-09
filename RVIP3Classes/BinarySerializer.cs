using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
namespace RVIP3Classes
{
    public static class BinarySerializer<T>
    {
        public static byte[] Serialize(T data)
        {
            var serializer = new BinaryFormatter();
            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, data);
                return stream.ToArray();
            }
        }
        public static T Deserialize(byte[] data)
        {
            var serializer = new BinaryFormatter();
            using (var stream = new MemoryStream(data))
            {
                var res = serializer.Deserialize(stream);
                return (T)res;
            }
        }
    }
}
