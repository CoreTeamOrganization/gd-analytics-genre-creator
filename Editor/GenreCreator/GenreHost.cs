using System;
using System.Collections.Generic;

namespace GameDistrict.MeticaAnalytics.Editor
{
    // HOST-OWNED, never synced. The Genre Creator core (every other file in this folder) is shared source:
    // it is vendored into the GDMonetization SDK by tools/sync-to-sdk.sh. Everything that differs between hosts
    // lives in this file and GenreHost.Editor.cs. Keep this file free of UnityEngine/UnityEditor so
    // tools/golden/golden.sh can compile it standalone.
    internal static partial class GenreHost
    {
        // Namespace of the generated genre code, and what its interface derives from (e.g. " : IGDMeticaAnalytics").
        public const string Ns            = "GameDistrict.MeticaAnalytics";
        public const string InterfaceBase = "";

        public const string ScriptsBase     = "Assets/MeticaGenres";
        public const string ResourcesPath   = "Assets/MeticaGenres/Resources";
        public const string PendingGenreKey = "GDMeticaAnalytics_PendingGenre";
        public const string LogTag          = "[MeticaGenreCreator]";

        // [CreateAssetMenu] prefix on the generated implementation class.
        public const string ImplAssetMenu = "MeticaAnalytics/";

        public const float  MinWindowHeight = 540;
        public const string AfterCreateHelp =
            "After Unity recompiles, the assets are created automatically at Assets/MeticaGenres/Resources.";

        // Extra lines emitted at the top of the generated implementation class body.
        public static IEnumerable<string> ImplMembers(string genre) => new[]
        {
            $"        private static {genre}Analytics _instance;",
            $"        public static {genre}Analytics Instance =>",
            $"            _instance != null ? _instance : (_instance = Resources.Load<{genre}Analytics>(\"{genre}Analytics\"));",
            "",
        };

        // Additional generated files per genre as (file name, content).
        public static IEnumerable<(string File, string Content)> ExtraFiles(string genre) =>
            Array.Empty<(string, string)>();

        public static string DoneMessage(string genre, string analyticsPath) =>
            $"'{genre}' scripts and asset are ready.\n\n" +
            $"Asset: {analyticsPath}\n\n" +
            "1. Assign the asset to your game.\n" +
            "2. Fill in App Id, Api Key.\n" +
            "3. Call analytics.Initialize() once at startup.";
    }
}
