using System;

class Program
{

    static void Main()
    {
        Game game = new Game();
        game.StartGame();
    }

    class Game
    {
        private Random random = new Random();
        private int turnCount = 0;

        public void StartGame()
        {
            Player player = new Player(100);
            Console.WriteLine("=== ТЕКСТОВЫЙ РОГАЛИК ===");

            while (player.HP > 0)
            {
                turnCount++;
                Console.WriteLine($"\n--- Ход {turnCount} ---");
                player.ShowStats();

                // Каждые 10 ходов - босс
                if (turnCount % 10 == 0)
                {
                    Enemy boss = GetRandomBoss();
                    StartBattle(player, boss);
                }
                else
                {
                    // 50/50 шанс сундука или врага
                    if (random.Next(2) == 0) // 0 - враг, 1 - сундук
                    {
                        Enemy enemy = GetRandomEnemy();
                        StartBattle(player, enemy);
                    }
                    else
                    {
                        Chest chest = new Chest();
                        chest.Open(player);
                    }
                }

                if (player.HP <= 0)
                {
                    Console.WriteLine("\n=== ИГРА ОКОНЧЕНА ===");
                    Console.WriteLine($"Вы продержались {turnCount} ходов!");
                    break;
                }

                Console.WriteLine("Нажмите любую клавишу для продолжения...");
                Console.ReadKey();
            }
        }

       