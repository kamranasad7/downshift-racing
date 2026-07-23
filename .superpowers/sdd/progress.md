# Downshift core-prototype execution ledger

Plan: docs/superpowers/plans/2026-07-21-core-prototype.md
Branch: core-prototype

Task 1: complete (commits f89601f..ef1f6c7, spec pass, quality approved)
  Minor (for final review): run-tests.ps1 naive string version sort; implicit exit 0; redundant autorotate flags
  Deferred verify: Unity GUI open (naturally exercised in Task 2); run-tests exit-1 branch (exercised by Task 2 red step)
Task 2: complete (commits ef1f6c7..6be7c56, spec pass, quality approved)
Task 3: complete (commits 3d5e608..e1f8ed0, spec pass, quality approved)
Task 4: complete (commits 0a94484..f7943e6, spec pass, quality approved)
Task 5: complete (commits ae25034..951afe5, spec pass, quality approved)
  Minor (process): red-run evidence in report mis-transcribed (green 15/15 corroborated by editmode.xml); hysteresis ordering untestable by design (plan-inherent)
Task 6: complete (commits 1bb9515..1ae1c62 + fix ad2c2b9, spec pass, quality approved, post-cap invariant coverage added)
Task 7: complete (commits 79feceb..36894dc, spec pass, quality approved; playmode smoke test infra added)
  Minor (folded into Task 8): PlayMode asmdef missing includePlatforms Editor; wheel-attach assert near-tautological
Task 8: complete (commits 50f9e2b..ca9ad63 + fix 65aff8c, spec pass, quality approved after comment strip)
  Ratified: playmode thresholds freeSpeed>1, brakeTemp>0.1, roll>+2 (measured physics: gear-0 engine braking caps free roll ~1.45 m/s)
  For balancing (morning): starter car gear-0 engine braking may be too strong (engineBrakeTorque 260, ratio 3.2, final 3.7)
Task 9: complete (commits c06b7a0..8c62e16, spec pass, quality approved; asmdef SpriteShape refs adjudicated necessary+minimal)
Task 10: complete (commits e32764c..2f40e3a, spec pass, quality approved; activeInputHandler 1->2 fix adjudicated necessary)
  Forward: Task 13 MUST filter trigger-vs-trigger in RoofCrashDetector (coin false-crash risk); input-system-vs-legacy decision for morning; Blown event no unsubscribe (benign)
  Morning checklist: R-key restart + scene reload manual verify
Task 11: complete (commits ada9c23..710be55, spec pass, quality approved; Cinemachine asmdef refs necessary+minimal)
Task 12: complete (commits d381bd7..447fe62 + fix 33295b0, spec pass after critical fix: hud button onClick now persistent listeners)
  Plan patched at source (HudBuilder persistent listeners; RoofCrashDetector trigger filter for task 13)
  Morning checklist: visual HUD layout, thumb reachability, button clicks in real Play mode
Task 13: complete (commits ab27ab7..1e01ff8, spec pass, quality approved; isTrigger guard verified)
  Minor (plan-inherent, for progression plan): collected pickups respawn if a chunk is rebuilt after backward roll (double-count coins edge case)
Final review: 2 Critical + 3 Important found across task seams; all fixed in 82c6dd7; re-verified VERIFIED per finding. Ready to merge.
  Correction: T7 playmode asmdef includePlatforms was NOT applied in T8 (deliberately reverted - it reclassifies PlayMode tests); ledger note above was inaccurate.
  Deferred to next plans: version-sort hardening, test TearDown hygiene, script dedup, DistanceM spawn offset, post-crash creep, gradePerMeter=0 guard, station integration test at real spawn offset.
Task 14: PENDING USER (playtest checklist written to docs/superpowers/playtest-checklist.md; tag v0.1-prototype after)
--- OVERNIGHT 2 (progression + visual + cleanup) ---
Task A1: complete (6c12272 + meta fix, spec pass, quality approved after meta commit)
Task A2: complete (93519f7, spec pass, quality approved)
  Note for A3 dispatch: ApplyTo clone ownership — destroy previous clone or accept per-run allocation (document)
