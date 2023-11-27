using System.Threading.Tasks;
using CitizenFX.Core;
using Dapper;
using MySqlConnector;
using Server.Database;
using Server.Database.Entities;

namespace Server.Services{
    public class CharacterCreatorService{
        private async Task<bool> CheckIfCharacterExist(string name){
            bool exists = false;
            await using MySqlConnection connection = DatabaseConnector.GetConnection();
            await connection.OpenAsync();

            const string checkExistenceQuery = "SELECT COUNT(*) FROM characters WHERE name = @Name";
            int recordCount = await connection.QueryFirstOrDefaultAsync<int>(checkExistenceQuery, new{ Name = name });

            if (recordCount > 0) exists = true;
            await connection.CloseAsync();
            return exists;
        }

        private async Task<Character> GetCharacter(string name){
            await using MySqlConnection dbConnection = DatabaseConnector.GetConnection();
            await dbConnection.OpenAsync();

            const string SelectQuery = @"SELECT * FROM characters WHERE Name = @Name";
            Character character =
                await dbConnection.QueryFirstOrDefaultAsync<Character>(SelectQuery, new{ Name = name });

            await dbConnection.CloseAsync();
            return character;
        }

        public async void SaveCharacterData([FromSource] Player player, string data){
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

        public async void LoadCharacterData([FromSource] Player player, int playerPed){
            await using MySqlConnection dbConnection = DatabaseConnector.GetConnection();
            await dbConnection.OpenAsync();
            string name = player.Name;
            const string SelectQuery = "SELECT * FROM characters WHERE name = @Name";
            Character character = await dbConnection.QueryFirstAsync<Character>(SelectQuery, new{ Name = name });
            await dbConnection.CloseAsync();

            player.TriggerEvent("player:character:data", character.SerializeToJson());
        }

        public async void Loader([FromSource] Player player){
            bool tasks = await CheckIfCharacterExist(player.Name);
            Character character = await GetCharacter(player.Name);
            VGPlayer vgPlayer = PlayerService.GetVgPlayerByPlayer(player);
            if (tasks)
                player.TriggerEvent("player:spawn:to:world", character.Sex, vgPlayer.PosX, vgPlayer.PosY,
                                    vgPlayer.PosZ, 110f);
            else
                player.TriggerEvent("player:spawn:to:creator");
        }
    }
}