using System.ComponentModel;

namespace Fibratek.Models
{
    public class LedModel
    {
        [DisplayName("Номер устройства")]
        public int DeviceId { get; set; }

        [DisplayName("Номер COM-порта")]
        public string DevicePort { get; set; }

        [DisplayName("Скорость канала")]
        public int BaudRate { get; set; }
    }
}
