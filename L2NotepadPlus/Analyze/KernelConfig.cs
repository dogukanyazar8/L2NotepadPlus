using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace L2NotepadPlus.Analyze
{
    public static class KernelConfigParser
    {
        private static Dictionary<string, string>? TagDefinitions;

        /// <summary>
        /// JSON dosyasından tag açıklamalarını yükler.
        /// </summary>
        private static void LoadTagDefinitions()
        {
            if (TagDefinitions != null) return; // daha önce yüklendiyse tekrar okuma

            string jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "KernelTags.json");
            if (!File.Exists(jsonPath))
                throw new FileNotFoundException("Kernel tag tanım dosyası bulunamadı.", jsonPath);

            string json = File.ReadAllText(jsonPath);
            TagDefinitions = JsonSerializer.Deserialize<Dictionary<string, string>>(json)
                             ?? new Dictionary<string, string>();
        }

        /// <summary>
        /// TLV formatındaki KernelConfig içeriğini parse eder.
        /// </summary>
        public static List<TlvEntry> Parse(string hexData)
        {
            LoadTagDefinitions();
            List<TlvEntry> result = new();
            byte[] buffer = StringToByteArray(hexData);
            int pos = 0;

            while (pos < buffer.Length)
            {
                // Tag belirleme
                byte[] tagBytes = GetTag(buffer, ref pos);
                string tag = ByteArrayToString(tagBytes);

                // Length belirleme
                int valueLength = GetLength(buffer, ref pos);

                // Value belirleme
                byte[] valueBytes = buffer.Skip(pos).Take(valueLength).ToArray();
                pos += valueBytes.Length;
                string value = ByteArrayToString(valueBytes);

                // Tanım bul
                string definition = TagDefinitions!.TryGetValue(tag, out var def)
                    ? def
                    : "UNDEFINED TAG";

                result.Add(new TlvEntry
                {
                    Tag = tag,
                    Length = valueLength.ToString("X2"),
                    Value = value,
                    Definition = definition
                });
            }

            return result;
        }

        // TLV Tag parse
        private static byte[] GetTag(byte[] buffer, ref int pos)
        {
            byte first = buffer[pos++];
            List<byte> tagBytes = new() { first };

            // Çok baytlı tag kontrolü
            if ((first & 0x1F) == 0x1F)
            {
                tagBytes.Add(buffer[pos++]);
                if (tagBytes[0] == 0xDF && tagBytes[1] == 0x81)
                    tagBytes.Add(buffer[pos++]);
            }

            return tagBytes.ToArray();
        }

        // TLV Length parse
        private static int GetLength(byte[] buffer, ref int pos)
        {
            byte first = buffer[pos++];
            if (first <= 0x7F)
                return first;

            int lengthBytes = first & 0x7F;
            byte[] lenBytes = buffer.Skip(pos).Take(lengthBytes).ToArray();
            pos += lengthBytes;

            int valueLength = 0;
            foreach (byte b in lenBytes)
                valueLength = (valueLength << 8) + b;

            return valueLength;
        }

        // Yardımcılar
        private static byte[] StringToByteArray(string hex) =>
            Enumerable.Range(0, hex.Length / 2)
                      .Select(x => Convert.ToByte(hex.Substring(x * 2, 2), 16))
                      .ToArray();

        private static string ByteArrayToString(byte[] ba) =>
            BitConverter.ToString(ba).Replace("-", "");
    }

    public class TlvEntry
    {
        public string Tag { get; set; } = "";
        public string Length { get; set; } = "";
        public string Value { get; set; } = "";
        public string Definition { get; set; } = "";

        public override string ToString() => $"{Tag} {Length} {Value} - {Definition}";
    }
}
