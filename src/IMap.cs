using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Eurodiffusion_from_abdulllah111
{
    public interface IMap
    {
        // Список стран
        public List<Country> Countries { get; set; }

        // Добавить новую страну
        void AddCountry(Country country);

        // Отрисовка карты
        void PrintMap();

        // Запуск симуляции модели
        public void SimulateDaysToCompletion();
    }
}