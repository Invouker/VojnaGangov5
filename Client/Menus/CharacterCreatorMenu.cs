using System.Collections.Generic;
using System.Drawing;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using Client.Entities;
using Client.Events;
using ScaleformUI;
using ScaleformUI.Menu;

namespace Client.Menus{
    public static class CharacterCreatorMenu{
        private static readonly CharacterCreatorEntity CharacterEntity = new CharacterCreatorEntity();

        public static UIMenu SelectSexUI(){
            UIMenu menu = new UIMenu("Character Creator", "Select your Sex", new PointF(20, 20)){
                BuildingAnimation = MenuBuildingAnimation.NONE,
                EnableAnimation = false,
                MaxItemsOnScreen = 8,
                ScrollingType = ScrollingType.CLASSIC,
                CanPlayerCloseMenu = false
            };
            UIMenuListItem sexListItem =
                new UIMenuListItem("Sex", new List<dynamic>{ "Male", "Female" }, 0, "Select your gender");
            sexListItem.OnListChanged += (sender, index) => ChangeSex(index);
            menu.AddItem(sexListItem);

            UIMenuItem confirm = new UIMenuItem("Confirm your selection");
            menu.AddItem(confirm);

            confirm.Activated += (sender, item) => {
                sender.SwitchTo(ConfirmationSexUI(), inheritOldMenuParams: true);
            };

            return menu;
        }

        private static UIMenu ConfirmationSexUI(){
            UIMenu menu = new UIMenu("Character Creator", "Confirm your selection", new PointF(20, 20)){
                BuildingAnimation = MenuBuildingAnimation.NONE,
                EnableAnimation = false,
                MaxItemsOnScreen = 8,
                ScrollingType = ScrollingType.CLASSIC,
                CanPlayerCloseMenu = false
            };

            UIMenuItem cancel = new UIMenuItem("Cancel");
            menu.AddItem(cancel);
            UIMenuItem confirm = new UIMenuItem("Confirm");
            menu.AddItem(confirm);

            confirm.Activated += (sender, item) => sender.SwitchTo(CharacterCreator(), inheritOldMenuParams: true);
            cancel.Activated += (sender, item) => sender.SwitchTo(SelectSexUI(), inheritOldMenuParams: true);

            return menu;
        }

