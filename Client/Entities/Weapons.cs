using System.Collections.Generic;
using CitizenFX.Core;
using Newtonsoft.Json;
using static CitizenFX.Core.Native.API;

namespace Client.Entities;

public class WeaponLoader{
    public static void LoadWeaponsData(){
        var json = LoadResourceFile(GetCurrentResourceName(), "weapon-data.json");
        Dictionary<string, WeaponData> weaponData = WeaponData.FromJson(json);

        foreach (KeyValuePair<string, WeaponData> keyValuePair in weaponData){
            Debug.WriteLine("key: " + keyValuePair.Key);
        }
    }
}

public partial class WeaponData{
    public string HashKey{ get; set; }
    public string NameGxt{ get; set; }
    public string DescriptionGxt{ get; set; }
    public string Name{ get; set; }
    public string Description{ get; set; }
    public string Group{ get; set; }
    public string ModelHashKey{ get; set; }
    public long DefaultClipSize{ get; set; }
    public string AmmoType{ get; set; }
    public Dictionary<string, Component> Components{ get; set; }
    public List<LiveryColor> Tints{ get; set; }
    public List<LiveryColor> LiveryColors{ get; set; }
    public string Dlc{ get; set; }
}

public class Component{
    public string HashKey{ get; set; }
    public string NameGxt{ get; set; }
    public string DescriptionGxt{ get; set; }
    public string Name{ get; set; }
    public string Description{ get; set; }
    public string ModelHashKey{ get; set; }
    public bool IsDefault{ get; set; }
    public string AmmoType{ get; set; }
}

public class LiveryColor{
    public string NameGxt{ get; set; }
    public string Name{ get; set; }
}

public partial class WeaponData{
    public static Dictionary<string, WeaponData> FromJson(string json) =>
        JsonConvert.DeserializeObject<Dictionary<string, WeaponData>>(json);
}