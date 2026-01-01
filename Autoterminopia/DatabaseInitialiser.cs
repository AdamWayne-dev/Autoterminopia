using Dapper;
using Microsoft.Data.Sqlite;

namespace Autoterminopia
{
    internal class DatabaseInitialiser
    {
        private readonly string _databasePath;
        private static readonly List<string> tables = new([
            "CREATE TABLE IF NOT EXISTS Players (\r\n                Id INTEGER PRIMARY KEY AUTOINCREMENT,\r\n                Name TEXT NOT NULL,\r\n                Level INTEGER NOT NULL CHECK(Level >= 1),\r\n                XP REAL NOT NULL,\r\n                Gold INTEGER NOT NULL,\r\n                CurrentHP REAL NOT NULL\r\n            );",
            "CREATE TABLE IF NOT EXISTS PlayerStats (\r\n                PlayerId INTEGER PRIMARY KEY NOT NULL,\r\n                BaseAttackPower REAL NOT NULL,\r\n                BaseAttackSpeed REAL NOT NULL,\r\n                BaseDefense REAL NOT NULL,\r\n                BaseMaxHP REAL NOT NULL,\r\n                FOREIGN KEY (PlayerId) REFERENCES Players(Id)\r\n            );",
            "CREATE TABLE IF NOT EXISTS Floors (\r\n                Id INTEGER PRIMARY KEY AUTOINCREMENT,\r\n                Name TEXT NOT NULL,\r\n                MinLevel INTEGER NOT NULL,\r\n                MaxLevel INTEGER NOT NULL\r\n            );",
            "CREATE TABLE IF NOT EXISTS EnemyTemplates (\r\n                Id INTEGER PRIMARY KEY AUTOINCREMENT,\r\n                Name TEXT NOT NULL,\r\n                FloorId INTEGER NOT NULL,\r\n                BaseHP REAL NOT NULL,\r\n                BaseAttackPower REAL NOT NULL,\r\n                BaseAttackSpeed REAL NOT NULL,\r\n                XPReward REAL NOT NULL,\r\n                                GoldReward REAL NOT NULL,\r\n                SpawnWeight INTEGER NOT NULL,\r\n                                Code TEXT NOT NULL UNIQUE,\r\n                                FOREIGN KEY (FloorId) REFERENCES Floors(Id)\r\n            );",
            "CREATE TABLE IF NOT EXISTS ItemTemplates (\r\n                Id INTEGER PRIMARY KEY AUTOINCREMENT,\r\n                FloorId INTEGER NOT NULL,\r\n                Name TEXT NOT NULL,\r\n                ItemType TEXT NOT NULL,\r\n                Rarity INTEGER NOT NULL CHECK (Rarity IN(1,2,3,4)),\r\n                RequiredLevel INTEGER NOT NULL,\r\n                AttackBonus REAL NOT NULL,\r\n                AttackSpeedBonus REAL NOT NULL,\r\n                DefenseBonus REAL NOT NULL,\r\n                MaxHPBonus REAL NOT NULL,\r\n                GoldValue REAL NOT NULL DEFAULT 0,\r\n                                Code TEXT NOT NULL UNIQUE,\r\n                FOREIGN KEY (FloorId) REFERENCES Floors(Id)\r\n            );",
            "CREATE TABLE IF NOT EXISTS FloorDrops (\r\n                FloorId INTEGER NOT NULL,\r\n                ItemTemplateId INTEGER NOT NULL,\r\n                Weight INTEGER NOT NULL CHECK (Weight > 0),\r\n                FOREIGN KEY (FloorId) REFERENCES Floors(Id),\r\n                FOREIGN KEY (ItemTemplateId) REFERENCES ItemTemplates(Id),\r\n                PRIMARY KEY (FloorId, ItemTemplateId)\r\n                );",
            "CREATE TABLE IF NOT EXISTS EnemyCommonDrops (\r\n                EnemyTemplateId INTEGER NOT NULL,\r\n                ItemTemplateId INTEGER NOT NULL,\r\n                Weight INTEGER NOT NULL CHECK (Weight > 0),\r\n                FOREIGN KEY (EnemyTemplateId) REFERENCES EnemyTemplates(Id),\r\n                FOREIGN KEY (ItemTemplateId) REFERENCES ItemTemplates(Id),\r\n                PRIMARY KEY (EnemyTemplateId, ItemTemplateId)\r\n                );",
            "CREATE TABLE IF NOT EXISTS EnemyRareDrops (\r\n                EnemyTemplateId INTEGER NOT NULL,\r\n                ItemTemplateId INTEGER NOT NULL,\r\n                Weight INTEGER NOT NULL CHECK (Weight > 0),\r\n                FOREIGN KEY (EnemyTemplateId) REFERENCES EnemyTemplates(Id),\r\n                FOREIGN KEY (ItemTemplateId) REFERENCES ItemTemplates(Id),\r\n                PRIMARY KEY (EnemyTemplateId, ItemTemplateId)\r\n                );",
            "CREATE TABLE IF NOT EXISTS PlayerInventory (\r\n                PlayerId INTEGER NOT NULL,\r\n                ItemTemplateId INTEGER NOT NULL,\r\n                Quantity INTEGER NOT NULL CHECK(Quantity >= 0),\r\n                FOREIGN KEY (PlayerId) REFERENCES Players(Id),\r\n                FOREIGN KEY (ItemTemplateId) REFERENCES ItemTemplates(Id),\r\n                PRIMARY KEY (PlayerId, ItemTemplateId)\r\n            );",
            "CREATE TABLE IF NOT EXISTS EquippedItems (\r\n                PlayerId INTEGER PRIMARY KEY NOT NULL,\r\n                WeaponItemTemplateId INTEGER,\r\n                ArmourItemTemplateId INTEGER,\r\n                                AccessoryItemTemplateId INTEGER,\r\n                FOREIGN KEY (PlayerId) REFERENCES Players(Id),\r\n                FOREIGN KEY (WeaponItemTemplateId) REFERENCES ItemTemplates(Id),\r\n                FOREIGN KEY (ArmourItemTemplateId) REFERENCES ItemTemplates(Id)\r\n            );"]);
        public DatabaseInitialiser(string databasePath)
        {
            _databasePath = databasePath;
        }

        public void Initialise()
        {
            using var connection = new SqliteConnection($"Data Source={_databasePath}");
            connection.Open();


            using var tx = connection.BeginTransaction();

            try
            {
                connection.Execute("PRAGMA foreign_keys = ON;", transaction: tx);
                foreach(var table in tables)
                {
                    connection.Execute(table, transaction: tx);
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