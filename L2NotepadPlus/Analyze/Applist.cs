using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L2NotepadPlus.Analyze
{
    public class ApplicationEntry
    {
        public int Index { get; set; }
        public string AID { get; set; } = "";
        public int AIDLength { get; set; }
        public int MultipleOccurrenceFlag { get; set; }
        public int KernelIDCount { get; set; }
        public List<string> KernelIDList { get; set; } = new();
    }

    public static class ApplistParser
    {
        /// <summary>
        /// APPLIST hex verisini parse eder ve ApplicationEntry listesi döner.
        /// </summary>
        /// <param name="hexData">APPLIST hex içeriği</param>
        public static List<ApplicationEntry> Parse(string hexData)
        {
            if (string.IsNullOrWhiteSpace(hexData))
                throw new ArgumentException("Veri boş olamaz.");

            List<ApplicationEntry> result = new();
            int i = 0;
            int appIndex = 1;

            try
            {
                while (i < hexData.Length)
                {
                    // --- AID Length ---
                    if (i + 2 > hexData.Length)
                        break;

                    int aidLen = Convert.ToInt32(hexData.Substring(i, 2), 16);
                    i += 2;

                    if (i + aidLen * 2 > hexData.Length)
                        throw new Exception("AID uzunluğu geçersiz.");

                    // --- AID ---
                    string aid = hexData.Substring(i, aidLen * 2);
                    i += 32; // AID 16 byte (32 hex karakter)
                    if (i + 2 > hexData.Length)
                        throw new Exception("Eksik Multiple Occurrence Flag.");

                    // --- Multiple Occurrence Flag ---
                    int moFlag = Convert.ToInt32(hexData.Substring(i, 2), 16);
                    i += 2;

                    // --- Kernel Count ---
                    if (i + 2 > hexData.Length)
                        throw new Exception("Eksik KernelIDCount alanı.");

                    int kernelCount = Convert.ToInt32(hexData.Substring(i, 2), 16);
                    i += 2;

                    // --- Kernel IDs ---
                    List<string> kernelIds = new();

                    for (int k = 0; k < kernelCount; k++)
                    {
                        if (i + 2 > hexData.Length)
                            throw new Exception("Eksik KernelID uzunluğu.");

                        int kidLen = Convert.ToInt32(hexData.Substring(i, 2), 16);
                        i += 2;

                        if (i + kidLen * 2 > hexData.Length)
                            throw new Exception("Eksik KernelID verisi.");

                        string kid = hexData.Substring(i, kidLen * 2);
                        i += kidLen * 2;
                        kernelIds.Add(kid);
                    }

                    // --- Listeye ekle ---
                    result.Add(new ApplicationEntry
                    {
                        Index = appIndex++,
                        AID = aid,
                        AIDLength = aidLen,
                        MultipleOccurrenceFlag = moFlag,
                        KernelIDCount = kernelCount,
                        KernelIDList = kernelIds
                    });
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"APPLIST parse hatası: {ex.Message}");
            }

            return result;
        }
    }
}