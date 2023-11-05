using CitizenFX.Core;
using CitizenFX.Core.Native;
using System;
using System.Collections.Generic;
using System.Numerics;
using static CitizenFX.Core.Native.API;
//---MySLQL


namespace Client
{
    public class Main : BaseScript
    {
        private int outValue;
        public Main()
        {
            EventHandlers["onClientMapStart"] += new Action(OnClientMapStart);
            EventHandlers["sendGiveWeapon"] += new Action(GiveWeapon);
        }
        public void SendMessage(string text)
        {
            TriggerEvent("chat:addMessage", new
            {
                color = new[] { 250, 250, 250 },
                multiline = false,
                args = new[] { text }
            }); ;
        }
        private void notify(string text)
        {
            SetNotificationTextEntry("STRING");
            AddTextComponentString(text);
            DrawNotification(true, false);
        }
        private async void OnClientMapStart()
        {
            while (Game.PlayerPed == null || !Game.PlayerPed.Exists())
            {
                await Delay(0);

            }
            SendMessage("OnClientMapStart");
            SetEntityCoords(Game.PlayerPed.Handle, 1786.84f, 3316.642f, 41.52966f, true, true, true, true);
            TriggerServerEvent("sendGiveWeapon");
        }

        private void GiveWeapon()
        {
            int playerid = PlayerId();
            TriggerEvent("chat:addMessage", new
            {
                color = new[] { 250, 250, 250 },
                multiline = false,
                args = new[] { "GiveWeaponClient" }
            });
            foreach (WeaponHash weapon in Enum.GetValues(typeof(WeaponHash)))
            {
                if (IsWeaponValid((uint)weapon))
                {
                    GiveWeaponToPed(playerid, (uint)weapon, 999, false, true);
                }
            }
        }
    }
}
