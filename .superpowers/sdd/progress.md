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
