using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using Dapper;
using MySqlConnector;
using Server.Database;
using Server.Database.Entities;

namespace Server.Services{
    public class CharacterCreatorService{
        private readonly Dictionary<string, Character> characterData = new Dictionary<string, Character>();

        private async Task<bool> CheckIfCharacterExist(string name){
            bool exists = false;
            try{
                await using MySqlConnection connection = Connector.GetConnection();
                await connection.OpenAsync();

                const string checkExistenceQuery = "SELECT COUNT(*) FROM characters WHERE name = @Name";
                int recordCount =
                    await connection.QueryFirstOrDefaultAsync<int>(checkExistenceQuery, new{ Name = name });

                if (recordCount > 0)
                    exists = true;

                Debug.WriteLine($"CheckIfCharacterExist - {(exists ? "Exists" : "Is not")} in database!");

                await connection.CloseAsync();
            }
            catch (Exception ex){
                Debug.WriteLine("An error occurred in CheckIfCharacterExist: " + ex.Message);
            }

            return exists;
        }

        public async Task<Character> GetCharacter(string name){
            await using MySqlConnection dbConnection = Connector.GetConnection();
            await dbConnection.OpenAsync();

            const string SelectQuery = "SELECT * FROM characters WHERE Name = @Name";
            Character character =
                await dbConnection.QueryFirstOrDefaultAsync<Character>(SelectQuery, new{ Name = name });

            await dbConnection.CloseAsync();
            return character;
        }

        public async void SaveCharacterData([FromSource] Player player){
            await using MySqlConnection dbConnection = Connector.GetConnection();
            await dbConnection.OpenAsync();

            string name = player.Name;
            characterData.TryGetValue(name, out Character character);
            if (character == null)
                throw new NullReferenceException("Character data is null in dictionary.");

            VGPlayer vgPlayer = PlayerService.GetVgPlayer(Utils.GetLicense(player));
            character.AccId = vgPlayer.Id;

            string InsertQuery = @"INSERT INTO characters
                                       (acc_id, name, sex, first_face_shape, second_face_shape, first_skin_tone, second_skin_tone, 
                                        parent_face_shape_percent, parent_skin_tone_percent, nose_width, nose_peak, nose_length, 
                                        nose_bone_curvness, nose_tip, nose_bone_twist, eyebrow, eyebrow2, cheek_bones, 
                                        cheek_bones_width,cheek_sideways_bone_size, eye_opening, lip_thickness, jaw_bone_width, jaw_bone_shape, 
                                        chin_bone, chin_bone_length, chin_bone_shape, chin_hole, neck_thickness, hair_type, 
                                        hair_color, torso, torso_texture, legs, legs_texture, foot, foot_texture, scarfs, 
                                        scarfs_texture, accessories, accessories_texture, torso2, torso2_texture)
                                    VALUES
                                        (@AccId, @Name, @Sex, @FirstFaceShape, @SecondFaceShape, @FirstSkinTone, @SecondSkinTone, 
                                        @ParentFaceShapePercent, @ParentSkinTonePercent, @NoseWidth, @NosePeak, @NoseLength, 
                                        @NoseBoneCurvness, @NoseTip, @NoseBoneTwist, @Eyebrow, @Eyebrow2, @CheekBones, 
                                        @CheekBonesWidth,@CheekSidewaysBoneSize, @EyeOpening, @LipThickness, @JawBoneWidth, @JawBoneShape, 
                                        @ChinBone, @ChinBoneLength, @ChinBoneShape, @ChinHole, @NeckThickness, @HairType, 
                                        @HairColor, @Torso, @TorsoTexture, @Legs, @LegsTexture, @Foot, @FootTexture, @Scarfs, 
                                        @ScarfsTexture, @Accessories, @AccessoriesTexture, @Torso2, @Torso2Texture)";

            characterData.Remove(name);
            await dbConnection.ExecuteAsync(InsertQuery, character);
            await dbConnection.CloseAsync();
        }


