using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace L2NotepadPlus.Analyze
{
    public static class AidMappingLoader
    {
        private static Dictionary<string, string>? _aidMap;

        public static Dictionary<string, string> Load(string filePath = "aid_map.json")
        {
            if (_aidMap != null)
                return _aidMap; // önceden yüklendiyse tekrar okuma

            try
            {
                if (!File.Exists(filePath))
                {
                    MessageBox.Show($"AID mapping dosyası bulunamadı:\n{filePath}",
                        "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    _aidMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    return _aidMap;
                }

                string json = File.ReadAllText(filePath);
                _aidMap = JsonSerializer.Deserialize<Dictionary<string, string>>(json)
                          ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                return _aidMap;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"AID eşleştirme dosyası okunamadı:\n{ex.Message}",
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);

                _aidMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                return _aidMap;
            }
        }
    }
}