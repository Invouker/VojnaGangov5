using System;
using System.Collections.Generic;
using CitizenFX.Core;
using Dapper;
using MySqlConnector;
using Server.Database;
using Server.Database.Entities;
using Server.Services;

namespace Server.Entities;

public class CharacterCreatorService{
    private Dictionary<string, Character> characterData = new Dictionary<string, Character>();

    public async void SaveCharacterData([FromSource] Player player){
        await using MySqlConnection dbConnection = Connector.GetConnection();
        await dbConnection.OpenAsync();

        string name = player.Name;
        characterData.TryGetValue(name, out Character character);
        if (character == null)
            throw new NullReferenceException("Character data is null in dictionary.");

        VGPlayer vgPlayer = PlayerService.getVGPlayer(name);
        character.Id = vgPlayer.Id;

        string InsertQuery = @"INSERT INTO characters
                                   (acc_id, name, first_face_shape, second_face_shape, first_skin_tone, second_skin_tone, 
                                    parent_face_shape_percent, parent_skin_tone_percent, nose_width, nose_peak, nose_length, 
                                    nose_bone_curvness, nose_tip, nose_bone_twist, eyebrow, eyebrow2, cheek_bones, 
                                    cheek_bones_width,cheek_sideways_bone_size, eye_opening, lip_thickness, jaw_bone_width, jaw_bone_shape, 
                                    chin_bone, chin_bone_length, chin_bone_shape, chin_hole, neck_thickness, hair_type, 
                                    hair_color, torso, torso_texture, legs, legs_texture, foot, foot_texture, scarfs, 
                                    scarfs_texture, accessories, accessories_texture, torso2, torso2_texture)
                                VALUES
                                    (@AccId, @Name, @FirstFaceShape, @SecondFaceShape, @FirstSkinTone, @SecondSkinTone, 
                                    @ParentFaceShapePercent, @ParentSkinTonePercent, @NoseWidth, @NosePeak, @NoseLength, 
                                    @NoseBoneCurvness, @NoseTip, @NoseBoneTwist, @Eyebrow, @Eyebrow2, @CheekBones, 
                                    @CheekBonesWidth,@CheekSidewaysBoneSize, @EyeOpening, @LipThickness, @JawBoneWidth, @JawBoneShape, 
                                    @ChinBone, @ChinBoneLength, @ChinBoneShape, @ChinHole, @NeckThickness, @HairType, 
                                    @HairColor, @Torso, @TorsoTexture, @Legs, @LegsTexture, @Foot, @FootTexture, @Scarfs, 
                                    @ScarfsTexture, @Accessories, @AccessoriesTexture, @Torso2, @Torso2Texture)";

        await dbConnection.ExecuteAsync(InsertQuery, character);
        await dbConnection.CloseAsync();
    }

    public void ClientDataBlend([FromSource] Player player, int firstFaceShape, int secondFaceShape, int firstSkinTone,
        int secondSkinTone, float parentFaceShapePercent, float parentSkinTonePercent){
        if (!characterData.ContainsKey(player.Name))
            characterData.Add(player.Name, new Character(player.Name));

        characterData.TryGetValue(player.Name, out Character character);
        if (character == null)
            throw new NullReferenceException("Character data is null in dictionary.");

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
        float jawBoneWidth, float jawBoneShape, float chinBone, float chinBoneLength, float chinBoneShape,
        float chinHole, float neckThickness){
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

    public void ClientDataDrawable([FromSource] Player player, int hairType, int hairColor, int torso, int torsoTexture,
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
}