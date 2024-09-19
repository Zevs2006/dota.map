using System;

class DotaMap
{
    const int size = 50;  // Размер карты 50x50
    char[,] originalMap = new char[size, size];  // Массив для сохранения исходного состояния карты
    char[,] map = new char[size, size];
    bool[,] isOccupied = new bool[size, size]; // Для отслеживания, занята ли ячейка крипами
    int heroX = 25, heroY = 25;  // Начальная позиция героя в центре карты
    Creeper[] creeps; // Классы для крипов
    RedCreeper[] redCreeps;
    NewCreeper[] newCreeps; // Массив для хранения новых крипов
    NewRedCreeper[] newRedCreepers;



    // Класс Creeper представляет крипов
    class Creeper
    {
        public int X { get; set; } // Координата X крипа на карте
        public int Y { get; set; } // Координата Y крипа на карте
        public bool HasPassedThrough { get; set; } // Флаг, прошел ли крип через контрольную точку
        public Direction CurrentDirection { get; set; } = Direction.MovingLeft; // Текущее направление движения крипа
    }

    // Класс RedCreeper наследует от Creeper и представляет красных крипов
    class RedCreeper
    {
        public int X { get; set; } // Координата X крипа на карте
        public int Y { get; set; } // Координата Y крипа на карте
        public bool HasPassedThrough { get; set; } // Флаг, прошел ли крип через контрольную точку
        public Direction CurrentDirection { get; set; } = Direction.MovingLeft; // Текущее направление движения крипа
    }

    // Класс NewCreeper наследует от Creeper и представляет новых крипов
    class NewCreeper : Creeper
    {
        public NewCreeper(int x, int y) // Конструктор для создания нового крипа с начальными координатами
        {
            X = x; // Устанавливаем начальную координату X
            Y = y; // Устанавливаем начальную координату Y
            HasPassedThrough = false; // Устанавливаем, что крип еще не прошел через контрольную точку
            CurrentDirection = Direction.MovingRight; // Начальное направление движения - право
        }
    }

    class NewRedCreeper
    {
        public int X { get; set; } // Координата X крипа на карте
        public int Y { get; set; } // Координата Y крипа на карте
        public bool HasPassedThrough { get; set; } // Флаг, прошел ли крип через контрольную точку
        public Direction CurrentDirection { get; set; } = Direction.MovingLeft; // Текущее направление движения крипа
    }

    //void InitializeNewRedCreeps() 
    //{
    //    newRedCreepers = new NewRedCreeper[4]; 
    //    for (int i = 0; i < newRedCreepers.Length; i++) 
    //    {
    //        newRedCreepers[i] = new NewRedCreeper(10, 47 + i); 
    //    }
    //}

    void InitializeNewRedCreeps() 
    {
        newRedCreepers = new NewRedCreeper[4]; // Создаем массив из 4 обычных крипов
        for (int i = 0; i < newRedCreepers.Length; i++) // Цикл для создания каждого крипа
        {
            newRedCreepers[i] = new NewRedCreeper { X = 14 - i, Y = 47, CurrentDirection = Direction.MovingDown }; // Устанавливаем начальную позицию для каждого крипа
        }
    }

    void InitializeNewCreeps() // Метод для инициализации новых крипов
    {
        newCreeps = new NewCreeper[4]; // Создаем массив из 4 крипов
        for (int i = 0; i < newCreeps.Length; i++) // Цикл для создания крипов
        {
            newCreeps[i] = new NewCreeper(47, 11 + i); // Устанавливаем начальные позиции для новых крипов
        }
    }

    void InitializeRedCreeps() // Метод для инициализации красных крипов
    {
        redCreeps = new RedCreeper[4]; // Создаем массив из 4 обычных крипов
        for (int i = 0; i < redCreeps.Length; i++) // Цикл для создания каждого крипа
        {
            redCreeps[i] = new RedCreeper { X = 2 , Y = 39 -i, CurrentDirection = Direction.MovingLeft }; // Устанавливаем начальную позицию для каждого крипа
        }
    }

