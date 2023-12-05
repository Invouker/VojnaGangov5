using System;
using CitizenFX.Core;

namespace Client.Utils;

public static class Trace{
    public static void Log(string log){
        Debug.WriteLine($"[Server] {DateTime.Now:HH:mm:ss} - {log}");
    }
}