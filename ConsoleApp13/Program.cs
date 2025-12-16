using System;
using static System.Net.Mime.MediaTypeNames;
using System.Numerics;

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

        private Enemy GetRandomEnemy()
        {
            int enemyType = random.Next(3);
            return enemyType switch
            {
                0 => new Goblin(),
                1 => new Skeleton(),
                2 => new Mage(),
                _ => new Goblin()
            };
        }

        private Enemy GetRandomBoss()
        {
            int bossType = random.Next(4);
            return bossType switch
            {
                0 => new VVG(),
                1 => new Kovalski(),
                2 => new ArchmageCPP(),
                3 => new Pestov(),
                _ => new VVG()
            };
        }
        public void StartBattle(Player player, Enemy enemy)
        {
            Console.WriteLine($"\n=== ВСТРЕЧА С {enemy.Name.ToUpper()} ===");
            enemy.ShowStats();

            bool playerFrozen = false;

            while (player.HP > 0 && enemy.IsAlive())
            {
                if (!playerFrozen)
                {
                    PlayerTurn(player, enemy);
                    if (!enemy.IsAlive()) break;
                }
                else
                {
                    Console.WriteLine("Вы заморожены и пропускаете ход!");
                    playerFrozen = false;
                }

                EnemyTurn(player, enemy);
                if (player.HP <= 0) break;

                // Проверяем заморозку
                if (enemy is Mage mage && mage.FreezeApplied)
                {
                    playerFrozen = true;
                    mage.ResetFreeze();
                }
                else if (enemy is ArchmageCPP archmage && archmage.FreezeApplied)
                {
                    playerFrozen = true;
                    archmage.ResetFreeze();
                }
                else if (enemy is Pestov pestov && pestov.FreezeApplied)
                {
                    playerFrozen = true;
                    pestov.ResetFreeze();
                }
            }

            if (player.HP > 0)
            {
                Console.WriteLine($"\nПобеда! {enemy.Name} повержен!");
            }
        }


       