        public async void LoadCharacterData([FromSource] Player player){
            await using MySqlConnection dbConnection = Connector.GetConnection();
            await dbConnection.OpenAsync();
            string name = player.Name;
            //string SelectQuery = "SELECT * FROM characters WHERE name = @Name";
            string SelectQuery =
                @"SELECT `id` AS Id,`acc_id` AS AccId,`name` AS Name,`sex` AS Sex,`first_face_shape` AS FirstFaceShape,`second_face_shape` AS SecondFaceShape,`first_skin_tone` AS FirstSkinTone,`second_skin_tone` AS SecondSkinTone,`parent_face_shape_percent` AS ParentFaceShapePercent,`parent_skin_tone_percent` AS ParentSkinTonePercent,`nose_width` AS NoseWidth,`nose_peak` AS NosePeak,`nose_length` AS NoseLength,`nose_bone_curvness` AS NoseBoneCurvness,`nose_tip` AS NoseTip,`nose_bone_twist` AS NoseBoneTwist,`eyebrow` AS Eyebrow,`eyebrow2` AS Eyebrow2,`cheek_bones` AS CheekBones,`cheek_bones_width` AS CheekBonesWidth,`cheek_sideways_bone_size` AS CheekSidewaysBoneSize,`eye_opening` AS EyeOpening,`lip_thickness` AS LipThickness,`jaw_bone_width` AS JawBoneWidth,`jaw_bone_shape` AS JawBoneShape,`chin_bone` AS ChinBone,`chin_bone_length` AS ChinBoneLength,`chin_bone_shape` AS ChinBoneShape,`chin_hole` AS ChinHole,`neck_thickness` AS NeckThickness,`hair_type` AS HairType,`hair_color` AS HairColor,`torso` AS Torso,`torso_texture` AS TorsoTexture,`legs` AS Legs,`legs_texture` AS LegsTexture,`foot` AS Foot,`foot_texture` AS FootTexture,`scarfs` AS Scarfs,`scarfs_texture` AS ScarfsTexture,`accessories` AS Accessories,`accessories_texture` AS AccessoriesTexture,`torso2` AS Torso2,`torso2_texture` AS Torso2Texture    FROM characters WHERE name = @Name";

            Character character = await dbConnection.QueryFirstAsync<Character>(SelectQuery, new{ Name = name });
            await dbConnection.CloseAsync();

            int playerPed = player.Character.Handle;
            API.SetPedHeadBlendData(playerPed, character.FirstFaceShape, character.SecondFaceShape, 0,
                                    character.FirstSkinTone, character.SecondSkinTone, 0,
                                    character.ParentFaceShapePercent, character.ParentSkinTonePercent, 0, true);

            API.SetPedFaceFeature(playerPed, 0, character.NoseWidth);
            API.SetPedFaceFeature(playerPed, 1, character.NosePeak);
            API.SetPedFaceFeature(playerPed, 2, character.NoseLength);
            API.SetPedFaceFeature(playerPed, 3, character.NoseBoneCurvness);
            API.SetPedFaceFeature(playerPed, 4, character.NoseTip);
            API.SetPedFaceFeature(playerPed, 5, character.NoseBoneTwist);
            API.SetPedFaceFeature(playerPed, 6, character.Eyebrow);
            API.SetPedFaceFeature(playerPed, 7, character.Eyebrow2);
            API.SetPedFaceFeature(playerPed, 8, character.CheekBones);
            API.SetPedFaceFeature(playerPed, 9, character.CheekSidewaysBoneSize);
            API.SetPedFaceFeature(playerPed, 10, character.CheekBonesWidth);
            API.SetPedFaceFeature(playerPed, 11, character.EyeOpening);
            API.SetPedFaceFeature(playerPed, 12, character.LipThickness);
            API.SetPedFaceFeature(playerPed, 13, character.JawBoneWidth);
            API.SetPedFaceFeature(playerPed, 14, character.JawBoneShape);
            API.SetPedFaceFeature(playerPed, 15, character.ChinBone);
            API.SetPedFaceFeature(playerPed, 16, character.ChinBoneLength);
            API.SetPedFaceFeature(playerPed, 17, character.ChinBoneShape);
            API.SetPedFaceFeature(playerPed, 18, character.ChinHole);
            API.SetPedFaceFeature(playerPed, 19, character.NeckThickness);
            API.SetPedComponentVariation(playerPed, 2, character.HairType, 0, 2);
            API.SetPedHairColor(playerPed, character.HairColor, 0);
            API.SetPedComponentVariation(playerPed, 3, character.Torso, character.TorsoTexture, 0);
            API.SetPedComponentVariation(playerPed, 4, character.Legs, character.LegsTexture, 0);
            API.SetPedComponentVariation(playerPed, 6, character.Foot, character.FootTexture, 0);
            API.SetPedComponentVariation(playerPed, 7, character.Scarfs, character.ScarfsTexture, 0);
            API.SetPedComponentVariation(playerPed, 8, character.Accessories, character.AccessoriesTexture, 0);
            API.SetPedComponentVariation(playerPed, 11, character.Torso2, character.Torso2Texture, 0);

            Debug.WriteLine("Load is completed!");
            Debug.WriteLine(character.ToString());
            BaseScript.TriggerClientEvent("player:spawn:to:world2");
        }

