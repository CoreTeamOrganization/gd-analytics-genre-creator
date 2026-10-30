using System;
using UnityEditor;
using UnityEngine;

namespace GameDistrict.MeticaAnalytics.Editor
{
    // HOST-OWNED, never synced. Unity-dependent half of the host seam — see GenreHost.cs.
    internal static partial class GenreHost
    {
        [MenuItem("GameDistrict/Metica Analytics/Create New Genre...", false, 20)]
        private static void OpenWindow() => GenreCreatorWindow.ShowWindow();

        // Type whose public Log*Event methods are the core events the Excel import must skip.
        public static Type CoreEventsType => typeof(GDMeticaAnalytics);

        // Extra window UI drawn under the Create button.
        public static void DrawExtraSection(string genreName, Action<string, MessageType> setStatus) { }

        // Called right after the genre's analytics asset is created post-compile.
        public static void OnAnalyticsAssetCreated(string genre, ScriptableObject analyticsAsset) { }
    }
}
