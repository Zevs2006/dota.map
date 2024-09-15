using System;

class DotaMap
{
    const int size = 50;  // Размер карты 50x50
    char[,] map = new char[size, size];
    int heroX = 25, heroY = 25;  // Начальная позиция героя в центре карты

    public DotaMap()
    {
        InitializeMap();
    }

    void InitializeMap()
    {
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                map[i, j] = '.';  // Пустая клетка
            }
        }

        // Фонтан и тироид для сил Света (полусфера в левом нижнем углу)
        for (int i = size - 10; i < size; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                map[i, j] = '▒';  // Полусфера фонтана сил Света
            }
        }
        map[size - 1, 0] = 'F'; // Сам фонтан сил Света

        // Фонтан и тироид для сил Тьмы (полусфера в правом верхнем углу)
        for (int i = 0; i < 10; i++)
        {
            for (int j = size - 10; j < size; j++)
            {
                map[i, j] = '▒';  // Полусфера фонтана сил Тьмы
            }
        }
        map[0, size - 1] = 'F';  // Сам фонтан сил Тьмы

        // Установим основные элементы карты
        map[20, 20] = 'R'; // Рошан

        // Река шириной 3 от (4, 4) до (46, 46)
        int startX = 4;
        int startY = 4;
        int endX = 46;
        int endY = 46;

        // Определяем угол наклона линии реки
        double slope = (double)(endY - startY) / (endX - startX);
        double intercept = startY - slope * startX;

        for (int x = startX; x <= endX; x++)
        {
            int y = (int)(slope * x + intercept);
            if (y >= 0 && y < size)
            {
                map[x, y] = '~';  // Основная линия реки
                if (x > 0)
                {
                    if (x - 1 < size && y >= 0 && y < size)
                        map[x - 1, y] = '~';  // Линия слева
                    if (y - 1 >= 0)
                        map[x, y - 1] = '~';  // Линия выше
                }
                if (x + 1 < size)
                {
                    if (y + 1 < size)
                        map[x + 1, y] = '~';  // Линия ниже
                }
            }
        }

        // Средняя диагональная линия шириной 3 символа от нижнего левого угла до верхнего правого угла
        for (int i = 0; i < size; i++)
        {
            // Основная линия
            if (i < 8 || i >= size - 8) // Убираем 8 символов с каждой стороны диагонали
            {
                map[size - 1 - i, i] = '▒'; // Замена символов на символы фонтана
            }
            else
            {
                // Основная диагональная линия шириной 3 символа
                map[size - 1 - i, i] = '■';  // Центральный символ
                if (i > 0)
                {
                    if (size - 1 - i + 1 < size)
                        map[size - 1 - i + 1, i] = '■';  // Под диагональю
                    if (size - 1 - i - 1 >= 0)
                        map[size - 1 - i - 1, i] = '■';  // Над диагональю
                }
            }
        }

        // Линия из квадратов от (1, 2) до (40, 2)
        for (int i = 1; i <= 40; i++)
        {
            map[2, i] = '■';  // Символ для линии
        }

        // Вертикальная линия от (3, 1) до (40, 1) включительно
        for (int i = 1; i <= 40; i++)
        {
            map[i, 1] = '■';  // Символ для вертикальной линии
        }

        // Дополнительные вертикальные линии сразу справа от текущей вертикальной линии
        int offset = 1;  // Расстояние между вертикальными линиями
        for (int k = 1; k <= 2; k++) // Для двух дополнительных линий
        {
            int column = 1 + offset * k; // Определяем столбец для каждой линии
            for (int i = 1; i <= 40; i++)
            {
                if (column < size)
                    map[i, column] = '■';  // Символ для дополнительных вертикальных линий
            }
        }

        // Горизонтальные линии по бокам от существующей линии
        int horizontalLineY1 = 1;  // Линия выше основной горизонтальной линии
        int horizontalLineY2 = 3;  // Линия ниже основной горизонтальной линии
        for (int i = 1; i <= 40; i++)
        {
            map[horizontalLineY1, i] = '■';  // Линия выше
            map[horizontalLineY2, i] = '■';  // Линия ниже
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
        AddTowersInRow(2, 5, 40, 3);

        // Расставляем 3 башни в столбце 2 от координат (5, 2) до (40, 2) на равном расстоянии
        AddTowersInColumn(5, 40, 2, 3);

        // Расставляем дополнительные 6 башен на диагонали от (39, 9) до (9, 39)
        AddTowersOnDiagonal(39, 9, 9, 39, 6);

        // Добавляем 3 башни в строке 47 от (47, 10) до (47, 44) на равном расстоянии
        AddTowersInRow(47, 10, 44, 3);

        // Добавляем 3 башни в столбце 47 от (10, 47) до (44, 47) на равном расстоянии
        AddTowersInColumn(10, 44, 47, 3);
        // Устанавливаем трон на координаты (3, 47) и (47, 3)
        map[2, 47] = 'H';  // Трон для первой команды
        map[47, 2] = 'H';  // Трон для второй команды

        // Добавляем дополнительные башни на указанные координаты
        map[2, 46] = 'T';  // Башня на (2, 46)
        map[3, 47] = 'T';  // Башня на (3, 47)
        map[46, 2] = 'T';  // Башня на (46, 2)
        map[47, 3] = 'T';  // Башня на (47, 3)
    }

    void AddTowersInRow(int row, int startCol, int endCol, int numberOfTowers)
    {
        if (numberOfTowers < 1 || startCol >= endCol || row < 0 || row >= size)
        {
            Console.WriteLine("Invalid parameters for adding towers.");
            return;
        }

        int spacing = (endCol - startCol) / (numberOfTowers - 1);

        for (int i = 0; i < numberOfTowers; i++)
        {
            int col = startCol + i * spacing;
            if (col >= size) break;
            map[row, col] = 'T';  // Символ башни
        }
    }

    void AddTowersInColumn(int startRow, int endRow, int column, int numberOfTowers)
    {
        if (numberOfTowers < 1 || startRow >= endRow || column < 0 || column >= size)
        {
            Console.WriteLine("Invalid parameters for adding towers.");
            return;
        }

        int spacing = (endRow - startRow) / (numberOfTowers - 1);

        for (int i = 0; i < numberOfTowers; i++)
        {
            int row = startRow + i * spacing;
            if (row >= size) break;
            map[row, column] = 'T';  // Символ башни
        }
    }

    void AddTowersOnDiagonal(int startX, int startY, int endX, int endY, int numberOfTowers)
    {
        if (numberOfTowers < 1 || startX <= endX || startY >= endY)
        {
            Console.WriteLine("Invalid parameters for adding towers.");
            return;
        }

        int dx = Math.Abs(endX - startX);
        int dy = endY - startY;
        int spacing = Math.Min(dx, dy) / (numberOfTowers - 1);

        for (int i = 0; i < numberOfTowers; i++)
        {
            int x = startX - i * spacing + 1;  // Вычитание для строк с добавлением 1
            int y = startY + i * spacing;      // Прибавление для столбцов

            if (x >= 0 && x < size && y >= 0 && y < size)
            {
                map[x, y] = 'T';  // Символ башни
            }
        }
    }

    public void DisplayMap()
    {
        Console.Clear();
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                if ((i == size - 1 && j == 0) || (i >= size - 10 && j < 10)) // Зелёный фонтан
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                }
                else if ((i == 0 && j == size - 1) || (i < 10 && j >= size - 10)) // Красный фонтан
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                }
                else if (map[i, j] == 'T') // Если это башня
                {
                    // Определим, находится ли башня ниже или выше реки (диагональной линии)
                    if (i > j) // Башни ниже реки
                    {
                        Console.ForegroundColor = ConsoleColor.Green; // Зелёные башни
                    }
                    else // Башни выше реки
                    {
                        Console.ForegroundColor = ConsoleColor.Red; // Красные башни
                    }
                }
                else if (map[i, j] == '~') // Если это река
                {
                    Console.ForegroundColor = ConsoleColor.Blue; // Синий цвет для реки
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.White; // Остальная карта
                }

                Console.Write(map[i, j] + " ");  // Добавляем пробел между символами
            }
            Console.WriteLine();
        }

        Console.SetCursorPosition(heroY * 2, heroX);  // Позиция героя
        Console.ResetColor(); // Сброс цвета
    }

    class Program
    {
        static void Main()
        {
            DotaMap map = new DotaMap();
            map.DisplayMap();

            while (true)
            {
                char command = Console.ReadKey().KeyChar;
            }
        }
    }
}





