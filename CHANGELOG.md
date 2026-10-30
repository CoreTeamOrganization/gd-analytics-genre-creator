# Changelog

Version numbers follow [SemVer](https://semver.org/): **MAJOR** = breaking change to an existing
call site, **MINOR** = new backward-compatible API, **PATCH** = fix with no API change.

Pin your project's `Packages/manifest.json` to a released tag —
`https://github.com/CoreTeamOrganization/gd-analytics-genre-creator.git#vX.Y.Z` — never track
`main` directly. `main` can carry changes ahead of what's released and documented here.

## [Unreleased]

### Changed
- Internal refactor, no behaviour change: everything that differs between this package and the GDMonetization
  SDK now lives in `Editor/GenreCreator/GenreHost.cs` + `GenreHost.Editor.cs`; the rest of `Editor/GenreCreator`
  is shared core, vendored into the SDK by `tools~/sync-to-sdk.sh` (see `.claude/skills/sync-genre-creator`).
  Generated genre code is byte-identical (checked by `tools~/golden/golden.sh`).
- The window's menu item moved from `GenreCreatorWindow` to `GenreHost.Editor.cs`; the menu path is unchanged.

## [2.1.0] - 2026-10-30

### Added
- `UpdateFirebaseInfo(appInstanceId)` — every event now carries a `firebaseAppInstanceId` param
  (empty string until set). The genre-creator wizard skips it as a reserved param.

## [2.0.0] - 2026-09-21

### Changed (BREAKING)
- `LogPerfStatsEvent` / `LogLoadTimeEvent` now fetch the GD Performance Tracker payload
  internally (`GDPerformance.ConsumePerfPayload()` / `GetStartupPayload()`) instead of requiring
  the caller to fetch and pass it in. **If your game already fetches one of those and passes the
  result straight into these methods, remove that fetch before upgrading** — the double-fetch
  will silently drop `perfStats` events, since `ConsumePerfPayload()` resets its recording window
  on every call.

### Added
- `ConfigurePerformanceTracking(perfEnabled, sampleIntervalSeconds, startupEnabled)` — forwards
  to `GDPerformance.Configure`.
- `MarkGameInteractive()` — forwards to `GDPerformance.MarkGameInteractive`.
- `GD_PERFORMANCE_TRACKER` scripting define, added automatically to Android/iOS when
  [GD Performance Tracker](https://github.com/CoreTeamOrganization/GDPerformanceTracker.git) is
  detected (same mechanism as `METICA_ANALYTICS`).
- Editor popup offering to install GD Performance Tracker via git URL when it's missing.

## [1.0.0] - 2026-09-07

- Initial standalone UPM package release (untagged at the time — tagged retroactively as the
  baseline for 2.0.0).
