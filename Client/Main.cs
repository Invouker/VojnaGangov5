using System;
using CitizenFX.Core;

namespace Client
{
    public class Main : BaseScript
    {
        public Main()
        {
            SendMessage("VG5C-Load");
            TriggerServerEvent("PlayerVIPSystemLoad");
        }
        /*
        private void OnPlayerVIPSystemLoad(){
            if(IsPlayerVip(Game.PlayerPed.Handle))
            {
                
                SendMessage("VIP Ano");
            }else
            {
                
                SendMessage("VIP Nie");
            }
        }

        private bool IsPlayerVip(int playerName)
        {
            bool isVip = false;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                DateTime currentTime = DateTime.Now;
                string query = "SELECT COUNT(*) FROM your_table_name WHERE name = @PlayerName AND tarif > 0 AND date <= @CurrentTime";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@PlayerName", playerName);
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
*/
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
