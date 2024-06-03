using System;

namespace task3.Models
{
    public class Фонарь : ОсветительныйПрибор
    {
        private readonly Random _random = new Random();
        public int ВероятностьСлома { get; private set; } = 10; // Вероятность выхода из строя в процентах

        public Фонарь()
        {
            Включено = false;
        }

        public override void Включить()
        {
            if (_random.Next(100) < ВероятностьСлома)
            {
                Сломать();
            }
            else
            {
                Включено = true;
                Console.WriteLine("Фонарь включен.");
            }
        }

        public override void Выключить()
        {
            Включено = false;
            Console.WriteLine("Фонарь выключен.");
        }
    }
}
