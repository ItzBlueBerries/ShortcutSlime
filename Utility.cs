using Il2Cpp;
using System.Reflection;
using static ShortcutSlimes.Utility;
using UnityEngine;

namespace ShortcutSlimes
{

    internal class Utility
    {
        public static class PrefabUtils
        {
            public static Transform DisabledParent;
            static PrefabUtils()
            {
                DisabledParent = new GameObject("DeactivedObject").transform;
                DisabledParent.gameObject.SetActive(false);
                UnityEngine.Object.DontDestroyOnLoad(DisabledParent.gameObject);
                DisabledParent.gameObject.hideFlags |= HideFlags.HideAndDontSave;
            }

            public static GameObject CopyPrefab(GameObject prefab)
            {
                var newG = UnityEngine.Object.Instantiate(prefab, DisabledParent);
                return newG;
            }
        }

        public static Texture2D LoadImage(string filename)
        {
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            Stream manifestResourceStream = executingAssembly.GetManifestResourceStream(executingAssembly.GetName().Name + "." + filename + ".png");
            byte[] array = new byte[manifestResourceStream.Length];
            manifestResourceStream.Read(array, 0, array.Length);
            Texture2D texture2D = new Texture2D(1, 1);
            ImageConversion.LoadImage(texture2D, array);
            texture2D.filterMode = FilterMode.Bilinear;
            return texture2D;
        }

        public static Sprite CreateSprite(Texture2D texture) => Sprite.Create(texture, new Rect(0f, 0f, (float)texture.width, (float)texture.height), new Vector2(0.5f, 0.5f), 1f);

        public static class Spawner
        {
            public static void ToSpawn(string name) => SRBehaviour.InstantiateActor(Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(x => x.name == name), SRSingleton<SceneContext>.Instance.RegionRegistry.CurrentSceneGroup, SRSingleton<SceneContext>.Instance.Player.transform.position, Quaternion.identity);
        }

        public static T Get<T>(string name) where T : UnityEngine.Object
        {
            return Resources.FindObjectsOfTypeAll<T>().FirstOrDefault(found => found.name.Equals(name));
        }
    }

    internal static class Extensions
    {
        public static void RandomizeList<T>(this List<T> list)
        {
            int count = list.Count;
            for (int i = 0; i < count; i++)
            {
                int index = UnityEngine.Random.Range(i, count);
                var value = list[i];
                list[i] = list[index];
                list[index] = value;
            }
        }

        public static T RandomObject<T>(this List<T> list)
        {
            List<T> list2 = new List<T>();
            list2.AddRange(list);
            list2.RandomizeList();
            return list2.FirstOrDefault();
        }

        public static T LoadFromObject<T>(this AssetBundle bundle, string name) where T : UnityEngine.Object
        { return bundle.LoadAsset(name).Cast<GameObject>().GetComponentInChildren<T>(); }
    }
}