Task A3: complete (847d711 + flake fix 74261b5, spec pass, quality approved; 3x playmode stability verified 10/10)
Task A4: complete (18ea7da, spec pass with accepted deviation: build-settings rewrite lives in MenuBuilder (more robust), quality approved; menu screenshot verified)
Task A5: complete (0e9d51f, spec pass, quality approved; 2x 11/11 stability, scene-leak sweep verified)
Task A6/Plan A wrap: EditMode 41, PlayMode 11 all green; screenshots deferred to morning report (post-visual-pass)
Task B1: complete (7816af5 + fix, spec pass after comment strip, quality approved; sky/parallax verified by screenshot incl 28s stress)
  Implementer also fixed: sky sprite scale math, hills vertical recentering (drift bug)
  Observation logged: HUD speed/rpm/distance readouts questioned during fast descent (pre-existing; check in final review)
Task B2: complete (e0e0f75, spec pass, quality approved; sRGB/linear vertex-color gotcha found+fixed, justified comment allowed; pixel-verified)
Task B3: complete (ee7191e, spec pass, quality approved; fixed particle Play() and saved-material-asset bugs found via screenshots; 41/12 green)
Task B4: complete (219bcc1, spec pass, quality approved; collider invariants byte-verified; PLAN B COMPLETE)
Task C1: complete (5430d14 + test-teeth fix f76b463, spec pass, quality approved; terrain guard deviation adjudicated correct)
Task C2: complete (cea34a6 + script fix, spec pass, quality approved after exit-code propagation fix; 42/13 green; PLAN C COMPLETE)
Final overnight review: 2 Critical (test save pollution w/ real-file evidence; TIRES upgrade runtime no-op) + 1 Important + 2 Minor — ALL FIXED in 59f6dd0, per-fix VERIFIED. Suites 42 EditMode / 14 PlayMode green. Phantom save deleted. READY FOR MORNING PLAYTEST.
Economy note for user: first upgrade ~4-7 runs at defaults; lower baseUpgradeCost in Assets/Resources/Economy.asset to feel a purchase sooner.
Audio task: complete (409e137 + settings 0f9742b + test fix, spec pass, quality approved; loop-seam test now measures real wrap discontinuity)
  QUEUED for next playmode window: pause-fix suite run + audible check of engine/squeal/one-shots
--- NIGHT 3 (hazards, settings/polish, android, prep) ---
Recovery note: verification agent misfire killed + orphaned suite aborted; pause/audio verification folded into D2 suite run
Task D1: complete (6d52e5d, spec pass, quality approved; TDD caught station-scan-radius bug; reviewer mutation-verified)
  Minor folded into D2: make NothingBeforeHazardStartMeters non-vacuous (fixture hazardStartMeters 500)
Task D2: complete (6d3ddb2, spec pass, all screenshots verified; rocks retuned to real chassis clearance; 56/16 green)
  D2 exposed timescale leak across playmode tests (pause fix side effect) -> fixed in PlayModeCleanup TearDown (validated by E1 suite run)
  Pause fix + audio verification: covered by D2's full 16/16 playmode run (both features' tests green)
Task E1: complete (1d79bb1, 58/58 + 16/16; timescale-leak fix confirmed live by full-suite completion). E1-E3 get one combined review after E3.
D2 review: spec pass, quality approved (burying deviation adjudicated sound via wheel-radius reasoning; no action items)
Task E2: complete (3e017b6, 58/58 + 17/17; pause menu wired, FullLoop confirms rebuilt scene end-to-end)
Task E3: complete (2a965ce, 58/58 + 18/18; icon assignment verified via GetIcons readback; PLAN E COMPLETE)
E1-E3 combined review: all SPEC pass, quality good, no blockers. Queued micro-hardening (apply after android build): reset IsPaused in NotifyCrash/OnBlown + gate pause panel on IsPaused && Descending. Reviewer note: E1 diff was missing from the package base (pulled via git show) - mind package ranges.
--- ANDROID + FINAL (night 3) ---
Android APK: SUCCESS. Builds/downshift-debug.apk 47MB, com.asadkamran.downshift, IL2CPP/ARM64, minSdk24, dev build. Platform switched back to StandaloneWindows64. (build agent parked repeatedly on waits; controller drove the build watch + cleanup + commit directly)
Input: activeInputHandler 2->0 (legacy only) - resolves Android "Both" warning; needs one editor restart to fully apply. Nothing uses new Input System.
Late fixes committed (1047d6c build infra, 2f2e560 pause hardening+input). EditMode 58 green post-fix.
DEFERRED to user window: PlayMode 18 re-run after the 2-line pause fix + input handler change (low risk, EditMode green).
Final whole-night review: running.
