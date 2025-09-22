using System;
class Program
{
    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        Console.Write("Введите количество операций (2-40): ");
        int n = int.Parse(Console.ReadLine());

        string[] names = new string[n];
        double[] amounts = new double[n];

        for (int i = 0; i < n; i++)
        {
            Console.Write($"\nНазвание товара/услуги {i + 1}: ");
            names[i] = Console.ReadLine();

            Console.Write("Сумма (руб): ");
            amounts[i] = double.Parse(Console.ReadLine());
        }

        int choice;
        do
        {
            Console.WriteLine("\nМеню:");
            Console.WriteLine("1. Вывод данных");
            Console.WriteLine("2. Статистика");
            Console.WriteLine("3. Сортировка по цене (пузырьком)");
            Console.WriteLine("4. Конвертация валюты");
            Console.WriteLine("5. Поиск по названию");
            Console.WriteLine("0. Выход");
            Console.Write("Ваш выбор: ");

            choice = int.Parse(Console.ReadLine());

            switch (choice)
            {
                case 1:
                    for (int i = 0; i < n; i++)
                        Console.WriteLine($"{names[i]} — {amounts[i]} руб.");
                    break;

                case 2:
                    double sum = 0, min = amounts[0], max = amounts[0];
                    for (int i = 0; i < n; i++)
                    {
                        sum += amounts[i];
                        if (amounts[i] < min) min = amounts[i];
                        if (amounts[i] > max) max = amounts[i];
                    }
                    Console.WriteLine($"Сумма: {sum}");
                    Console.WriteLine($"Среднее: {sum / n:F2}");
                    Console.WriteLine($"Мин: {min}");
                    Console.WriteLine($"Макс: {max}");
                    break;

                case 3:
                    for (int i = 0; i < n - 1; i++)
                        for (int j = 0; j < n - i - 1; j++)
                            if (amounts[j] > amounts[j + 1])
                            {
                                double tmpA = amounts[j];
                                amounts[j] = amounts[j + 1];
                                amounts[j + 1] = tmpA;

                                string tmpN = names[j];
                                names[j] = names[j + 1];
                                names[j + 1] = tmpN;
                            }
                    Console.WriteLine("Сортировка завершена.");
                    break;

                case 4:
                    Console.Write("Введите курс: ");
                    double rate = double.Parse(Console.ReadLine());
                    for (int i = 0; i < n; i++)
                        Console.WriteLine($"{names[i]} — {(amounts[i] / rate):F2}");
                    break;

                case 5:
                    Console.Write("Введите часть названия: ");
                    string q = Console.ReadLine().ToLower();
                    for (int i = 0; i < n; i++)
                        if (names[i].ToLower().Contains(q))
                            Console.WriteLine($"{names[i]} — {amounts[i]} руб.");
                    break;
            }
        } while (choice != 0);
    }
}

