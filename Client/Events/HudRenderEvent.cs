using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Client.Handlers;
using Client.Menus;
using Client.Utils;
using Newtonsoft.Json;
using ScaleformUI;
using ScaleformUI.Elements;
using ScaleformUI.Menu;
using ScaleformUI.Scaleforms;
using static CitizenFX.Core.Native.API;

namespace Client.Events{
    public static class HudRenderEvent{
        private static bool IsRadarExtended;
        private static readonly PlayerListHandler PlayerListInstance = ScaleformUI.Main.PlayerListInstance;
        private static MinimapHandler.Minimap minimap = MinimapHandler.GetMinimapAnchor();

        static HudRenderEvent(){
            KeyHandler.CreateKeyPair(Control.MultiplayerInfo, RenderProps);
            KeyHandler.CreateKeyPair(Control.InteractionMenu,
                                     () => {
                                         UIMenu menu = InteractiveMenu.GetInteractiveUI();
                                         menu.Visible = !menu.Visible;
                                     });

             EventDispatcher.Mount("player:hud:update:money", new Action<int, int>(ChangeMoney));
             EventDispatcher.Mount("player:hud:update:show:rank", new Action<int>(ShowRankBar));
             EventDispatcher.Mount("player:hud:update:xp", new Action<int, int>(ChangeXp));
        }

        public static Task OnRender(){
            if (Var.HideAllHud) return Task.FromResult(true);
            string moneyWallet = $"Cash ${FormatWithDotSeparator(Var.Money)}";
            string moneyBank = $"Bank ${FormatWithDotSeparator(Var.BankMoney)}";
            renderText(moneyWallet, GetMoneyOffset(moneyWallet), 0.045f, 153, 255, 153); // Wallet
            renderText(moneyBank, GetMoneyOffset(moneyBank), 0.075f, 0, 145, 0); // Bank
            //renderMoney($"Cash ${Utils.FormatWithDotSeparator(Var.Money)}", 0.83f, 0.045f, 153, 255, 153); // Wallet
            //renderMoney($"Bank ${Utils.FormatWithDotSeparator(Var.BankMoney)}", 0.85f, 0.075f, 0, 155, 0); // Bank
            Vector3 playerPos = Game.Player.Character.Position;
            string zoneName = GetNameOfZone(playerPos.X, playerPos.Y, playerPos.Z);
            Locations.TryGetValue(zoneName, out string zone);
            renderText(zone, minimap.x, minimap.y, 255, 255, 255, font: 1, size: 0.4f);

            return Task.FromResult(true);
        }

