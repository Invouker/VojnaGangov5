using ScaleformUI.Elements;
using ScaleformUI.Menu;
using Pointer = System.Drawing.PointF;
using static CitizenFX.Core.Native.API;

namespace Client.ScaleformUI{
    public class CharacterCreatorUI{
        public static void Interact(int id){
            if (id == 1)
                createUI();
        }

        private static UIMenu createUI(){
            long _titledui = CreateDui("https://i.imgur.com/3yrFYbF.gif", 288, 130);
            long txd = 0L;
            CreateRuntimeTextureFromDuiHandle(txd, "bannerbackground", GetDuiHandle(_titledui));
            //DrawingPointF point = new DrawingPointF(20f, 20f);

            UIMenu menu = new UIMenu("ScaleformUI", "ScaleformUI ~o~SHOWCASE", new Pointer(20f, 20f), false, false, 1f);
            //menu.Offset = new PointF(10, 10);
            var menuItem = new UIMenuItem("string text, uint descriptionHash", "test", SColor.Gold, SColor.White);
            menu.AddItem(menuItem);

            menu.Visible = true;
            return menu;
        }
    }
}