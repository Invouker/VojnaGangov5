using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace Client.Events{
    public static class Hud{
        private static bool IsRadarExtended;

        public static async Task OnRender(){
            renderMoney($"Cash ${Utils.FormatWithDotSeparator(Var.Money)}", 0.83f, 0.01f, 153, 255, 153); // Wallet
            renderMoney($"Bank ${Utils.FormatWithDotSeparator(Var.BankMoney)}", 0.83f, 0.04f, 0, 155, 0); // Bank

            renderMap();
        }

        private static async void renderMap(){
            if (!IsControlJustPressed(0, 20) || IsRadarExtended) return;
            IsRadarExtended = true;
            SetRadarBigmapEnabled(true, false);
            await BaseScript.Delay(7000);
            SetRadarBigmapEnabled(false, false);
            IsRadarExtended = false;
        }

        private static void renderMoney(string text, float x, float y, int r, int g, int b){
            SetTextFont(7);
            SetTextScale(0.0f, .55f);
            SetTextColour(r, g, b, 255);
            SetTextDropshadow(1, 0, 0, 0, 255);
            SetTextDropShadow();
            SetTextOutline();
            SetTextEntry("STRING");
            AddTextComponentString(text);
            DrawText(x, y);
        }

        public static void ShowRankBar(int giveXP, int maxXP){
            ScaleformUI.Main.RankBarInstance.OverrideAnimationSpeed(3000);
            ScaleformUI.Main.RankBarInstance.OverrideOnscreenDuration(7000);
            ScaleformUI.Main.RankBarInstance.SetScores(0, maxXP, Var.XP, (Var.XP + giveXP), Var.Level);
        }

        public static async void ChangeMoney(int moneyType, int value){
            int currentMoney = moneyType switch{
                0 => Var.Money,
                1 => Var.BankMoney,
                _ => -1 // Indicating an invalid money type
            };

            if (currentMoney == -1){
                Debug.WriteLine("Invalid money type.");
                return;
            }

            int diff = value - currentMoney;
            int increment = Math.Max(100, Math.Abs(diff) / 100);

            while (currentMoney != value){
                currentMoney += diff > 0 ? increment : -increment;

                if ((diff > 0 && currentMoney > value) || (diff < 0 && currentMoney < value)){
                    currentMoney = value;
                }

                switch (moneyType){
                    case 0:
                        Var.Money = currentMoney;
                        break;
                    case 1:
                        Var.BankMoney = currentMoney;
                        break;
                }

                await BaseScript.Delay(40);
            }
        }

        public static void Rank(){
            ShowRankBar(1000, 100);
        }

        public static void ChangeXp(int xp, int level){
            Var.XP = xp;
            Var.Level = level;
        }
    }
}