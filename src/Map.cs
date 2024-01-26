namespace Eurodiffusion_from_abdulllah111
{
    public class Map : IMap
    {
        // Коллекция стран
        public List<Country> Countries { get; set; }
        public int DaysToCompletion { get; private set; }
        public int ValidCountries { get; private set; }

        public Map()
        {
            Countries = new List<Country>();
        }
        // Метод для добавления страны на карту
        public void AddCountry(Country country)
        {
            // Проверяем пересечение с каждой существующей страной
            foreach (var existing in Countries)
            {
                // Проверка угловых точек новой страны
                if (IsPointInside(existing, country.X1, country.Y1) ||
                    IsPointInside(existing, country.X2, country.Y2))
                {
                    throw new Exception($"Страна {country.Name} пересекает границу страны {existing.Name}");
                }

            }

            // Страна не пересекается, добавляем
            Countries.Add(country);
            ConnectCities();

        }

        // Метод для проверки границ на пересечение
        // (если точка входит в прямоугольник страны или её контур)
        private static bool IsPointInside(Country country, int x, int y)
        {
            return x >= country.X1 &&
                   x <= country.X2 &&
                   y >= country.Y1 &&
                   y <= country.Y2;
        }
        // Метод для добавления/обновления связей с соседями у городов
        private void ConnectCities()
        {
            foreach (var country in Countries)
            {
                foreach (var city in country.Cities)
                {
                    city.AddNeighbors();
                }
            }
        }


        // Метод для отрисовки карты
        public void PrintMap()
        {
            int maxX = Countries.SelectMany(country => country.Cities.Select(city => city.X)).Max();
            int maxY = Countries.SelectMany(country => country.Cities.Select(city => city.Y)).Max();

            for (int y = 0; y <= maxY; y++)
            {
                for (int x = 0; x <= maxX; x++)
                {
                    PrintCoordinateLabel(x, y);

                    var city = FindCityAtCoordinates(Countries, x, y);

                    if (city != null)
                    {
                        Console.Write($"*:{city.X}:{city.Y}");

                        if (HasNeighborToTheRight(Countries, city))
                        {
                            Console.Write(" - ");
                        }
                        else
                        {
                            Console.Write("   ");
                        }
                    }
                    else
                    {
                        Console.Write("        ");
                    }
                }
                Console.WriteLine();
            }
        }

        // Метод для отрисовки координат
        private static void PrintCoordinateLabel(int x, int y)
        {
            if (x == 0 && y >= 1) Console.Write($"y{y}");
            if (y == 0 && x >= 1) Console.Write($"x{x}");
        }

        // Метод для проверки соседей с права
        private static bool HasNeighborToTheRight(List<Country> countries, City city)
        {
            int x = city.X;
            int y = city.Y;

            return city.Neighbors.Any(neighbor => neighbor.X == x + 1 && neighbor.Y == y);
        }


        // Метод для проверки города по координатам x:y
        private static City? FindCityAtCoordinates(List<Country> countries, int x, int y)
        {
            foreach (var country in countries)
            {
                var city = country.Cities.FirstOrDefault(c => c.X == x && c.Y == y);
                if (city != null)
                {
                    return city;
                }
            }

            return null;
        }


        // Симуляция модели
        public void SimulateDaysToCompletion()
        {
            DaysToCompletion = 1;
            ValidCountries = Countries.Count;

            // Проверить каждую страну на наличие соседей
            // Если страна не имеет соседей, то она завершена, а количество валют уменьшается на 1
            foreach (var country in Countries)
            {
                if (!country.HasConnectionsToOtherCountries())
                {
                    country.IsComplete = true;
                    foreach(var city in country.Cities)
                    {
                        city.IsComplete = true;
                    }
                    ValidCountries--;
                }
            }
            while (true)
            {
                TransferCoinsBetweenCountries();
                if (Countries.All(country => country.IsComplete)) break;
                DaysToCompletion++;
            }

            foreach (var country in Countries)
            {
                Console.WriteLine($"Страна {country.Name} завершилась за {country.CompletionDay} дней.");
            }
            Console.WriteLine($"Все страны завершены за {DaysToCompletion} дней.");
        }

        // Метод для передачи монет между городами
        private void TransferCoinsBetweenCountries()
        {
            foreach (Country country in Countries)
            {
                foreach (City city in country.Cities)
                {
                    // Передача текущего баланса соседям
                    TransferOutgoing(city);

                    // Получение входящих монет от соседей
                    ReceiveIncoming(city);
                }

                // Обновление баланса городов внутри страны за день
                UpdateBalances(country);

            }
        }

        // Метод передача исходящих монет соседям
        private static void TransferOutgoing(City city)
        {
            foreach (City neighbor in city.Neighbors)
            {

                int amount = city.Balance[city.Country.Index] / 1000;
                if (amount > 0)
                {
                    if (!neighbor.Incoming.ContainsKey(city.Country.Index)) neighbor.Incoming[city.Country.Index] = 0;
                    city.Balance[city.Country.Index] -= amount;
                    neighbor.Incoming[city.Country.Index] += amount;

                }
            }
        }
        // Метод получение входящих монет от соседей
        private static void ReceiveIncoming(City city)
        {

            foreach (City neighbor in city.Neighbors)
            {

                foreach (var motive in neighbor.Balance.Keys)
                {
                    int amount = neighbor.Balance[motive] / 1000;
                    if (amount > 0)
                    {
                        if (!city.Incoming.ContainsKey(motive)) city.Incoming[motive] = 0;
                        city.Incoming[motive] += amount;

                    }

                }
            }

        }

        // Метод обновление баланса городов внутри страны
        private void UpdateBalances(Country country)
        {
            foreach (City city in country.Cities)
            {

                foreach (var motive in city.Incoming.Keys)
                {
                    if (!city.Balance.ContainsKey(motive))
                    {
                        city.Balance[motive] = 0;
                    }
                    city.Balance[motive] += city.Incoming[motive];
                }

                city.Incoming.Clear();

                CheckCompletionCity(city);
                CheckCompletionCountry(country);
            }
        }

        // Метод для проверки завершения города
        private void CheckCompletionCity(City city)
        {
            if (city.Balance.All(motive => motive.Value > 0) && city.Balance.Keys.ToList().Count == ValidCountries)
            {
                city.IsComplete = true;
            }
        }

        // Метод для проверки завершения страны
        private void CheckCompletionCountry(Country country)
        {
            country.IsComplete = country.Cities.All(city => city.IsComplete);

            if (country.CompletionDay == 0 && country.IsComplete)
            {
                country.CompletionDay = DaysToCompletion;
                return;
            }
        }
    }
}