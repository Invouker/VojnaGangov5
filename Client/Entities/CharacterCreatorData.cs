using System;
using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace Client.Entities;

public class CharacterCreatorData{
    #region HeadBlendData // Player.Local.Character.GetHeadBlendData();

    public int FirstFaceShape{ get; set; }

    public int SecondFaceShape{ get; set; }

    //public int ThirdFaceShape{ get; set; }
    public int FirstSkinTone{ get; set; }

    public int SecondSkinTone{ get; set; }

    //public int ThirdSkinTone{ get; set; }
    public float ParentFaceShapePercent{ get; set; }

    public float ParentSkinTonePercent{ get; set; }
    //public float ParentThirdUnkPercent{ get; set; }
    //public bool IsParentInheritance{ get; set; }

    #endregion

    #region FaceFeature // GetPedFaceFeature(int /* Ped */ ped, int index);

    public float NoseWidth{ get; set; }
    public float NosePeak{ get; set; }
    public float NoseLength{ get; set; }
    public float NoseBoneCurvness{ get; set; }
    public float NoseTip{ get; set; }
    public float NoseBoneTwist{ get; set; }
    public float Eyebrow{ get; set; } //Up/Down
    public float Eyebrow2{ get; set; } // In/Out
    public float CheekBones{ get; set; }
    public float CheekBonesWidth{ get; set; }
    public float CheekSidewaysBoneSize{ get; set; }
    public float EyeOpening{ get; set; }
    public float LipThickness{ get; set; }
    public float JawBoneWidth{ get; set; }
    public float JawBoneShape{ get; set; }
    public float ChinBone{ get; set; }
    public float ChinBoneLength{ get; set; }
    public float ChinBoneShape{ get; set; }
    public float ChinHole{ get; set; }
    public float NeckThickness{ get; set; }

    #endregion

    #region HairCut and MakeUp //  GetPedDrawableVariation, GetPedHairColor // GetNumMakeupColors // 2,3,4,6,7,8,11

    private int HairType{ get; set; } // 2
    private int HairColor{ get; set; } // 2
    private int Torso{ get; set; } // 3
    private int TorsoTexture{ get; set; } // 3
    private int Legs{ get; set; } // 4
    private int LegsTexture{ get; set; } // 4
    private int Foot{ get; set; } // 6
    private int FootTexture{ get; set; } // 6
    private int Scarfs{ get; set; } // 7
    private int ScarfsTexture{ get; set; } // 7
    private int Accesories{ get; set; } // 8
    private int AccesoriesTexture{ get; set; } // 8
    private int Torso2{ get; set; } // 11
    private int Torso2Texture{ get; set; } // 11

    #endregion

    private bool IsInitialized = false;

    public CharacterCreatorData(){
        InitBlendData();
        InitFaceFeature();
        InitDrawable();
        IsInitialized = true;
    }

    private void InitDrawable(){
        int pedId = API.PlayerPedId();
        HairType = API.GetPedDrawableVariation(pedId, 2);
        HairColor = API.GetPedHairColor(pedId);
        Torso = API.GetPedDrawableVariation(pedId, 3);
        TorsoTexture = API.GetPedTextureVariation(pedId, 3);
        Legs = API.GetPedDrawableVariation(pedId, 4);
        LegsTexture = API.GetPedTextureVariation(pedId, 4);
        Foot = API.GetPedDrawableVariation(pedId, 6);
        FootTexture = API.GetPedTextureVariation(pedId, 6);
        Scarfs = API.GetPedDrawableVariation(pedId, 7);
        ScarfsTexture = API.GetPedTextureVariation(pedId, 7);
        Accesories = API.GetPedDrawableVariation(pedId, 8);
        AccesoriesTexture = API.GetPedTextureVariation(pedId, 8);
        Torso2 = API.GetPedDrawableVariation(pedId, 11);
        Torso2Texture = API.GetPedTextureVariation(pedId, 11);
    }

