using Dapper;
using Microsoft.Data.Sqlite;
using CsvHelper;

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

                var playerStatsCount = connection.ExecuteScalar<int>("SELECT COUNT(*) FROM PlayerStats;", transaction: tx);
                if (playerStatsCount == 0)
                {
                    const string seedPlayerStatsQuery = @"
                    INSERT INTO PlayerStats (PlayerId, BaseAttackPower, BaseAttackSpeed, BaseDefense, BaseMaxHP) VALUES
                    (1, 10, 1.0, 5, 100);";
                    connection.Execute(seedPlayerStatsQuery, transaction: tx);
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
                //var enemyCount = connection.ExecuteScalar<int>("SELECT COUNT(*) FROM EnemyTemplates;", transaction: tx);
                //if (enemyCount == 0)
                //{
                //    var items = GetCSVData<ItemTemplate>("Data/ItemTemplates.csv");

                //    const string seedEnemiesQuery = @"
                //    INSERT INTO EnemyTemplates (Code, Name, FloorId, BaseHP, BaseAttackPower, BaseAttackSpeed, XPReward, GoldReward)
                //    VALUES
                //    (@Name, @)
                    
                //";
                //    connection.Execute(seedEnemiesQuery, transaction: tx);
                //}
                var itemCount = connection.ExecuteScalar<int>("SELECT COUNT(*) FROM ItemTemplates;", transaction: tx);
                if (itemCount == 0)
                {
                    var items = GetCSVData<ItemTemplate>("Data/ItemTemplates.csv");

                    const string seedItemsQuery = @"
                    INSERT INTO ItemTemplates
                    (Code,Name,ItemType,Rarity,RequiredLevel,AttackBonus,AttackSpeedBonus,DefenseBonus,MaxHPBonus,GoldValue)
                    VALUES
                    (@Code,@Name,@ItemType,@Rarity,@RequiredLevel,@AttackBonus,@AttackSpeedBonus,@DefenseBonus,@MaxHPBonus,@GoldValue)
                    ";
                    foreach (var item in items)
                    {
                        connection.Execute(seedItemsQuery, item, transaction: tx);
                    }
                }
                                   
                var floorDropsCount = connection.ExecuteScalar<int>("SELECT COUNT(*) FROM FloorDrops;", transaction: tx);
                if (floorDropsCount == 0)
                {
                    const string seedFloorDropsQuery = @"
                    INSERT INTO FloorDrops (FloorId, ItemTemplateId, Weight) VALUES
                    (1, 5, 70),
                    (1, 6, 30),
                    (2, 5, 60),
                    (2, 6, 40);";
                    connection.Execute(seedFloorDropsQuery, transaction: tx);
                }

                //var enemyCommonDropsCount = connection.ExecuteScalar<int>("SELECT COUNT(*) FROM EnemyCommonDrops;", transaction: tx);
                //if (enemyCommonDropsCount == 0)
                //{
                //    const string seedEnemyCommonDropsQuery = @"
                //    INSERT INTO EnemyCommonDrops (EnemyTemplateId, ItemTemplateId, Weight) VALUES
                //    (1, 1, 60),
                //    (1, 2, 40),
                //    (2, 1, 50),
                //    (2, 2, 50);";
                //    connection.Execute(seedEnemyCommonDropsQuery, transaction: tx);
                //}

                //var enemyDropsCount = connection.ExecuteScalar<int>("SELECT COUNT(*) FROM EnemyDrops;", transaction: tx);
                //if (enemyDropsCount == 0)
                //{
                //    const string seedEnemyDropsQuery = @"
                //    INSERT INTO EnemyRareDrops (EnemyTemplateId, ItemTemplateId, Weight) VALUES
                //    (1, 3, 80),
                //    (1, 4, 20),
                //    (2, 3, 50),
                //    (2, 4, 50)";
                //    connection.Execute(seedEnemyDropsQuery, transaction: tx);

                //}
                tx.Commit();
            }
            catch
            {
                tx.Rollback();
                throw;
            }

        }
        /// <summary>
        /// Reads a CSV file and maps each row to an instance of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The target type to map CSV rows to. 
        /// Property names must match the CSV column headers.
        /// </typeparam>
        /// <param name="filePath">
        /// The file path to the CSV file to read.
        /// </param>
        /// <returns>
        /// A list of <typeparamref name="T"/> populated from the CSV file.
        /// </returns>
        public List<T> GetCSVData<T>(string filePath)
        {
            using var reader = new StreamReader($"{filePath}");
            using var csv = new CsvReader(reader, System.Globalization.CultureInfo.InvariantCulture);
            return csv.GetRecords<T>().ToList();
        }
        public class EnemyCommonDrop
        {
            public int EnemyTemplateId { get; set; }
            public int ItemTemplateId { get; set; }
            public int Weight { get; set; }
        }

        public class EnemyRareDrop
        {
            public int EnemyTemplateId { get; set; }
            public int ItemTemplateId { get; set; }
            public int Weight { get; set; }
        }

        public class FloorDrop
        {
            public int FloorId { get; set; }
            public int ItemTemplateId { get; set; }
            public int Weight { get; set; }
        }

        public class ItemTemplate
        {
            public string Code { get; set; }
            public string Name { get; set; }
            public string ItemType { get; set; }
            public int Rarity { get; set; }
            public int RequiredLevel { get; set; }
            public double AttackBonus { get; set; }
            public double AttackSpeedBonus { get; set; }
            public double DefenseBonus { get; set; }
            public double MaxHPBonus { get; set; }
            public double GoldValue { get; set; }
        }
    }
}
