# Changelog

Version numbers follow [SemVer](https://semver.org/): **MAJOR** = breaking change to an existing
call site, **MINOR** = new backward-compatible API, **PATCH** = fix with no API change.

Pin your project's `Packages/manifest.json` to a released tag —
`https://github.com/CoreTeamOrganization/gd-analytics-genre-creator.git#vX.Y.Z` — never track
`main` directly. `main` can carry changes ahead of what's released and documented here.

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
