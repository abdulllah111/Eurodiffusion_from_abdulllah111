using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Eurodiffusion_from_abdulllah111
{
    public class Country
    {
        private readonly IMap _map;

        // Индекс страны
        public Guid Index { get; private set; }
        // Название страны
        public string Name { get; set; }
        // Список городов в стране
        private readonly List<City> cities;

        public ReadOnlyCollection<City> Cities
        {
            get { return cities.AsReadOnly(); }
        }
        // Флаг завершенности страны
        public bool IsComplete { get; set; }
        // День завершения страны
        public int CompletionDay { get; set; }

        // Координаты страны
        public int X1 { get; private set; }
        public int Y1 { get; private set; }
        public int X2 { get; private set; }
        public int Y2 { get; private set; }


        // Конструктор страны
        public Country(string name, int x1, int y1, int x2, int y2, IMap map)
        {
            _map = map;

            // Инициализируем свойства
            Index = Guid.NewGuid();
            Name = name;
            cities = new List<City>();
            IsComplete = false;
            CompletionDay = 0;

            X1 = x1;
            Y1 = y1;
            X2 = x2;
            Y2 = y2;

            // Заполняем страну городами
            // Так как город - это точка координаты в стране, то их количество равно количеству точек в стране
            // Исходя из этого - количество городов после создания страны - неизменно
            for (int city_x = X1; city_x <= X2; city_x++)
            {
                for (int city_y = Y1; city_y <= Y2; city_y++)
                {
                    cities.Add(new City(city_x, city_y, this, _map));
                }
            }
        }

        // Метод для проверки наличия связей с другими странами
        public bool HasConnectionsToOtherCountries()
        {
            // Перебираем только граничные города
            foreach (var city in GetBorderCities())
            {
                foreach (var neighbor in city.Neighbors)
                {
                    if (neighbor.Country != this)
                        return true;
                }
            }

            return false;
        }

        private List<City> GetBorderCities()
        {
            // Добавляем города с координатами на границах 
            return Cities.Where(c =>
              c.X == X1 || c.X == X2 ||
              c.Y == Y1 || c.Y == Y2).ToList();
        }
    }
}