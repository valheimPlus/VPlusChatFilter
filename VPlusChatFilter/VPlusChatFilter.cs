using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace VPlusChatFilter
{
    [BepInPlugin(ID, "Valheim Plus Chat Filter", version)]
    public class VPlusChatFilter : BaseUnityPlugin
    {
        public const string ID = "mixone.valheimplus.chatfilter";
        public const string version = "0.0.1.0";

        public Harmony harmony;

        public static ConfigEntry<bool> isEnabled;
        public static ConfigEntry<bool> wrapWords;
        public static ConfigEntry<string> replaceKey;
        public static ConfigEntry<bool> addCustomWords;
        public static ConfigEntry<string> customWordsFilename;
        public static ConfigEntry<bool> removeWords;
        public static ConfigEntry<string> removeWordsFilename;

        public static List<string> bannedWords = new List<string>() {"test", "dev", "insult"};
        public static BepInEx.Logging.ManualLogSource chatFilterLogger;

        void Awake()
        {
            isEnabled = Config.Bind("General.Toggles", "EnableFilter", true, "Whether or not to filter the chat.");
            wrapWords = Config.Bind("General.Toggles", "WrapWords", true, "Whether or not to wrap around words.");
            replaceKey = Config.Bind("General", "ReplaceKey", "*", "Symbol to censor words with.");
            addCustomWords = Config.Bind("Filtering.Toggles", "AddCustomWords", false, "Add your own custom words.");
            customWordsFilename = Config.Bind("Filtering", "CustomWordsFilename", "customAdd.filter", "Filename where custom words to add are.");
            removeWords = Config.Bind("Filtering.Toggles", "RemoveWords", false, "Remove certain words from filters.");
            removeWordsFilename = Config.Bind("Filtering", "RemoveWordsFilename", "customRemove.filter", "Filename where words to remove from filter are.");
            harmony = new Harmony(ID);
            harmony.PatchAll();
            chatFilterLogger = Logger;
            FamilyFriendlyfier.Initialise();
            if (addCustomWords.Value)
            {
                string[] wordsToAdd = FamilyFriendlyfier.GetWordsFromFile(customWordsFilename.Value);
                if(wordsToAdd != null)
                    FamilyFriendlyfier.filter.AddProfanity(wordsToAdd);
            }
            if(removeWords.Value)
            {
                string[] wordsToRemove = FamilyFriendlyfier.GetWordsFromFile(removeWordsFilename.Value);
                if (wordsToRemove != null)
                {
                    foreach(string word in wordsToRemove)
                    {
                        if(FamilyFriendlyfier.filter.IsProfanity(word))
                           FamilyFriendlyfier.filter.RemoveProfanity(word);
                    }
                }
            }
            chatFilterLogger.LogInfo("This server is using VPlusChatFilter.");
        }
    }

    #region Patches 

    #region ZRoutedRPC

    [HarmonyPatch(typeof(ZRoutedRpc), nameof(ZRoutedRpc.RouteRPC))]
    public static class ZRoutedRPC_RouteRPC_Patch
    {
        public static bool Prefix(ref ZRoutedRpc __instance, ref ZRoutedRpc.RoutedRPCData rpcData)
        {
            if (VPlusChatFilter.isEnabled.Value)
            {
                if (rpcData.m_methodHash == "ChatMessage".GetStableHashCode())
                {
                    ZPackage payload = rpcData.m_parameters;
                    VPlusChatFilter.chatFilterLogger.LogDebug("ChatMessage Sent");
                    payload.SetPos(0);
                    VPlusChatFilter.chatFilterLogger.LogDebug("Size of package is " + payload.Size());
                    VPlusChatFilter.chatFilterLogger.LogDebug("Read byte test : " + payload.ReadInt());
                    payload.SetPos(0);
                    Vector3 headPoint = payload.ReadVector3();
                    VPlusChatFilter.chatFilterLogger.LogDebug("Read head : " + headPoint.ToString());
                    int messageType = payload.ReadInt();
                    VPlusChatFilter.chatFilterLogger.LogDebug("Read type : " + messageType);
                    string playerName = payload.ReadString();
                    VPlusChatFilter.chatFilterLogger.LogDebug("Read name : " + playerName);
                    string message = payload.ReadString();
                    VPlusChatFilter.chatFilterLogger.LogDebug("Read message : " + message);
                    var profanities = FamilyFriendlyfier.filter.DetectAllProfanities(message, true);
                    if (profanities.Count > 0)
                    {
                        foreach (string bannable in profanities)
                        {
                            VPlusChatFilter.chatFilterLogger.LogInfo("Bad word from " + playerName + " : "+bannable);
                        }
                        message = FamilyFriendlyfier.filter.CensorString(message, VPlusChatFilter.replaceKey.Value[0]);
                    }
                    VPlusChatFilter.chatFilterLogger.LogDebug("New message : " + message);
                    ZPackage newpayload = new ZPackage();
                    ZRpc.Serialize(new object[] { headPoint, messageType, playerName, message }, ref newpayload);
                    rpcData.m_parameters = newpayload;
                }
                else if (rpcData.m_methodHash == "Say".GetStableHashCode())
                {
                    ZPackage payload = rpcData.m_parameters;
                    VPlusChatFilter.chatFilterLogger.LogDebug("Say Sent");
                    payload.SetPos(0);
                    VPlusChatFilter.chatFilterLogger.LogDebug("Size of package is " + payload.Size());
                    VPlusChatFilter.chatFilterLogger.LogDebug("Read byte test : " + payload.ReadInt());
                    payload.SetPos(0);
                    int messageType = payload.ReadInt();
                    VPlusChatFilter.chatFilterLogger.LogDebug("Read type : " + messageType);
                    string playerName = payload.ReadString();
                    VPlusChatFilter.chatFilterLogger.LogDebug("Read name : " + playerName);
                    string message = payload.ReadString();
                    VPlusChatFilter.chatFilterLogger.LogDebug("Read message : " + message);
                    var profanities = FamilyFriendlyfier.filter.DetectAllProfanities(message, true);
                    if (profanities.Count > 0)
                    {
                        foreach (string bannable in profanities)
                        {
                            VPlusChatFilter.chatFilterLogger.LogInfo("Bad word from " + playerName + " : " + bannable);
                        }
                        message = FamilyFriendlyfier.filter.CensorString(message, VPlusChatFilter.replaceKey.Value[0]);
                    }
                    VPlusChatFilter.chatFilterLogger.LogDebug("New message : " + message);
                    ZPackage newpayload = new ZPackage();
                    ZRpc.Serialize(new object[] { messageType, playerName, message }, ref newpayload);
                    rpcData.m_parameters = newpayload;
                }
                ZPackage zpackage = new ZPackage();
                rpcData.Serialize(zpackage);
                if (__instance.m_server)
                {
                    if (rpcData.m_targetPeerID != 0L)
                    {
                        ZNetPeer peer = __instance.GetPeer(rpcData.m_targetPeerID);
                        if (peer != null && peer.IsReady())
                        {
                            peer.m_rpc.Invoke("RoutedRPC", new object[] { zpackage });
                            return false;
                        }
                        return false;
                    }
                    else
                    {
                        using (List<ZNetPeer>.Enumerator enumerator = __instance.m_peers.GetEnumerator())
                        {
                            while (enumerator.MoveNext())
                            {
                                ZNetPeer znetPeer = enumerator.Current;
                                if (rpcData.m_senderPeerID != znetPeer.m_uid && znetPeer.IsReady())
                                    znetPeer.m_rpc.Invoke("RoutedRPC", new object[] { zpackage });
                            }
                            return false;
                        }
                    }
                }
                foreach (ZNetPeer znetPeer2 in __instance.m_peers)
                {
                    if (znetPeer2.IsReady())
                        znetPeer2.m_rpc.Invoke("RoutedRPC", new object[] { zpackage });
                }
            }
            return true;
        }
    }

    #endregion

    #endregion
}
