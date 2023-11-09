using CitizenFX.Core;
using System;
using MySqlConnector;
//using System.Collections.Generic;
using static CitizenFX.Core.Native.API;
// ReSharper disable All

namespace Server
{
    public class Main : BaseScript
    {
        private string connectionString;
        public Main()
        {
            SendMessage("VG5Server-Load");
            connectionString = GetConvar("mysql_connection_string", "default_connection_string_if_not_set");
            Debug.WriteLine(connectionString);
            EventHandlers["PlayerVIPSystemLoad"] += new Action<Player>(OnPlayerVIPSystemLoad);
        }
        
        private void OnPlayerVIPSystemLoad([FromSource] Player player)
        {
            Debug.WriteLine($"Player {player.Handle} true");
            if(IsPlayerVip(player.Handle))
            {
                Debug.WriteLine($"Player {player.Handle} true");
                SendMessage("VIP Ano");
            }else
            {
                Debug.WriteLine($"Player {player.Handle} false");
                SendMessage("VIP Nie");
            }
        }

        private bool IsPlayerVip(string playerName)
        {
            bool isVip = false;
            string menoHraca = GetPlayerName(playerName);
            Debug.WriteLine($"Hrac {menoHraca} skuska");
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                DateTime currentTime = DateTime.Now;
                string query = "SELECT COUNT(*) FROM accounts_vip WHERE name = @PlayerName AND tariff > 0 AND date <= @CurrentTime";
                Debug.WriteLine(query);
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@PlayerName", menoHraca);
                    cmd.Parameters.AddWithValue("@CurrentTime", currentTime);
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    if (count > 0)
                    {
                        isVip = true;
                    }
                }
            }
            return isVip;
        }
        
        private void SendMessage(string text)
        {
            TriggerEvent("chat:addMessage", new
            {
                color = new[] { 250, 250, 250 },
                multiline = false,
                args = new[] { text }
            });
        }
    }
}