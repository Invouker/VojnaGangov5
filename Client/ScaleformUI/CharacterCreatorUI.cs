using System;
using System.Collections.Generic;
using System.Drawing;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using ScaleformUI;
using ScaleformUI.Elements;
using ScaleformUI.Menu;

namespace Client.ScaleformUI{
    public class CharacterCreatorUI{
        public static void Interact(int id){
            /* if (id == 1)
                 createUI();*/
        }

        public static UIMenu createUI(){
            UIMenu menu = new("Character creator", "Change your character", new PointF(20, 20), true);
            menu.BuildingAnimation = MenuBuildingAnimation.NONE;
            menu.EnableAnimation = false;
            menu.MaxItemsOnScreen = 7;
            menu.ScrollingType = ScrollingType.ENDLESS;

            var sex = new List<dynamic>{
                "Male",
                "Female"
            };
            UIMenuListItem sexListItem = new("Sex", sex, 0, "Select your gender");
            menu.AddItem(sexListItem);

            menu.OnListChange += (sender, item, index) => {
                if (item == sexListItem)
                    ChangeSex(item.Items[index].ToString());
            };

            ByParentsCreatorUI(menu);
            menu.Visible = true;
            return menu;
        }

        private static void ByParentsCreatorUI(UIMenu mainMenu){
            int momIndex = 0, dadIndex = 0;
            double[] amount ={ 0, 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1.0 };
            double shapeMixData = .5d, skinMixData = .5d;

            UIMenuItem windowsItem = new("Change your parents", "You can change your face by your parents.");
            windowsItem.SetRightLabel("»");
            mainMenu.AddItem(windowsItem);


            UIMenuItem back = new("Back to creator");
            back.SetRightLabel("«");


            UIMenu windowSubmenu = new("Parents", "Select your parents");
            windowSubmenu.BuildingAnimation = MenuBuildingAnimation.NONE;
            windowSubmenu.EnableAnimation = false;
            windowSubmenu.MaxItemsOnScreen = 7;
            windowSubmenu.ScrollingType = ScrollingType.ENDLESS;

            UIMenuHeritageWindow heritageWindow = new(0, 0);
            UIMenuDetailsWindow statsWindow =
                new("Parents resemblance", "Dad:", "Mom:", true, new List<UIDetailStat>());
            windowSubmenu.AddWindow(heritageWindow);
            windowSubmenu.AddWindow(statsWindow);
            List<dynamic> momfaces = new List<dynamic>(){
                "Hannah", "Audrey", "Jasmine", "Giselle", "Amelia", "Isabella", "Zoe", "Ava", "Camilla", "Violet",
                "Sophia", "Eveline", "Nicole", "Ashley", "Grace", "Brianna", "Natalie", "Olivia", "Elizabeth",
                "Charlotte", "Emma", "Misty"
            };
            List<dynamic> dadfaces = new List<dynamic>(){
                "Benjamin", "Daniel", "Joshua", "Noah", "Andrew", "Joan", "Alex", "Isaac", "Evan", "Ethan", "Vincent",
                "Angel", "Diego", "Adrian", "Gabriel", "Michael", "Santiago", "Kevin", "Louis", "Samuel", "Anthony",
                "Claude", "Niko", "John"
            };
            UIMenuListItem mom = new("Mother", momfaces, 0);
            UIMenuListItem dad = new("Father", dadfaces, 0);
            UIMenuSliderItem ShapeMixItem = new("Shape Mix Slider", "This is Useful on Shape mix", 10, 1, 5, true);
            UIMenuSliderItem SkinMixItem = new("Skin Mix Slider", "This is Useful on Skin mix", 10, 1, 5, true);
            //UIMenuSliderItem SkinMixItem = new("Heritage Slider", "This is Useful on heritage", 100, 5, 50, true);
            windowSubmenu.AddItem(mom);
            windowSubmenu.AddItem(dad);
            windowSubmenu.AddItem(ShapeMixItem);
            windowSubmenu.AddItem(SkinMixItem);
            statsWindow.DetailMid = "Dad: " + ShapeMixItem.Value + "%";
            statsWindow.DetailBottom = "Mom: " + (100 - ShapeMixItem.Value) + "%";
            statsWindow.DetailStats = new List<UIDetailStat>(){
                new(100 - ShapeMixItem.Value, SColor.Pink),
                new(ShapeMixItem.Value, SColor.HUD_Freemode),
            };

            windowSubmenu.AddItem(back);

            windowSubmenu.OnListChange += (sender, item, index) => {
                if (item == mom)
                    momIndex = index;
                else if (item == dad)
                    dadIndex = index;

                heritageWindow.Index(momIndex, dadIndex);
                API.SetPedHeadBlendData(API.GetPlayerPed(-1), momIndex, dadIndex, 0, momIndex, dadIndex, 0,
                                        (float)shapeMixData,
                                        (float)skinMixData, .5f, true);
            };
            windowSubmenu.OnSliderChange += (sender, item, index) => {
                if (item == ShapeMixItem){
                    shapeMixData = amount[index];
                }
                else if (item == SkinMixItem)
                    skinMixData = amount[index];

                Debug.WriteLine($"Index: {index}, amount: {amount[index]}, amount.Count: {amount}");
                API.SetPedHeadBlendData(API.GetPlayerPed(-1), momIndex, dadIndex, 0, momIndex, dadIndex, 0,
                                        (float)shapeMixData,
                                        (float)skinMixData, .5f, true);
            };
            windowsItem.Activated += (sender, e) => { sender.SwitchTo(windowSubmenu, inheritOldMenuParams: true); };

            back.Activated += (sender, e) => { sender.SwitchTo(mainMenu, inheritOldMenuParams: true); };
        }