        private static UIMenu CharacterCreator(){
            UIMenu menu = new UIMenu("Character creator", "Change your character", new PointF(20, 20)){
                BuildingAnimation = MenuBuildingAnimation.NONE,
                EnableAnimation = false,
                MaxItemsOnScreen = 8,
                ScrollingType = ScrollingType.CLASSIC,
                CanPlayerCloseMenu = false
            };

            ByParentsCreatorUI(menu);

            #region Face UI

            AddGridToMenu(menu, "Nose", "Up", "Narrow", "Wide", "Down", 0, 1);
            AddGridToMenu(menu, "Nose Profile", "Crooked", "Short", "Long", "Curved", 2, 3);
            AddGridToMenu(menu, "Nose Tip", "Tip Up", "Broken Left", "Broken Right", "Tip Down", 4, 5);
            AddGridToMenu(menu, "Eye Brows", "Up", "In", "Out", "Down", 6, 7);
            AddGridToMenu(menu, "Cheeks Bones", "Up", "In", "Out", "Down", 8, 9);
            AddHorizontalGridToMenu(menu, "Cheeks", "Gaunt", "Puffed", 10);
            AddHorizontalGridToMenu(menu, "Eyes", "Squint", "Wide", 11);
            AddHorizontalGridToMenu(menu, "Lips", "Thin", "Fat", 12);
            AddGridToMenu(menu, "Jaw", "Round", "Narrow", "Wide", "Square", 13, 14);
            AddGridToMenu(menu, "Chin", "Up", "In", "Out", "Down", 15, 16);
            AddGridToMenu(menu, "Chin Shape", "Rounded", "Square", "Pointed", "Bum", 17, 18);
            AddHorizontalGridToMenu(menu, "Neck Thickness", "Thin", "Fat", 19);

            #endregion

            #region Appearance UI

            List<dynamic> mHairNames = new List<dynamic>{
                "Close Shave", "Buzzcut", "Faux Hawk", "Hipster", "Side Parting", "Shorter Cut", "Biker", "Ponytail",
                "Cornrows", "Slicked", "Short Brushed", "Spikey",
                "Caesar", "Chopped", "Dreads", "Long Hair", "Shaggy Curls", "Surfer Dude", "Short Side Part",
                "High Slicked Sides", "Long Slicked", "Hipster Youth", "Mullet"
            }; //Last was: "Nightvision"
            List<dynamic> fHairNames = new List<dynamic>{
                "Close Shave", "Short", "Layered Bob", "Pigtails", "Ponytail", "Braided Mohawk", "Braids", "Bob",
                "Faux Hawk", "French Twist", "Long Bob", "Loose Tied",
                "Pixie", "Shaved Bangs", "Top Knot", "Wavy Bob", "Pin Up Girl", "Messy Bun", "Unknown", "Tight Bun",
                "Twisted Bob", "Big Bangs", "Braided Top Knot", "Mullet"
            }; //Last was: "Nightvision"

            List<dynamic> hairNames = CharacterEntity.Sex == 0 ? mHairNames : fHairNames;

            UIMenuListItem hairCuts = new UIMenuListItem("Haircut", hairNames, 0);
            UIMenuColorPanel hairCutsColor = new UIMenuColorPanel("Colour", ColorPanelType.Hair);
            hairCuts.AddPanel(hairCutsColor);

            hairCuts.OnListChanged += (selectedItem, index) => {
                API.SetPedComponentVariation(Game.Player.Character.Handle, 2, index, 0, 2);
                CharacterEntity.HairType = index;
            };

            hairCutsColor.OnColorPanelChange += (menu, panel, index) => {
                API.SetPedHairColor(Game.Player.Character.Handle, index, 0);
                CharacterEntity.HairColor = index;
            };

            menu.AddItem(hairCuts);

            #endregion

            #region Appearance UI

            List<dynamic> beard = new List<dynamic>{
                "Light Stubble", "Balbo", "Circle Beard", "Goatee", "Chin", "Chin Fuzz",
                "Pencil Chin Strap", "Scruffy", "Musketeer", "Mustache",
                "Trimmed Beard", "Stubble", "Thin Circle Beard", "Horseshoe", "Pencil and Chops", "Chin Strap Beard",
                "Balbo and Sideburns", "Mutton Chops", "Scruffy Beard", "Curly",
                "Curly and Deep Stranger", "Handlebar", "Faustic", "Otto and Patch", "Otto and Full Stranger",
                "Light Franz", "The Hampstead", "The Ambrose", "Lincoln Curtain", "Clean Shaven"
            };
            AddAppearanceMenu(menu, "Beard", 1, beard, 29);

            List<dynamic> eyebrow = new List<dynamic>{
                "Balanced", "Fashion", "Cleopatra", "Quizzical", "Femme", "Seductive", "Pinched", "Chola",
                "Triomphe", "Carefree", "Curvaceous", "Rodent",
                "Double Tram", "Thin", "Penciled", "Mother Plucker", "Straight and Narrow", "Natural", "Fuzzy",
                "Unkempt", "Caterpillar", "Regular", "Mediterranean", "Groomed", "Bushels",
                "Feathered", "Prickly", "Monobrow", "Winged", "Triple Tram", "Arched Tram", "Cutouts", "Fade Away",
                "Solo Tram", "None"
            };
            AddAppearanceMenu(menu, "Eye Brow", 2, eyebrow, 34);

            List<dynamic> ageing = new List<dynamic>{
                "Crow's Feet", "First Signs", "Middle Aged", "Worry Lines", "Depression", "Distinguished",
                "Aged", "Weathered", "Wrinkled", "Sagging", "Tough Life",
                "Vintage", "Retired", "Junkie", "Geriatric", "None"
            };
            AddAppearanceMenu(menu, "Skin Ageing", 3, ageing, 15);

            List<dynamic> makeup = new List<dynamic>{
                "None", "Smoky Black", "Bronze", "Soft Gray", "Retro Glam", "Natural Look", "Cat Eyes", "Chola", "Vamp",
                "Vinewood Glamour", "Bubblegum", "Aqua Dream",
                "Pin up", "Purple Passion", "Smoky Cat Eye", "Smoldering Ruby", "Pop Princess"
                //face paint
                /* "Kiss My Axe", "Panda Pussy", "The Bat", "Skull in Scarlet", "Serpentine", "The Veldt", "Unknown 1",
                 "Unknown 2", "Unknown 3", "Unknown 4", "Tribal Lines", "Tribal Swirls",
                 "Tribal Orange", "Tribal Red", "Trapped in A Box", "Clowning",
                 // makeup pt 2
                 "Guyliner", "Unknown 5", "Blood Tears", "Heavy Metal", "Sorrow", "Prince of Darkness", "Rocker", "Goth",
                 "Punk", "Devastated", "1"*/
            };
            AddAppearanceMenu(menu, "Makeup", 4, makeup, 0, ColorPanelType.Makeup);

            List<dynamic> complexion = new List<dynamic>{
                "Rosy Cheeks", "Stubble Rash", "Hot Flush", "Sunburn", "Bruised", "Alchoholic", "Patchy",
                "Totem", "Blood Vessels", "Damaged", "Pale", "Ghostly", "None"
            };
            AddAppearanceMenu(menu, "Skin Complexion", 6, complexion, 12);

            List<dynamic> sunDamage = new List<dynamic>{
                "Uneven", "Sandpaper", "Patchy", "Rough", "Leathery", "Textured", "Coarse", "Rugged", "Creased",
                "Cracked", "Gritty", "None"
            };
            AddAppearanceMenu(menu, "Skin Damage", 7, sunDamage, 11);

            List<dynamic> lipstick = new List<dynamic>{
                "Color Matte", "Color Gloss", "Lined Matte", "Lined Gloss", "Heavy Lined Matte",
                "Heavy Lined Gloss", "Lined Nude Matte", "Liner Nude Gloss",
                "Smudged", "Geisha", "None"
            };
            AddAppearanceMenu(menu, "Lipstick", 8, lipstick, 10);

            List<dynamic> moleFreckle = new List<dynamic>{
                "Cherub", "All Over", "Irregular", "Dot Dash", "Over the Bridge", "Baby Doll", "Pixie",
                "Sun Kissed", "Beauty Marks", "Line Up", "Modelesque",
                "Occasional", "Speckled", "Rain Drops", "Double Dip", "One Sided", "Pairs", "Growth", "None"
            };
            AddAppearanceMenu(menu, "Mole & Freckle", 9, moleFreckle, 18);

            List<dynamic> blemishes = new List<dynamic>{
                "None", "Measles", "Pimples", "Spots", "Break Out", "Blackheads", "Build Up", "Pustules", "Zits",
                "Full Acne", "Acne", "Cheek Rash", "Face Rash",
                "Picker", "Puberty", "Eyesore", "Chin Rash", "Two Face", "T Zone", "Greasy", "Marked", "Acne Scarring",
                "Full Acne Scarring", "Cold Sores", "Impetigo"
            };
            AddAppearanceMenu(menu, "Skin Blemishes", 11, blemishes);

            #endregion

            EditClotheUI(menu);

            #region Spawn To World

            UIMenuItem spawnPlayer = new UIMenuItem("Spawn to world", "");
            menu.AddItem(spawnPlayer);
            spawnPlayer.Activated += (sender, item) => {
                CharacterEntity.SendDataToServer();
                SpawnManager.TeleportToWorld(CharacterEntity.Sex, -2246.927f, 269.0242f, 174.6095f, 110f);
                menu.Visible = false;
            };

            #endregion

            return menu;
        }

