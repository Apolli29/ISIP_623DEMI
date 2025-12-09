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
            var player = Core.Context.player.FirstOrDefault(p => p.ID == 1);
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

        private static void ShowPlayerStatus(player player)
        {
            Console.WriteLine($"АВТОСЕРВИС");
            Console.WriteLine($"Баланс: {player.cash} руб.");
            Console.WriteLine($"Обслужено машин: {carsProcessed}");
            Console.WriteLine($"Успешные ремонты: {successfulRepairs}");
            Console.WriteLine($"Неудачные ремонты: {failedRepairs}");

            var pendingOrders = Core.Context.parts_player.Where(o => o.ID == 1).ToList();
            if (pendingOrders.Any())
            {
                Console.WriteLine("\nОжидаются поставки:");
                foreach (var order in pendingOrders)
                {
                    var part = Core.Context.parts_player.FirstOrDefault(p => p.ID == order.ID_part);
                    Console.WriteLine($"{part.Name_part}: {order.count} шт. (через {order.carsUntilDeivery} машин)");
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


        private static cars GenerateRandomClient()
        {
            var defects = Core.Context.defects.ToList();
            var carsList = Core.Context.cars.ToList();

            var randomDefect = defects[random.Next(defects.Count)];
            var randomCar = carsList[random.Next(carsList.Count)];

            return new cars
            {
                Name_car = randomCar.Name_car,
                ID_Defect = randomDefect.ID
            };
        }
        private static void ShowClientInfo(cars car)
        {
            var defect = Core.Context.defects.FirstOrDefault(d => d.ID == car.ID_Defect);
            var neededPart = Core.Context.parts.FirstOrDefault(p => p.ID == defect.ID_part_need);
            var repairCost = CalculateRepairCost(neededPart);

            Console.WriteLine($"Приехал клиент на {car.Name_car}");
            Console.WriteLine($"Неисправность: {defect.Name_defect}");
            Console.WriteLine($"Нужна деталь: {neededPart.Name_part}");
            Console.WriteLine($"Стоимость ремонта: {repairCost} руб.");
            Console.WriteLine();
        }
        private static decimal CalculateRepairCost(parts part)
        {
            return part.Price + (part.Price * (decimal)(part.Work_cost));
        }

        private static void ProcessPlayerChoice(player player, cars clientCar)
        {
            var defect = Core.Context.defects.FirstOrDefault(d => d.ID == clientCar.ID_Defect);
            var neededPartId = defect.ID_part_need;

            Console.WriteLine("Ваш склад:");
            var inventory = Core.Context.parts_player.Where(i => i.ID_player == player.ID && i.count > 0).ToList();

            if (inventory.Any())
            {
                int index = 1;
                foreach (var item in inventory)
                {
                    var part = Core.Context.parts.FirstOrDefault(p => p.ID == item.ID_part);
                    Console.WriteLine($"{index}. {part.Name_part} - {item.count} шт.");
                    index++;
                }

                Console.WriteLine($"0. Отказать (штраф 1000 руб.)");
                Console.WriteLine("Выберите деталь для замены:");

                if (int.TryParse(Console.ReadLine(), out int choice))
                {
                    if (choice == 0)
                    {
                        // Отказ от обслуживания
                        player.cash -= 1000;
                        Core.Context.SaveChanges();
                        Console.WriteLine("Вы отказали клиенту. Штраф 1000 руб.");
                    }
                    else if (choice > 0 && choice <= inventory.Count)
                    {
                        var selectedItem = inventory[choice - 1];
                        var selectedPartId = selectedItem.ID_part;
                        TryRepair(player, clientCar, selectedPartId, (int)neededPartId);
                    }
                }
            }
            else
            {
                Console.WriteLine("Склад пуст! Придется отказать клиенту.");
                player.cash -= 1000;
                Core.Context.SaveChanges();
                Console.WriteLine("Штраф 1000 руб.");
            }
        }
        private static void TryRepair(player player, cars clientCar, int selectedPartId, int neededPartId)
        {
            var inventory = Core.Context.parts_player.FirstOrDefault(i => i.ID_player == player.ID && i.ID_part == selectedPartId);

            if (inventory == null || inventory.count <= 0)
            {
                player.cash -= 1000;
                Console.WriteLine("Недостаточно деталей! Штраф 1000 руб.");
                Core.Context.SaveChanges();
                return;
            }

            inventory.count--;

            bool isCorrectPart = (selectedPartId == neededPartId);

            if (isCorrectPart)
            {
                var part = Core.Context.parts.FirstOrDefault(p => p.ID == selectedPartId);
                var repairCost = CalculateRepairCost(part);
                player.cash += repairCost;
                Console.WriteLine($"Успешный ремонт! Получено {repairCost} руб.");
                successfulRepairs++;
            }
            else
            {
                var part = Core.Context.parts.FirstOrDefault(p => p.ID == selectedPartId);
                var penalty = part.Price * 2;
                player.cash -= penalty;
                Console.WriteLine($"Неправильная деталь! Штраф {penalty} руб.");
                failedRepairs++;
            }

            Core.Context.SaveChanges();
        }
        private static void ShowStoreMenu(player player)
        {
            Console.Clear();
            var availableParts = Core.Context.parts.ToList();
            Console.WriteLine("Доступные запчасти:");

            for (int i = 0; i < availableParts.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {availableParts[i].Name_part} - {availableParts[i].Price} руб.");
            }

            Console.WriteLine("\nВведите номер детали для покупки (0 - отмена):");
            if (int.TryParse(Console.ReadLine(), out int partChoice) && partChoice > 0 && partChoice <= availableParts.Count)
            {
                Console.WriteLine("Введите количество:");
                if (int.TryParse(Console.ReadLine(), out int quantity) && quantity > 0)
                {
                    PurchaseParts(player, availableParts[partChoice - 1].ID, quantity);
                }
            }
        }
        private static void PurchaseParts(player player, int partId, int quantity)
        {
            var part = Core.Context.parts.FirstOrDefault(p => p.ID == partId);
            var totalCost = part.Price * quantity;

            if (player.cash >= totalCost)
            {
                player.cash -= totalCost;

                var pendingParts = new parts_player
                {
                    ID = player.ID,
                    ID_part = partId,
                    count = quantity,
                    carsUntilDeivery = 2
                };
                Core.Context.parts.Add(pendingParts);

                Core.Context.SaveChanges();
                Console.WriteLine($"Заказ на {quantity} {part.Name_part} создан! Поставка через 2 машины.");
            }
            else
            {
                Console.WriteLine("Недостаточно денег!");
            }
        }
        private static void ProcessDeliveries(player player)
        {
            var orders = Core.Context.OrderParts.Where(o => o.PlayerID == player.ID).ToList();
            foreach (var order in orders)
            {
                order.carsUntilDeivery--;
                if (order.carsUntilDeivery <= 0)
                {
                    // Доставляем детали на склад
                    var inventory = Core.Context.parts_player.FirstOrDefault(i =>
                        i.ID_player == player.ID && i.ID_part == order.PartID);

                    if (inventory == null)
                    {
                        inventory = new parts_player
                        {
                            ID_player = player.ID,
                            ID_part = order.PartID,
                            count = 0
                        };
                        Core.Context.parts_player.Add(inventory);
                    }

                    inventory.count += order.count;
                    Core.Context.OrderParts.Remove(order);
                }
            }
            Core.Context.SaveChanges();
        }
        private static void ShowInventory(player player)
        {
            var inventory = Core.Context.parts_player.Where(i => i.ID_player == 1).ToList();
            Console.WriteLine("Ваш склад:");

            foreach (var item in inventory)
            {
                var part = Core.Context.parts.FirstOrDefault(p => p.ID == item.ID_part);
                Console.WriteLine($"{part.Name_part}: {item.count} шт.");
            }
        }
    }

}




