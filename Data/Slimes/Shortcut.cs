using HarmonyLib;
using Il2Cpp;
using Il2CppMonomiPark.SlimeRancher.UI;
// using ShortcutSlimes.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static ShortcutSlimes.ShortcutEntry;

namespace ShortcutSlimes.Data.Slimes
{
    internal class Shortcut
    {
        internal static SlimeDefinition shortcutSlime;
        internal static IdentifiableType enrichedPlort;
        internal static IdentifiableType inDebtPlort;

        public static void Initialize()
        {
            shortcutSlime = ScriptableObject.CreateInstance<SlimeDefinition>();
            shortcutSlime.name = "Shortcut";
            shortcutSlime.hideFlags |= HideFlags.HideAndDontSave;
            shortcutSlime.color = Color.black;

            enrichedPlort = ScriptableObject.CreateInstance<IdentifiableType>();
            enrichedPlort.name = "EnrichedPlort";
            enrichedPlort.hideFlags |= HideFlags.HideAndDontSave;
            enrichedPlort.color = Color.green;

            inDebtPlort = ScriptableObject.CreateInstance<IdentifiableType>();
            inDebtPlort.name = "InDebtPlort";
            inDebtPlort.hideFlags |= HideFlags.HideAndDontSave;
            inDebtPlort.color = Color.red;
        }

