using Fibratek.Models;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fibratek.Hardware
{
    /// <summary>
    /// Устройство вывода информации
    /// Рассчитываем на RS-232 или RS-485
    /// </summary>
    public class LedDevice
    {
        public int DeviceId { get; set; }

        private SerialPort _sp;

        public string PortName { get; private set; }    

        public InitState State { get; set; } = InitState.Disabled;

        public void Init(string port, int baudRate)
        {
            try
            {
                State = InitState.Init;
                PortName = port;
                _sp = new SerialPort(port, baudRate);
                _sp.Open();
                _sp.ReadTimeout = 500;
                _sp.WriteTimeout = 500;
                State = InitState.On;
            }
            catch (Exception ex)
            {
                State = InitState.Error;
            }
        }

        public void SendPacket(string data)
        {
            try
            {
                if (State == InitState.On)
                    _sp.WriteLine(data);
            }
            catch (Exception)
            {
                State = InitState.Error;
                Logger.Log(this, $"Табло {PortName} перестало отвечать");
            }
        }
    }
}
