---
name: sync-genre-creator
description: Sync the shared Genre Creator wizard core from this repo into the GDMonetization SDK checkout, verify nothing changed for existing games, and review host drift. Use when the wizard core (Editor/GenreCreator/*.cs except GenreHost*) changed here, before an SDK release, or when asked to "sync / vendor / check the genre creator in the SDK".
---

# Sync Genre Creator → GDMonetization SDK

This repo is the single source of truth for the wizard core. The SDK ships as a `.unitypackage`, so it carries a
**vendored copy**. The copy is produced by a script, never by hand — your job is to run it, verify it, and review
the parts a script can't judge.

Shared core = `GenreAssetCreator, GenreCodeGenerator, GenreCreatorWindow, GenreDefinition, GenreExcelParser,
IdentifierValidator`. Host-owned (never synced, differ per repo) = `GenreHost.cs`, `GenreHost.Editor.cs`.

## Steps — stop and report at the first failure

1. **Locate the SDK checkout** (usually `../Monetization-SDK-Unity`). Work on a branch there
   (`uzair/GDSDK-{ticket}-…`); never sync onto `main` or a dirty release branch. Ask the user if unsure.
2. **Drift check:** `tools~/sync-to-sdk.sh <sdk> --check`. Exit 0 → nothing to do, say so and stop.
3. **Sync:** `tools~/sync-to-sdk.sh <sdk>`. Show `git diff --stat` in the SDK. Never edit synced files by hand, and
   never touch `.meta` files (GUIDs must stay stable for existing SDK users).
4. **Golden output** (proves generated genre code is byte-identical, so existing games are unaffected):
   - `tools~/golden/golden.sh standalone Editor/GenreCreator | diff tools~/golden/standalone.expected.txt -`
   - `tools~/golden/golden.sh sdk <sdk>/Assets/GDMonetization/Editor/Scripts/GenreCreator | diff tools~/golden/sdk.expected.txt -`
   A diff is only acceptable if the change was intended: then regenerate the matching `*.expected.txt`, and call out
   the generated-code change to the user explicitly — it will alter every game's next regeneration.
5. **Compile in Unity** (the golden harness covers only the pure files): with the Unity MCP run `refresh_unity`, then
   `read_console` for errors. If Unity isn't open on the SDK project, say compile was NOT verified.
6. **Host parity review** (the judgement part):
   - Compare the `Log*Event` methods of `IGDMeticaAnalytics` (SDK) and `GDMeticaAnalytics` (here). A core event
     present in one but not the other means a runtime drift — report it, don't paper over it.
   - Params BOTH runtimes add to every event themselves (adid, appToken, firebaseAppInstanceId, …) belong in `GenreExcelParser.SkipParams`. If only one runtime sends a new param, port it to the other runtime first — otherwise the shared skip list makes the other host silently drop that Excel column.
   - Did the change need a new seam member in `GenreHost`? Then both hosts' files must get it — update this repo's
     and tell the user the SDK's needs the same (it fails to compile otherwise).
7. **Release notes:** add the entry to `CHANGELOG.md` (here) and tell the user which SDK release will carry
   "genre-creator core vX.Y.Z" (the synced file header records the version).

## Guardrails
- Don't commit, push or open a PR unless the user asks. Report the SDK branch and the changed files.
- Behavioural difference the user must hear about: none is expected from a pure sync. If the golden or compile step
  disagrees, that is a finding, not something to fix by editing generated output or synced files.
- Don't "improve" the shared core while syncing. Core changes are made here first, in their own change.
