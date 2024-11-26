namespace Fibratek.Hardware
{
    /// <summary>
    /// Класс для формирования журнала событий в программе. В основном - ошибки и искллючения
    /// </summary>
    public class Logger
    {
        public static StreamWriter sw = new StreamWriter("Logs.txt", true);
        public static void Log(object obj, string message)
        {
            sw.WriteLine(DateTime.Now + "\t" + obj.GetType() + "\t" + message);
            sw.Flush();
        }
    }
}