        private static async void ChangeSex(string hash){
            string sexHash = hash switch{
                "Male" => "mp_m_freemode_01",
                "Female" => "mp_f_freemode_01",
                _ => null
            };
            if (sexHash == null)
                throw new NullReferenceException("SexHash cannot be null!");
            uint intHash = (uint)API.GetHashKey(sexHash);
            API.RequestModel(intHash);

            while (!API.HasModelLoaded(intHash))
                await BaseScript.Delay(0);

            API.SetPlayerModel(Game.Player.Handle, intHash);
            API.SetPedDefaultComponentVariation(API.GetPlayerPed(-1));
            API.SetModelAsNoLongerNeeded(intHash);
        }

        private static readonly string[] MaleAmbient ={
            "a_m_m_afriamer_01", "a_m_m_beach_01", "a_m_m_beach_02", "a_m_m_bevhills_01", "a_m_m_bevhills_02",
            "a_m_m_business_01",
            "a_m_m_eastsa_01", "a_m_m_eastsa_02", "a_m_m_farmer_01", "a_m_m_fatlatin_01", "a_m_m_genfat_01",
            "a_m_m_genfat_02",
            "a_m_m_golfer_01", "a_m_m_hasjew_01", "a_m_m_hillbilly_01", "a_m_m_hillbilly_02", "a_m_m_indian_01",
            "a_m_m_ktown_01", "a_m_m_malibu_01", "a_m_m_mexcntry_01",
            "a_m_m_mexlabor_01", "a_m_m_og_boss_01", "a_m_m_paparazzi_01"
        };

        private static readonly string[] FemaleAmbient ={
            "a_f_m_bevhills_01", "a_f_m_bevhills_02", "a_f_m_bodybuild_01", "a_f_m_business_02", "a_f_m_eastsa_01",
            "a_f_m_eastsa_02", "a_f_m_fatbla_01", "a_f_m_fatcult_01", "a_f_m_fatwhite_01", "a_f_m_ktown_01",
            "a_f_m_ktown_02", "a_f_m_prolhost_01",
            "a_f_m_salton_01", "a_f_m_skidrow_01", "a_f_m_soucent_01", "a_f_m_soucent_02", "a_f_m_soucentmc_01",
            "a_f_m_tourist_01", "a_f_m_tramp_01",
            "a_f_m_trampbeac_01", "a_f_o_genstreet_01", "a_f_o_indian_01", "a_f_o_salton_01"
        };
    }
}