        public static void Load(string sceneName)
        {
            switch (sceneName)
            {
                case "GameCore":
                    {
                        enrichedPlort.localizedName = HarmonyPatches.LocalizationDirectorLoadTablePatch.AddTranslation("Actor", "l.enriched_plort", "Enriched Plort");
                        inDebtPlort.localizedName = HarmonyPatches.LocalizationDirectorLoadTablePatch.AddTranslation("Actor", "l.indebt_plort", "In-Debt Plort");
                        shortcutSlime.localizedName = HarmonyPatches.LocalizationDirectorLoadTablePatch.AddTranslation("Actor", "l.shortcut_slime", "Shortcut Slime");

                        #region ENRICHED_PLORT && IN_DEBT_PLORT
                        Material enrichedMaterial = UnityEngine.Object.Instantiate(Utility.Get<GameObject>("plortPink").GetComponent<MeshRenderer>().sharedMaterial);
                        enrichedMaterial.SetColor("_TopColor", Color.green);
                        enrichedMaterial.SetColor("_MiddleColor", Color.white);
                        enrichedMaterial.SetColor("_BottomColor", Color.yellow);

                        Material inDebtMaterial = UnityEngine.Object.Instantiate(Utility.Get<GameObject>("plortPink").GetComponent<MeshRenderer>().sharedMaterial);
                        inDebtMaterial.SetColor("_TopColor", Color.red);
                        inDebtMaterial.SetColor("_MiddleColor", Color.white);
                        inDebtMaterial.SetColor("_BottomColor", Color.black);

                        enrichedPlort.prefab = Utility.PrefabUtils.CopyPrefab(Utility.Get<IdentifiableType>("PinkPlort").prefab);
                        enrichedPlort.prefab.name = "EnrichedPlort";
                        // enrichedPlort.prefab.AddComponent<DisappearOnSpawn>();
                        enrichedPlort.prefab.GetComponent<Identifiable>().identType = enrichedPlort;
                        enrichedPlort.prefab.GetComponent<MeshRenderer>().sharedMaterial = enrichedMaterial;
                        enrichedPlort.icon = Utility.CreateSprite(Utility.LoadImage("Files.Icons.enriched_plort_ico"));

                        inDebtPlort.prefab = Utility.PrefabUtils.CopyPrefab(Utility.Get<IdentifiableType>("PinkPlort").prefab);
                        inDebtPlort.prefab.name = "InDebtPlort";
                        // inDebtPlort.prefab.AddComponent<DisappearOnSpawn>();
                        inDebtPlort.prefab.GetComponent<Identifiable>().identType = inDebtPlort;
                        inDebtPlort.prefab.GetComponent<MeshRenderer>().sharedMaterial = inDebtMaterial;
                        inDebtPlort.icon = Utility.CreateSprite(Utility.LoadImage("Files.Icons.indebt_plort_ico"));

                        GameObject enrichedPlortParts = new GameObject("plortParts");
                        enrichedPlortParts.AddComponent<MeshFilter>().mesh = AB.shortcut_slime.LoadFromObject<MeshFilter>("plortEnrichedParts").mesh;
                        enrichedPlortParts.AddComponent<MeshRenderer>().sharedMaterial = enrichedMaterial;
                        enrichedPlortParts.transform.parent = enrichedPlort.prefab.transform;
                        enrichedPlortParts.transform.localScale *= 0.4f;

                        GameObject inDebtPlortParts = new GameObject("plortParts");
                        inDebtPlortParts.AddComponent<MeshFilter>().mesh = AB.shortcut_slime.LoadFromObject<MeshFilter>("plortEnrichedParts").mesh;
                        inDebtPlortParts.AddComponent<MeshRenderer>().sharedMaterial = inDebtMaterial;
                        inDebtPlortParts.transform.parent = inDebtPlort.prefab.transform;
                        inDebtPlortParts.transform.localScale *= 0.4f;

                        plortsToPatch.Add(new MarketUI.PlortEntry{ identType = enrichedPlort });
                        plortsToPatch.Add(new MarketUI.PlortEntry { identType = inDebtPlort });
                        valueMapsToPatch.Add(new EconomyDirector.ValueMap
                        {
                            accept = enrichedPlort.prefab.GetComponent<Identifiable>(),
                            fullSaturation = 1,
                            value = 500
                        });
                        valueMapsToPatch.Add(new EconomyDirector.ValueMap
                        {
                            accept = inDebtPlort.prefab.GetComponent<Identifiable>(),
                            fullSaturation = 1,
                            value = -500
                        });
                        #endregion

                        #region SHORTCUT_SLIME
                        shortcutSlime.prefab = Utility.PrefabUtils.CopyPrefab(Utility.Get<GameObject>("slimeCotton"));
                        shortcutSlime.prefab.name = "ShortcutSlime";

                        // shortcutSlime.prefab.AddComponent<AutoDepositAssistance>();
                        shortcutSlime.prefab.AddComponent<DestroyOutsideHoursOfDay>();
                        shortcutSlime.prefab.GetComponent<Identifiable>().identType = shortcutSlime;
                        shortcutSlime.prefab.GetComponent<SlimeEat>().slimeDefinition = shortcutSlime;
                        shortcutSlime.prefab.GetComponent<PlayWithToys>().slimeDefinition = shortcutSlime;
                        shortcutSlime.prefab.GetComponent<ReactToToyNearby>().slimeDefinition = shortcutSlime;

                        shortcutSlime.Diet = UnityEngine.Object.Instantiate(Utility.Get<SlimeDefinition>("Pink")).Diet;
                        shortcutSlime.Diet.MajorFoodGroups = new SlimeEat.FoodGroup[] { SlimeEat.FoodGroup.MEAT };
                        shortcutSlime.Diet.MajorFoodIdentifiableTypeGroups = new IdentifiableTypeGroup[] { Utility.Get<IdentifiableTypeGroup>("MeatGroup") };
                        shortcutSlime.Diet.ProduceIdents = new IdentifiableType[] { enrichedPlort };
                        shortcutSlime.Diet.RefreshEatMap(SRSingleton<GameContext>.Instance.SlimeDefinitions, shortcutSlime);

                        shortcutSlime.properties = UnityEngine.Object.Instantiate(Utility.Get<SlimeDefinition>("Pink").properties);
                        shortcutSlime.defaultPropertyValues = UnityEngine.Object.Instantiate(Utility.Get<SlimeDefinition>("Pink")).defaultPropertyValues;

                        SlimeAppearance slimeAppearance = UnityEngine.Object.Instantiate(Utility.Get<SlimeAppearance>("CottonDefault"));
                        SlimeAppearanceApplicator slimeAppearanceApplicator = shortcutSlime.prefab.GetComponent<SlimeAppearanceApplicator>();
                        slimeAppearance.name = "ShortcutDefault";
                        slimeAppearanceApplicator.Appearance = slimeAppearance;
                        slimeAppearanceApplicator.SlimeDefinition = shortcutSlime;

                        Material material = UnityEngine.Object.Instantiate<Material>(Utility.Get<SlimeAppearance>("CottonDefault").Structures[4].DefaultMaterials[0]);
                        material.hideFlags |= HideFlags.HideAndDontSave;
                        material.SetColor("_TopColor", Color.white);
                        material.SetColor("_MiddleColor", Color.white);
                        material.SetColor("_BottomColor", Color.white);

                        Material material2 = UnityEngine.Object.Instantiate<Material>(Utility.Get<SlimeAppearance>("TarrDefault").Structures[0].DefaultMaterials[0]);
                        material2.hideFlags |= HideFlags.HideAndDontSave;
                        material2.SetColor("_TopColor", Color.black);
                        material2.SetColor("_MiddleColor", Color.black);
                        material2.SetColor("_BottomColor", Color.black);

                        Material material3 = UnityEngine.Object.Instantiate<Material>(Utility.Get<SlimeAppearance>("CottonDefault").Structures[4].DefaultMaterials[0]);
                        material3.hideFlags |= HideFlags.HideAndDontSave;
                        material3.SetColor("_TopColor", Color.black);
                        material3.SetColor("_MiddleColor", Color.black);
                        material3.SetColor("_BottomColor", Color.black);

                        slimeAppearance.Structures[0].DefaultMaterials[0] = material;
                        slimeAppearance.Structures[1].DefaultMaterials[0] = material3;
                        slimeAppearance.Structures[2].DefaultMaterials[0] = material;
                        slimeAppearance.Structures[3].DefaultMaterials[0] = material2;
                        slimeAppearance.Structures[4].DefaultMaterials[0] = material3;

                        slimeAppearance.Face = UnityEngine.Object.Instantiate(Utility.Get<SlimeAppearance>("CottonDefault").Face);
                        slimeAppearance.Face.name = "ShortcutFace";

                        SlimeExpressionFace[] expressionFaces = new SlimeExpressionFace[0];
                        foreach (SlimeExpressionFace slimeExpressionFace in slimeAppearance.Face.ExpressionFaces)
                        {
                            Material slimeEyes = null;
                            Material slimeMouth = null;

                            if (slimeExpressionFace.Eyes)
                                slimeEyes = UnityEngine.Object.Instantiate(slimeExpressionFace.Eyes);
                            if (slimeExpressionFace.Mouth)
                                slimeMouth = UnityEngine.Object.Instantiate(slimeExpressionFace.Mouth);

                            if (slimeEyes)
                            {
                                slimeEyes.SetColor("_EyeRed", Color.white);
                                slimeEyes.SetColor("_EyeGreen", Color.white);
                                slimeEyes.SetColor("_EyeBlue", Color.white);
                            }
                            if (slimeMouth)
                            {
                                slimeMouth.SetColor("_MouthBot", Color.white);
                                slimeMouth.SetColor("_MouthMid", Color.white);
                                slimeMouth.SetColor("_MouthTop", Color.white);
                            }

                            slimeExpressionFace.Eyes = slimeEyes;
                            slimeExpressionFace.Mouth = slimeMouth;
                            expressionFaces = expressionFaces.AddToArray(slimeExpressionFace);
                        }
                        slimeAppearance.Face.ExpressionFaces = expressionFaces;
                        slimeAppearance.Face.OnEnable();

                        slimeAppearance.Icon = Utility.CreateSprite(Utility.LoadImage("Files.Icons.shortcut_slime_ico"));
                        slimeAppearance.SplatColor = Color.black;
                        slimeAppearance.ColorPalette = new SlimeAppearance.Palette
                        {
                            Ammo = Color.black,
                            Top = Color.black,
                            Middle = Color.white,
                            Bottom = Color.black
                        };
                        shortcutSlime.AppearancesDefault = new SlimeAppearance[] { slimeAppearance };
                        shortcutSlime.prefab.hideFlags |= HideFlags.HideAndDontSave;
                        #endregion
                        break;
                    }
                case "zoneCore":
                    {
                        SRSingleton<SceneContext>.Instance.SlimeAppearanceDirector.RegisterDependentAppearances(Utility.Get<SlimeDefinition>("Shortcut"), Utility.Get<SlimeDefinition>("Shortcut").AppearancesDefault[0]);
                        SRSingleton<SceneContext>.Instance.SlimeAppearanceDirector.UpdateChosenSlimeAppearance(Utility.Get<SlimeDefinition>("Shortcut"), Utility.Get<SlimeDefinition>("Shortcut").AppearancesDefault[0]);
                        SRSingleton<GameContext>.Instance.SlimeDefinitions.Slimes = SRSingleton<GameContext>.Instance.SlimeDefinitions.Slimes.AddItem(shortcutSlime).ToArray();
                        SRSingleton<GameContext>.Instance.SlimeDefinitions.slimeDefinitionsByIdentifiable.TryAdd(shortcutSlime, shortcutSlime);
                        break;
                    }
            }

        }
    }
}