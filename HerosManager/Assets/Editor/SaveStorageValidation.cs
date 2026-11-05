using System;
using System.IO;
using UnityEngine;

/// <summary>Runs save-system regression checks without touching the player's save directory.</summary>
public static class SaveStorageValidation
{
    public static void Run()
    {
        string directory = Path.Combine(Path.GetTempPath(), "HerosManager-save-validation-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        try
        {
            var original = Snapshot(1);
            original.hero2Stat = Array.Empty<float>();
            original.hero4Stat = new[] { 1f, 2f, 3f, 4f };
            SaveGameData snapshot = original.Clone();
            original.items[0] = 999;
            original.hero1Stat[0] = 999f;
            original.hero4Stat[0] = 999f;
            Require(snapshot.items[0] == 10 && snapshot.hero1Stat[0] == 10f && snapshot.hero4Stat[0] == 1f,
                "Captured inventories and hero stats must not share source arrays.");
            Require(snapshot.hero2Stat.Length == 0 && snapshot.hero3Stat == null && SaveGameData.IsValid(snapshot),
                "Legacy empty and missing hero snapshots must remain valid after cloning.");
            var originalOptions = new SaveOptionData { BgmVolume = 2f, EffVolume = -1f };
            SaveOptionData optionSnapshot = originalOptions.Clone();
            optionSnapshot.Normalize();
            Require(originalOptions.BgmVolume == 2f && originalOptions.EffVolume == -1f,
                "Normalizing a captured option snapshot must not alter the caller's pending settings.");

            string path = Path.Combine(directory, "save.json");
            Require(!JsonSaveStore.TrySave("", Snapshot(1), SaveGameData.IsValid, out _) &&
                    !JsonSaveStore.TryLoad("", SaveGameData.IsValid, out SaveGameData noPath, out _),
                "Empty paths must fail before touching the working directory.");
            Require(!JsonSaveStore.TryLoad(path, SaveGameData.IsValid, out SaveGameData missing, out _),
                "A missing save must not create a game.");

            var first = Snapshot(1);
            Require(JsonSaveStore.TrySave(path, first, SaveGameData.IsValid, out string error), error);
            Require(JsonSaveStore.TryLoad(path, SaveGameData.IsValid, out SaveGameData loaded, out error), error);
            Require(loaded.day == 1 && loaded.items[2] == 30 && loaded.hero1Stat[2] == 80,
                "Save fields did not round-trip.");

            Require(JsonSaveStore.TrySave(path, Snapshot(2), SaveGameData.IsValid, out error), error);
            Require(File.Exists(path + ".bak"), "Replacing a valid save must create a backup.");
            Require(JsonSaveStore.TryLoad(path, SaveGameData.IsValid, out loaded, out error) && loaded.day == 2,
                "A valid primary save must take priority over its backup.");

            File.WriteAllText(path, "{broken");
            Require(JsonSaveStore.TryLoad(path, SaveGameData.IsValid, out loaded, out error) && loaded.day == 1,
                "A corrupt primary must recover from the last valid backup.");
            Require(error == null, "Successful backup reads must not report an error.");
            File.WriteAllText(path, "{broken}");
            Require(JsonSaveStore.TryLoad(path, SaveGameData.IsValid, out loaded, out error) && loaded.day == 1,
                "Malformed JSON objects must also recover from backup.");
            Require(JsonSaveStore.TrySave(path, Snapshot(3), SaveGameData.IsValid, out error), error);
            Require(Directory.GetFiles(directory, "save.json.corrupt-*").Length == 1,
                "Saving after recovery must preserve the corrupt original.");
            Require(JsonSaveStore.TryLoad(path + ".bak", SaveGameData.IsValid, out loaded, out error) && loaded.day == 1,
                "Recovery must not replace a valid backup with corrupt data.");

            string before = File.ReadAllText(path);
            string backupBefore = File.ReadAllText(path + ".bak");
            var invalid = Snapshot(4);
            invalid.items = new int[2];
            Require(!JsonSaveStore.TrySave(path, invalid, SaveGameData.IsValid, out error),
                "Malformed inventory must be rejected.");
            Require(File.ReadAllText(path) == before, "Rejected snapshots must leave the current save unchanged.");
            Require(File.ReadAllText(path + ".bak") == backupBefore, "Rejected snapshots must leave the recovery backup unchanged.");
            string blockedDirectory = Path.Combine(directory, "blocked");
            File.WriteAllText(blockedDirectory, "This file prevents creation of a save directory.");
            Require(!JsonSaveStore.TrySave(Path.Combine(blockedDirectory, "save.json"), Snapshot(4),
                    SaveGameData.IsValid, out error) && !string.IsNullOrEmpty(error),
                "An unwritable destination must report failure.");
            invalid = Snapshot(4);
            invalid.hero1Stat[0] = float.NaN;
            Require(!SaveGameData.IsValid(invalid), "Nonfinite hero stats must be rejected.");

            File.WriteAllText(path, "{}");
            File.WriteAllText(path + ".bak", "{}");
            Require(!JsonSaveStore.TryLoad(path, SaveGameData.IsValid, out loaded, out error),
                "Empty snapshots must not become valid games.");

            string optionsPath = Path.Combine(directory, "options.json");
            File.WriteAllText(optionsPath, "{\"isWindow\":false}");
            Require(JsonSaveStore.TryLoad(optionsPath, SaveOptionData.IsValid, out SaveOptionData options, out error), error);
            Require(!options.isWindow && options.EffVolume == 1f && options.BgmVolume == 1f,
                "Missing option fields must retain their defaults.");
            options.EffVolume = -1f;
            options.BgmVolume = 2f;
            options.Normalize();
            Require(options.EffVolume == 0f && options.BgmVolume == 1f, "Volumes must be clamped.");
            options.BgmVolume = float.NaN;
            options.Normalize();
            Require(options.BgmVolume == 1f, "Nonfinite volumes must reset safely.");

            ValidateEventSnapshots(directory);
            Require(Directory.GetFiles(directory, "*.tmp-*").Length == 0, "Successful and rejected writes must not leave temporary files.");
            Debug.Log("SaveStorageValidation: all save regression checks passed.");
        }
        finally
        {
            Directory.Delete(directory, true);
        }
    }

    private static void ValidateEventSnapshots(string directory)
    {
        string path = Path.Combine(directory, "events.json");
        File.WriteAllText(path, "{\"stage\":1,\"day\":2,\"items\":[10,20,30,40,50],\"fenceLevel\":1}");
        Require(JsonSaveStore.TryLoad(path, SaveGameData.IsValid, out SaveGameData loaded, out string error), error);
        Require(!loaded.hasEventSnapshot, "Legacy JSON must use the initial event queue.");
        Require(JsonSaveStore.TrySave(path, loaded, SaveGameData.IsValid, out error), error);
        Require(JsonSaveStore.TryLoad(path, SaveGameData.IsValid, out loaded, out error), error);
        Require(!loaded.hasEventSnapshot, "Serializing a legacy null array must not mark its queue exhausted.");

        var empty = Snapshot(2);
        empty.hasEventSnapshot = true;
        empty.pendingEventIds = Array.Empty<string>();
        Require(JsonSaveStore.TrySave(path, empty, SaveGameData.IsValid, out error), error);
        Require(JsonSaveStore.TryLoad(path, SaveGameData.IsValid, out loaded, out error), error);
        Require(loaded.hasEventSnapshot && loaded.pendingEventIds != null && loaded.pendingEventIds.Length == 0,
            "An exhausted event queue must stay empty after continuing.");

        var pending = Snapshot(3);
        pending.hasEventSnapshot = true;
        pending.pendingEventIds = new[] { "Event_Norm4", "Event_Acis2" };
        SaveGameData captured = pending.Clone();
        pending.pendingEventIds[0] = "Event_Norm1";
        Require(captured.hasEventSnapshot && captured.pendingEventIds[0] == "Event_Norm4",
            "Capturing a queue must deep-copy its array and initialization marker.");
        Require(JsonSaveStore.TrySave(path, captured, SaveGameData.IsValid, out error), error);
        Require(JsonSaveStore.TryLoad(path, SaveGameData.IsValid, out loaded, out error), error);
        Require(loaded.hasEventSnapshot && loaded.pendingEventIds.Length == 2 &&
                loaded.pendingEventIds[0] == "Event_Norm4" && loaded.pendingEventIds[1] == "Event_Acis2",
            "Pending normal and follow-up events must round-trip in order.");

        var invalid = captured.Clone();
        invalid.pendingEventIds = new[] { "Event_Norm4", "Event_Norm4" };
        Require(!SaveGameData.IsValid(invalid), "Duplicate pending events must not enable repeated rewards.");
        invalid.pendingEventIds = null;
        Require(!SaveGameData.IsValid(invalid), "An explicitly initialized queue cannot be missing.");
    }

    private static SaveGameData Snapshot(int day)
    {
        return new SaveGameData
        {
            stage = 1, day = day, fenceLevel = 1,
            items = new[] { 10, 20, 30, 40, 50 },
            hero1Stat = new[] { 10f, 100f, 80f, 15f }
        };
    }

    private static void Require(bool condition, string message)
    {
        if (!condition)
            throw new InvalidOperationException("Save storage validation failed: " + message);
    }
}
