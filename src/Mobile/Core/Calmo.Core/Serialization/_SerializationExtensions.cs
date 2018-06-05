using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace ResourceIT.Core.Serialization
{
    public static class SerializationExtensions
    {
        public static byte[] ToBytes<T>(this T objeto)
        {
            if (objeto == null)
                return null;

            var json = JsonConvert.SerializeObject(objeto);
            var bytes = Encoding.UTF8.GetBytes(json);

            byte[] buffer;

            using (var ms = new MemoryStream(bytes))
            {
                ms.Flush();
                ms.Position = 0;

                buffer = new byte[ms.Length];
                ms.Read(buffer, 0, buffer.Length);
            }

            return buffer;
        }

        public static T FromBytes<T>(this byte[] bytes)
        {
            if (bytes != null)
            {
                using (var ms = new MemoryStream(bytes))
                {
                    try
                    {
                        ms.Flush();
                        ms.Position = 0;

                        var buffer = new byte[ms.Length];
                        ms.Read(buffer, 0, buffer.Length);
                        
                        var json = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
                        return JsonConvert.DeserializeObject<T>(json);
                    }
                    catch (Exception)
                    {
                        throw new InvalidCastException(String.Format("Não foi possível converter os bytes informados para o tipo {0}.", typeof(T).FullName));
                    }

                }
            }

            return default(T);
        }

        public static string ToBase64<T>(this T objeto)
        {
            var bytes = ToBytes(objeto);

            var encoding = Encoding.GetEncoding("iso-8859-1");
            var binaryText = encoding.GetString(bytes, 0, bytes.Length);
            var base64Text = StringToBase64(binaryText);

            return base64Text;
        }

        public static T FromBase64<T>(this string conteudo)
        {
            var binaryText = Base64ToString(conteudo);

            var encoding = Encoding.GetEncoding("iso-8859-1");
            var bytes = encoding.GetBytes(binaryText);

            return FromBytes<T>(bytes);
        }

        private static string StringToBase64(string text)
        {
            var textBytes = Encoding.Unicode.GetBytes(text);
            var returnValue = Convert.ToBase64String(textBytes);

            return returnValue;
        }

        private static string Base64ToString(string text)
        {
            var textBytes = Convert.FromBase64String(text);
            var returnValue = Encoding.Unicode.GetString(textBytes, 0, textBytes.Length);

            return returnValue;
        }

        public static string ToXml<T>(this T objeto)
        {
            string result;
            using (var sw = new StringWriter())
            {
                var serializador = new XmlSerializer(typeof(T));
                serializador.Serialize(sw, objeto);
                result = sw.ToString();
            }
            return result;
        }

        public static void SerializeTo<T>(this T obj, Stream stream)
        {
            var json = JsonConvert.SerializeObject(obj);

            using (var sw = new StreamWriter(stream))
            {
                sw.Flush();
                sw.Write(json);
            }
            stream.Position = 0;
        }

        public static T Deserialize<T>(this Stream stream)
        {
            var reader = new StreamReader(stream);
            var json = reader.ReadToEnd();

            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
