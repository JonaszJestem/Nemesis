using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using MimicAPI.GameAPI;
using Nemesis.Core;
using Nemesis.Modules.DifficultyDirector.Patches;

namespace Nemesis.Modules.RoleSystem.Patches
{
    [HarmonyPatch]
    internal class SessionStartPatch
    {
        static IEnumerable<MethodBase> TargetMethods()
        {
            var methods = new List<MethodBase>();

            try
            {
                var assembly = ServerNetworkAPI.GetGameAssembly();
                if (assembly == null) return methods;

                // Only use one reliable session start signal
                // Prefer GameSessionInfo.StartSession if available
                var sessionType = assembly.GetType("GameSessionInfo");
                if (sessionType != null)
                {
                    var startMethod = sessionType.GetMethod("StartSession",
                        BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public |
                        BindingFlags.Static);
                    if (startMethod != null)
                    {
                        methods.Add(startMethod);
                        return methods; // Use only this one
                    }
                }

                // Fallback: VRoomManager.EnterWaitingRoom
                var vroomManagerType = assembly.GetType("VRoomManager");
                if (vroomManagerType != null)
                {
                    var enterMethod = vroomManagerType.GetMethod("EnterWaitingRoom",
                        BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                    if (enterMethod != null)
                        methods.Add(enterMethod);
                }
            }
            catch { }

            return methods;
        }

        static void Postfix()
        {
            try
            {
                MonsterStatPatch.ClearTracking();
                ModuleEventBus.RaiseSessionStarted();
            }
            catch { }
        }
    }
}
