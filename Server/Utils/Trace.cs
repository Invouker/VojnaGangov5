using System;
using CitizenFX.Core;

namespace Server;

public class Trace{
    public static void Log(string log){
        Debug.WriteLine($"[Server] {DateTime.Now:HH:mm:ss} - {log}");
    }
}