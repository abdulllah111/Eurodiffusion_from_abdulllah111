using Eurodiffusion_from_abdulllah111;

// Создаем карту
IMap MAP = new Map();


//Добавляем страны
MAP.AddCountry(
    new Country("Россия", 9, 5, 18, 13, MAP)
);
MAP.AddCountry(
    new Country("Киргизия", 1, 1, 2, 1, MAP)
);
MAP.AddCountry(
    new Country("Белорусь", 1, 3, 2, 4, MAP)
);
MAP.AddCountry(
    new Country("Казахстан", 3, 2, 8, 5, MAP)
);



//Выводим карту по желанию (не стал запариватся с выравниванием, просто для визуализации данных)
MAP.PrintMap();

// Запускаем модель расчета дней
MAP.SimulateDaysToCompletion();