        private static void AddGridToMenu(UIMenu menu, string title, string topText, string leftText, string rightText,
            string bottomText, int indexOne, int indexTwo){
            UIMenuItem uiMenuItem = new(title);
            UIMenuGridPanel uiGrid = new(topText, leftText, rightText, bottomText, new PointF(.5f, .5f));
            uiMenuItem.AddPanel(uiGrid);
            menu.AddItem(uiMenuItem);
            uiGrid.OnGridPanelChange += (menu, panel, value) => {
                switch (indexOne){
                    case 0:
                        CharacterEntity.NoseWidth = value.X;
                        break;
                    case 2:
                        CharacterEntity.NoseLength = value.X;
                        break;
                    case 4:
                        CharacterEntity.NoseTip = value.X;
                        break;
                    case 6:
                        CharacterEntity.Eyebrow = value.X;
                        break;
                    case 8:
                        CharacterEntity.CheekBones = value.X;
                        break;
                    case 13:
                        CharacterEntity.JawBoneWidth = value.X;
                        break;
                    case 15:
                        CharacterEntity.ChinBone = value.X;
                        break;
                    case 17:
                        CharacterEntity.ChinBoneShape = value.X;
                        break;
                }

                switch (indexTwo){
                    case 1:
                        CharacterEntity.NosePeak = value.Y;
                        break;
                    case 3:
                        CharacterEntity.NoseBoneCurvness = value.Y;
                        break;
                    case 5:
                        CharacterEntity.NoseBoneTwist = value.Y;
                        break;
                    case 7:
                        CharacterEntity.Eyebrow2 = value.Y;
                        break;
                    case 9:
                        CharacterEntity.CheekSidewaysBoneSize = value.Y;
                        break;
                    case 14:
                        CharacterEntity.JawBoneShape = value.Y;
                        break;
                    case 16:
                        CharacterEntity.ChinBoneLength = value.Y;
                        break;
                    case 18:
                        CharacterEntity.ChinHole = value.Y;
                        break;
                }

                API.SetPedFaceFeature(Game.Player.Character.Handle, indexOne, value.X);
                API.SetPedFaceFeature(Game.Player.Character.Handle, indexTwo, value.Y);
            };
        }

