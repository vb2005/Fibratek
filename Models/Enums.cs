namespace Fibratek.Models
{
    public enum InitState { 
        Disabled,  // Отключено программно
        Init,      // Еще не инициализировано
        On,        // В работе
        Error      // Ошибка
    }
}
