using System;

namespace Server.Database.Entities {

	// CREATE TABLE `vg5_fivem`.`accounts` ( `id` INT NOT NULL AUTO_INCREMENT , `name` VARCHAR(255) NOT NULL , `licence` VARCHAR(255) NOT NULL , `wantedLevel` INT(6) NOT NULL , `money` BIGINT NOT NULL , `bankMoney` BIGINT NOT NULL , `Level` INT(255) NOT NULL , `Xp` INT NOT NULL , `posX` FLOAT NOT NULL , `posY` FLOAT NOT NULL , `posZ` FLOAT NOT NULL , `Dimension` INT(1000) NOT NULL , PRIMARY KEY (`id`)) ENGINE = InnoDB;
	class VGPlayer {

		public const string TABLE_NAME = "accounts";

		/* TODO: 
			Add saving system for this:

			health + max health
			armour + max armour
			weapons + ammo 
		 */

		public int Id { get; set; }
		public string Name { get; set; }
		public string Licence { get; set; }
		public int WantedLevel { get; set; }
		public long Money { get; set; }
		public long BankMoney { get; set; }

		public int Level { get; set; }
		public int Xp { get; set; }

		#region Position data of player
		public float PosX { get; set; }
		public float PosY { get; set; }
		public float PosZ { get; set; }
		public int Dimension { get; set; }

		#endregion

		public VGPlayer(int id, string name, string licence, int wantedLevel, int money, int bankMoney, int level, int xp, float posX, float posY, float posZ, int dimension) {
			Id = id;
			Name = name ?? throw new ArgumentNullException(nameof(name));
			Licence = licence ?? throw new ArgumentNullException(nameof(licence));
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

		public VGPlayer(string name, string licence, int wantedLevel, int money, int bankMoney, int level, int xp, float posX, float posY, float posZ, int dimension) {
			Name = name ?? throw new ArgumentNullException(nameof(name));
			Licence = licence ?? throw new ArgumentNullException(nameof(licence));
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


		public VGPlayer() {}

		public override string ToString() {
		return $"Id: {Id}, Name: {Name}, License: {Licence}, WantedLevel: {WantedLevel}, Money: {Money}, BankMoney: {BankMoney}, Level: {Level},Xp: {Xp}, PosX: {PosX}, PosY: {PosY}, PosZ: {PosZ}, Dimension: {Dimension}";
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