        private static void AddHorizontalGridToMenu(UIMenu menu, string title, string leftText, string rightText,
            int index){
            UIMenuItem uiMenuItem = new(title);
            UIMenuGridPanel uiGrid = new(leftText, rightText, new PointF(.5f, .5f));
            uiMenuItem.AddPanel(uiGrid);
            menu.AddItem(uiMenuItem);
            uiGrid.OnGridPanelChange += (menu, panel, value) => {
                switch (index){
                    case 10:
                        CharacterEntity.CheekBonesWidth = value.X;
                        break;
                    case 11:
                        CharacterEntity.EyeOpening = value.X;
                        break;
                    case 12:
                        CharacterEntity.LipThickness = value.X;
                        break;
                    case 19:
                        CharacterEntity.NeckThickness = value.X;
                        break;
                }

                API.SetPedFaceFeature(Game.Player.Character.Handle, index, value.X);
            };
        }

        private static void AddAppearanceMenu(UIMenu menu, string title, int overlayId, List<dynamic> objects,
            int selectedIndex = 0,
            ColorPanelType panelType = ColorPanelType.Hair){
            UIMenuListItem item = new UIMenuListItem(title, objects, 0); // overlayId == 1 ? 1:0
            UIMenuColorPanel colorPanel = new UIMenuColorPanel("Colour", panelType);
            UIMenuPercentagePanel opacityPanel = new UIMenuPercentagePanel("Opacity", "0%", "100%", 100f);
            item.AddPanel(colorPanel);
            item.AddPanel(opacityPanel);

            float objectOpacity = 100f;
            int objectColor = 0;
            int objectType = 0;
            item.Index = selectedIndex;
            item.OnListChanged += (sender, index) => {
                int objectColor = ((UIMenuColorPanel)sender.Panels[0]).CurrentSelection;
                float objectOpacity = ((UIMenuPercentagePanel)sender.Panels[1]).Percentage / 100;
                API.SetPedHeadOverlay(Game.Player.Character.Handle, overlayId, index, objectOpacity);
                API.SetPedHeadOverlayColor(Game.Player.Character.Handle, overlayId, 1, objectColor, 0);
                objectType = index;

                updateData(overlayId, objectType, objectColor, objectOpacity);
            };

            opacityPanel.OnPercentagePanelChange += (item, panel, value) => {
                float opacity = value / 100;
                API.SetPedHeadOverlay(Game.Player.Character.Handle, overlayId, objectType, opacity);
                API.SetPedHeadOverlayColor(Game.Player.Character.Handle, overlayId, 1, objectColor, 0);
                objectOpacity = opacity;

                updateData(overlayId, objectType, objectColor, objectOpacity);
            };

            colorPanel.OnColorPanelChange += (item, panel, index) => {
                API.SetPedHeadOverlay(Game.Player.Character.Handle, overlayId, objectType, objectOpacity);
                API.SetPedHeadOverlayColor(Game.Player.Character.Handle, overlayId, 1, index, 0);
                objectColor = index;

                updateData(overlayId, objectType, objectColor, objectOpacity);
            };
            menu.AddItem(item);
        }

