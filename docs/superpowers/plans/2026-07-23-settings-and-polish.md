# Downshift Settings & Polish Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: superpowers:subagent-driven-development. Checkbox steps.

**Goal:** Player-facing completeness: SFX/music toggles that persist, an in-run pause menu, a NEW BEST celebration on results, and an app icon.

**Constraints:** as previous plans. Suites at start (post-D2): EditMode 56, PlayMode 16.

---

### Task E1: Persistent audio settings

**Files:** SaveModel, GameAudio, MenuController/MenuBuilder, SaveTests additions.

- SaveModel gains `public bool sfxMuted;` and `public bool musicMuted;` (mute-semantics so JsonUtility's default-false on old saves means sound stays ON — non-obvious constraint, justified comment allowed on these two fields only if needed).
- GameAudio: respects `GameSession.Save.sfxMuted` (engine/squeal/one-shot volumes forced 0) and `musicMuted` (music source). Poll in Update (cheap) — no event plumbing needed.
- Menu: two toggle buttons bottom-left ("SFX ON/OFF", "MUSIC ON/OFF") — MenuController methods `ToggleSfx()`/`ToggleMusic()` flip + Persist + refresh labels; MenuBuilder wires persistent listeners; labels reflect state in Refresh().
- EditMode tests: SaveModel roundtrip with mutes; legacy-save JSON (hand-written string without the new fields) loads with both false.
- Suites: EditMode 58+, PlayMode 16. Commit: `add persistent sfx and music mute settings with menu toggles`

### Task E2: In-run pause menu

**Files:** HudController/HudBuilder, RunManager, PlayMode test.

- RunManager: `public bool IsPaused { get; private set; }` + `TogglePause()` — only valid while Descending: pause sets Time.timeScale 0 + IsPaused, resume restores 1. Restart/ToMenu/Results all clear pause state implicitly (they already set timeScale). Escape key toggles too.
- HUD: pause button top-right (⏸ as "II" text) → TogglePause persistent listener; PausePanel (mirrors results panel construction): "PAUSED" title + RESUME (TogglePause) + RESTART + MENU buttons; HudController shows/hides on a new RunManager event or by polling IsPaused in Update (poll — simplest).
- PlayMode test: TogglePause → timeScale 0 & panel active; TogglePause → timeScale 1 & panel hidden; NotifyCrash while paused not required (pause only valid Descending; crash can't occur at timeScale 0).
- Suites: EditMode 58+, PlayMode 17. Commit: `add in-run pause menu with resume restart and menu`

### Task E3: NEW BEST banner + app icon

**Files:** HudController/HudBuilder, icon generator in editor tools, ProjectSettings icon.

- Results panel: "NEW BEST!" label (gold, above distance) — HudController activates it when `runManager.IsNewBest` on the Results StateChanged callback.
- App icon: editor tool generates 512x512 PNG procedurally (dawn-gradient background, stylized white downhill road zigzag, red car dot — simple bold shapes readable at 48px), saves Assets/Art/icon.png, assigns via PlayerSettings.SetIcons for Unknown platform (default). Menu item `Downshift/Build/App Icon`.
- PlayMode test: banking flow with a fresh save asserts the banner active on results (IsNewBest true on first run always).
- Suites: EditMode 58+, PlayMode 18. Commit: `add new best banner and generated app icon`

## Out of scope
- Custom splash scene (Unity default splash stays for now), language/localization, credits screen (add when music attribution decided).
