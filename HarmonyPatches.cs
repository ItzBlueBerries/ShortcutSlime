using Il2Cpp;
using Il2CppMonomiPark.SlimeRancher.Script.Util;
using Il2CppMonomiPark.SlimeRancher.UI.Localization;
using Il2CppMonomiPark.SlimeRancher.UI;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Localization.Tables;
using UnityEngine.Localization;
using UnityEngine;
using HarmonyLib;
using ShortcutSlimes.Data.Slimes;
using System.Collections;
using static ShortcutSlimes.Utility;
// using ShortcutSlimes.Components;

namespace ShortcutSlimes
{
    internal class HarmonyPatches
    {
        [HarmonyPatch(typeof(AutoSaveDirector), "Awake")]
        public static class PatchAutoSaveDirectorAwake
        {
            public static void Prefix(AutoSaveDirector __instance)
            {
                Utility.Get<IdentifiableTypeGroup>("PlortGroup").memberTypes.Add(Shortcut.enrichedPlort);
                Utility.Get<IdentifiableTypeGroup>("PlortGroup").memberTypes.Add(Shortcut.inDebtPlort);

                Utility.Get<IdentifiableTypeGroup>("BaseSlimeGroup").memberTypes.Add(Shortcut.shortcutSlime);
                Utility.Get<IdentifiableTypeGroup>("VaccableBaseSlimeGroup").memberTypes.Add(Shortcut.shortcutSlime);
                Utility.Get<IdentifiableTypeGroup>("SlimesGroup").memberTypes.Add(Shortcut.shortcutSlime);

                __instance.identifiableTypes.memberTypes.Add(Shortcut.enrichedPlort);
                __instance.identifiableTypes.memberTypes.Add(Shortcut.inDebtPlort);
                __instance.identifiableTypes.memberTypes.Add(Shortcut.shortcutSlime);
            }
        }

        [HarmonyPatch(typeof(MarketUI), "Start")]
        public static class PatchMarketUIStart
        {
            public static void Prefix(MarketUI __instance)
            {
                __instance.plorts = (from x in __instance.plorts
                                     where !ShortcutEntry.plortsToPatch.Exists((MarketUI.PlortEntry y) => y == x)
                                     select x).ToArray();
                __instance.plorts = __instance.plorts.ToArray().AddRangeToArray(ShortcutEntry.plortsToPatch.ToArray());
            }
        }

        [HarmonyPatch(typeof(EconomyDirector), "InitModel")]
        public static class PatchEconomyDirectorInitModel
        {
            public static void Prefix(EconomyDirector __instance)
            {
                __instance.baseValueMap = __instance.baseValueMap.ToArray().AddRangeToArray(ShortcutEntry.valueMapsToPatch.ToArray());
            }
        }

        [HarmonyPatch(typeof(LocalizationDirector), "LoadTables")]
        internal static class LocalizationDirectorLoadTablePatch
        {
            public static void Postfix(LocalizationDirector __instance) => MelonCoroutines.Start(LoadTable(__instance));

            private static IEnumerator LoadTable(LocalizationDirector director)
            {
                WaitForSecondsRealtime waitForSecondsRealtime = new WaitForSecondsRealtime(0.01f);
                yield return waitForSecondsRealtime;
                foreach (Il2CppSystem.Collections.Generic.KeyValuePair<string, StringTable> keyValuePair in director.Tables)
                {
                    if (addedTranslations.TryGetValue(keyValuePair.Key, out var dictionary))
                    {
                        foreach (System.Collections.Generic.KeyValuePair<string, string> keyValuePair2 in dictionary)
                        {
                            keyValuePair.Value.AddEntry(keyValuePair2.Key, keyValuePair2.Value);
                        }
                    }
                }
                yield break;
            }

            public static LocalizedString AddTranslation(string table, string key, string localized)
            {
                System.Collections.Generic.Dictionary<string, string> dictionary;
                if (!addedTranslations.TryGetValue(table, out dictionary))
                {
                    dictionary = new System.Collections.Generic.Dictionary<string, string>();
                    addedTranslations.Add(table, dictionary);
                }
                dictionary.TryAdd(key, localized);
                StringTable table2 = LocalizationUtil.GetTable(table);
                StringTableEntry stringTableEntry = table2.AddEntry(key, localized);
                return new LocalizedString(table2.SharedData.TableCollectionName, stringTableEntry.SharedEntry.Id);
            }

            public static System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string, string>> addedTranslations = new System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string, string>>();
        }
    }

    internal class OtherHarmonyPatches
    {
        [HarmonyPatch(typeof(SlimeEat), "EatAndProduce")]
        internal class PatchSlimeEatProduce
        {
            private static bool Prefix(SlimeEat __instance, SlimeDiet.EatMapEntry em)
            {
                if (__instance.slimeDefinition == Get<SlimeDefinition>("Shortcut"))
                {
                    IdentifiableType[] toRandomize = new IdentifiableType[]
                    {
                        Get<IdentifiableType>("EnrichedPlort"),
                        Get<IdentifiableType>("InDebtPlort")
                    };
                    List<IdentifiableType> list = new List<IdentifiableType>()
                    {
                        toRandomize[new System.Random().Next(0, toRandomize.Length)],
                        toRandomize[new System.Random().Next(0, toRandomize.Length)],
                        toRandomize[new System.Random().Next(0, toRandomize.Length)],
                        toRandomize[new System.Random().Next(0, toRandomize.Length)],
                        toRandomize[new System.Random().Next(0, toRandomize.Length)],
                        toRandomize[new System.Random().Next(0, toRandomize.Length)],
                    };
                    /*foreach (var item in toRandomize)
                        MelonLogger.Msg(item.ToString());
                    MelonLogger.Msg("------");
                    foreach (var item in list)
                        MelonLogger.Msg(item.ToString());*/

                    em.producesIdent = list.RandomObject();
                    // SceneContext.Instance.EconomyDirector.RegisterSold(em.producesIdent, 1);
                }
                return true;
            }
        }
    }
}