        private static void updateData(int overlayId, int objectType, int objectColor, float objectOpacity){
            switch (overlayId){ // 1,2,3,4,6,7,8,9,11
                case 1:
                    CharacterEntity.FacialHair = objectType;
                    CharacterEntity.FacialHairColor = objectColor;
                    CharacterEntity.FacialHairOpacity = objectOpacity;
                    break;
                case 2:
                    CharacterEntity.Eyebrows = objectType;
                    CharacterEntity.EyebrowsColor = objectColor;
                    CharacterEntity.EyebrowsOpacity = objectOpacity;
                    break;
                case 3:
                    CharacterEntity.Ageing = objectType;
                    CharacterEntity.AgeingColor = objectColor;
                    CharacterEntity.AgeingOpacity = objectOpacity;
                    break;
                case 4:
                    CharacterEntity.Makeup = objectType;
                    CharacterEntity.MakeupColor = objectColor;
                    CharacterEntity.MakeupOpacity = objectOpacity;
                    break;
                case 6:
                    CharacterEntity.Complexion = objectType;
                    CharacterEntity.ComplexionColor = objectColor;
                    CharacterEntity.ComplexionOpacity = objectOpacity;
                    break;
                case 7:
                    CharacterEntity.SunDamage = objectType;
                    CharacterEntity.SunDamageColor = objectColor;
                    CharacterEntity.SunDamageOpacity = objectOpacity;
                    break;
                case 8:
                    CharacterEntity.Lipstick = objectType;
                    CharacterEntity.LipstickColor = objectColor;
                    CharacterEntity.LipstickOpacity = objectOpacity;
                    break;
                case 9:
                    CharacterEntity.MolesFreckles = objectType;
                    CharacterEntity.MolesFrecklesColor = objectColor;
                    CharacterEntity.MolesFrecklesOpacity = objectOpacity;
                    break;
                case 11:
                    CharacterEntity.BodyBlemishes = objectType;
                    CharacterEntity.BodyBlemishesColor = objectColor;
                    CharacterEntity.BodyBlemishesOpacity = objectOpacity;
                    break;
            }
        }

