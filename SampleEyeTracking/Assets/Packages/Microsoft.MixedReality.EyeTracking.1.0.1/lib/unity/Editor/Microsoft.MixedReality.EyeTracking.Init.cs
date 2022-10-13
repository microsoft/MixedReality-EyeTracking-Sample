using System;
using System.IO;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.EyeTracking
{
    [InitializeOnLoad]
    internal class Init : MonoBehaviour
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern int AddDllDirectory(string lpPathName);

        [DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetDefaultDllDirectories(uint directoryFlags);

        const uint LOAD_LIBRARY_SEARCH_DEFAULT_DIRS = 0x00001000;

        static Init()
        {
            if (!SetDefaultDllDirectories(LOAD_LIBRARY_SEARCH_DEFAULT_DIRS))
            {
                Debug.LogError(string.Format("Failed to SetDefaultDllDirectories: Win32 error {0}", Marshal.GetLastWin32Error()));
                return;
            }

            // Find the path to this script
            string assetName = $"{typeof(Init).FullName}.cs";
            var assets = AssetDatabase.FindAssets(Path.GetFileNameWithoutExtension(assetName));
            if (assets.Length != 1)
            {
                Debug.LogError(string.Format("Failed to find single asset for {0}; found {1} instead!", assetName, assets.Length));
                return;
            }

            char[] delims = { '/', '\\' };
            var assetDirectoryPath = Application.dataPath;
            var lastDelim = assetDirectoryPath.TrimEnd(delims).LastIndexOfAny(delims); // trim off Assets folder since it's also included in asset path
            var dllDirectory = Path.Combine(assetDirectoryPath.Substring(0, lastDelim), Path.GetDirectoryName(AssetDatabase.GUIDToAssetPath(assets[0]))).Replace('/', '\\');
            dllDirectory = Path.Combine(dllDirectory.Substring(0, dllDirectory.LastIndexOf("Editor")), @"x64");
            if (AddDllDirectory(dllDirectory) == 0)
            {
                Debug.LogError(string.Format("Failed to set DLL directory {0}: Win32 error {1}", dllDirectory, Marshal.GetLastWin32Error()));
                return;
            }

            Debug.Log(string.Format("Added DLL directory {0} to the user search path.", dllDirectory));
        }
    }
}