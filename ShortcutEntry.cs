using HarmonyLib;
using Il2Cpp;
using Il2CppInterop.Runtime.Injection;
using Il2CppMonomiPark.SlimeRancher.UI;
using MelonLoader;
// using ShortcutSlimes.Components;
using ShortcutSlimes.Data.Slimes;
using UnityEngine;

[assembly: MelonInfo(typeof(ShortcutSlimes.ShortcutEntry), "Shortcut Slimes", "2.0.1", "FruitsyOG", "https://www.nexusmods.com/slimerancher2/mods/17")]
[assembly: MelonGame("MonomiPark", "SlimeRancher2")]
namespace ShortcutSlimes
{
    public class ShortcutEntry : MelonMod
    {
        internal static List<MarketUI.PlortEntry> plortsToPatch = new List<MarketUI.PlortEntry>();
        internal static List<EconomyDirector.ValueMap> valueMapsToPatch = new List<EconomyDirector.ValueMap>();

        public override void OnInitializeMelon()
        { 
            // ClassInjector.RegisterTypeInIl2Cpp<AutoDepositAssistance>(); 
            // ClassInjector.RegisterTypeInIl2Cpp<DisappearOnSpawn>(); 
            Shortcut.Initialize();
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            Shortcut.Load(sceneName);
            switch (sceneName.Contains("zoneFields"))
            {
                case true:
                    {
                        SlimeSet.Member[] members = new SlimeSet.Member[]
                        {
                            new SlimeSet.Member()
                            {
                                IdentType = Utility.Get<SlimeDefinition>("Shortcut"),
                                _prefab = Utility.Get<SlimeDefinition>("Shortcut").prefab,
                                Weight = 0.5f
                            }
                        };
                        IEnumerable<DirectedSlimeSpawner> source = UnityEngine.Object.FindObjectsOfType<DirectedSlimeSpawner>();
                        foreach (DirectedSlimeSpawner directedSlimeSpawner in source)
                        {
                            foreach (DirectedActorSpawner.SpawnConstraint spawnConstraint in directedSlimeSpawner.Constraints)
                            {
                                if (spawnConstraint.Window.TimeMode == DirectedActorSpawner.TimeMode.NIGHT)
                                {
                                    foreach (SlimeSet.Member member in members)
                                    {
                                        if (spawnConstraint.Slimeset.Members.Contains(member))
                                            continue;

                                        spawnConstraint.Slimeset.Members = spawnConstraint.Slimeset.Members.AddItem(member).ToArray();
                                    }
                                }
                            }
                        }
                        break;
                    }
            }
        }
    }
}