        private static void EditClotheUI(UIMenu mainMenu){
            UIMenuItem windowsItem = new UIMenuItem("Change your appearance");
            windowsItem.SetRightLabel("»");
            mainMenu.AddItem(windowsItem);

            UIMenuItem back = new("Back to creator");
            back.SetRightLabel("«");

            UIMenu subMenu = new("Change your skin set", "Skin set of character");

            List<dynamic> Draweble = new List<dynamic>
                { "1/10", "2/10", "3/10", "4/10", "5/10", "6/10", "7/10", "8/10", "9/10", "10/10" };

            #region Define of skinset for Male

            SkinSet setM0 = new SkinSet(0, 8, 0, 24, 44, 6);
            SkinSet setM1 = new SkinSet(0, 15, 0, 26, 44, 16);
            SkinSet setM2 = new SkinSet(0, 23, 0, 14, 16, 7);
            SkinSet setM3 = new SkinSet(0, 26, 0, 24, 16, 16);
            SkinSet setM4 = new SkinSet(14, 47, 0, 32, 20, 37);
            SkinSet setM5 = new SkinSet(5, 62, 0, 48, 40, 63);
            SkinSet setM6 = new SkinSet(0, 25, 0, 1, 4, 69);
            SkinSet setM7 = new SkinSet(5, 39, 0, 1, 12, 70);
            SkinSet setM8 = new SkinSet(5, 37, 0, 22, 15, 105);
            SkinSet setM9 = new SkinSet(1, 72, 0, 24, 21, 118);

            #endregion

            #region Define of skinset for Female

            SkinSet setFm0 = new SkinSet(15, 4, 6, 31, 6, 26);
            SkinSet setFm1 = new SkinSet(15, 14, 7, 31, 15, 21);
            SkinSet setFm2 = new SkinSet(5, 16, 6, 31, 30, 35);
            SkinSet setFm3 = new SkinSet(0, 25, 6, 41, 2, 40);
            SkinSet setFm4 = new SkinSet(3, 25, 0, 49, 2, 43);
            SkinSet setFm5 = new SkinSet(15, 32, 0, 49, 2, 46);
            SkinSet setFm6 = new SkinSet(14, 37, 7, 63, 2, 49);
            SkinSet setFm7 = new SkinSet(5, 37, 0, 68, 2, 50);
            SkinSet setFm8 = new SkinSet(15, 47, 3, 70, 2, 79);
            SkinSet setFm9 = new SkinSet(29, 51, 2, 77, 2, 105);

            #endregion

            List<dynamic> skinSetMale = new List<dynamic>
                { setM0, setM1, setM2, setM3, setM4, setM5, setM6, setM7, setM8, setM9 };
            List<dynamic> skinSetFemale = new List<dynamic>
                { setFm0, setFm1, setFm2, setFm3, setFm4, setFm5, setFm6, setFm7, setFm8, setFm9 };

            List<dynamic> skinSets = API.IsPedMale(Game.Player.Character.Handle) ? skinSetMale : skinSetFemale;

            UIMenuListItem uiMenuListItem = new UIMenuListItem("Skin Set", Draweble, 0);
            subMenu.AddItem(uiMenuListItem);

            uiMenuListItem.OnListChanged += (selectedItem, index) => {
                SkinSet skinSet = (SkinSet)skinSets.ToArray()[index];
                SetComp(3, skinSet.Torso);
                SetComp(4, skinSet.Pants);
                SetComp(6, skinSet.Shoes);
                SetComp(7, skinSet.Accessory);
                SetComp(8, skinSet.UnderShirt);
                SetComp(11, skinSet.Torso2);
            };
            subMenu.AddItem(back);
            windowsItem.Activated += (sender, e) => { sender.SwitchTo(subMenu, inheritOldMenuParams: true); };
            back.Activated += (sender, e) => { sender.SwitchTo(mainMenu, inheritOldMenuParams: true); };
        }

        private static void SetComp(int compId, int index){
            API.SetPedComponentVariation(Game.Player.Character.Handle, compId, index, 0, 0);

            switch (compId){
                case 3:
                    CharacterEntity.Torso = index;
                    CharacterEntity.TorsoTexture = 0;
                    break;
                case 4:
                    CharacterEntity.Legs = index;
                    CharacterEntity.LegsTexture = 0;
                    break;
                case 6:
                    CharacterEntity.Foot = index;
                    CharacterEntity.FootTexture = 0;
                    break;
                case 7:
                    CharacterEntity.Accesories = index;
                    CharacterEntity.AccesoriesTexture = 0;
                    break;
                case 8:
                    CharacterEntity.Scarfs = index;
                    CharacterEntity.ScarfsTexture = 0;
                    break;
                case 11:
                    CharacterEntity.Torso2 = index;
                    CharacterEntity.Torso2Texture = 0;
                    break;
            }
        }

        private class SkinSet{
            public int Torso{ get; }
            public int Pants{ get; }
            public int Accessory{ get; }
            public int Shoes{ get; }
            public int UnderShirt{ get; }
            public int Torso2{ get; }

            public SkinSet(int torso, int pants, int accessory, int shoes, int underShirt, int torso2){
                Torso = torso;
                Pants = pants;
                Accessory = accessory;
                Shoes = shoes;
                UnderShirt = underShirt;
                Torso2 = torso2;
            }
        }

