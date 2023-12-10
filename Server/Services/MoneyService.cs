using System;
using CitizenFX.Core;
using Server.Database.Entities.Player;

namespace Server.Services;

public class MoneyService : IService{
    public enum MoneyType{
        Wallet = 0,
        Bank = 1
    }

    public static bool HasMoreThanMoney(Player player, MoneyType moneyType, int value){
        VGPlayer vgPlayer = PlayerService.GetVgPlayerByPlayer(player);
        return moneyType switch{
            MoneyType.Bank => vgPlayer.BankMoney >= value,
            MoneyType.Wallet => vgPlayer.Money >= value,
            _ => throw new ArgumentOutOfRangeException(nameof(moneyType), moneyType,
                                                       "There is no other registred MoneyType than (Wallet,Bank).")
        };
    }

    public static long GetMoney(Player player, MoneyType moneyType){
        VGPlayer vgPlayer = PlayerService.GetVgPlayerByPlayer(player);
        return moneyType switch{
            MoneyType.Bank => vgPlayer.BankMoney,
            MoneyType.Wallet => vgPlayer.Money,
            _ => throw new ArgumentOutOfRangeException(nameof(moneyType), moneyType,
                                                       "There is no other registred MoneyType than (Wallet,Bank).")
        };
    }

    public static void SetMoney(Player player, MoneyType moneyType, uint value){
        VGPlayer vgPlayer = PlayerService.GetVgPlayerByPlayer(player);
        switch (moneyType){
            case MoneyType.Bank:
                vgPlayer.BankMoney = value;
                break;
            case MoneyType.Wallet:
                vgPlayer.Money = value;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(moneyType), moneyType,
                                                      "There is no other registred MoneyType than (Wallet,Bank).");
        }

        BaseScript.TriggerClientEvent(player, "player:hud:update:money", (int)moneyType, value);
    }

    public static void AddMoney(Player player, MoneyType moneyType, uint value){
        VGPlayer vgPlayer = PlayerService.GetVgPlayerByPlayer(player);
        switch (moneyType){
            case MoneyType.Bank:
                vgPlayer.BankMoney += value;
                BaseScript.TriggerClientEvent(player, "player:hud:update:money", (int)moneyType,
                                              vgPlayer.BankMoney);
                break;
            case MoneyType.Wallet:
                vgPlayer.Money += value;
                BaseScript.TriggerClientEvent(player, "player:hud:update:money", (int)moneyType, vgPlayer.Money);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(moneyType), moneyType,
                                                      "There is no other registred MoneyType than (Wallet,Bank).");
        }
    }

    public static void TakeMoney(Player player, MoneyType moneyType, uint value){
        VGPlayer vgPlayer = PlayerService.GetVgPlayerByPlayer(player);
        switch (moneyType){
            case MoneyType.Bank:
                vgPlayer.BankMoney -= value;
                BaseScript.TriggerClientEvent(player, "player:hud:update:money", (int)moneyType,
                                              vgPlayer.BankMoney);
                break;
            case MoneyType.Wallet:
                vgPlayer.Money -= value;
                BaseScript.TriggerClientEvent(player, "player:hud:update:money", (int)moneyType, vgPlayer.Money);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(moneyType), moneyType, null);
        }
    }

    public void Init() { }
}