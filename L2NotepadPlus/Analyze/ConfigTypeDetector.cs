using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L2NotepadPlus.Analyze
{
    public static class ConfigTypeDetector
    {
        public enum ConfigType
        {
            Unknown,
            Applist,
            Entrypoint,
            KernelConfig,
            L2ManagerConfig,
            DRL
        }

        /// <summary>
        /// Dosya adını inceleyerek konfigürasyon türünü belirler.
        /// </summary>
        public static ConfigType DetectFromFileName(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return ConfigType.Unknown;

            string fileName = Path.GetFileName(filePath).ToLowerInvariant();

            if (fileName == "applist.txt")
                return ConfigType.Applist;

            if (fileName.StartsWith("config_ep"))
                return ConfigType.Entrypoint;

            if (fileName.StartsWith("config_07.txt"))
                return ConfigType.KernelConfig;

            if (fileName == "config_l2manager.txt")
                return ConfigType.L2ManagerConfig;

            if (fileName.StartsWith("config_drl"))
                return ConfigType.DRL;

            return ConfigType.Unknown;
        }
    }
}