        /*
          private static void EditClotheUI(UIMenu mainMenu){
             UIMenuItem windowsItem = new UIMenuItem("Change your appearance");
             windowsItem.SetRightLabel("»");
             mainMenu.AddItem(windowsItem);

             UIMenuItem back = new("Back to creator");
             back.SetRightLabel("«");

             UIMenu subMenu = new("Change your clothes", "");

            // string[] clothingCategoryNames = { "Masks", "Unused (hair)", "Gloves", "Pants", "Bags & Parachutes", "Shoes", "Necklace and Ties", "Under Shirt", "Body Armor", "Decals & Logos", "Shirt & Jackets" };
             string[] clothingCategoryNames = { "Face", "Mask", "Hair", "Torso", "Pants", "Bags & Parachutes", "Shoes", "Accessory", "Under Shirt", "Kevlar", "Badge", "Torso" };

            // int[] blockedPants ={2, 33, 11, 34,38,39,40,44,56,57,59,67, 72, 74,77, 84, 85, 87,92,93, 95, 97, 106, 107, 108, 109,110, 111};
             Blocked pants = new Blocked(4,105, new[]{ 2, 33, 11, 34, 38, 39, 40, 44, 56, 57, 59, 67, 72, 74, 77, 84, 85, 87, 92, 93, 95, 97, 106, 107, 108, 109, 110, 111 });

             int[] allowedDrawable ={3, 4, 6, 7, 8, 11 };
             List<dynamic> drawed = new List<dynamic>();
             foreach (int allow in allowedDrawable){
                 int maxDrawables = API.GetNumberOfPedDrawableVariations(playerPid, allow);


                 for (var i = 0; i < maxDrawables; i++){
                    /* if (allow == pants.id){
                         if (maxDrawables >= pants.maxRender){
                             continue;
                         }

                         if (!Utils.IsNumberInArray(pants.blockeds, i)){
                             Debug.WriteLine($"{i}/{pants.maxRender}");
                             drawed.Add($"{i}/{pants.maxRender}");
                         }
                             //if (API.IsPedComponentVariationValid(playerPid, allow, i, 0))

                     }else * /
                         if (API.IsPedComponentVariationValid(playerPid, allow, i, 0))
                             drawed.Add($"{i}/{maxDrawables}");
                 }

                 UIMenuListItem uiMenuListItem = new UIMenuListItem($"Clothes {allow}:{clothingCategoryNames[allow]}", drawed, 0);
                 UIMenuColorPanel uiMenuColorPanel = new UIMenuColorPanel("Colour", ColorPanelType.Hair);
                 uiMenuListItem.AddPanel(uiMenuColorPanel);
                 subMenu.AddItem(uiMenuListItem);

                 uiMenuListItem.OnListChanged += (SelectedItem, Index) => {
                     int color = ((UIMenuColorPanel)SelectedItem.Panels[0]).CurrentSelection;
                     if (allow == pants.id) {
                         if (!Utils.IsNumberInArray(pants.blockeds, Index))
                             API.SetPedComponentVariation(playerPid, allow, Index, color, 0);
                     } else
                         API.SetPedComponentVariation(playerPid, allow, Index, color, 0);


                 };
             }
             subMenu.AddItem(back);
             windowsItem.Activated += (sender, e) => { sender.SwitchTo(subMenu, inheritOldMenuParams: true); };
             back.Activated += (sender, e) => { sender.SwitchTo(mainMenu, inheritOldMenuParams: true); };
         }

         class Blocked {
             public int id{ get; set; }
             public int maxRender{ get; set; }
             public int[] blockeds{ get; set; }


             public Blocked(int _id, int _maxRender, int[] _blockeds){
                 id = _id;
                 maxRender = _maxRender;
                 blockeds = _blockeds;

             }
         }
         */