    private void InitFaceFeature(){
        int pedId = API.PlayerPedId();

        NoseWidth = API.GetPedFaceFeature(pedId, 0);
        NosePeak = API.GetPedFaceFeature(pedId, 1);
        NoseLength = API.GetPedFaceFeature(pedId, 2);
        NoseBoneCurvness = API.GetPedFaceFeature(pedId, 3);
        NoseTip = API.GetPedFaceFeature(pedId, 4);
        NoseBoneTwist = API.GetPedFaceFeature(pedId, 5);
        Eyebrow = API.GetPedFaceFeature(pedId, 6);
        Eyebrow2 = API.GetPedFaceFeature(pedId, 7);
        CheekBones = API.GetPedFaceFeature(pedId, 8);
        CheekSidewaysBoneSize = API.GetPedFaceFeature(pedId, 9);
        CheekBonesWidth = API.GetPedFaceFeature(pedId, 10);
        EyeOpening = API.GetPedFaceFeature(pedId, 11);
        LipThickness = API.GetPedFaceFeature(pedId, 12);
        JawBoneWidth = API.GetPedFaceFeature(pedId, 13);
        JawBoneShape = API.GetPedFaceFeature(pedId, 14);
        ChinBone = API.GetPedFaceFeature(pedId, 15);
        ChinBoneLength = API.GetPedFaceFeature(pedId, 16);
        ChinBoneShape = API.GetPedFaceFeature(pedId, 17);
        ChinHole = API.GetPedFaceFeature(pedId, 18);
        NeckThickness = API.GetPedFaceFeature(pedId, 19);
    }

    private void InitBlendData(){
        Ped ped = Player.Local.Character;

        FirstFaceShape = ped.GetHeadBlendData().FirstFaceShape;
        SecondFaceShape = ped.GetHeadBlendData().SecondFaceShape;
        FirstSkinTone = ped.GetHeadBlendData().FirstSkinTone;
        SecondSkinTone = ped.GetHeadBlendData().SecondSkinTone;
        ParentFaceShapePercent = ped.GetHeadBlendData().ParentFaceShapePercent;
        ParentSkinTonePercent = ped.GetHeadBlendData().ParentSkinTonePercent;
    }

    public void SendDataToServer(){
        if (!IsInitialized)
            throw new
                NullReferenceException("You should create instance of CharacterCreatorData and initialize it before calling this.");

        BaseScript.TriggerServerEvent("player:data:character:blend",
                                      //1st
                                      FirstFaceShape, SecondFaceShape, FirstSkinTone, SecondSkinTone,
                                      ParentFaceShapePercent, ParentSkinTonePercent
                                     );
        BaseScript.TriggerServerEvent("player:data:character:facefeature",
                                      //2nd
                                      NoseWidth, NosePeak, NoseLength, NoseBoneCurvness, NoseTip, NoseBoneTwist,
                                      Eyebrow, Eyebrow2, CheekBones, CheekBonesWidth,
                                      CheekSidewaysBoneSize
                                     );
        BaseScript.TriggerServerEvent("player:data:character:facefeature2",
                                      //2nd
                                      EyeOpening, LipThickness, JawBoneWidth, JawBoneShape, ChinBone, ChinBoneLength,
                                      ChinBoneShape,
                                      ChinHole, NeckThickness
                                     );
        BaseScript.TriggerServerEvent("player:data:character:drawable",
                                      //3st
                                      HairType, HairColor, Torso, TorsoTexture, Legs, LegsTexture, Foot, FootTexture,
                                      Scarfs, ScarfsTexture, Accesories,
                                      AccesoriesTexture, Torso2, Torso2Texture
                                     );

        BaseScript.TriggerServerEvent("player:data:character:save");
    }

    private static CharacterCreatorData characterCreatorDataInstance;

    public static CharacterCreatorData GetCharacterCreatorData(){
        if (characterCreatorDataInstance == null)
            return characterCreatorDataInstance = new CharacterCreatorData();
        return characterCreatorDataInstance;
    }
}