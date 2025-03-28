using System;
using System.Collections.Generic;

class SeaBattle
{
    static int[,] playerField = new int[10, 10]; // 0 - пусто, 1 - корабль, 2 - попадание, 3 - промах
    static int[,] computerField = new int[10, 10];
    static Random rand = new Random();
    static List<(int x, int y)> computerHits = new List<(int, int)>();
    static bool huntMode = true;
    static void Main()
    {
        Console.WriteLine("Добро пожаловать в Морской бой!");
        InitializeGame();
        PlayGame();
    }
    static void InitializeGame()
    {
        PlacePlayerShips();
        PlaceShips(computerField);
    }
    static void PlacePlayerShips()
    {
        int[] ships = { 4, 3, 3, 2, 2, 2, 1, 1, 1, 1 };
        DisplayFields();
        foreach (int size in ships)
        {
            bool placed = false;
            while (!placed)
            {
                Console.WriteLine($"Разместите корабль размером {size} (формат: x y horizontal/vertically(true,false):");
                string[] input = Console.ReadLine().Split();
                int x = int.Parse(input[0]);
                int y = int.Parse(input[1]);
                bool horizontal = bool.Parse(input[2]);

                if (CanPlaceShip(playerField, x, y, size, horizontal))
                {
                    for (int i = 0; i < size; i++)
                    {
                        if (horizontal)
                            playerField[x, y + i] = 1;
                        else
                            playerField[x + i, y] = 1;
                    }
                    placed = true;
                    DisplayFields();
                }
                else
                {
                    Console.WriteLine("Невозможно разместить корабль здесь. Попробуйте снова.");
                }
            }
        }
    }
    static void PlaceShips(int[,] field)
    {
        int[] ships = { 4, 3, 3, 2, 2, 2, 1, 1, 1, 1 };
        foreach (int size in ships)
        {
            bool placed = false;
            while (!placed)
            {
                bool horizontal = rand.Next(2) == 0;
                int x = rand.Next(10);
                int y = rand.Next(10);

                if (CanPlaceShip(field, x, y, size, horizontal))
                {
                    for (int i = 0; i < size; i++)
                    {
                        if (horizontal)
                            field[x, y + i] = 1;
                        else
                            field[x + i, y] = 1;
                    }
                    placed = true;
                }
            }
        }
    }
    static bool CanPlaceShip(int[,] field, int x, int y, int size, bool horizontal)
    {
        if (horizontal)
        {
            if (y + size > 10) return false;
            for (int i = -1; i <= size; i++)
                for (int j = -1; j <= 1; j++)
                    if (x + j >= 0 && x + j < 10 && y + i >= 0 && y + i < 10 && field[x + j, y + i] != 0)
                        return false;
        }
        else
        {
            if (x + size > 10) return false;
            for (int i = -1; i <= size; i++)
                for (int j = -1; j <= 1; j++)
                    if (x + i >= 0 && x + i < 10 && y + j >= 0 && y + j < 10 && field[x + i, y + j] != 0)
                        return false;
        }
        return true;
    }
    static void PlayGame()
    {
        int playerHits = 0;
        int computerHitsCount = 0;
        int totalShips = 20;

        while (playerHits < totalShips && computerHitsCount < totalShips)
        {
            DisplayFields();
            bool validShot = false;
            int x = 0, y = 0; 
            while (!validShot)
            {
                Console.WriteLine("Ваш ход (формат: x y): ");
                string[] input = Console.ReadLine().Split();
                x = int.Parse(input[0]);
                y = int.Parse(input[1]);

                if (x < 0 || x >= 10 || y < 0 || y >= 10)
                {
                    Console.WriteLine("Координаты должны быть от 0 до 9. Попробуйте снова.");
                }
                else if (computerField[x, y] == 2 || computerField[x, y] == 3)
                {
                    Console.WriteLine("Вы уже стреляли сюда! Выберите другую клетку.");
                }
                else
                {
                    validShot = true;
                }
            }

            if (computerField[x, y] == 1)
            {
                Console.WriteLine("Попадание!");
                computerField[x, y] = 2;
                playerHits++;
            }
            else
            {
                Console.WriteLine("Промах!");
                computerField[x, y] = 3;
            }
            if (playerHits == totalShips) break;

            // Ход компьютера
            ComputerTurn(ref computerHitsCount);
        }

        Console.WriteLine(playerHits == totalShips ? "Вы победили!" : "Компьютер победил!");
    }
    static void ComputerTurn(ref int computerHitsCount)
    {
        int x, y;
        if (huntMode && computerHits.Count > 0)
        {
            var lastHit = computerHits[computerHits.Count - 1];
            (x, y) = HuntShip(lastHit.x, lastHit.y);
        }
        else
        {
            (x, y) = GetRandomShot();
        }

        if (playerField[x, y] == 1)
        {
            Console.WriteLine($"Компьютер попал в {x} {y}!");
            playerField[x, y] = 2;
            computerHitsCount++;
            computerHits.Add((x, y));
            huntMode = true;
        }
        else
        {
            Console.WriteLine($"Компьютер промахнулся: {x} {y}");
            playerField[x, y] = 3;
            huntMode = computerHits.Count == 0;
        }
    }
    static (int x, int y) GetRandomShot()
    {
        int x, y;
        do
        {
            x = rand.Next(10);
            y = rand.Next(10);
        } while (playerField[x, y] == 2 || playerField[x, y] == 3);
        return (x, y);
    }
    static (int x, int y) HuntShip(int x, int y)
    {
        int[] dx = { 0, 1, 0, -1 };
        int[] dy = { 1, 0, -1, 0 };

        for (int i = 0; i < 4; i++)
        {
            int newX = x + dx[i];
            int newY = y + dy[i];

            if (newX >= 0 && newX < 10 && newY >= 0 && newY < 10 &&
                playerField[newX, newY] != 2 && playerField[newX, newY] != 3)
            {
                return (newX, newY);
            }
        }
        huntMode = false;
        return GetRandomShot(); 
    }
    static void DisplayFields()
    {
        Console.WriteLine("\nВаше поле:         Поле компьютера:");
        Console.WriteLine("  0123456789        0123456789");
        for (int i = 0; i < 10; i++)
        {
            Console.Write(i + " ");
            for (int j = 0; j < 10; j++)
            {
                switch (playerField[i, j])
                {
                    case 0: Console.Write("."); break;
                    case 1: Console.Write("█"); break;
                    case 2: Console.Write("X"); break;
                    case 3: Console.Write("O"); break;
                }
            }
            Console.Write("    " + i + " ");
            for (int j = 0; j < 10; j++)
            {
                switch (computerField[i, j])
                {
                    case 0: Console.Write("."); break;
                    case 1: Console.Write("."); break;
                    case 2: Console.Write("X"); break;
                    case 3: Console.Write("O"); break;
                }
            }
            Console.WriteLine();
        }
        Console.WriteLine();
    }
}