using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

/// <summary>Adversarial storage checks use fake protected files, never attempted writes to real saves.</summary>
public static class SaveIsolationValidation
{
    public static void Run()
    {
        if (Application.isBatchMode && !SaveStoragePaths.HasValidationSession) RequireDirectoryDenied();
        using var scope = ValidationSaveSession.Open("SaveIsolationValidation.");
        string root = SaveStoragePaths.Directory;
        string key = SaveStoragePaths.ValidationKey;
        string sibling = Path.Combine(SaveStoragePaths.ValidationRoot, "protected-fixture-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(sibling);
        string protectedFile = Path.Combine(sibling, "SaveGameData.Json");
        const string sentinel = "DO NOT CHANGE THIS FAKE PLAYER SAVE";
        File.WriteAllText(protectedFile, sentinel);
        File.WriteAllText(protectedFile + ".bak", sentinel);
        try
        {
            string driveRoot = Path.GetPathRoot(root);
            Require(Path.IsPathFullyQualified(SaveStoragePaths.Canonical(driveRoot)) &&
                SaveStoragePaths.Canonical(driveRoot) == driveRoot && SaveStoragePaths.IsWithin(root, driveRoot),
                "A drive root must not become a drive-relative save directory.");
            var host = new GameObject("Isolated save adapter validation");
            try
            {
                var adapter = host.AddComponent<SaveDataManager>();
                adapter.LoadOptionDatas();
                Require(File.Exists(Path.Combine(root, "SaveOptionData.Json")), "Default options escaped the validation profile.");
                Require(!adapter.LoadGameDatas() && !File.Exists(Path.Combine(root, "SaveGameData.Json")),
                    "An empty validation profile imported a legacy/player game.");
            }
            finally { UnityEngine.Object.DestroyImmediate(host); }
            var options = new SaveOptionData();
            string ownFile = Path.Combine(root, "SaveOptionData.Json");
            Require(JsonSaveStore.TrySave(ownFile, options, SaveOptionData.IsValid, out var error), error);
            Require(JsonSaveStore.TryLoad(ownFile, SaveOptionData.IsValid, out SaveOptionData loaded, out error), error);
            foreach (string path in new[] { protectedFile, protectedFile.ToUpperInvariant(),
                Path.Combine(root, "..", Path.GetFileName(sibling), "SaveGameData.Json"),
                Path.Combine(root + "-other", "save.json"), "save.json", "" })
            {
                Require(!JsonSaveStore.TrySave(path, options, SaveOptionData.IsValid, out _), "Escaping save was accepted: " + path);
                Require(!JsonSaveStore.TryLoad(path, SaveOptionData.IsValid, out loaded, out _), "Escaping load was accepted: " + path);
            }
            // Policy-only checks of the real path: never send it to TrySave, even in a regression test.
            Require(!SaveStoragePaths.TryAuthorizeFile(Path.Combine(Application.persistentDataPath, "SaveGameData.Json"), out _),
                "Standalone profile must be protected even from editor code.");
            Require(SaveStoragePaths.SamePath(sibling, Path.Combine(sibling, ".") + Path.DirectorySeparatorChar),
                "Equivalent directory spellings must normalize.");

            foreach (string wrongRoot in new[] { "", Application.persistentDataPath,
                Path.Combine(root, "..", Path.GetFileName(sibling)), sibling.ToUpperInvariant(), root + "-other" })
            {
                SessionState.SetString(key + "Directory", wrongRoot);
                RequireDirectoryDenied();
                Require(!JsonSaveStore.TrySave(ownFile, options, SaveOptionData.IsValid, out _), "Missing/tampered ownership allowed a write.");
            }
            SessionState.SetString(key + "Directory", root);
            File.Move(Path.Combine(root, ".validation-owner"), Path.Combine(root, ".validation-owner-held"));
            try { RequireDirectoryDenied(); }
            finally { File.Move(Path.Combine(root, ".validation-owner-held"), Path.Combine(root, ".validation-owner")); }

            foreach (string state in new[] { "initializing", "sealed" })
            {
                SessionState.SetString(key + "State", state);
                RequireDirectoryDenied();
                Require(!JsonSaveStore.TrySave(ownFile, options, SaveOptionData.IsValid, out _), "Late callback could write after sealing.");
            }
            SessionState.SetString(key + "State", "active");
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                string link = Path.Combine(root, "linked-profile");
                CreateJunction(link, sibling);
                Require(!JsonSaveStore.TrySave(Path.Combine(link, "SaveGameData.Json"), options, SaveOptionData.IsValid, out _),
                    "A junction escaped the owned profile.");
                Require(!JsonSaveStore.TryLoad(Path.Combine(link, "SaveGameData.Json"), SaveOptionData.IsValid, out loaded, out _),
                    "A junction exposed protected saves.");
            }
            Require(File.ReadAllText(protectedFile) == sentinel && File.ReadAllText(protectedFile + ".bak") == sentinel &&
                Directory.GetFiles(sibling).Length == 2, "Rejected I/O changed a protected primary, backup or sidecar.");
            Debug.Log("SAVE ISOLATION VALIDATION PASSED: ownership, aliases, missing paths, late writes and linked directories.");
        }
        finally
        {
            SessionState.SetString(key + "Directory", root);
            SessionState.SetString(key + "State", "active");
            // Keep all fixtures for review. Never recursively delete a possibly linked directory.
        }
    }

    static void RequireDirectoryDenied()
    {
        try { _ = SaveStoragePaths.Directory; }
        catch (Exception e) when (e is ArgumentException || e is InvalidOperationException) { return; }
        throw new Exception("Unsafe validation path fell back to a usable profile.");
    }

    static void CreateJunction(string link, string target)
    {
        string command = "$ErrorActionPreference = 'Stop'; New-Item -ItemType Junction -Path '" +
            link.Replace("'", "''") + "' -Target '" + target.Replace("'", "''") + "' | Out-Null";
        using var process = Process.Start(new ProcessStartInfo("powershell.exe",
            "-NoProfile -NonInteractive -EncodedCommand " + Convert.ToBase64String(Encoding.Unicode.GetBytes(command)))
        { UseShellExecute = false, CreateNoWindow = true, WindowStyle = ProcessWindowStyle.Hidden });
        if (!process.WaitForExit(10000) || process.ExitCode != 0) throw new Exception("Could not create junction fixture.");
    }

    static void Require(bool value, string message) { if (!value) throw new InvalidOperationException(message); }
}