        private static void ByParentsCreatorUI(UIMenu mainMenu){
            double[] amount ={ 0, 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1.0 };

            UIMenuItem windowsItem = new UIMenuItem("Change your parents", "You can change your face by your parents.");
            windowsItem.SetRightLabel("»");
            mainMenu.AddItem(windowsItem);

            UIMenuItem back = new("Back to creator");
            back.SetRightLabel("«");

            UIMenu windowSubmenu = new UIMenu("Parents", "Select your parents"){
                BuildingAnimation = MenuBuildingAnimation.NONE,
                EnableAnimation = false,
                MaxItemsOnScreen = 7,
                ScrollingType = ScrollingType.ENDLESS
            };

            UIMenuHeritageWindow heritageWindow = new(0, 0);
            windowSubmenu.AddWindow(heritageWindow);
            List<dynamic> momFaces = new List<dynamic>{
                "Hannah", "Audrey", "Jasmine", "Giselle", "Amelia", "Isabella", "Zoe", "Ava", "Camilla", "Violet",
                "Sophia", "Eveline", "Nicole", "Ashley", "Grace", "Brianna", "Natalie", "Olivia", "Elizabeth",
                "Charlotte", "Emma", "Misty"
            };
            List<dynamic> dadFaces = new List<dynamic>{
                "Benjamin", "Daniel", "Joshua", "Noah", "Andrew", "Joan", "Alex", "Isaac", "Evan", "Ethan", "Vincent",
                "Angel", "Diego", "Adrian", "Gabriel", "Michael", "Santiago", "Kevin", "Louis", "Samuel", "Anthony",
                "Claude", "Niko", "John"
            };
            UIMenuListItem mom = new("Mother", momFaces, 0);
            UIMenuListItem dad = new("Father", dadFaces, 0);
            UIMenuSliderItem ShapeMixItem = new("Shape Mix Slider", "This is Useful on Shape mix", 10, 1, 5, true);
            UIMenuSliderItem SkinMixItem = new("Skin Mix Slider", "This is Useful on Skin mix", 10, 1, 5, true);
            windowSubmenu.AddItem(mom);
            windowSubmenu.AddItem(dad);
            windowSubmenu.AddItem(ShapeMixItem);
            windowSubmenu.AddItem(SkinMixItem);

            windowSubmenu.AddItem(back);
            windowSubmenu.OnListChange += (sender, item, index) => {
                if (item == mom)
                    CharacterEntity.Mother = index;
                else if (item == dad)
                    CharacterEntity.Father = index;


                heritageWindow.Index(CharacterEntity.Mother, CharacterEntity.Father);
                API.SetPedHeadBlendData(Game.Player.Character.Handle,
                                        CharacterEntity.Mother, CharacterEntity.Father, 0,
                                        CharacterEntity.Mother, CharacterEntity.Father, 0,
                                        CharacterEntity.ParentFaceShapePercent,
                                        CharacterEntity.ParentSkinTonePercent, .5f, true);
            };
            windowSubmenu.OnSliderChange += (sender, item, index) => {
                if (item == ShapeMixItem){
                    CharacterEntity.ParentFaceShapePercent = (float)amount[index];
                }
                else if (item == SkinMixItem){
                    CharacterEntity.ParentSkinTonePercent = (float)amount[index];
                }

                API.SetPedHeadBlendData(Game.Player.Character.Handle,
                                        CharacterEntity.Mother, CharacterEntity.Father, 0,
                                        CharacterEntity.Mother, CharacterEntity.Father, 0,
                                        CharacterEntity.ParentFaceShapePercent,
                                        CharacterEntity.ParentSkinTonePercent, .5f, true);
            };
            windowsItem.Activated += (sender, e) => { sender.SwitchTo(windowSubmenu, inheritOldMenuParams: true); };

            back.Activated += (sender, e) => { sender.SwitchTo(mainMenu, inheritOldMenuParams: true); };
        }

        private static async void ChangeSex(int sexIndex){
            CharacterEntity.Sex = (short)sexIndex;
            string sexHash = sexIndex switch{
                0 => "mp_m_freemode_01",
                1 => "mp_f_freemode_01",
                _ => null
            };

            uint intHash = (uint)API.GetHashKey(sexHash);
            API.RequestModel(intHash);

            while (!API.HasModelLoaded(intHash))
                await BaseScript.Delay(0);

            API.SetPlayerModel(Game.Player.Handle, intHash);
            API.SetPedDefaultComponentVariation(Game.Player.Character.Handle);
            API.SetModelAsNoLongerNeeded(intHash);

            API.SetPedCanHeadIk(Game.Player.Character.Handle, true);
            API.TaskStandStill(Game.Player.Character.Handle, int.MaxValue);
            Game.Player.Character.IsPositionFrozen = true;
            Game.Player.Character.IsInvincible = true;

            API.SetPedHeadBlendData(Game.Player.Character.Handle, 0, 0, 0, 0, 0, 0, 0, 0f, 0f, false);
        }
    }
}