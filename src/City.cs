using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Eurodiffusion_from_abdulllah111
{
    public class City
    {
        private readonly IMap _map;

        // Координаты города
        public int X { get; private set; }
        public int Y { get; private set; }

        // Страна, к которой принадлежит город
        public Country Country { get; private set; }

        // Баланс монет по валютам стран
        public Dictionary<Guid, int> Balance { get; set; }

        // Входящие монеты по валютам стран
        public Dictionary<Guid, int> Incoming { get; set; }

        // Флаг завершенности города
        public bool IsComplete { get; set; }

        // Список соседей города
        public List<City> Neighbors { get; set; }


        // Конструктор города
        public City(int x, int y, Country country, IMap map)
        {
            _map = map;
            X = x;
            Y = y;
            Country = country;

            // Инициализируем коллекции
            Balance = new Dictionary<Guid, int>();
            Incoming = new Dictionary<Guid, int>();
            Neighbors = new List<City>();

            IsComplete = false;
            
            // Изначально лям монет для каждого города
            Balance[country.Index] = 1000000;
        }
        public void AddNeighbors()
        {
            Neighbors.Clear();
            AddNeighbor(-1, 0); // Сосед сверху
            AddNeighbor(1, 0);  // Сосед снизу
            AddNeighbor(0, 1);  // Сосед справа
            AddNeighbor(0, -1); // Сосед слева
        }

        private void AddNeighbor(int deltaX, int deltaY)
        {
            int newX = X + deltaX;
            int newY = Y + deltaY;

            // Ищем соседа по координатам
            var neighbor = _map.Countries
                .SelectMany(c => c.Cities)
                .FirstOrDefault(c => c.X == newX && c.Y == newY);

            // Если сосед найден
            if (neighbor != null)
            {
                Neighbors.Add(neighbor!);
            }
        }
    }
}