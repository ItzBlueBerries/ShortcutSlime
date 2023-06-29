using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ShortcutSlimes
{
    internal class AB
    {
        public static byte[] GetAsset(string path)
        {
            Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(Assembly.GetExecutingAssembly().GetName().Name + "." + path);
            byte[] array = new byte[manifestResourceStream.Length];
            manifestResourceStream.Read(array, 0, array.Length);
            return array;
        }

        internal static AssetBundle shortcut_slime = AssetBundle.LoadFromMemory(GetAsset("Files.ABs.shortcut_slime"));
    }
}
