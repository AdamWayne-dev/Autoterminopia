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
                // Will seed data only if tables are empty
                var playerCount = connection.ExecuteScalar<int>("SELECT COUNT(*) FROM Players;", transaction: tx);
                if (playerCount == 0)
                {

                    var initialPlayers = GetCSVData<InitialPlayerData>("Data/InitialPlayerData.csv");

                    const string seedPlayersQuery = @"
                    INSERT INTO Players (Name, Level, XP, Gold, CurrentHP) 
                    VALUES
                    (@Name, @Level, @XP, @Gold, @CurrentHP);
                    ";
                    foreach (var player in initialPlayers)
                    {
                        connection.Execute(seedPlayersQuery, player, transaction: tx);
                    }
                }

                var playerStatsCount = connection.ExecuteScalar<int>("SELECT COUNT(*) FROM PlayerStats;", transaction: tx);
                if (playerStatsCount == 0)
                {
                    var initialPlayerStats = GetCSVData<InitialPlayerStatsData>("Data/InitialPlayerStatsData.csv");

                    const string seedPlayerStatsQuery = @"
                    INSERT INTO PlayerStats (PlayerId, BaseAttackPower, BaseAttackSpeed, BaseDefense, BaseMaxHP)
                    VALUES
                    (@PlayerId, @BaseAttackPower, @BaseAttackSpeed, @BaseDefense, @BaseMaxHP);
                    ";
                    foreach (var stats in initialPlayerStats)
                    {
                        connection.Execute(seedPlayerStatsQuery, stats, transaction: tx);
                    }
                }

                var floorCount = connection.ExecuteScalar<int>("SELECT COUNT(*) FROM Floors;", transaction: tx);
                if (floorCount == 0)
                {

                    var floors = GetCSVData<FloorsData>("Data/FloorsData.csv");
                    const string seedFloorsQuery = @"
                    INSERT INTO Floors (Code, Name, MinLevel, MaxLevel) 
                    VALUES
                    (@Code, @Name, @MinLevel, @MaxLevel);
                    ";

                    foreach (var floor in floors)
                    {
                        connection.Execute(seedFloorsQuery, floor, transaction: tx);
                    }
                }
                var enemyCount = connection.ExecuteScalar<int>("SELECT COUNT(*) FROM EnemyTemplates;", transaction: tx);
                if (enemyCount == 0)
                {
                    var enemies = GetCSVData<EnemyTemplate>("Data/EnemyTemplates.csv");

                    const string seedEnemiesQuery = @"
                    INSERT INTO EnemyTemplates 
                    (Code, Name, FloorId, BaseHP, BaseAttackPower, BaseAttackSpeed, XPReward, GoldReward, SpawnWeight)
                    VALUES
                    (@Code, @Name, @FloorId, @BaseHP, @BaseAttackPower, @BaseAttackSpeed, @XPReward, @GoldReward, @SpawnWeight)
                    
                    ";
                    foreach (var enemy in enemies)
                    {
                        connection.Execute(seedEnemiesQuery, enemy, transaction: tx);
                    }
                }

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
                    var floorDrops = GetCSVData<FloorDrop>("Data/FloorDrops.csv");

                    const string seedFloorDropsQuery = @"
                    INSERT INTO FloorDrops (FloorId, ItemTemplateId, Weight)
                    VALUES
                    (@FloorId, @ItemTemplateId, @Weight)
                    ;";
                    foreach (var drop in floorDrops)
                    {
                        connection.Execute(seedFloorDropsQuery, drop, transaction: tx);
                    }
                }

                var enemyCommonDropsCount = connection.ExecuteScalar<int>("SELECT COUNT(*) FROM EnemyCommonDrops;", transaction: tx);
                if (enemyCommonDropsCount == 0)
                {
                    var commonDrops = GetCSVData<EnemyCommonDrop>("Data/EnemyCommonDrops.csv");

                    const string seedEnemyCommonDropsQuery = @"
                    INSERT INTO EnemyCommonDrops (EnemyTemplateId, ItemTemplateId, Weight)
                    VALUES
                    (@EnemyTemplateId, @ItemTemplateId, @Weight)
                    ";
                    foreach (var drop in commonDrops)
                    {
                        connection.Execute(seedEnemyCommonDropsQuery, drop, transaction: tx);
                    }
                }

                var enemyDropsCount = connection.ExecuteScalar<int>("SELECT COUNT(*) FROM EnemyRareDrops;", transaction: tx);
                if (enemyDropsCount == 0)
                {
                    var rareDrops = GetCSVData<EnemyRareDrop>("Data/EnemyRareDrops.csv");
                    const string seedEnemyRareDropsQuery = @"
                    INSERT INTO EnemyRareDrops (EnemyTemplateId, ItemTemplateId, Weight)
                    VALUES
                    (@EnemyTemplateId, @ItemTemplateId, @Weight)
                    ";
                    foreach (var item in rareDrops)
                    {
                        connection.Execute(seedEnemyRareDropsQuery, item, transaction: tx);
                    }

                }
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

        public class InitialPlayerData
        {
            public string Name { get; set; }
            public int Level { get; set; }
            public double XP { get; set; }
            public double Gold { get; set; }
            public double CurrentHP { get; set; }
        }

        public class InitialPlayerStatsData
        {
            public int PlayerId { get; set; }
            public double BaseAttackPower { get; set; }
            public double BaseAttackSpeed { get; set; }
            public double BaseDefense { get; set; }
            public double BaseMaxHP { get; set; }
        }

        public class FloorsData
        {
            public string Code { get; set; }
            public string Name { get; set; }
            public int MinLevel { get; set; }
            public int MaxLevel { get; set; }
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

        public class EnemyTemplate
        {
            public string Code { get; set; }
            public string Name { get; set; }
            public int FloorId { get; set; }
            public double BaseHP { get; set; }
            public double BaseAttackPower { get; set; }
            public double BaseAttackSpeed { get; set; }
            public double XPReward { get; set; }
            public double GoldReward { get; set; }
            public int SpawnWeight { get; set; }

        }
    }
}
