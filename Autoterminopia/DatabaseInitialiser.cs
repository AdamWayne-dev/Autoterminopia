using Dapper;
using Microsoft.Data.Sqlite;

internal class DatabaseInitialiser
{
    public DatabaseInitialiser(string databasePath)
    {
        using var connection = new SqliteConnection($"Data Source={databasePath}");
        connection.Open();

        connection.Execute("PRAGMA foreign_keys = ON;");

        const string createTableQuery = @"
                CREATE TABLE IF NOT EXISTS Players (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Level INTEGER NOT NULL CHECK(Level >= 1),
                XP REAL NOT NULL,
                Gold INTEGER NOT NULL,
                CurrentHP REAL NOT NULL
            );
                CREATE TABLE IF NOT EXISTS PlayerStats (
                PlayerId INTEGER PRIMARY KEY NOT NULL,
                BaseAttackPower REAL NOT NULL,
                BaseAttackSpeed REAL NOT NULL,
                BaseDefense REAL NOT NULL,
                BaseMaxHP REAL NOT NULL,
                FOREIGN KEY (PlayerId) REFERENCES Players(Id)
            );
                
                CREATE TABLE IF NOT EXISTS Floors (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                MinLevel INTEGER NOT NULL,
                MaxLevel INTEGER NOT NULL
            );

                CREATE TABLE IF NOT EXISTS EnemyTemplates (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                FloorId INTEGER NOT NULL,
                BaseHP REAL NOT NULL,
                BaseAttackPower REAL NOT NULL,
                BaseAttackSpeed REAL NOT NULL,
                XPReward REAL NOT NULL,
                SpawnWeight INTEGER NOT NULL,
                FOREIGN KEY (FloorId) REFERENCES Floors(Id)
            );

                CREATE TABLE IF NOT EXISTS ItemTemplates (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                FloorId INTEGER NOT NULL,
                Name TEXT NOT NULL,
                ItemType TEXT NOT NULL,
                Rarity INTEGER NOT NULL CHECK (Rarity IN(1,2,3,4)),
                RequiredLevel INTEGER NOT NULL,
                AttackBonus REAL NOT NULL,
                AttackSpeedBonus REAL NOT NULL,
                DefenseBonus REAL NOT NULL,
                MaxHPBonus REAL NOT NULL,
                GoldValue REAL NOT NULL,
                FOREIGN KEY (FloorId) REFERENCES Floors(Id)
            );

                CREATE TABLE IF NOT EXISTS FloorDrops (
                FloorId INTEGER NOT NULL,
                ItemTemplateId INTEGER NOT NULL,
                Weight INTEGER NOT NULL CHECK (Weight > 0),
                FOREIGN KEY (FloorId) REFERENCES Floors(Id),
                FOREIGN KEY (ItemTemplateId) REFERENCES ItemTemplates(Id),
                PRIMARY KEY (FloorId, ItemTemplateId)
                );
                
                CREATE TABLE IF NOT EXISTS EnemyDrops (
                EnemyTemplateId INTEGER NOT NULL,
                ItemTemplateId INTEGER NOT NULL,
                Weight INTEGER NOT NULL CHECK (Weight > 0),
                FOREIGN KEY (EnemyTemplateId) REFERENCES EnemyTemplates(Id),
                FOREIGN KEY (ItemTemplateId) REFERENCES ItemTemplates(Id),
                PRIMARY KEY (EnemyTemplateId, ItemTemplateId)
                );

                CREATE TABLE IF NOT EXISTS PlayerInventory (
                PlayerId INTEGER NOT NULL,
                ItemTemplateId INTEGER NOT NULL,
                Quantity INTEGER NOT NULL CHECK(Quantity >= 0),
                FOREIGN KEY (PlayerId) REFERENCES Players(Id),
                FOREIGN KEY (ItemTemplateId) REFERENCES ItemTemplates(Id),
                PRIMARY KEY (PlayerId, ItemTemplateId)
            );
            
                CREATE TABLE IF NOT EXISTS EquippedItems (
                PlayerId INTEGER PRIMARY KEY NOT NULL,
                WeaponItemTemplateId INTEGER,
                ArmourItemTemplateId INTEGER,
                FOREIGN KEY (PlayerId) REFERENCES Players(Id),
                FOREIGN KEY (WeaponItemTemplateId) REFERENCES ItemTemplates(Id),
                FOREIGN KEY (ArmourItemTemplateId) REFERENCES ItemTemplates(Id)
            );

                ";
        connection.Execute(createTableQuery);
    }
}