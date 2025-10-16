using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace L2NotepadPlus.Core
{
    public static class L2ConfigWriter
    {
        private const byte StartByte = 0x00;
        private const byte EndByte = 0x55;
        private const int AdditionalLength = 9;
        private const int ContentStartIndex = 6;
        private const int CrcStartIndex = 3;

        /// <summary>
        /// Converts a HEX string into L2Config binary file format.
        /// </summary>
        /// <param name="hexInput">HEX data string</param>
        /// <returns>Binary formatted L2Config file data</returns>
        private static byte[] BuildBinary(string hexInput)
        {
            ushort length = (ushort)(hexInput.Length / 2);
            byte[] binary = new byte[length + AdditionalLength];

            binary[0] = StartByte;

            // Write length (big endian)
            binary[1] = (byte)((length >> 8) & 0xFF);
            binary[2] = (byte)(length & 0xFF);

            // Flags[3] placeholder (TODO: implement if required)
            binary[3] = 0;
            binary[4] = 0;
            binary[5] = 0;

            // Convert HEX string into bytes
            byte[] fileContent = Enumerable.Range(0, length)
                .Select(i => Convert.ToByte(hexInput.Substring(i * 2, 2), 16))
                .ToArray();

            Buffer.BlockCopy(fileContent, 0, binary, ContentStartIndex, length);

            // Calculate CRC over Flags + FILE_CONTENT
            ushort crc = CRC16Calculator.Compute(binary, CrcStartIndex, length + CrcStartIndex);
            binary[length + ContentStartIndex] = (byte)((crc >> 8) & 0xFF);
            binary[length + ContentStartIndex + 1] = (byte)(crc & 0xFF);

            // Append end marker
            binary[length + ContentStartIndex + 2] = EndByte;

            return binary;
        }

        /// <summary>
        /// Writes text to disk as either an L2Config binary file or plain text.
        /// </summary>
        /// <param name="filePath">Target file path</param>
        /// <param name="content">File content</param>
        /// <param name="isL2ConfigFile">If true, saves as L2Config format</param>
        /// <param name="encoding">Optional text encoding for plain text mode</param>
        /// <returns>True if successful</returns>
        public static bool WriteFile(string filePath, string content, bool isL2ConfigFile, Encoding? encoding = null)
        {
            try
            {
                if (isL2ConfigFile)
                {
                    if (string.IsNullOrWhiteSpace(content))
                    {
                        MessageBox.Show("Yazılacak içerik boş olamaz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }

                    if (content.Length % 2 != 0)
                    {
                        MessageBox.Show("Veri uzunluğu çift olmalıdır (her byte 2 HEX karakter).", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }

                    if (!Regex.IsMatch(content, @"\A[0-9a-fA-F]+\Z"))
                    {
                        MessageBox.Show("Sadece HEX karakterlere izin verilir (0-9, A-F).", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }

                    byte[] binaryData = BuildBinary(content);
                    File.WriteAllBytes(filePath, binaryData);
                }
                else
                {
                    if (encoding != null)
                        File.WriteAllText(filePath, content, encoding);
                    else
                        File.WriteAllText(filePath, content);
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Dosya yazılırken hata oluştu:\n{ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
    }
}