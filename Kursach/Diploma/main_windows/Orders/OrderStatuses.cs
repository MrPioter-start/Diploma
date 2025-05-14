using System.Collections.Generic;

namespace Diploma.main_windows
{
    public static class OrderStatuses
    {
        public static List<string> All { get; } = new List<string>
        {
        "Оформление",
        "В обработке",
        "Доставка",
        "Завершен"
        };
    }
}