        private static float GetMoneyOffset(string money){
            int moneyCharCount = money.Length;
            float moneyCharCountOffset = 1f; // set initial offset
            for (int i = 0; i < moneyCharCount; i++){
                moneyCharCountOffset -= 0.0091f; // add offset for each character
            }

            return moneyCharCountOffset;
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
            Trace.Log("playerlist:list");
            EventDispatcher.Send("playerlist:list", new Action<string>(Action));

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

        private static void renderText(string text, float x, float y, int r, int g, int b, float size = 0.55f,
            int font = 7){
            SetTextFont(font);
            SetTextScale(0.0f, size);
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
            ScaleformUI.Main.RankBarInstance.SetScores(0, GetReputationToLevel(Var.Level + 1), Var.XP,
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

        private static Dictionary<string, string> Locations = new Dictionary<string, string>{
            { "AIRP", "Los Santos International Airport" },
            { "ALAMO", "Alamo Sea" },
            { "ALTA", "Alta" },
            { "ARMYB", "Fort Zancudo" },
            { "BANHAMC", "Banham Canyon Dr" },
            { "BANNING", "Banning" },
            { "BEACH", "Vespucci Beach" },
            { "BHAMCA", "Banham Canyon" },
            { "BRADP", "Braddock Pass" },
            { "BRADT", "Braddock Tunnel" },
            { "BURTON", "Burton" },
            { "CALAFB", "Calafia Bridge" },
            { "CANNY", "Raton Canyon" },
            { "CCREAK", "Cassidy Creek" },
            { "CHAMH", "Chamberlain Hills" },
            { "CHIL", "Vinewood Hills" },
            { "CHU", "Chumash" },
            { "CMSW", "Chiliad Mountain State Wilderness" },
            { "CYPRE", "Cypress Flats" },
            { "DAVIS", "Davis" },
            { "DELBE", "Del Perro Beach" },
            { "DELPE", "Del Perro" },
            { "DELSOL", "La Puerta" },
            { "DESRT", "Grand Senora Desert" },
            { "DOWNT", "Downtown" },
            { "DTVINE", "Downtown Vinewood" },
            { "EAST_V", "East Vinewood" },
            { "EBURO", "El Burro Heights" },
            { "ELGORL", "El Gordo Lighthouse" },
            { "ELYSIAN", "Elysian Island" },
            { "GALFISH", "Galilee" },
            { "GOLF", "GWC and Golfing Society" },
            { "GRAPES", "Grapeseed" },
            { "GREATC", "Great Chaparral" },
            { "HARMO", "Harmony" },
            { "HAWICK", "Hawick" },
            { "HORS", "Vinewood Racetrack" },
            { "HUMLAB", "Humane Labs and Research" },
            { "JAIL", "Bolingbroke Penitentiary" },
            { "KOREAT", "Little Seoul" },
            { "LACT", "Land Act Reservoir" },
            { "LAGO", "Lago Zancudo" },
            { "LDAM", "Land Act Dam" },
            { "LEGSQU", "Legion Square" },
            { "LMESA", "La Mesa" },
            { "LOSPUER", "La Puerta" },
            { "MIRR", "Mirror Park" },
            { "MORN", "Morningwood" },
            { "MOVIE", "Richards Majestic" },
            { "MTCHIL", "Mount Chiliad" },
            { "MTGORDO", "Mount Gordo" },
            { "MTJOSE", "Mount Josiah" },
            { "MURRI", "Murrieta Heights" },
            { "NCHU", "North Chumash" },
            { "NOOSE", "N.O.O.S.E" },
            { "OCEANA", "Pacific Ocean" },
            { "PALCOV", "Paleto Cove" },
            { "PALETO", "Paleto Bay" },
            { "PALFOR", "Paleto Forest" },
            { "PALHIGH", "Palomino Highlands" },
            { "PALMPOW", "Palmer-Taylor Power Station" },
            { "PBLUFF", "Pacific Bluffs" },
            { "PBOX", "Pillbox Hill" },
            { "PROCOB", "Procopio Beach" },
            { "RANCHO", "Rancho" },
            { "RGLEN", "Richman Glen" },
            { "RICHM", "Richman" },
            { "ROCKF", "Rockford Hills" },
            { "RTRAK", "Redwood Lights Track" },
            { "SANAND", "San Andreas" },
            { "SANCHIA", "San Chianski Mountain Range" },
            { "SANDY", "Sandy Shores" },
            { "SKID", "Mission Row" },
            { "SLAB", "Stab City" },
            { "STAD", "Maze Bank Arena" },
            { "STRAW", "Strawberry" },
            { "TATAMO", "Tataviam Mountains" },
            { "TERMINA", "Terminal" },
            { "TEXTI", "Textile City" },
            { "TONGVAH", "Tongva Hills" },
            { "TONGVAV", "Tongva Valley" },
            { "VCANA", "Vespucci Canals" },
            { "VESP", "Vespucci" },
            { "VINE", "Vinewood" },
            { "WINDF", "Ron Alternates Wind Farm" },
            { "WVINE", "West Vinewood" },
            { "ZANCUDO", "Zancudo River" },
            { "ZP_ORT", "Port of South Los Santos" },
            { "ZQ_UAR", "Davis Quartz" },
            { "PROL", "Prologue / North Yankton" },
            { "ISHeist", "Cayo Perico Island" }
        };
    }
}