    public enum Direction // Перечисление для направления движения крипов
    {
        MovingDown,  // Движение вниз
        MovingRight, // Движение вправо
        MovingUp,    // Движение вверх
        MovingLeft,  // Движение влево
        Completed    // Завершение движения
    }

    public DotaMap() // Конструктор карты Dota
    {
        InitializeMap();         // Инициализируем карту
        SaveOriginalMap();       // Сохраняем исходную карту
        InitializeCreeps();      // Инициализируем обычных крипов
        InitializeRedCreeps();   // Инициализируем красных крипов
        InitializeNewCreeps();   // Инициализируем новых крипов
        InitializeNewRedCreeps();
    }

    void InitializeCreeps() // Метод для инициализации обычных крипов
    {
        creeps = new Creeper[4]; // Создаем массив из 4 обычных крипов
        for (int i = 0; i < creeps.Length; i++) // Цикл для создания каждого крипа
        {
            creeps[i] = new Creeper { X = 39 - i, Y = 2, CurrentDirection = Direction.MovingLeft }; // Устанавливаем начальную позицию для каждого крипа
        }
    }

    void InitializeMap() // Метод для инициализации карты
    {
        for (int i = 0; i < size; i++) // Цикл для заполнения карты
        {
            for (int j = 0; j < size; j++) // Вложенный цикл для каждой ячейки карты
            {
                map[i, j] = '.';  // Устанавливаем пустую клетку
            }
        }

        // Фонтан и тироид для сил Света (полусфера в левом нижнем углу)
        for (int i = size - 10; i < size; i++) // Цикл для создания полусферы фонтана
        {
            for (int j = 0; j < 10; j++) // Цикл для горизонтальной части фонтана
            {
                map[i, j] = '▒';  // Заполняем символом '▒' для обозначения полусферы фонтана сил Света
            }
        }
        map[size - 1, 0] = 'F'; // Устанавливаем сам фонтан сил Света в левом нижнем углу

        // Фонтан и тироид для сил Тьмы (полусфера в правом верхнем углу)
        for (int i = 0; i < 10; i++) // Цикл для создания полусферы фонтана сил Тьмы
        {
            for (int j = size - 10; j < size; j++) // Цикл для горизонтальной части фонтана
            {
                map[i, j] = '▒';  // Заполняем символом '▒' для обозначения полусферы фонтана сил Тьмы
            }
        }
        map[0, size - 1] = 'F';  // Устанавливаем сам фонтан сил Тьмы в правом верхнем углу

        // Установим основные элементы карты
        map[20, 20] = 'R'; // Устанавливаем Рошана в центре карты

        // Река шириной 3 от (4, 4) до (46, 46)
        int startX = 4; // Начальная координата X реки
        int startY = 4; // Начальная координата Y реки
        int endX = 46;  // Конечная координата X реки
        int endY = 46;  // Конечная координата Y реки

        // Определяем угол наклона линии реки
        double slope = (double)(endY - startY) / (endX - startX); // Вычисляем наклон линии
        double intercept = startY - slope * startX; // Вычисляем смещение

        for (int x = startX; x <= endX; x++) // Цикл для заполнения линии реки
        {
            int y = (int)(slope * x + intercept); // Вычисляем координату Y для текущего X
            if (y >= 0 && y < size) // Проверяем, что координата Y находится в пределах карты
            {
                map[x, y] = '~';  // Заполняем основную линию реки символом '~'
                if (x > 0) // Проверяем возможность размещения побочных линий
                {
                    if (x - 1 < size && y >= 0 && y < size)
                        map[x - 1, y] = '~';  // Линия слева от основной
                    if (y - 1 >= 0)
                        map[x, y - 1] = '~';  // Линия выше основной
                }
                if (x + 1 < size) // Проверяем возможность размещения нижней линии
                {
                    if (y + 1 < size)
                        map[x + 1, y] = '~';  // Линия ниже основной
                }
            }
        }



        // Средняя диагональная линия шириной 3 символа от нижнего левого угла до верхнего правого угла
        for (int i = 0; i < size; i++)
        {
            // Основная линия
            if (i < 8 || i >= size - 8) // Убираем 8 символов с каждой стороны диагонали, заменяя их на символы фонтана
            {
                map[size - 1 - i, i] = '▒'; // Замена символов на символы фонтана
            }
            else
            {
                // Основная диагональная линия шириной 3 символа
                map[size - 1 - i, i] = '■';  // Центральный символ диагонали
                if (i > 0)
                {
                    if (size - 1 - i + 1 < size) // Проверка на границу карты
                        map[size - 1 - i + 1, i] = '■';  // Символ под диагональю
                    if (size - 1 - i - 1 >= 0) // Проверка на границу карты
                        map[size - 1 - i - 1, i] = '■';  // Символ над диагональю
                }
            }
        }

        // Линия из квадратов от (1, 2) до (40, 2)
        for (int i = 1; i <= 40; i++)
        {
            map[2, i] = '■';  // Символ для горизонтальной линии
        }

        // Вертикальная линия от (3, 1) до (40, 1) включительно
        for (int i = 1; i <= 40; i++)
        {
            map[i, 1] = '■';  // Символ для вертикальной линии
        }

        // Дополнительные вертикальные линии сразу справа от текущей вертикальной линии
        int offset = 1;  // Расстояние между вертикальными линиями
        for (int k = 1; k <= 2; k++) // Добавляем две дополнительные вертикальные линии
        {
            int column = 1 + offset * k; // Определяем столбец для каждой линии
            for (int i = 1; i <= 40; i++)
            {
                if (column < size)
                    map[i, column] = '■';  // Символ для дополнительных вертикальных линий
            }
        }

        // Горизонтальные линии по бокам от существующей линии
        int horizontalLineY1 = 1;  // Координата Y для линии выше основной горизонтальной линии
        int horizontalLineY2 = 3;  // Координата Y для линии ниже основной горизонтальной линии
        for (int i = 1; i <= 40; i++)
        {
            map[horizontalLineY1, i] = '■';  // Линия выше основной линии
            map[horizontalLineY2, i] = '■';  // Линия ниже основной линии
        }

        // Горизонтальная линия от (48, 10) до (48, 48)
        for (int i = 10; i <= 48; i++)
        {
            map[48, i] = '■';  // Символ для горизонтальной линии
        }

        // Две новые горизонтальные линии выше существующей линии
        for (int i = 10; i <= 48; i++)
        {
            map[47, i] = '■';  // Первая линия выше
            map[46, i] = '■';  // Вторая линия выше
        }

        // Новая вертикальная линия от (9, 48) до (48, 48)
        for (int i = 9; i <= 48; i++)
        {
            map[i, 48] = '■';  // Символ для вертикальной линии
        }

        // Две новые вертикальные линии слева от существующей вертикальной линии
        for (int i = 9; i <= 48; i++)
        {
            if (47 < size)
                map[i, 47] = '■';  // Первая линия слева
            if (46 < size)
                map[i, 46] = '■';  // Вторая линия слева
        }

        // Расставляем 3 башни в строке 2 от координат (5, 2) до (40, 2) на равном расстоянии
        AddTowersInRow(2, 5, 40, 3); // Вызов функции для установки башен в строке 2

        // Расставляем 3 башни в столбце 2 от координат (5, 2) до (40, 2) на равном расстоянии
        AddTowersInColumn(5, 40, 2, 3); // Вызов функции для установки башен в столбце 2

        // Расставляем дополнительные 6 башен на диагонали от (39, 9) до (9, 39)
        AddTowersOnDiagonal(39, 9, 9, 39, 6); // Устанавливаем башни на диагонали

        // Добавляем 3 башни в строке 47 от (47, 10) до (47, 44) на равном расстоянии
        AddTowersInRow(47, 10, 44, 3); // Вызов функции для установки башен в строке 47

        // Добавляем 3 башни в столбце 47 от (10, 47) до (44, 47) на равном расстоянии
        AddTowersInColumn(10, 44, 47, 3); // Вызов функции для установки башен в столбце 47

        // Устанавливаем трон на координаты (3, 47) и (47, 3)
        map[2, 47] = 'H';  // Трон для первой команды (силы Света)
        map[47, 2] = 'H';  // Трон для второй команды (силы Тьмы)

        // Добавляем дополнительные башни на указанные координаты
        map[2, 46] = 'T';  // Башня на (2, 46)
        map[3, 47] = 'T';  // Башня на (3, 47)
        map[46, 2] = 'T';  // Башня на (46, 2)
        map[47, 3] = 'T';  // Башня на (47, 3)
    }


