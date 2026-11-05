# Repository working guide

## Project and scope

- Open `HerosManager/` with Unity **6000.4.7f1**. `ff/` is a separate historical sample project.
- Read `docs/ARCHITECTURE.md` for runtime responsibilities and serialization contracts.
- Own game code lives in `HerosManager/Assets/02.Scripts` and `Assets/TutorialMgr.cs`.
- Preserve MonoBehaviour class names, serialized field names, scene callbacks, and existing `.meta` GUIDs. Renaming a serialized field requires an explicit migration.
- Preserve unrelated edits. Do not add packages or change external asset code without task-specific need and authorization.

## Maintainable changes

- Keep deterministic game rules independent of MonoBehaviour, UI, Resources, and global random state. Pass random samples into rules; apply their results in scene adapters.
- Use `LoadGameData.Catalog` for stage/day/group queries. Treat loaded table rows as read-only. Add reference checks to `GameDataRules` when introducing a required table link.
- Use `ItemManager.TrySpend` for purchases/actions. Multiple costs must validate and commit together. `AddItem` is for rewards or clamped damage, not a replacement for affordability checks.
- Normalize legacy absent-value sentinels at table adapters. Reject invalid negative costs for actual items.
- Capture a detached save snapshot, update it, and call `TrySaveGameSnapshot`. Transition to a new game only after saving succeeds.
- Make close, stop, cancel, and completion paths safe to call twice. Remove owned listeners and stop owned tweens/coroutines when their owner goes away.
- Use `MiniGameSetMgr.Spawn` for mini-game instances. Bind the spawned object, never mutate a prefab asset to inject a manager.
- Keep unfinished content explicit. Do not invent game balance, rewards, or ending behavior while refactoring.

## Validation

Close Unity for this project before running the command-line checks. Run only one Unity process against this project at a time.

```powershell
./tools/Validate-Unity.ps1 -Check Rules
./tools/Validate-Unity.ps1 -Check Save
./tools/Validate-Unity.ps1 -Check Project
./tools/Validate-Unity.ps1 -Check Play
./tools/Validate-Unity.ps1 -Check Build
```

- Use the checks relevant to the changed behavior; broad game-flow changes require Play and Build as well as the affected rules tests.
- Runtime smoke tests must not overwrite real user saves. Storage tests use temporary directories.
- Preserve tests of behavior boundaries, failure atomicity, actual scene connections, and repeated actions; avoid tests that only mirror implementation.
- Report failed/unrun checks and remaining content limitations. Logs and builds are generated under `HerosManager/Logs` and `HerosManager/Builds` and are not committed.
- Commit or push only within the user's requested scope. Keep commit messages within three lines.
