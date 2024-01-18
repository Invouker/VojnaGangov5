using Newtonsoft.Json;

namespace Client.Entities;

public class CharacterCreatorEntity{
    #region HeadBlendData // Player.Local.Character.GetHeadBlendData();

    public short Sex{ get; set; }

    public int Mother{ get; set; }

    public int Father{ get; set; }

    /* public int FirstFaceShape{ get; set; }
     public int SecondFaceShape{ get; set; }
     public int FirstSkinTone{ get; set; }
     public int SecondSkinTone{ get; set; }*/
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

    public string SerializeToJson(){
        return JsonConvert.SerializeObject(this);
    }

    // Deserialization method
    public static CharacterCreatorEntity DeserializeFromJson(string json){
        return JsonConvert.DeserializeObject<CharacterCreatorEntity>(json);
    }

    public void SendDataToServer(){
        string data = SerializeToJson();
        //BaseScript.TriggerLatentServerEvent("player:data:character", 6000, data);
        EventDispatcher.Send("player:data:character", data);
    }
}