using System;

namespace Server.Database.Entities{
    /* CREATE TABLE `accounts` (  `id` int(11) NOT NULL,  `name` varchar(255) NOT NULL,  `licence` varchar(255) NOT NULL,  `hp` int (255) NOT NULL DEFAULT 100,  `max_hp` int (255) DEFAULT 100,  `armour` int (255) NOT NULL DEFAULT 100,  `max_armour` int (255) NOT NULL DEFAULT 100,  `wantedLevel` int (6) NOT NULL DEFAULT 0,  `money` bigint(20) NOT NULL DEFAULT 0,  `bankMoney` bigint(20) NOT NULL DEFAULT 0,  `Level` int (255) DEFAULT 1,  `Xp` int (11) NOT NULL DEFAULT 0,  `posX` float NOT NULL,  `posY` float NOT NULL,  `posZ` float NOT NULL,  `Dimension` int (255) NOT NULL DEFAULT 0) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE = utf8mb4_general_ci;ALTER TABLE `accounts`  ADD PRIMARY KEY(`id`);ALTER TABLE `accounts`  MODIFY `id` int (11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=1;COMMIT;
*/
    class VGPlayer{
        public const string TABLE_NAME = "accounts";

        /* TODO:
            Add saving system for this:

            health + max health
            armour + max armour
            weapons + ammo
         */

        public int Id{ get; set; }
        public string Name{ get; set; }
        public string Licence{ get; set; }

        public int Hp{ get; set; }
        public int Max_hp{ get; set; }
        public int Armour{ get; set; }
        public int Max_armour{ get; set; }
        public int WantedLevel{ get; set; }
        public long Money{ get; set; }
        public long BankMoney{ get; set; }

        public int Level{ get; set; }
        public int Xp{ get; set; }

        public bool Registred{ get; set; }

        public int WalkingStyle{ get; set; }

        #region Position data of player

        public float PosX{ get; set; }
        public float PosY{ get; set; }
        public float PosZ{ get; set; }
        public int Dimension{ get; set; }

        #endregion

        public VGPlayer(int id, string name, string licence, int hp, int max_hp, int armour, int max_armour,
            int wantedLevel, int money, int bankMoney, int level, int xp, float posX, float posY, float posZ,
            int dimension){
            Id = id;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Licence = licence ?? throw new ArgumentNullException(nameof(licence));
            Hp = hp;
            Max_hp = max_hp;
            Armour = armour;
            Max_armour = max_armour;
            WantedLevel = wantedLevel;
            Money = money;
            BankMoney = bankMoney;
            Level = level;
            Xp = xp;
            PosX = posX;
            PosY = posY;
            PosZ = posZ;
            Dimension = dimension;
        }

        public VGPlayer(string name, string licence, int hp, int max_hp, int armour, int max_armour, int wantedLevel,
            int money, int bankMoney, int level, int xp, float posX, float posY, float posZ, int dimension){
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Licence = licence ?? throw new ArgumentNullException(nameof(licence));
            Hp = hp;
            Max_hp = max_hp;
            Armour = armour;
            Max_armour = max_armour;
            WantedLevel = wantedLevel;
            Money = money;
            BankMoney = bankMoney;
            Level = level;
            Xp = xp;
            PosX = posX;
            PosY = posY;
            PosZ = posZ;
            Dimension = dimension;
        }


        public VGPlayer(){ }

        public override string ToString(){
            return
                $"Id: {Id}, Name: {Name}, License: {Licence}, Hp: {Hp}, Max HP: {Max_hp}, Armour: {Armour}, Max Armour: {Max_armour}, WantedLevel: {WantedLevel}, Money: {Money}, BankMoney: {BankMoney}, Level: {Level},Xp: {Xp}, PosX: {PosX}, PosY: {PosY}, PosZ: {PosZ}, Dimension: {Dimension}";
        }


        /*
         Weapon data should be saved with:
        https://docs.fivem.net/natives/?_0x3A87E44BB9A01D54 // GetCurrentPedWeapon
        https://docs.fivem.net/natives/?_0x7FEAD38B326B9F74 // GetPedAmmoTypeFromWeapon

        https://docs.fivem.net/natives/?_0x5FD1E1F011E76D7E // SetPedAmmoByType

        https://docs.fivem.net/natives/?_0xF0A60040BE558F2D // GetPedWeaponLiveryColor
        https://docs.fivem.net/natives/?_0x2B9EEDC07BD06B9F // GetPedWeaponTintIndex
        https://docs.fivem.net/natives/?_0xD966D51AA5B28BB9 // GiveWeaponComponentToPed
         */
    }
}