using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    internal class Program
    {

        static Random random = new Random();
        static int carsProcessed = 0;
        static int successfulRepairs = 0;
        static int failedRepairs = 0;

        static void Main(string[] args)
        {
            var player = Core.Context.player.FirstOrDefault();
            if (player == null)
            {
                player = new player { cash = 5000 };
                Core.Context.player.Add(player);
                Core.Context.SaveChanges();
                Console.WriteLine("Создан новый игрок!");
            }

            bool gameRunning = true;

            while (gameRunning)
            {

                ShowPlayerStatus(player);
                Console.WriteLine("\n1 - Обслужить следующего клиента");
                Console.WriteLine("2 - Купить запчасти");
                Console.WriteLine("3 - Посмотреть склад");
                Console.WriteLine("4 - Выход");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ProcessNextCar(player);
                        break;
                    case "2":
                        ShowStoreMenu(player);
                        break;
                    case "3":
                        ShowInventory(player);
                        break;
                    case "4":
                        gameRunning = false;
                        break;
                }

                if (choice != "4")
                {
                    Console.WriteLine("\nНажмите любую клавишу для продолжения...");
                    Console.ReadKey();
                }
            }

            Console.WriteLine("Игра завершена!");
        }

        private static void ShowInventory(object player)
        {
            throw new NotImplementedException();
        }

        private static void ShowStoreMenu(object player)
        {
            throw new NotImplementedException();
        }

        private static void ProcessNextCar(object player)
        {
            throw new NotImplementedException();
        }

        private static void ShowPlayerStatus(object player)
        {
            throw new NotImplementedException();
        }

        private static void ShowPlayerStatus(player player)
        {
            Console.WriteLine($"АВТОСЕРВИС");
            Console.WriteLine($"Баланс: {player.MyMoney} руб.");
            Console.WriteLine($"Обслужено машин: {carsProcessed}");
            Console.WriteLine($"Успешные ремонты: {successfulRepairs}");
            Console.WriteLine($"Неудачные ремонты: {failedRepairs}");

            var pendingOrders = Core.Context.OrderParts.Where(o => o.PlayerID == 1).ToList();
            if (pendingOrders.Any())
            {
                Console.WriteLine("\nОжидаются поставки:");
                foreach (var order in pendingOrders)
                {
                    var part = Core.Context.parts.FirstOrDefault(p => p.partID == order.PartID);
                    Console.WriteLine($"{part.partName}: {order.count} шт. (через {order.carsUntilDelivery} машин)");
                }
            }
        }
        private static void ProcessNextCar(player player)
        {
            Console.Clear();


            ProcessDeliveries(player);

            // случ клиент
            var clientCar = GenerateRandomClient();
            carsProcessed++;

            ShowClientInfo(clientCar);
            ProcessPlayerChoice(player, clientCar);
        }

        private static void ProcessPlayerChoice(player player, object clientCar)
        {
            throw new NotImplementedException();
        }

        private static void ShowClientInfo(object clientCar)
        {
            throw new NotImplementedException();
        }

        private static void ProcessDeliveries(player player)
        {
            throw new NotImplementedException();
        }
        private static cars GenerateRandomClient()
        {
            var defects = Core.Context.defects.ToList();
            var carsList = Core.Context.cars.ToList();

            var randomDefect = defects[random.Next(defects.Count)];
            var randomCar = carsList[random.Next(carsList.Count)];

            return new cars
            {
                carName = randomCar.carName,
                defectID = randomDefect.id
            };
        }
        private static void ShowClientInfo(cars car)
        {
            var defect = Core.Context.defects.FirstOrDefault(d => d.id == car.defectID);
            var neededPart = Core.Context.parts.FirstOrDefault(p => p.partID == defect.partNeedID);
            var repairCost = CalculateRepairCost(neededPart);

            Console.WriteLine($"Приехал клиент на {car.carName}");
            Console.WriteLine($"Неисправность: {defect.defectName}");
            Console.WriteLine($"Нужна деталь: {neededPart.partName}");
            Console.WriteLine($"Стоимость ремонта: {repairCost} руб.");
            Console.WriteLine();
        }
        private static decimal CalculateRepairCost(parts part)
        {
            return part.basePrice + (part.basePrice * (decimal)(part.workCost));
        }

        private static void ProcessPlayerChoice(player player, cars clientCar)
        {
            var defect = Core.Context.defects.FirstOrDefault(d => d.id == clientCar.defectID);
            var neededPartId = defect.partNeedID;

            Console.WriteLine("Ваш склад:");
            var inventory = Core.Context.parts_player.Where(i => i.idPlayer == player.id && i.countParts > 0).ToList();

            if (inventory.Any())
            {
                int index = 1;
                foreach (var item in inventory)
                {
                    var part = Core.Context.parts.FirstOrDefault(p => p.partID == item.idPart);
                    Console.WriteLine($"{index}. {part.partName} - {item.countParts} шт.");
                    index++;
                }

                Console.WriteLine($"0. Отказать (штраф 1000 руб.)");
                Console.WriteLine("Выберите деталь для замены:");

                if (int.TryParse(Console.ReadLine(), out int choice))
                {
                    if (choice == 0)
                    {
                        // Отказ от обслуживания
                        player.MyMoney -= 1000;
                        Core.Context.SaveChanges();
                        Console.WriteLine("Вы отказали клиенту. Штраф 1000 руб.");
                    }
                    else if (choice > 0 && choice <= inventory.Count)
                    {
                        var selectedItem = inventory[choice - 1];
                        var selectedPartId = selectedItem.idPart;
                        TryRepair(player, clientCar, selectedPartId, (int)neededPartId);
                    }
                }
            }
            else
            {
                Console.WriteLine("Склад пуст! Придется отказать клиенту.");
                player.MyMoney -= 1000;
                Core.Context.SaveChanges();
                Console.WriteLine("Штраф 1000 руб.");
            }
        }

    }



