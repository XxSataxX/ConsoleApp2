using System;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        Console.WriteLine("Доброго пожаловать в храм, Подаван!");
        Console.Write("Назови свое имя:\n> ");
        string playerName = Console.ReadLine();

        Player player = new Player(playerName, 1000, 1000);
        player.Weapon = new Weapon("Двуручный меч", 90);
        player.AidKit = new Aid("хилка", 50);

        Console.WriteLine($"Your Name {player.Name}!");
        Console.WriteLine($"Вам было выданно оружие {player.Weapon.Name} ({player.Weapon.Damage}), а также {player.AidKit.Name} ({player.AidKit.HealAmount}hp).");
        Console.WriteLine($"У вас {player.CurrentHealth}hp и {player.Score} очков.\n");

        Random rnd = new Random();
        Game game = new Game(player, rnd);
        game.Start();
    }
}

public class Player
{
    public string Name { get; private set; }
    public int CurrentHealth { get; private set; }
    public int MaxHealth { get; private set; }
    public Aid AidKit { get; set; }
    public Weapon Weapon { get; set; }
    public int Score { get; private set; }
    public Enemy Enemy { get; set; }

    public Player(string name, int maxHealth, int currentHealth)
    {
        Name = name;
        MaxHealth = maxHealth;
        CurrentHealth = currentHealth;
        Score = 0;
    }

    public void Heal()
    {
        if (AidKit != null)
        {
            CurrentHealth = Math.Min(MaxHealth, CurrentHealth + AidKit.HealAmount);
            Console.WriteLine($"{Name} использовал аптечку");
            Console.WriteLine($"У противника {Enemy.CurrentHealth}hp, у вас {CurrentHealth}hp");
            AidKit = null;
        }
        else
        {
            Console.WriteLine("У вас нет аптечки!");
        }
    }

    public void Attack(Enemy enemy)
    {
        if (Weapon != null)
        {
            Console.WriteLine($"{Name} ударил противника {enemy.Name}");
            enemy.TakeDamage(Weapon.Damage);
            Console.WriteLine($"У противника {enemy.CurrentHealth}hp, у вас {CurrentHealth}hp");
        }
    }

    public void TakeDamage(int damage)
    {
        CurrentHealth -= damage;
        if (CurrentHealth <= 0)
        {
            Console.WriteLine($"{Name} был побежден!");
        }
    }

    public void AddScore(int score)
    {
        Score += score;
    }
}

public class Enemy
{
    public string Name { get; private set; }
    public int CurrentHealth { get; private set; }
    public int MaxHealth { get; private set; }
    public Weapon Weapon { get; private set; }

    public Enemy(string name, int maxHealth, Weapon weapon)
    {
        Name = name;
        MaxHealth = maxHealth;
        CurrentHealth = maxHealth;
        Weapon = weapon;
    }

    public void TakeDamage(int damage)
    {
        CurrentHealth -= damage;
        if (CurrentHealth <= 0)
        {
            Console.WriteLine($"{Name} был побежден!");
        }
        else
        {
            Console.WriteLine($"У противника {Name} осталось {CurrentHealth}hp.");
        }
    }

    public void Attack(Player player)
    {
        player.TakeDamage(Weapon.Damage);
        Console.WriteLine($"Противник {Name} ударил вас!");
        Console.WriteLine($"У противника {CurrentHealth}hp, у вас {player.CurrentHealth}hp");
    }
}

public class Aid
{
    public string Name { get; private set; }
    public int HealAmount { get; private set; }

    public Aid(string name, int healAmount)
    {
        Name = name;
        HealAmount = healAmount;
    }
}

public class Weapon
{
    public string Name { get; private set; }
    public int Damage { get; private set; }

    public Weapon(string name, int damage)
    {
        Name = name;
        Damage = damage;
    }
}

public class Game
{
    private Player Player { get; set; }
    private Random Random { get; set; }

    public Game(Player player, Random random)
    {
        Player = player;
        Random = random;
    }

    public void Start()
    {
        while (Player.CurrentHealth > 0)
        {
            GenerateRandomEnemyAndWeapon();
            while (Player.Enemy.CurrentHealth > 0 && Player.CurrentHealth > 0)
            {
                Console.WriteLine("Выберите действие?");
                Console.WriteLine("1. сделать крученный удар");
                Console.WriteLine("2. отступить");
                Console.WriteLine("3. Использовать хилку");
                Console.Write("> ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Player.Attack(Player.Enemy);
                        if (Player.Enemy.CurrentHealth > 0)
                        {
                            Player.Enemy.Attack(Player);
                        }
                        break;
                    case "2":
                        Player.Enemy.Attack(Player);
                        break;
                    case "3":
                        Player.Heal();
                        if (Player.Enemy.CurrentHealth > 0)
                        {
                            Player.Enemy.Attack(Player);
                        }
                        break;
                    default:
                        Console.WriteLine("Попробуй иначе.");
                        break;
                }

                if (Player.Enemy.CurrentHealth <= 0)
                {
                    Player.AddScore(10);
                    Console.WriteLine($"{Player.Name} победил вражеского персонажа {Player.Enemy.Name} и получил 10 очков!");
                    Console.WriteLine($"счет: {Player.Score} очков");
                }


                if (Player.CurrentHealth <= 0)
                {
                    Console.WriteLine("Вы прошли игру! Ваш счет: " + Player.Score);
                    break;
                }
            }
        }
    }

    private void GenerateRandomEnemyAndWeapon()
    {
        var enemyNames = new List<string> { "Дракон темного облака", "Гоблин древних руин", "Слизень", "Ведьма с высокой горы", "Король демонов" };
        var weaponNames = new List<string> { "Двуручное копье", "Dugger", "Катана", "Посох хранителя", "Двуручный меч", "Меч с щитом", "Лук эльфов" };

        string enemyName = enemyNames[Random.Next(enemyNames.Count)];
        int enemyHealth = Random.Next(50, 300);
        string weaponName = weaponNames[Random.Next(weaponNames.Count)];
        int weaponDamage = Random.Next(55, 100);

        Enemy enemy = new Enemy(enemyName, enemyHealth, new Weapon(weaponName, weaponDamage));
        Player.Enemy = enemy;
        Console.WriteLine($"{Player.Name} встречает  {enemy.Name} ({enemy.CurrentHealth}hp), у врага за спиной виднеется {enemy.Weapon.Name} ({enemy.Weapon.Damage})");
    }
}