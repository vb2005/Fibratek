using Fibratek;
using Fibratek.Data;
using Fibratek.Hardware;
using Fibratek.Models;
using Fibratek.UI;

using OpenCvSharp;

namespace Fibratek.Controllers
{
    public class MainController
    {
        public delegate void DataRecievedHandler(int CameraID, int PixValue, double MmValue);
        public static event DataRecievedHandler DataRecieved;

        public static Thread thread = new Thread(_start);

        private static void Timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            // Если система не в работе
            if (SessionSettings.State != UI.Controls.Utils.StateMode.Running)
                return;

            // Обновляем интервал таймера если его меняли в настройках
            if (SessionSettings.timer.Interval != LocalSettings.Instance.Interval)
                SessionSettings.timer.Interval = LocalSettings.Instance.Interval;

            // Вызываем триггер у камер
            SessionSettings.Cameras.ForEach(s => s.ExecuteTrigger());
        }


        public static void _start()
        {
            RestController.Start();

            foreach (var s in LocalSettings.Instance.LedDevices)
            {
                LedDevice device = new LedDevice();
                device.DeviceId = s.DeviceId;
                device.Init(s.DevicePort, s.BaudRate);
                SessionSettings.ledDevices.Add(device);
                SessionSettings.timer.Elapsed += Timer_Elapsed;
                SessionSettings.timer.Start();
                // TODO: Сделать реакцию на ошибки
            }


            var cameraList = Camera.Enumerate();
            foreach (var s in LocalSettings.Instance.Cameras)
            {
                Camera camera = cameraList.FirstOrDefault(c => c.GetID() == s.CameraHwName);
                if (camera == null) continue;
                camera.DeviceID = s.CameraID;
                camera.Open();
                camera.StartStream();
                camera.SendImage += FrameRecieved;
                SessionSettings.Cameras.Add(camera);
                // TODO: Сделать реакцию на ошибки
            }
        }

        private static void FrameRecieved(int deviceID, Mat mat)
        {
            int pix = Calculation.Calc.GetMaxLine(mat, LocalSettings.Instance.ThreshValue);
            double mm = Calculation.Calc.GetTrueValue(pix);

            if (SessionSettings.State == UI.Controls.Utils.StateMode.Running)
                DataRecieved?.Invoke(deviceID, pix, mm);
        }

        public static void Start()
        {
            thread.Start();
            thread.Name = "Main Ctrl";
            thread.IsBackground = true;
        }
    }
}