    void SaveOriginalMap()
    {
        // Копируем текущее состояние карты в originalMap для восстановления
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                originalMap[i, j] = map[i, j];
            }
        }
    }

    void MoveCreeps()
    {
        // Удаляем крипов из текущих позиций на карте
        foreach (var creep in creeps)
        {
            if (creep.X >= 0 && creep.X < size && creep.Y >= 0 && creep.Y < size)
            {
                isOccupied[creep.X, creep.Y] = false; // Освобождаем ячейку
                map[creep.X, creep.Y] = creep.HasPassedThrough ? 'О' : originalMap[creep.X, creep.Y]; // Восстанавливаем исходное состояние
            }
        }

        foreach (var redCreep in redCreeps)
        {
            if (redCreep.X >= 0 && redCreep.X < size && redCreep.Y >= 0 && redCreep.Y < size)
            {
                isOccupied[redCreep.X, redCreep.Y] = false; // Освобождаем ячейку
                map[redCreep.X, redCreep.Y] = redCreep.HasPassedThrough ? 'о' : originalMap[redCreep.X, redCreep.Y]; // Восстанавливаем исходное состояние
            }
        }

        foreach (var newCreep in newCreeps)
        {
            if (newCreep.X >= 0 && newCreep.X < size && newCreep.Y >= 0 && newCreep.Y < size)
            {
                isOccupied[newCreep.X, newCreep.Y] = false; // Освобождаем ячейку
                map[newCreep.X, newCreep.Y] = newCreep.HasPassedThrough ? '0' : originalMap[newCreep.X, newCreep.Y]; // Восстанавливаем исходное состояние
            }
        }

        foreach (var newRedCreepers in newRedCreepers)
        {
            if (newRedCreepers.X >= 0 && newRedCreepers.X < size && newRedCreepers.Y >= 0 && newRedCreepers.Y < size)
            {
                isOccupied[newRedCreepers.X, newRedCreepers.Y] = false; // Освобождаем ячейку
                map[newRedCreepers.X, newRedCreepers.Y] = newRedCreepers.HasPassedThrough ? '0' : originalMap[newRedCreepers.X, newRedCreepers.Y]; // Восстанавливаем исходное состояние
            }
        }

        // Перемещаем обычных крипов по карте
        foreach (var creep in creeps)
        {
            switch (creep.CurrentDirection)
            {
                case Direction.MovingLeft:
                    creep.X--; // Двигаемся влево
                    if (creep.X == 2 && creep.Y == 2)
                    {
                        creep.CurrentDirection = Direction.MovingUp; // Поворачиваем вверх
                    }
                    break;

                case Direction.MovingUp:
                    creep.Y++; // Двигаемся вверх
                    if (creep.X == 2 && creep.Y >= 47)
                    {
                        creep.CurrentDirection = Direction.Completed; // Поворачиваем вправо
                    }
                    break;

                case Direction.MovingRight:
                    creep.X++; // Двигаемся вправо
                    break;
            }

            if (creep.X >= 0 && creep.X < size && creep.Y >= 0 && creep.Y < size)
            {
                if (creep.X == 34 && creep.Y == 2)
                {
                    creep.HasPassedThrough = true; // Отмечаем, что крип прошел через определенную точку
                }

                isOccupied[creep.X, creep.Y] = true; // Занимаем ячейку
                map[creep.X, creep.Y] = 'О'; // Обновляем карту

                if (creep.HasPassedThrough)
                {
                    Console.ForegroundColor = ConsoleColor.Green; // Устанавливаем цвет для крипов, которые прошли через точку
                }
            }
        }

        // Перемещаем красных крипов по карте
        foreach (var redCreep in redCreeps)
        {
            if (redCreep.X >= 0 && redCreep.X < size && redCreep.Y >= 0 && redCreep.Y < size)
            {
                switch (redCreep.CurrentDirection)
                {
                    case Direction.MovingLeft:
                        redCreep.Y--; // Двигаемся влево
                        if (redCreep.X == 2 && redCreep.Y == 2)
                        {
                            redCreep.CurrentDirection = Direction.MovingDown; // Поворачиваем вверх
                        }
                        break;

                    case Direction.MovingDown:
                        redCreep.X++; // Двигаемся вверх
                        if (redCreep.X == 47 && redCreep.Y == 2)
                        {
                            redCreep.CurrentDirection = Direction.Completed; // Поворачиваем вправо
                        }
                        break;
                }

                if (redCreep.X >= 0 && redCreep.X < size && redCreep.Y >= 0 && redCreep.Y < size)
                {
                    isOccupied[redCreep.X, redCreep.Y] = true; // Занимаем ячейку
                    map[redCreep.X, redCreep.Y] = 'о'; // Обновляем карту

                    if (redCreep.HasPassedThrough)
                    {
                        Console.ForegroundColor = ConsoleColor.Red; // Устанавливаем цвет для красных крипов, которые прошли через точку
                    }
                }
            }
        }

        // Перемещаем новые крипы по карте
        foreach (var newCreep in newCreeps)
        {
            switch (newCreep.CurrentDirection)
            {
                case Direction.MovingRight:
                    newCreep.Y++; // Двигаемся влево
                    if (newCreep.X == 47 && newCreep.Y == 47)
                    {
                        newCreep.CurrentDirection = Direction.MovingUp; // Поворачиваем вверх
                    }
                    break;

                case Direction.MovingUp:
                    newCreep.X--; // Двигаемся вверх
                    if (newCreep.X == 47 && newCreep.Y == 2)
                    {
                        newCreep.CurrentDirection = Direction.Completed; // Поворачиваем вправо
                    }
                    break;
            }

            if (newCreep.X >= 0 && newCreep.X < size && newCreep.Y >= 0 && newCreep.Y < size)
            {
                if (newCreep.X == 47 && newCreep.Y == 17)
                {
                    newCreep.HasPassedThrough = true; // Отмечаем, что крип прошел через определенную точку
                }

                isOccupied[newCreep.X, newCreep.Y] = true; // Занимаем ячейку
                map[newCreep.X, newCreep.Y] = 'О'; // Обновляем карту

                if (newCreep.HasPassedThrough)
                {
                    Console.ForegroundColor = ConsoleColor.Green; // Устанавливаем цвет для крипов, которые прошли через точку
                }
            }
        }

        foreach (var newRedCreepers in newRedCreepers)
        {
            switch (newRedCreepers.CurrentDirection)
            {
                case Direction.MovingLeft:
                    newRedCreepers.Y--; // Двигаемся влево
                    if (newRedCreepers.X == 47 && newRedCreepers.Y == 47)
                    {
                        newRedCreepers.CurrentDirection = Direction.Completed; // Поворачиваем вверх
                    }
                    break;

                case Direction.MovingDown:
                    newRedCreepers.X++; // Двигаемся вверх
                    if (newRedCreepers.X == 47 && newRedCreepers.Y == 47)
                    {
                        newRedCreepers.CurrentDirection = Direction.MovingLeft; // Поворачиваем вправо
                    }
                    break;
            }

            if (newRedCreepers.X >= 0 && newRedCreepers.X < size && newRedCreepers.Y >= 0 && newRedCreepers.Y < size)
            {
                if (newRedCreepers.X == 17 && newRedCreepers.Y == 47)
                {
                    newRedCreepers.HasPassedThrough = true; // Отмечаем, что крип прошел через определенную точку
                }

                isOccupied[newRedCreepers.X, newRedCreepers.Y] = true; // Занимаем ячейку
                map[newRedCreepers.X, newRedCreepers.Y] = 'o'; // Обновляем карту

                if (newRedCreepers.HasPassedThrough)
                {
                    Console.ForegroundColor = ConsoleColor.Red; // Устанавливаем цвет для крипов, которые прошли через точку
                }
            }
        }

    }





    void RestoreMap()
    {
        // Восстанавливаем карту из оригинальной карты для всех не занятых ячеек
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                if (!isOccupied[i, j])
                {
                    map[i, j] = originalMap[i, j]; // Восстанавливаем исходное состояние ячейки
                }
            }
        }
    }

    void AddTowersInRow(int row, int startCol, int endCol, int numberOfTowers)
    {
        // Добавляем башни в указанную строку от startCol до endCol
        if (numberOfTowers < 1 || startCol >= endCol || row < 0 || row >= size)
        {
            Console.WriteLine("Invalid parameters for adding towers."); // Проверяем валидность параметров
            return;
        }

        int spacing = (endCol - startCol) / (numberOfTowers - 1); // Расчитываем расстояние между башнями

        for (int i = 0; i < numberOfTowers; i++)
        {
            int col = startCol + i * spacing; // Вычисляем колонку для текущей башни
            if (col >= size) break; // Если колонка выходит за пределы карты, выходим из цикла
            map[row, col] = 'T';  // Устанавливаем символ башни
        }
    }

    void AddTowersInColumn(int startRow, int endRow, int column, int numberOfTowers)
    {
        // Добавляем башни в указанную колонку от startRow до endRow
        if (numberOfTowers < 1 || startRow >= endRow || column < 0 || column >= size)
        {
            Console.WriteLine("Invalid parameters for adding towers."); // Проверяем валидность параметров
            return;
        }

        int spacing = (endRow - startRow) / (numberOfTowers - 1); // Расчитываем расстояние между башнями

        for (int i = 0; i < numberOfTowers; i++)
        {
            int row = startRow + i * spacing; // Вычисляем строку для текущей башни
            if (row >= size) break; // Если строка выходит за пределы карты, выходим из цикла
            map[row, column] = 'T';  // Устанавливаем символ башни
        }
    }

    void AddTowersOnDiagonal(int startX, int startY, int endX, int endY, int numberOfTowers)
    {
        // Добавляем башни по диагонали от startX, startY до endX, endY
        if (numberOfTowers < 1 || startX <= endX || startY >= endY)
        {
            Console.WriteLine("Invalid parameters for adding towers."); // Проверяем валидность параметров
            return;
        }

        int dx = Math.Abs(endX - startX); // Вычисляем изменение по X
        int dy = endY - startY; // Вычисляем изменение по Y
        int spacing = Math.Min(dx, dy) / (numberOfTowers - 1); // Расчитываем расстояние между башнями

        for (int i = 0; i < numberOfTowers; i++)
        {
            int x = startX - i * spacing + 1;  // Вычисляем координату X для текущей башни (с учетом вычитания и добавления 1)
            int y = startY + i * spacing;      // Вычисляем координату Y для текущей башни

            if (x >= 0 && x < size && y >= 0 && y < size)
            {
                map[x, y] = 'T';  // Устанавливаем символ башни
            }
        }
    }

    void DisplayMap()
    {
        Console.Clear(); // Очищаем консольный экран

        for (int i = 0; i < size; i++) // Проходим по строкам карты
        {
            for (int j = 0; j < size; j++) // Проходим по столбцам карты
            {
                bool isCreep = false; // Флаг для проверки, находится ли клетка в позиции крипа
                ConsoleColor defaultColor = Console.ForegroundColor; // Сохраняем цвет по умолчанию

                foreach (var creep in creeps) // Проходим по всем крипам
                {
                    if (creep.X == i && creep.Y == j) // Проверяем, совпадает ли текущая позиция с позицией крипа
                    {
                        isCreep = true; // Помечаем клетку как содержащую крипа

                        // Если это крип, который начинается с (38, 2), устанавливаем зелёный цвет
                        if (creep.X == 38 && creep.Y == 2)
                        {
                            Console.ForegroundColor = ConsoleColor.Green; // Устанавливаем цвет для этого крипа
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.White; // Устанавливаем цвет для других крипов
                        }
                        break; // Выходим из цикла после нахождения крипа
                    }

                    if (creep.X == i && creep.Y == j) // Проверяем, совпадает ли текущая позиция с позицией другого крипа
                    {
                        isCreep = true; // Помечаем клетку как содержащую крипа

                        // Если это крип, который начинается с (2, 33), устанавливаем красный цвет
                        if (creep.X == 2 && creep.Y == 33)
                        {
                            Console.ForegroundColor = ConsoleColor.Red; // Устанавливаем цвет для этого крипа
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.White; // Устанавливаем цвет для других крипов
                        }
                        break; // Выходим из цикла после нахождения крипа
                    }
                }

                if (map[i, j] == 'о') // Проверяем, если текущая ячейка содержит символ 'о'
                {
                    Console.ForegroundColor = ConsoleColor.Red; // Устанавливаем цвет для символа крипов
                }
                else if (map[i, j] == 'О') // Проверяем, если текущая ячейка содержит символ 'О'
                {
                    Console.ForegroundColor = ConsoleColor.Green; // Устанавливаем цвет для символа крипов
                }
                else // Если ячейка не содержит символы 'о' или 'О'
                {
                    Console.ForegroundColor = ConsoleColor.White; // Устанавливаем цвет по умолчанию
                }

                // Если это не крип, устанавливаем цвет для других объектов
                if (!isCreep)
                {
                    if ((i == size - 1 && j == 0) || (i >= size - 10 && j < 10)) // Проверяем для зелёного фонтана
                    {
                        Console.ForegroundColor = ConsoleColor.Green; // Устанавливаем зелёный цвет для фонтана
                    }
                    else if ((i == 0 && j == size - 1) || (i < 10 && j >= size - 10)) // Проверяем для красного фонтана
                    {
                        Console.ForegroundColor = ConsoleColor.Red; // Устанавливаем красный цвет для фонтана
                    }
                    else if (map[i, j] == 'T') // Проверяем, если это башня
                    {
                        if (i > j) // Башни ниже реки
                        {
                            Console.ForegroundColor = ConsoleColor.Green; // Устанавливаем зелёный цвет для башен ниже реки
                        }
                        else // Башни выше реки
                        {
                            Console.ForegroundColor = ConsoleColor.Red; // Устанавливаем красный цвет для башен выше реки
                        }
                    }
                    else if (map[i, j] == '~') // Проверяем, если это река
                    {
                        Console.ForegroundColor = ConsoleColor.Blue; // Устанавливаем синий цвет для реки
                    }
                    else // Для всех остальных объектов
                    {
                        Console.ForegroundColor = ConsoleColor.White; // Устанавливаем цвет по умолчанию
                    }
                }

                Console.Write(map[i, j] + " "); // Выводим символ текущей ячейки с пробелом
                Console.ForegroundColor = defaultColor; // Возвращаем цвет по умолчанию
            }
            Console.WriteLine(); // Переходим на новую строку после завершения строки
        }

        Console.SetCursorPosition(heroY * 2, heroX); // Устанавливаем курсор на позицию героя
        Console.ResetColor(); // Сбрасываем цвет в консоли
    }





    class Program
    {
        static void Main()
        {
            DotaMap map = new DotaMap(); // Создаем экземпляр класса DotaMap

            while (true) // Запускаем бесконечный цикл для обновления и отображения карты
            {
                map.DisplayMap(); // Отображаем текущее состояние карты
                Thread.Sleep(500); // Задержка в 500 миллисекунд для визуализации

                map.MoveCreeps(); // Перемещаем крипов по карте
                map.DisplayMap(); // Отображаем обновленное состояние карты с крипами
                Thread.Sleep(500); // Задержка в 500 миллисекунд для визуализации

                map.RestoreMap(); // Восстанавливаем исходное состояние карты
                map.DisplayMap(); // Отображаем восстановленное состояние карты
                Thread.Sleep(500); // Задержка в 500 миллисекунд для визуализации

                // Дополнительная логика игры
            }
        }
    }
}
