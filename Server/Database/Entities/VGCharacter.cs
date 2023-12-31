namespace Server.Database.Entities{
    public class VGCharacter : ITable {
        
        public string Model { get; set; }
        
        public string GetTableName(){
            return "characters";
        }
    }
}