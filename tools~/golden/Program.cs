// Golden-output harness: prints everything the Genre Creator generator emits for a fixed sample.
// Built by golden.sh against whichever host's files are passed in; the output is diffed against *.expected.txt.
using System;
using System.Collections.Generic;
#if GOLDEN_SDK
using E = GDMonetization.Editor;
#else
using E = GameDistrict.MeticaAnalytics.Editor;
#endif

static class Golden
{
    static void Emit(string name, string content) =>
        Console.Write($"===== {name}\n{content.Replace("\r\n", "\n")}\n");

    static void Main()
    {
        var events = new List<E.GenreEventDef>
        {
            new E.GenreEventDef("LevelStarted", new List<E.GenreFieldDef>
            {
                new E.GenreFieldDef("Level", E.GenreFieldType.Int),
                new E.GenreFieldDef("LevelType", E.GenreFieldType.String),
            }),
            new E.GenreEventDef("LevelFinished", new List<E.GenreFieldDef>
            {
                new E.GenreFieldDef("Success", E.GenreFieldType.Bool),
                new E.GenreFieldDef("Duration", E.GenreFieldType.Float),
                new E.GenreFieldDef("Reward", E.GenreFieldType.String, "reward_Amount"),
            }),
            new E.GenreEventDef("AppBackgrounded", new List<E.GenreFieldDef>()),
        };

        foreach (var g in new[] { "Arcade", "puzzleGame" })
        {
            Emit($"{g}/Interface", E.GenreCodeGenerator.Interface(g, events));
            Emit($"{g}/Data", E.GenreCodeGenerator.Data(g, events));
            Emit($"{g}/Implementation", E.GenreCodeGenerator.Implementation(g, events));
#if GOLDEN_PRE
  #if GOLDEN_SDK
            Emit($"{g}/Extra {g}AnalyticsSO.cs", E.GenreCodeGenerator.ScriptableObject(g));
  #endif
#else
            foreach (var (file, content) in E.GenreHost.ExtraFiles(g))
                Emit($"{g}/Extra {file}", content);
#endif
        }
    }
}
