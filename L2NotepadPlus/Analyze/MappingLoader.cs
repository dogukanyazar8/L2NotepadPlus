using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace L2NotepadPlus.Analyze
{
    public static class MappingLoader
    {
        private static readonly Dictionary<string, Dictionary<string, string>> _cache = new();

        /// <summary>
        /// Verilen mapping tipine göre (örneğin "applist", "kernelconfig") ilgili JSON dosyasını yükler.
        /// JSON dosyaları Data klasöründe bulunur (örnek: Data/applist.json).
        /// </summary>
        public static Dictionary<string, string> Load(string mappingType)
        {
            mappingType = mappingType.ToLowerInvariant();

            if (_cache.ContainsKey(mappingType))
                return _cache[mappingType];

            try
            {
                string jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", $"{mappingType}.json");

                if (!File.Exists(jsonPath))
                {
                    MessageBox.Show($"JSON dosyası bulunamadı:\n{jsonPath}",
                        "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    _cache[mappingType] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    return _cache[mappingType];
                }

                string json = File.ReadAllText(jsonPath);
                var map = JsonSerializer.Deserialize<Dictionary<string, string>>(json)
                          ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                _cache[mappingType] = map;
                return map;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"JSON dosyası okunamadı ({mappingType}):\n{ex.Message}",
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);

                _cache[mappingType] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                return _cache[mappingType];
            }
        }
    }
}