        public void ClientDataBlend([FromSource] Player player, short sex, int firstFaceShape, int secondFaceShape,
            int firstSkinTone, int secondSkinTone, float parentFaceShapePercent, float parentSkinTonePercent){
            if (!characterData.ContainsKey(player.Name))
                characterData.Add(player.Name, new Character(player.Name));

            characterData.TryGetValue(player.Name, out Character character);
            if (character == null)
                throw new NullReferenceException("Character data is null in dictionary.");

            character.Sex = sex;
            character.FirstFaceShape = firstFaceShape;
            character.SecondFaceShape = secondFaceShape;
            character.FirstSkinTone = firstSkinTone;
            character.SecondSkinTone = secondSkinTone;
            character.ParentFaceShapePercent = parentFaceShapePercent;
            character.ParentSkinTonePercent = parentSkinTonePercent;
        }

        public void ClientDataFaceFeature([FromSource] Player player, float noseWidth, float nosePeak, float noseLength,
            float noseBoneCurvness, float noseTip, float noseBoneTwist, float eyeBrow, float eyeBrow2, float cheekBones,
            float cheekBonesWidth, float cheekSidewaysBoneSize){
            characterData.TryGetValue(player.Name, out Character character);
            if (character == null)
                throw new NullReferenceException("Character data is null in dictionary.");

            character.NoseWidth = noseWidth;
            character.NosePeak = nosePeak;
            character.NoseLength = noseLength;
            character.NoseBoneCurvness = noseBoneCurvness;
            character.NoseTip = noseTip;
            character.NoseBoneTwist = noseBoneTwist;
            character.Eyebrow = eyeBrow;
            character.Eyebrow2 = eyeBrow2;
            character.CheekBones = cheekBones;
            character.CheekBonesWidth = cheekBonesWidth;
            character.CheekSidewaysBoneSize = cheekSidewaysBoneSize;
        }

        public void ClientDataFaceFeature2([FromSource] Player player, float eyeOpening, float lipThickness,
            float jawBoneWidth,
            float jawBoneShape, float chinBone, float chinBoneLength, float chinBoneShape, float chinHole,
            float neckThickness){
            characterData.TryGetValue(player.Name, out Character character);
            if (character == null)
                throw new NullReferenceException("Character data is null in dictionary.");

            character.EyeOpening = eyeOpening;
            character.LipThickness = lipThickness;
            character.JawBoneWidth = jawBoneWidth;
            character.JawBoneShape = jawBoneShape;
            character.ChinBone = chinBone;
            character.ChinBoneLength = chinBoneLength;
            character.ChinBoneShape = chinBoneShape;
            character.ChinHole = chinHole;
            character.NeckThickness = neckThickness;
        }

        public void ClientDataDrawable([FromSource] Player player, int hairType, int hairColor, int torso,
            int torsoTexture,
            int legs, int legsTexture, int foot, int footTexture, int scarfs, int scarfsTexture, int accessories,
            int accessoriesTexture, int torso2, int torso2Texture){
            characterData.TryGetValue(player.Name, out Character character);
            if (character == null)
                throw new NullReferenceException("Character data is null in dictionary.");

            character.HairType = hairType;
            character.HairColor = hairColor;
            character.Torso = torso;
            character.TorsoTexture = torsoTexture;
            character.Legs = legs;
            character.LegsTexture = legsTexture;
            character.Foot = foot;
            character.FootTexture = footTexture;
            character.Scarfs = scarfs;
            character.ScarfsTexture = scarfsTexture;
            character.Accessories = accessories;
            character.AccessoriesTexture = accessoriesTexture;
            character.Torso2 = torso2;
            character.Torso2Texture = torso2Texture;
        }

        public async void Loader([FromSource] Player player){
            var tasks = await CheckIfCharacterExist(player.Name);
            var character = await GetCharacter(player.Name);
            var vgPlayer = PlayerService.GetVgPlayer(Utils.GetLicense(player));
            if (tasks){
                //Character character = await LoadCharacterData(player);
                BaseScript.TriggerClientEvent("player:spawn:to:world", character.Sex, vgPlayer.PosX, vgPlayer.PosY,
                                              vgPlayer.PosZ, 110f);
                //Todo: Load Data
            }
            else
                BaseScript.TriggerClientEvent("player:spawn:to:creator");
        }

        public void DataLoader(Player player, string json){
            Debug.WriteLine($"Player: {player.Name},Data: {json}");
        }
    }
}