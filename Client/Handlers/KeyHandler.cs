using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace Client.Handlers;

public class KeyHandler{
    private static readonly List<KeyPair> KeyPairs = new List<KeyPair>();

    public static void CreateKeyPair(Control key, Action doAction, int control = 0,
        ControlType controlType = ControlType.IsControlJustPressed){
        KeyPairs.Add(new KeyPair(key, doAction, control));
    }

    public enum ControlType{
        IsControlJustPressed,
        IsControlJustReleased,
        IsDisabledControlJustPressed,
        IsDisabledControlJustReleased
    }

    public static Task Tick(){
        if (KeyPairs.Count <= 0) return Task.FromResult(true);

        foreach (KeyPair keyPair in KeyPairs){
            switch (keyPair.ControlType){
                case ControlType.IsControlJustPressed:
                    if (API.IsControlJustPressed(keyPair.Control, keyPair.Key.GetHashCode()))
                        keyPair.DoAction.Invoke();
                    break;
                case ControlType.IsControlJustReleased:
                    if (API.IsControlJustReleased(keyPair.Control, keyPair.Key.GetHashCode()))
                        keyPair.DoAction.Invoke();
                    break;
                case ControlType.IsDisabledControlJustPressed:
                    if (API.IsDisabledControlJustPressed(keyPair.Control, keyPair.Key.GetHashCode()))
                        keyPair.DoAction.Invoke();
                    break;
                case ControlType.IsDisabledControlJustReleased:
                    if (API.IsDisabledControlJustReleased(keyPair.Control, keyPair.Key.GetHashCode()))
                        keyPair.DoAction.Invoke();
                    break;
            }
        }

        return Task.FromResult(true);
    }

    public class KeyPair{
        public int Control{ get; set; }
        public Control Key{ get; set; }
        public Action DoAction{ get; set; }
        public ControlType ControlType{ get; set; }

        public KeyPair(Control key, Action doAction, int control = 0,
            ControlType controlType = ControlType.IsControlJustPressed){
            Control = control;
            Key = key;
            DoAction = doAction;
            ControlType = controlType;
        }
    }
}