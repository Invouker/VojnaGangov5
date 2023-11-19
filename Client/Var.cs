namespace Client;

public static class Var{
    public static int Money{ get; set; }
    public static int BankMoney{ get; set; }

    public static void MoneyUpdate(int money, int bankMoney){
        Money = money;
        BankMoney = bankMoney;
    }
}