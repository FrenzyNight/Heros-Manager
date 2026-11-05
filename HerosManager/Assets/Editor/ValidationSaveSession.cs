using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;

/// <summary>Owns validation saves across reloads, cancellation and late callbacks.</summary>
[InitializeOnLoad]
public static class ValidationSaveSession
{
    const string Key = SaveStoragePaths.ValidationKey;

    static ValidationSaveSession()
    {
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
    }

    public static string Begin(string owner)
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
            throw new InvalidOperationException("Start validation outside Play mode.");
        string directory = SaveStoragePaths.BeginValidation();
        SessionState.SetString(Key + "Owner", owner);
        SessionState.SetString(Key + "Snapshot", ProtectedSnapshot());
        SessionState.SetBool(Key + "Verified", false);
        Debug.Log("VALIDATION SAVE PROFILE: " + directory);
        return directory;
    }

    public static IDisposable Open(string owner)
    {
        Begin(owner);
        return new Scope();
    }

    sealed class Scope : IDisposable
    {
        bool disposed;
        public void Dispose() { if (!disposed) { disposed = true; Complete(); } }
    }

    public static void RequireActive() => SaveStoragePaths.RequireValidationDirectory();
    public static bool IsOwner(string owner) => SaveStoragePaths.HasValidationSession &&
        SessionState.GetString(Key + "Owner", "") == owner;

    public static void CompleteRunner(string owner)
    {
        try { Complete(); }
        catch (Exception exception)
        {
            SessionState.SetInt(owner + "ExitCode", 1);
            Debug.LogException(exception);
        }
    }

    public static void Complete()
    {
        try
        {
            if (SessionState.GetBool(Key + "Verified", false)) return;
            if (!SaveStoragePaths.HasValidationSession)
                throw new InvalidOperationException("Validation session is missing.");
            string expected = SessionState.GetString(Key + "Snapshot", "");
            if (expected == "" || expected != ProtectedSnapshot())
                throw new InvalidOperationException("Protected save files changed during validation. Preserve evidence; do not restore automatically.");
            SessionState.SetBool(Key + "Verified", true);
            Debug.Log("PROTECTED SAVES UNCHANGED: player, editor and legacy file lists, SHA256 and write times.");
        }
        finally { SaveStoragePaths.SealValidation(); }
    }

    static void OnPlayModeChanged(PlayModeStateChange state)
    {
        string owner = SessionState.GetString(Key + "Owner", "");
        if (state == PlayModeStateChange.ExitingEditMode && !Application.isBatchMode &&
            SessionState.GetString(Key + "State", "") == "sealed" && SessionState.GetBool(Key + "Verified", false) &&
            !SessionState.GetBool(owner + "Active", false) && !SessionState.GetBool(owner + "Exiting", false))
        {
            SessionState.EraseString(Key + "State");
            SessionState.EraseString(Key + "Directory");
        }
        if (state != PlayModeStateChange.EnteredEditMode || !SaveStoragePaths.HasValidationSession) return;
        // A manual Stop is a failed run, not permission for the old runner to restart later.
        if (SessionState.GetBool(owner + "Active", false))
        {
            SessionState.SetBool(owner + "Active", false);
            SessionState.SetBool(owner + "Exiting", true);
            SessionState.SetInt(owner + "ExitCode", 1);
            Debug.LogWarning("Validation interrupted; its save profile remains isolated.");
        }
        try { Complete(); }
        catch (Exception exception)
        {
            SessionState.SetInt(owner + "ExitCode", 1);
            Debug.LogException(exception);
        }
    }

    // An explicit next manual Play starts a separate editor profile, never the player profile.
    [MenuItem("Tools/Heros Manager/Use Editor Save Profile After Validation")]
    public static void UseEditorProfile()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode || Application.isBatchMode)
            throw new InvalidOperationException("Switch profiles outside Play mode and automation.");
        if (SaveStoragePaths.HasValidationSession) Complete();
        SessionState.EraseString(Key + "State");
        SessionState.EraseString(Key + "Directory");
        Debug.Log("EDITOR SAVE PROFILE: " + SaveStoragePaths.EditorProfile);
    }

    static string ProtectedSnapshot()
    {
        var files = new SortedSet<string>(StringComparer.OrdinalIgnoreCase);
        Collect(Application.persistentDataPath, files, true);
        Collect(SaveStoragePaths.EditorProfile, files, true);
        Collect(Application.dataPath, files, false);
        using var sha = SHA256.Create();
        return "protected-save-snapshot-v1\n" + string.Join("\n", files.Select(path =>
            path + "|" + new FileInfo(path).LastWriteTimeUtc.Ticks + "|" +
            Convert.ToBase64String(sha.ComputeHash(File.ReadAllBytes(path)))));
    }

    static void Collect(string directory, ISet<string> files, bool recurse)
    {
        if (!Directory.Exists(directory)) return;
        SaveStoragePaths.RequireUnlinkedPath(directory);
        foreach (string path in Directory.GetFiles(directory, "Save*.Json*", SearchOption.TopDirectoryOnly))
        { SaveStoragePaths.RequireUnlinkedPath(path); files.Add(Path.GetFullPath(path)); }
        if (recurse)
            foreach (string child in Directory.GetDirectories(directory)) Collect(child, files, true);
    }
}
