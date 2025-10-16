using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L2NotepadPlus.Core
{
    public class L2ConfigReader
    {
        private const byte StartByte = 0x00;
        private const byte EndByte = 0x55;
        private const int AdditionalLength = 9;
        private const int ContentStartIndex = 6;
        private const int CrcStartIndex = 3;

        /// <summary>
        /// Reads a file and detects whether it's an L2Config file or a normal text file.
        /// </summary>
        /// <param name="filePath">Path of the file</param>
        /// <param name="fallbackText">Default text in case of error</param>
        /// <param name="isL2ConfigFile">True if detected file is L2Config format</param>
        /// <returns>Content as HEX (if L2Config) or text (if normal file)</returns>
        public static string Read(string filePath, string fallbackText, out bool isL2ConfigFile)
        {
            isL2ConfigFile = false;

            try
            {
                if (!File.Exists(filePath))
                {
                    MessageBox.Show($"Dosya bulunamadı:\n{filePath}", "Hata",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return fallbackText;
                }

                byte[] fileData = File.ReadAllBytes(filePath);
                if (fileData.Length == 0)
                {
                    MessageBox.Show("Dosya boş.", "Hata",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return fallbackText;
                }

                // L2Config format kontrolü
                if (fileData[0] == StartByte && fileData[^1] == EndByte && fileData.Length > AdditionalLength)
                {
                    ushort length = (ushort)(((fileData[1] << 8) & 0xFF00) | (fileData[2] & 0xFF));
                    if ((length + AdditionalLength) == fileData.Length)
                    {
                        ushort fileCrc = (ushort)(((fileData[^3] << 8) & 0xFF00) | (fileData[^2] & 0xFF));
                        ushort calculatedCrc = CRC16Calculator.Compute(fileData, CrcStartIndex, length + CrcStartIndex);

                        if (fileCrc == calculatedCrc)
                        {
                            isL2ConfigFile = true;
                            string hexContent = BitConverter
                                .ToString(fileData, ContentStartIndex, length)
                                .Replace("-", string.Empty);
                            return hexContent;
                        }
                    }
                }

                // Eğer L2Config formatına uymuyorsa: düz metin olarak oku
                string textContent = File.ReadAllText(filePath);
                return textContent;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Dosya okunurken hata oluştu:\n{ex.Message}", "Hata",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return fallbackText;
            }
        }
    }
}