using System;

namespace Server.Database.Entities.Player{
    /* CREATE TABLE `accounts` (  `id` int(11) NOT NULL,  `name` varchar(255) NOT NULL,  `licence` varchar(255) NOT NULL,  `hp` int (255) NOT NULL DEFAULT 100,  `max_hp` int (255) DEFAULT 100,  `armour` int (255) NOT NULL DEFAULT 100,  `max_armour` int (255) NOT NULL DEFAULT 100,  `wantedLevel` int (6) NOT NULL DEFAULT 0,  `money` bigint(20) NOT NULL DEFAULT 0,  `bankMoney` bigint(20) NOT NULL DEFAULT 0,  `Level` int (255) DEFAULT 1,  `Xp` int (11) NOT NULL DEFAULT 0,  `posX` float NOT NULL,  `posY` float NOT NULL,  `posZ` float NOT NULL,  `Dimension` int (255) NOT NULL DEFAULT 0) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE = utf8mb4_general_ci;ALTER TABLE `accounts`  ADD PRIMARY KEY(`id`);ALTER TABLE `accounts`  MODIFY `id` int (11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=1;COMMIT;
*/
    public class VGPlayer : IPlayerMetaData{
        public const string TABLE_NAME = "accounts";

        //public List<Inventory.InventoryItem> Inventory { get; private set; }
        /* TODO:
            Add saving system for this:

            health + max health // DONE
            armour + max armour // DONE
            weapons + ammo
            inventory + items;
         */

        public int Id{ get; set; }
        public string Name{ get; set; }

        public int Hp{ get; set; }
        public int MaxHp{ get; set; }
        public int Armour{ get; set; }
        public int MaxArmour{ get; set; }
        public int WantedLevel{ get; set; }
        public long Money{ get; set; }
        public long BankMoney{ get; set; }

        public int Level{ get; set; }
        public int Xp{ get; set; }

        public int WalkingStyle{ get; set; }

        #region Position data of player

        public float PosX{ get; set; }
        public float PosY{ get; set; }
        public float PosZ{ get; set; }
        public int Dimension{ get; set; }

        #endregion

        public VGPlayer(string name, int hp, int max_hp, int armour, int max_armour, int wantedLevel,
            int money, int bankMoney, int level, int xp, float posX, float posY, float posZ, int dimension){
            Name = name ?? throw new ArgumentNullException(nameof(name));
            //License = licence ?? throw new ArgumentNullException(nameof(licence));
            Hp = hp;
            MaxHp = max_hp;
            Armour = armour;
            MaxArmour = max_armour;
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
                $"Id: {Id}, Name: {Name}, Hp: {Hp}, Max HP: {MaxHp}, Armour: {Armour}, Max Armour: {MaxArmour}, WantedLevel: {WantedLevel}, Money: {Money}, BankMoney: {BankMoney}, Level: {Level},Xp: {Xp}, PosX: {PosX}, PosY: {PosY}, PosZ: {PosZ}, Dimension: {Dimension}, WalkingStyle: {WalkingStyle}";
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