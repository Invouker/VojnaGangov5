using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using CitizenFX.Core;
using Client.Handlers;
using Client.UIHandlers;
using Newtonsoft.Json;
using ScaleformUI;
using ScaleformUI.Elements;
using ScaleformUI.Scaleforms;
using static CitizenFX.Core.Native.API;

namespace Client.Events{
    public static class Hud{
        private static bool IsRadarExtended;
        private static readonly PlayerListHandler PlayerListInstance = ScaleformUI.Main.PlayerListInstance;

        static Hud(){
            KeyHandler.CreateKeyPair(Control.MultiplayerInfo, RenderProps);
            KeyHandler.CreateKeyPair(Control.InteractionMenu,
                                     () => {
                                         InteractiveUI.GetInteractiveUI().Visible =
                                             !InteractiveUI.GetInteractiveUI().Visible;
                                     });
        }

        public static Task OnRender(){
            if (Var.HideAllHud) return Task.FromResult(true);

            renderMoney($"Cash ${Utils.FormatWithDotSeparator(Var.Money)}", 0.83f, 0.01f, 153, 255, 153); // Wallet
            renderMoney($"Bank ${Utils.FormatWithDotSeparator(Var.BankMoney)}", 0.83f, 0.04f, 0, 155, 0); // Bank

            return Task.FromResult(true);
        }

        private static async void renderPlayerList(){
            if (MenuHandler.IsAnyMenuOpen) return;
            await PlayerListInstance.Load();
            int playerCount = Main.Instance.GetPlayers().Count();
            PlayerListInstance.SetTitle(Var.ServerName, $"({playerCount}/{Var.MaxPlayers})", 1);

            List<PlayerRow> playerRows = await LoadPlayer();
            PlayerListInstance.PlayerRows = playerRows;

            PlayerListInstance.CurrentPage = 1;
            PlayerListInstance.Enabled = true;
        }

        private static async Task<List<PlayerRow>> LoadPlayer(){
            List<PlayerRow> playerRows = new List<PlayerRow>();
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

            BaseScript.TriggerServerEvent("playerlist:list", new Action<string>(Action));

            await tcs.Task;
            return playerRows;

            async void Action(string data){
                Dictionary<string, PlayerSlot> playerSlots =
                    JsonConvert.DeserializeObject<Dictionary<string, PlayerSlot>>(data);

                foreach (KeyValuePair<string, PlayerSlot> playerSlotDic in playerSlots){
                    var handle = RegisterPedheadshot(PlayerPedId());

                    while (!IsPedheadshotReady(handle) || !IsPedheadshotValid(handle))
                        await BaseScript.Delay(100);

                    var txd = GetPedheadshotTxdString(handle);

                    PlayerSlot playerSlot = playerSlotDic.Value;
                    playerRows.Add(new PlayerRow{
                        Name = playerSlot.Name,
                        RightText = playerSlot.Level.ToString(),
                        Color = SColor.Olive.ArgbValue,
                        IconOverlayText = "",
                        JobPointsText = "",
                        CrewLabelText = playerSlot.GangName,
                        TextureString = txd,
                        RightIcon = ScoreRightIconType.RANK_FREEMODE,
                        JobPointsDisplayType = ScoreDisplayType.NONE,
                        FriendType = ' '
                    });
                    UnregisterPedheadshot(handle);
                }

                tcs.SetResult(true);
            }
        }

        [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
        public class PlayerSlot{
            public string Name{ get; set; }
            public int Level{ get; set; }
            public string JobPointsText{ get; set; }
            public string GangName{ get; set; }

            public override string ToString(){
                return $"Name: {Name}, Level: {Level}, JobPointsText: {JobPointsText}, GangName: {GangName}";
            }
        }

        private static async void RenderProps(){
            if (MenuHandler.IsAnyMenuOpen) return;
            if (IsRadarExtended) return;
            IsRadarExtended = true;
            renderPlayerList();
            SetRadarBigmapEnabled(true, false);
            ShowRankBar(0);
            Debug.WriteLine($"Xp: {Var.XP}, Level: {Var.Level}");
            await BaseScript.Delay(7000);
            SetRadarBigmapEnabled(false, false);
            PlayerListInstance.Enabled = false;
            IsRadarExtended = false;
            ScaleformUI.Main.RankBarInstance.Remove();
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

        public static void ShowRankBar(int giveXP){
            ScaleformUI.Main.RankBarInstance.OverrideAnimationSpeed(3000);
            ScaleformUI.Main.RankBarInstance.OverrideOnscreenDuration(7000);
            ScaleformUI.Main.RankBarInstance.SetScores(0, Utils.GetReputationToLevel(Var.Level + 1), Var.XP,
                                                       (Var.XP + giveXP), Var.Level);
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

        public static void ChangeXp(int xp, int level){
            Var.XP = xp;
            Var.Level = level;
        }
    }
}