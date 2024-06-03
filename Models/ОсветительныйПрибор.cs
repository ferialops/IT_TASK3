using System;

namespace task3.Models
{
    public abstract class ОсветительныйПрибор
    {
        public bool Включено { get; protected set; }
        public event EventHandler Сломан;

        public abstract void Включить();
        public abstract void Выключить();

        protected void Сломать()
        {
            Включено = false;
            Сломан?.Invoke(this, EventArgs.Empty);
            Console.WriteLine("Прибор сломан.");
        }
    }
}
