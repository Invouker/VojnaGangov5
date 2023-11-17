using CitizenFX.Core;
using Newtonsoft.Json;

namespace Client.Entities;

public class CharacterCreatorData{
    #region HeadBlendData // Player.Local.Character.GetHeadBlendData();

    public short Sex{ get; set; }
    public int FirstFaceShape{ get; set; }
    public int SecondFaceShape{ get; set; }
    public int FirstSkinTone{ get; set; }
    public int SecondSkinTone{ get; set; }
    public float ParentFaceShapePercent{ get; set; }
    public float ParentSkinTonePercent{ get; set; }

    #endregion

    #region FaceFeature // GetPedFaceFeature(int /* Ped */ ped, int index);

    public float NoseWidth{ get; set; }
    public float NosePeak{ get; set; }
    public float NoseLength{ get; set; }
    public float NoseBoneCurvness{ get; set; }
    public float NoseTip{ get; set; }
    public float NoseBoneTwist{ get; set; }
    public float Eyebrow{ get; set; }
    public float Eyebrow2{ get; set; }
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

    #region Head Overlay // 1,2,3,4,6,7,8,9,11

    /*
     * Add to sql:
     */
    public int FacialHair{ get; set; } // 1
    public int FacialHairColor{ get; set; } // 1
    public float FacialHairOpacity{ get; set; } // 1
    public int Eyebrows{ get; set; } // 2
    public int EyebrowsColor{ get; set; } // 2
    public float EyebrowsOpacity{ get; set; } // 2
    public int Ageing{ get; set; } // 3
    public int AgeingColor{ get; set; } // 3
    public float AgeingOpacity{ get; set; } // 3
    public int Makeup{ get; set; } // 4
    public int MakeupColor{ get; set; } // 4
    public float MakeupOpacity{ get; set; } // 4
    public int Complexion{ get; set; } // 6
    public int ComplexionColor{ get; set; } // 6
    public float ComplexionOpacity{ get; set; } // 6
    public int SunDamage{ get; set; } // 7
    public int SunDamageColor{ get; set; } // 7
    public float SunDamageOpacity{ get; set; } // 7
    public int Lipstick{ get; set; } // 8
    public int LipstickColor{ get; set; } // 8
    public float LipstickOpacity{ get; set; } // 8
    public int MolesFreckles{ get; set; } // 9
    public int MolesFrecklesColor{ get; set; } // 9
    public float MolesFrecklesOpacity{ get; set; } // 9
    public int BodyBlemishes{ get; set; } // 11
    public int BodyBlemishesColor{ get; set; } // 11
    public float BodyBlemishesOpacity{ get; set; } // 11

    #endregion

    #region HairCut and MakeUp //  GetPedDrawableVariation, GetPedHairColor // GetNumMakeupColors // 2,3,4,6,7,8,11

    public int HairType{ get; set; } // 2
    public int HairColor{ get; set; } // 2
    public int Torso{ get; set; } // 3
    public int TorsoTexture{ get; set; } // 3
    public int Legs{ get; set; } // 4
    public int LegsTexture{ get; set; } // 4
    public int Foot{ get; set; } // 6
    public int FootTexture{ get; set; } // 6
    public int Scarfs{ get; set; } // 7
    public int ScarfsTexture{ get; set; } // 7
    public int Accesories{ get; set; } // 8
    public int AccesoriesTexture{ get; set; } // 8
    public int Torso2{ get; set; } // 11
    public int Torso2Texture{ get; set; } // 11

    #endregion

    /*
        public CharacterCreatorData(){
            InitBlendData();
            InitFaceFeature();
            InitDrawable();


            Debug.WriteLine("SerializeToJson(): " + SerializeToJson());
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
            short sex = 1;
            if (API.IsPedMale(ped.Handle))
                sex = 0;

            Sex = sex;
            FirstFaceShape = ped.GetHeadBlendData().FirstFaceShape;
            SecondFaceShape = ped.GetHeadBlendData().SecondFaceShape;
            FirstSkinTone = ped.GetHeadBlendData().FirstSkinTone;
            SecondSkinTone = ped.GetHeadBlendData().SecondSkinTone;
            ParentFaceShapePercent = ped.GetHeadBlendData().ParentFaceShapePercent;
            ParentSkinTonePercent = ped.GetHeadBlendData().ParentSkinTonePercent;
        }
    */
    public string SerializeToJson(){
        return JsonConvert.SerializeObject(this);
    }

    // Deserialization method
    public static CharacterCreatorData DeserializeFromJson(string json){
        return JsonConvert.DeserializeObject<CharacterCreatorData>(json);
    }

    public void SendDataToServer(){
        BaseScript.TriggerLatentServerEvent("player:data:character", 5000, SerializeToJson());
        Debug.WriteLine("Sending to the server! 0/6");
        /* BaseScript.TriggerServerEvent("player:data:character:blend",
                                       //1st
                                       Sex, FirstFaceShape, SecondFaceShape, FirstSkinTone, SecondSkinTone,
                                       ParentFaceShapePercent, ParentSkinTonePercent
                                      );
         Debug.WriteLine("Sending to the server! 1/6");
         BaseScript.TriggerServerEvent("player:data:character:facefeature",
                                       //2nd
                                       NoseWidth, NosePeak, NoseLength, NoseBoneCurvness, NoseTip, NoseBoneTwist,
                                       Eyebrow, Eyebrow2, CheekBones, CheekBonesWidth,
                                       CheekSidewaysBoneSize
                                      );
         Debug.WriteLine("Sending to the server! 2/6");
         BaseScript.TriggerServerEvent("player:data:character:facefeature2",
                                       //2nd
                                       EyeOpening, LipThickness, JawBoneWidth, JawBoneShape, ChinBone, ChinBoneLength,
                                       ChinBoneShape,
                                       ChinHole, NeckThickness
                                      );
         Debug.WriteLine("Sending to the server! 3/6");
         BaseScript.TriggerServerEvent("player:data:character:drawable",
                                       //3st
                                       HairType, HairColor, Torso, TorsoTexture, Legs, LegsTexture, Foot, FootTexture,
                                       Scarfs, ScarfsTexture, Accesories,
                                       AccesoriesTexture, Torso2, Torso2Texture
                                      );

         Debug.WriteLine("Sending to the server! 5/6");*/
        BaseScript.TriggerServerEvent("player:data:character:save");
        Debug.WriteLine("Sending to the server! 6/6");
    }

    private static CharacterCreatorData characterCreatorDataInstance;

    public static CharacterCreatorData GetCharacterCreatorData(){
        if (characterCreatorDataInstance == null)
            return characterCreatorDataInstance = new CharacterCreatorData();
        return characterCreatorDataInstance;
    }
}