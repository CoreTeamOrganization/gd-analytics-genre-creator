using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;

namespace GameDistrict.MeticaAnalytics.Editor
{
    // Adds METICA_ANALYTICS to Android and iOS when Metica SDK is installed.
    [InitializeOnLoad]
    internal static class MeticaSymbolInstaller
    {
        private const string Symbol = "METICA_ANALYTICS";

        static MeticaSymbolInstaller()
        {
            if (!IsMeticaSdkInstalled()) return;
            AddSymbol(NamedBuildTarget.Android);
            AddSymbol(NamedBuildTarget.iOS);
        }

        private static bool IsMeticaSdkInstalled()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                if (assembly.GetName().Name == "Metica.SDK")
                    return true;
            return false;
        }

        private static void AddSymbol(NamedBuildTarget target)
        {
            PlayerSettings.GetScriptingDefineSymbols(target, out string[] symbols);
            if (symbols.Contains(Symbol)) return;
            var list = new List<string>(symbols) { Symbol };
            PlayerSettings.SetScriptingDefineSymbols(target, list.ToArray());
        }
    }
}