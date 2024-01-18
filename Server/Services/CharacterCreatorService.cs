using System.Threading.Tasks;
using Dapper;
using MySqlConnector;
using Server.Database;
using Server.Database.Entities.Player;

namespace Server.Services{
    public class CharacterCreatorService:IService{
        public CharacterCreatorService(){
             EventDispatcher.Mount("player:data:character",
                                          new Action<Player, string>(SaveCharacterData));
             EventDispatcher.Mount("player:spawn:to:world:server",
                                          new Action<Player, int>(LoadCharacterData));
        }


        public static async Task<bool> CheckIfCharacterExist(string name){
            bool exists = false;
            await using MySqlConnection connection = DatabaseConnector.GetConnection();
            await connection.OpenAsync();

            const string checkExistenceQuery = "SELECT COUNT(*) FROM characters WHERE name = @Name";
            int recordCount = await connection.QueryFirstOrDefaultAsync<int>(checkExistenceQuery, new{ Name = name });

            if (recordCount > 0) exists = true;
            await connection.CloseAsync();
            return exists;
        }

        public static async Task<Character> GetCharacter(string name){
            await using MySqlConnection dbConnection = DatabaseConnector.GetConnection();
            await dbConnection.OpenAsync();

            const string SelectQuery = @"SELECT * FROM characters WHERE Name = @Name";
            Character character =
                await dbConnection.QueryFirstOrDefaultAsync<Character>(SelectQuery, new{ Name = name });

            await dbConnection.CloseAsync();
            return character;
        }

        public static async void SaveCharacterData([FromSource] Player player, string data){
            await using MySqlConnection dbConnection = DatabaseConnector.GetConnection();
            await dbConnection.OpenAsync();

            string name = player.Name;

            Character character = Character.DeserializeFromJson(data);
            VGPlayer vgPlayer = PlayerService.GetVgPlayerByPlayer(player);
            character.AccId = vgPlayer.Id;
            character.Name = name;

            const string InsertQuery = """
                                       INSERT INTO characters (
                                                         Id, AccId, Name, Sex,
                                                         Mother, Father, ParentFaceShapePercent, ParentSkinTonePercent,
                                                         NoseWidth, NosePeak, NoseLength, NoseBoneCurvness, NoseTip, NoseBoneTwist, Eyebrow, Eyebrow2, CheekBones, CheekBonesWidth, CheekSidewaysBoneSize,
                                                         EyeOpening, LipThickness, JawBoneWidth, JawBoneShape, ChinBone, ChinBoneLength, ChinBoneShape, ChinHole, NeckThickness, FacialHair, FacialHairColor,
                                                         FacialHairOpacity, Eyebrows, EyebrowsColor, EyebrowsOpacity, Ageing, AgeingColor, AgeingOpacity, Makeup, MakeupColor, MakeupOpacity, Complexion,
                                                         ComplexionColor, ComplexionOpacity, SunDamage, SunDamageColor, SunDamageOpacity, Lipstick, LipstickColor, LipstickOpacity, MolesFreckles,
                                                         MolesFrecklesColor, MolesFrecklesOpacity, BodyBlemishes, BodyBlemishesColor, BodyBlemishesOpacity, HairType, HairColor, Torso, TorsoTexture, Legs,
                                                         LegsTexture, Foot, FootTexture, Scarfs, ScarfsTexture, Accesories, AccesoriesTexture, Torso2, Torso2Texture)
                                                       VALUES (
                                                         @Id, @AccId, @Name, @Sex,
                                                         @Mother, @Father, @ParentFaceShapePercent, @ParentSkinTonePercent,
                                                         @NoseWidth, @NosePeak, @NoseLength, @NoseBoneCurvness, @NoseTip, @NoseBoneTwist, @Eyebrow, @Eyebrow2, @CheekBones, @CheekBonesWidth, @CheekSidewaysBoneSize,
                                                         @EyeOpening, @LipThickness, @JawBoneWidth, @JawBoneShape, @ChinBone, @ChinBoneLength, @ChinBoneShape, @ChinHole, @NeckThickness, @FacialHair, @FacialHairColor,
                                                         @FacialHairOpacity, @Eyebrows, @EyebrowsColor, @EyebrowsOpacity, @Ageing, @AgeingColor, @AgeingOpacity, @Makeup, @MakeupColor, @MakeupOpacity, @Complexion,
                                                         @ComplexionColor, @ComplexionOpacity, @SunDamage, @SunDamageColor, @SunDamageOpacity, @Lipstick, @LipstickColor, @LipstickOpacity, @MolesFreckles,
                                                         @MolesFrecklesColor, @MolesFrecklesOpacity, @BodyBlemishes, @BodyBlemishesColor, @BodyBlemishesOpacity, @HairType, @HairColor, @Torso, @TorsoTexture, @Legs,
                                                         @LegsTexture, @Foot, @FootTexture, @Scarfs, @ScarfsTexture, @Accesories, @AccesoriesTexture, @Torso2, @Torso2Texture)
                                       """;

            await dbConnection.ExecuteAsync(InsertQuery, character);
            await dbConnection.CloseAsync();
        }

        public static async void LoadCharacterData([FromSource] Player player, int playerPed){
            await using MySqlConnection dbConnection = DatabaseConnector.GetConnection();
            await dbConnection.OpenAsync();
            string name = player.Name;
            const string SelectQuery = "SELECT * FROM characters WHERE name = @Name";
            Character character = await dbConnection.QueryFirstAsync<Character>(SelectQuery, new{ Name = name });
            await dbConnection.CloseAsync();

            EventDispatcher.Send(player, "player:character:data", character.SerializeToJson());
        }

        public void Init() { }
    }
}