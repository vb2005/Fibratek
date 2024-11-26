using MvCamCtrl.NET;

using OpenCvSharp;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Fibratek.Hardware
{
    /// <summary>
    /// Камера HikRobot. В принципе, SDK поддерживает и некоторые другие
    /// </summary>
    public class Camera
    {
        public int DeviceID { get; set; }
        static MyCamera.MV_CC_DEVICE_INFO_LIST m_stDeviceList = new MyCamera.MV_CC_DEVICE_INFO_LIST();

        public delegate void SendImageHandler(int sender, Mat mat);
        public event ErrorEventHandler Error;
        public event SendImageHandler SendImage;


        public bool isGrabbing = false;
        public bool Connected { get; set; }
        public int FrameWidth { get; set; } = 0;
        public int FrameRate { get; set; }
        public int FrameHeight { get; set; } = 0;
        public string UserDefinedName { get; set; }

        public string SerialNumber { get; set; }
        private MyCamera m_MyCamera = null;
        private Mat frame;

        public static List<Camera> Enumerate()
        {
            m_stDeviceList.nDeviceNum = 0;
            var list = new List<Camera>();

            int nRet = MyCamera.MV_CC_EnumDevices_NET(MyCamera.MV_GIGE_DEVICE | MyCamera.MV_USB_DEVICE, ref m_stDeviceList);
            if (0 != nRet)
                return list;

            for (int i = 0; i < m_stDeviceList.nDeviceNum; i++)
            {
                MyCamera.MV_CC_DEVICE_INFO device = (MyCamera.MV_CC_DEVICE_INFO)Marshal.PtrToStructure(m_stDeviceList.pDeviceInfo[i], typeof(MyCamera.MV_CC_DEVICE_INFO));
                try
                {
                    MyCamera.MV_GIGE_DEVICE_INFO gigeInfo = (MyCamera.MV_GIGE_DEVICE_INFO)MyCamera.ByteToStruct(device.SpecialInfo.stGigEInfo, typeof(MyCamera.MV_GIGE_DEVICE_INFO));
                    list.Add(new Camera(gigeInfo.chSerialNumber, m_stDeviceList));
                }
                catch (Exception ex)
                {

                }
            }
            return list;
        }

        public Camera(string serialNumber, MyCamera.MV_CC_DEVICE_INFO_LIST m_stDeviceList)
        {
            this.SerialNumber = serialNumber;
            for (int i = 0; i < m_stDeviceList.nDeviceNum; i++)
            {
                MyCamera.MV_CC_DEVICE_INFO device = (MyCamera.MV_CC_DEVICE_INFO)Marshal.PtrToStructure(m_stDeviceList.pDeviceInfo[i], typeof(MyCamera.MV_CC_DEVICE_INFO));
                MyCamera.MV_GIGE_DEVICE_INFO gigeInfo = (MyCamera.MV_GIGE_DEVICE_INFO)MyCamera.ByteToStruct(device.SpecialInfo.stGigEInfo, typeof(MyCamera.MV_GIGE_DEVICE_INFO));
                UserDefinedName = gigeInfo.chUserDefinedName;
                if (gigeInfo.chSerialNumber == SerialNumber) return;
            }
            throw new Exception("Camera not found");
        }

        public string GetID()
        {
            return SerialNumber;
        }

        public bool SetFrameRate2(float rate)
        {
            if (m_MyCamera == null) return false;
            int nRet = m_MyCamera.MV_CC_SetFrameRate_NET(rate);
            return (MyCamera.MV_OK != nRet) ? false : true;
        }

        public int GetFrameRate()
        {
            if (m_MyCamera == null) return 24;
            MyCamera.MVCC_FLOATVALUE result = new MyCamera.MVCC_FLOATVALUE();

            int nRet = m_MyCamera.MV_CC_GetFrameRate_NET(ref result);
            return (int)result.fCurValue;
        }

        public int GetHeight()
        {
            if (m_MyCamera == null) return -1;
            MyCamera.MVCC_INTVALUE result = new MyCamera.MVCC_INTVALUE();

            int nRet = m_MyCamera.MV_CC_GetHeight_NET(ref result);
            return (int)result.nCurValue;
        }

        public int GetWidth()
        {
            if (m_MyCamera == null) return -1;
            MyCamera.MVCC_INTVALUE result = new MyCamera.MVCC_INTVALUE();

            int nRet = m_MyCamera.MV_CC_GetWidth_NET(ref result);
            return (int)result.nCurValue;
        }

        public bool SetJumbo(uint cnt)
        {
            if (m_MyCamera == null) return false;
            int nRet = m_MyCamera.MV_GIGE_SetGevSCPSPacketSize_NET(cnt);
            return (MyCamera.MV_OK != nRet) ? false : true;
        }

        /// <summary>
        /// Функция, которая открывает устройство по serial number
        /// </summary>
        /// <returns></returns>
        public bool Open()
        {
            int nRet = 0;

            if (m_stDeviceList.nDeviceNum == 0)
            {
                nRet = MyCamera.MV_CC_EnumDevices_NET(MyCamera.MV_GIGE_DEVICE | MyCamera.MV_USB_DEVICE, ref m_stDeviceList);
                if (0 != nRet)
                    return false;
            }

            for (int i = 0; i < m_stDeviceList.nDeviceNum; i++)
            {
                MyCamera.MV_CC_DEVICE_INFO device = (MyCamera.MV_CC_DEVICE_INFO)Marshal.PtrToStructure(m_stDeviceList.pDeviceInfo[i], typeof(MyCamera.MV_CC_DEVICE_INFO));
                if (device.nTLayerType == MyCamera.MV_GIGE_DEVICE)
                {
                    MyCamera.MV_GIGE_DEVICE_INFO gigeInfo = (MyCamera.MV_GIGE_DEVICE_INFO)MyCamera.ByteToStruct(device.SpecialInfo.stGigEInfo, typeof(MyCamera.MV_GIGE_DEVICE_INFO));

                    if (gigeInfo.chSerialNumber == SerialNumber)
                    {
                        //Open device
                        m_MyCamera = new MyCamera();
                        if (m_MyCamera == null)
                            return false;

                        nRet = m_MyCamera.MV_CC_CreateDevice_NET(ref device);
                        if (MyCamera.MV_OK != nRet)
                            return false;

                        nRet = m_MyCamera.MV_CC_OpenDevice_NET();
                        if (MyCamera.MV_OK != nRet)
                        {
                            m_MyCamera.MV_CC_DestroyDevice_NET();
                            return false;
                        }

                        if (device.nTLayerType == MyCamera.MV_GIGE_DEVICE)
                        {
                            int nPacketSize = m_MyCamera.MV_CC_GetOptimalPacketSize_NET();
                            if (nPacketSize > 0)
                                nRet = m_MyCamera.MV_CC_SetIntValue_NET("GevSCPSPacketSize", (uint)nPacketSize);
                        }
                        break;
                    }
                    else
                    {
                        // TODO:  реакция если ничего не проинициализированли
                    }
                }
            }
            Connected = true;
            FrameHeight = GetHeight();
            FrameWidth = GetWidth();

            if ((FrameHeight == -1) || (FrameWidth == -1))
            {
                MessageBox.Show("Ошибка чтения настроек с камеры. Программа будет закрыта");
                return false;
            }

            return Connected;
        }

        /// <summary>
        /// Функция, которая запускает сьемку с камеры
        /// </summary>
        public bool StartStream()
        {
            int nRet = m_MyCamera.MV_CC_StartGrabbing_NET();
            if (MyCamera.MV_OK != nRet)
            {
                Console.WriteLine("Start grabbing failed:{0:x8}", nRet);
                return false;
            }
            isGrabbing = true;
            ReceiveImageWorkThread();
            return true;
        }

        /// <summary>
        /// Функция, которая останавливает съемку с камеры
        /// </summary>
        public bool EndStream()
        {
            isGrabbing = false;

            int nRet = m_MyCamera.MV_CC_StopGrabbing_NET();
            if (MyCamera.MV_OK != nRet)
            {
                Console.WriteLine("Stop grabbing failed:{0:x8}", nRet);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Функция для запуска съемки по триггеру
        /// </summary>
        public bool ExecuteTrigger()
        {
            int nRet = m_MyCamera.MV_CC_SetCommandValue_NET("TriggerSoftware");
            return (MyCamera.MV_OK != nRet) ? false : true;
        }

        /// <summary>
        /// Функция отключения от устройства
        /// </summary>
        public bool Close()
        {
            //Close Device
            m_MyCamera.MV_CC_CloseDevice_NET();
            m_MyCamera.MV_CC_DestroyDevice_NET();
            Connected = false;
            return true;
        }

        /// <summary>
        /// Функция получения изображения
        /// </summary>
        private void ReceiveImageWorkThread()
        {
            int nRet = MyCamera.MV_OK;
            MyCamera.MV_FRAME_OUT stImageOut = new MyCamera.MV_FRAME_OUT();
            MyCamera.MV_CC_INPUT_FRAME_INFO stInputFrameInfo = new MyCamera.MV_CC_INPUT_FRAME_INFO();
            nRet = m_MyCamera.MV_CC_GetImageBuffer_NET(ref stImageOut, 100);
            FrameWidth = stImageOut.stFrameInfo.nWidth;
            FrameHeight = stImageOut.stFrameInfo.nHeight;

            // Приход кадра в отдельном потоке
            Thread th = new Thread(() =>
            {
                // Пока статус "Захват"
                while (isGrabbing)
                {
                    // Получаем кадр
                    nRet = m_MyCamera.MV_CC_GetImageBuffer_NET(ref stImageOut, 100);
                    if (nRet == MyCamera.MV_OK)
                    {
                        FrameWidth = stImageOut.stFrameInfo.nWidth;
                        FrameHeight = stImageOut.stFrameInfo.nHeight;

                        // Формируем кадр
                        Mat m = new Mat(FrameHeight, FrameWidth, MatType.CV_8UC1, stImageOut.pBufAddr);

                        // Кидаем обработку события дальше
                        if (m_MyCamera != null)
                            SendImage?.Invoke(DeviceID, m);

                    }
                    else
                    {
                        Error?.Invoke(this, new ErrorEventArgs(new Exception("Камера была отключена")));
                    }
                    m_MyCamera.MV_CC_FreeImageBuffer_NET(ref stImageOut);
                }
            })
            { IsBackground = true };
            th.Name = "Camera " + SerialNumber;
            th.Start();
        }
    }
}
