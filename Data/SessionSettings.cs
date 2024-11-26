using Fibratek.Hardware;
using Fibratek.UI.Controls.Utils;

namespace Fibratek.Data
{
    public class SessionSettings
    {
        public static List<LedDevice> ledDevices = new List<LedDevice>();
        public static List<Camera> Cameras = new List<Camera>();

        public static StateMode State { get; set; } = StateMode.Stopped;
        public static System.Timers.Timer timer = new System.Timers.Timer(new TimeSpan(0, 0, 0, 0, LocalSettings.Instance.Interval));

        public static List<List<double>> Values = new List<List<double>>();
    }
}
