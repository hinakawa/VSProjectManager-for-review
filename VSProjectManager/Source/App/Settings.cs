using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace VSProjectManager
{
    /// <summary>
    /// Сериализуемые настройки приложения
    /// </summary>
    [Serializable]
    public class Settings
    {
        /// <summary>
        /// Пути по умолчанию
        /// </summary>
        public List<string> DefaultPaths;
        /// <summary>
        /// Игнорируемые пути
        /// </summary>
        public List<string> SkipPaths;
        /// <summary>
        /// Известные параметры и значения
        /// </summary>
        public Dictionary<string, HashSet<string>> KnownParamsAndValues;
        public HashSet<string> KnownConfigurations;
        public bool IsFirstEcexcution;

        #region Singleton
        [NonSerialized]
        private static Settings _instance;
        public static Settings Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Settings();
                }
                return _instance;
            }
        }
        #endregion

        public Settings()
        {
            // Десериализуем настройки из файла при запуске приложения.
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            Settings deserialized;
            using (FileStream fs = new FileStream("settings.dat", FileMode.OpenOrCreate))
            {
                try
                {
                    deserialized = binaryFormatter.Deserialize(fs) as Settings;
                    DefaultPaths = deserialized.DefaultPaths;
                    SkipPaths = deserialized.SkipPaths;
                    KnownParamsAndValues = deserialized.KnownParamsAndValues;
                    KnownConfigurations = deserialized.KnownConfigurations;
                    IsFirstEcexcution = false;
                }
                catch
                {
                    DefaultPaths = new List<string>();
                    SkipPaths = new List<string>();
                    IsFirstEcexcution = true;
                    KnownParamsAndValues = new Dictionary<string, HashSet<string>>();
                    KnownConfigurations = new HashSet<string>();
                }
            }
        }
        
        public void Save()
        {
            // Сериализуем настройки
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            using (FileStream fs = new FileStream("settings.dat", FileMode.OpenOrCreate))
            {
                binaryFormatter.Serialize(fs, this);
            }
        }
        /// <summary>
        /// Добавляет известный параметр для параметризации и подсказок поиска.
        /// </summary>
        public void AddKnownParameter(string parameter, string value)
        {
            if (parameter != null && value != null)
            {
                if (!KnownParamsAndValues.ContainsKey(parameter))
                {
                    KnownParamsAndValues.Add(parameter, new HashSet<string>());
                }
                KnownParamsAndValues[parameter].Add(value);
            }
        }
    }
}
