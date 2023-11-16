namespace Server.Database.Entities;

//"characters" Table
public class Character{
    public int Id{ get; set; }
    public int AccId{ get; set; }
    public string Name{ get; set; }

    public Character(string name){
        Name = name;
    }

    public short Sex{ get; set; }

    #region HeadBlendData // Player.Local.Character.GetHeadBlendData();

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

    #region HairCut and MakeUp //  GetPedDrawableVariation, GetPedHairColor // GetNumMakeupColors // 2,3,4,6,7,8,11

    /* public int HairType{ get; set; } // 2
     public int HairColor{ get; set; } // 2
     public int Torso{ get; set; } // 3
     public int TorsoTexture{ get; set; } // 3
     public int Legs{ get; set; } // 4
     public int LegsTexture{ get; set; } // 4
     public int Foot{ get; set; } // 6
     public int FootTexture{ get; set; } // 6
     public int Scarfs{ get; set; } // 7
     public int ScarfsTexture{ get; set; } // 7
     public int Accessories{ get; set; } // 8
     public int AccessoriesTexture{ get; set; } // 8
     public int Torso2{ get; set; } // 11
     public int Torso2Texture{ get; set; } // 11
     */
    public int HairType{ get; set; }
    public int HairColor{ get; set; }
    public int Torso{ get; set; }
    public int TorsoTexture{ get; set; }
    public int Legs{ get; set; }
    public int LegsTexture{ get; set; }
    public int Foot{ get; set; }
    public int FootTexture{ get; set; }
    public int Scarfs{ get; set; }
    public int ScarfsTexture{ get; set; }
    public int Accessories{ get; set; }
    public int AccessoriesTexture{ get; set; }
    public int Torso2{ get; set; }
    public int Torso2Texture{ get; set; }

    #endregion

    public Character(){ }

    public override string ToString(){
        return $"Id: {Id}, AccId: {AccId}, Name: {Name}, Sex: {Sex}, " +
               $"FirstFaceShape: {FirstFaceShape}, SecondFaceShape: {SecondFaceShape}, " +
               $"FirstSkinTone: {FirstSkinTone}, SecondSkinTone: {SecondSkinTone}, " +
               $"ParentFaceShapePercent: {ParentFaceShapePercent}, ParentSkinTonePercent: {ParentSkinTonePercent}, " +
               $"NoseWidth: {NoseWidth}, NosePeak: {NosePeak}, NoseLength: {NoseLength}, " +
               $"NoseBoneCurvness: {NoseBoneCurvness}, NoseTip: {NoseTip}, NoseBoneTwist: {NoseBoneTwist}, " +
               $"Eyebrow: {Eyebrow}, Eyebrow2: {Eyebrow2}, CheekBones: {CheekBones}, " +
               $"CheekBonesWidth: {CheekBonesWidth}, CheekSidewaysBoneSize: {CheekSidewaysBoneSize}, " +
               $"EyeOpening: {EyeOpening}, LipThickness: {LipThickness}, JawBoneWidth: {JawBoneWidth}, " +
               $"JawBoneShape: {JawBoneShape}, ChinBone: {ChinBone}, ChinBoneLength: {ChinBoneLength}, " +
               $"ChinBoneShape: {ChinBoneShape}, ChinHole: {ChinHole}, NeckThickness: {NeckThickness}, " +
               $"HairType: {HairType}, HairColor: {HairColor}, Torso: {Torso}, TorsoTexture: {TorsoTexture}, " +
               $"Legs: {Legs}, LegsTexture: {LegsTexture}, Foot: {Foot}, FootTexture: {FootTexture}, " +
               $"Scarfs: {Scarfs}, ScarfsTexture: {ScarfsTexture}, Accessories: {Accessories}, " +
               $"AccessoriesTexture: {AccessoriesTexture}, Torso2: {Torso2}, Torso2Texture: {Torso2Texture}";
    }
}