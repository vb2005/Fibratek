using Fibratek.Hardware;
using Fibratek.Models;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using static System.Environment;

namespace Fibratek.Data
{
    /// <summary>
    /// Класс с настройками программы
    /// </summary>
    public class LocalSettings
    {
        [DisplayName("Порты дисплеев")]
        [Description("Укажите COM-порты и скорости устройств визуализации")]
        [Category("Устройства")]
        public List<LedModel> LedDevices { get; set; } = new List<LedModel>();

        [DisplayName("Список камер на проекте")]
        [Description("Укажите названия для всех камер, используемых для анализа")]
        [Category("Устройства")]
        public List<CameraModel> Cameras { get; set; } = new List<CameraModel>();



        [DisplayName("Пароль администратора")]
        [Description("Укажите административного доступа")]
        [Category("Общие настройки")]
        public string AdminPassword { get; set; } = "default_0001";

        [DisplayName("Порог определения полоски")]
        [Description("Укажите порог для определния лазерного луча")]
        [Category("Общие настройки")]
        public int ThreshValue { get; set; } = 124;

        [DisplayName("Опрос камер")]
        [Description("Укажите интервал опроса камер в миллисекундах")]
        [Category("Общие настройки")]
        public int Interval { get; set; } = 124;




        #region Singleton

        private static LocalSettings _instance;
        public static LocalSettings Instance
        {
            get
            {
                if (_instance == null)
                {
                    try
                    {
                        var path = Path.Combine(GetFolderPath(SpecialFolder.CommonApplicationData), "Fibratek", "Settings.json");
                        _instance = JsonSerializer.Deserialize<LocalSettings>(File.ReadAllText(path));
                    }
                    catch (Exception)
                    {
                        _instance = new LocalSettings();
                    }
                }
                return _instance;
            }
        }


        public LocalSettings()
        {

        }

        public void Save()
        {
            JsonSerializerOptions js = new JsonSerializerOptions();
            js.WriteIndented = true;
            string data = JsonSerializer.Serialize(this, js);
            var path = Path.Combine(GetFolderPath(SpecialFolder.CommonApplicationData), "Fibratek", "Settings.json");
            File.WriteAllText(path, data);
        }

        #endregion
    }
}
