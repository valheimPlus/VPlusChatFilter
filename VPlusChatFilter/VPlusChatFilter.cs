using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace VPlusChatFilter
{
    [BepInPlugin(ID, "Valheim Plus Chat Filter", version)]
    public class VPlusChatFilter : BaseUnityPlugin
    {

        public const string ID = "com.mixone.valheimplus.chatfilter";
        public const string version = "0.0.1.0";

        public Harmony harmony;

        public static List<string> bannedWords = new List<string>() {"test", "dev", "insult"};

        void Awake()
        {
            harmony = new Harmony(ID);
            harmony.PatchAll();
        }
    }

    #region Patches

    #region ZRoutedRPC

    [HarmonyPatch(typeof(ZRoutedRpc), nameof(ZRoutedRpc.RouteRPC))]
    public static class ZRoutedRPC_RouteRPC_Patch
    {
        public static void Prefix(ref ZRoutedRpc __instance, ref ZRoutedRpc.RoutedRPCData rpcData)
        {            
            if (rpcData.m_methodHash == "ChatMessage".GetStableHashCode())
            {
                ZPackage payload = rpcData.m_parameters;
                ZPackage newpayload = new ZPackage();
                Debug.Log("ChatMessage Sent");
                payload.SetPos(0);
                Debug.Log("Size of package is " + payload.Size());
                Debug.Log("Read byte test : " + payload.ReadInt());
                payload.SetPos(0);
                Vector3 headPoint = payload.ReadVector3();
                Debug.Log("Read head : " + headPoint.ToString());              
                int messageType = payload.ReadInt();
                Debug.Log("Read type : " + messageType);
                string playerName = payload.ReadString();
                Debug.Log("Read name : " + playerName);
                string message = payload.ReadString();
                Debug.Log("Read message : " + message);
                Debug.Log(message);
                foreach (string bannable in VPlusChatFilter.bannedWords)
                {
                    if(message.ToLower().Contains(bannable))
                    {
                        Debug.Log("Bad word from "+playerName);
                        message.ToLower().Replace(bannable, "DENIED");                        
                    }
                }
                ZRpc.Serialize(new object[] { headPoint, messageType, playerName, message}, ref newpayload);
                rpcData.m_parameters = newpayload;
            } else if (rpcData.m_methodHash == "Say".GetStableHashCode())
            {
                ZPackage payload = rpcData.m_parameters;
                ZPackage newpayload = new ZPackage();
                Debug.Log("Say Sent");
                payload.SetPos(0);
                Debug.Log("Size of package is "+payload.Size());
                Debug.Log("Read byte test : " + payload.ReadInt());
                payload.SetPos(0);
                int messageType = payload.ReadInt();
                Debug.Log("Read type : " + messageType);                
                string playerName = payload.ReadString();
                Debug.Log("Read name : "+playerName);
                string message = payload.ReadString();
                Debug.Log("Read message : " + message);
                Debug.Log(message);
                foreach (string bannable in VPlusChatFilter.bannedWords)
                {
                    if (message.ToLower().Contains(bannable))
                    {
                        Debug.Log("Bad word from " + playerName);
                        message.ToLower().Replace(bannable, "DENIED");
                    }
                }
                ZRpc.Serialize(new object[] { messageType, playerName, message }, ref newpayload);
                rpcData.m_parameters = newpayload;
            }
        }
    }

    #endregion

    #endregion
}
