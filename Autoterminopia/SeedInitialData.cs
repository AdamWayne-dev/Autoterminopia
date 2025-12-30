using Dapper;
using Microsoft.Data.Sqlite;

namespace Autoterminopia
{
    internal class SeedInitialData
    {

        private readonly string _databasePath;

        public SeedInitialData(string databasePath)
        {
            _databasePath = databasePath;
        }
        
        public void Seed()
        {
            using var connection = new SqliteConnection($"Data Source={_databasePath}");
            connection.Open();

            using var tx = connection.BeginTransaction();

            try
            {
                // Seeds initial data only if tables are empty
                var playerCount = connection.ExecuteScalar<int>("SELECT COUNT(*) FROM Players;", transaction: tx);
                if (playerCount == 0)
                {
                    const string seedPlayersQuery = @"
                    INSERT INTO Players (Name, Level, XP, Gold, CurrentHP) VALUES
                    ('Hero', 1, 0, 100, 100);
                ";
                    connection.Execute(seedPlayersQuery, transaction: tx);
                }
                var floorCount = connection.ExecuteScalar<int>("SELECT COUNT(*) FROM Floors;", transaction: tx);
                if (floorCount == 0)
                {
                    const string seedFloorsQuery = @"
                    INSERT INTO Floors (Name, MinLevel, MaxLevel) VALUES
                    ('Goblin Caves', 1, 5),
                    ('Skeleton Crypt', 6, 10);
                ";
                    connection.Execute(seedFloorsQuery, transaction: tx);
                }
                var enemyCount = connection.ExecuteScalar<int>("SELECT COUNT(*) FROM EnemyTemplates;", transaction: tx);
                if (enemyCount == 0)
                {
                    const string seedEnemiesQuery = @"
                    INSERT INTO EnemyTemplates (Name, FloorId, BaseHP, BaseAttackPower, BaseAttackSpeed, XPReward, SpawnWeight) VALUES
                    ('Goblin', 1, 30, 5, 1.0, 20, 70),
                    ('Hobgoblin', 1, 50, 10, 0.8, 50, 30),
                    ('Skeleton Warrior', 2, 80, 15, 0.9, 100, 50),
                    ('Skeleton Archer', 2, 60, 12, 1.2, 80, 50);
                ";
                    connection.Execute(seedEnemiesQuery, transaction: tx);
                }
                var itemCount = connection.ExecuteScalar<int>("SELECT COUNT(*) FROM ItemTemplates;", transaction: tx);
                if (itemCount == 0)
                {
                    const string seedItemsQuery = @"
                    INSERT INTO ItemTemplates (FloorId, Name, ItemType, Rarity, RequiredLevel, AttackBonus, AttackSpeedBonus, DefenseBonus, MaxHPBonus) VALUES
                    (1, 'Rusty Sword', 'Weapon', 1, 1, 5, 0.0, 0, 0),
                    (1, 'Wooden Shield', 'Armour', 1, 1, 0, 0.0, 3, 0),
                    (2, 'Iron Sword', 'Weapon', 2, 6, 10, 0.0, 0, 0),
                    (2, 'Iron Shield', 'Armour', 2, 6, 0, 0.0, 6, 0);
                ";
                    connection.Execute(seedItemsQuery, transaction: tx);
                }

                tx.Commit();
            }
            catch
            {
                tx.Rollback();
                throw;
            }
           
        }
    }
}
