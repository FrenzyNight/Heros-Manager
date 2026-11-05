using System;
using System.IO;
using UnityEngine;

/// <summary>Resolves profiles and enforces the editor validation storage boundary.</summary>
public static class SaveStoragePaths
{
#if UNITY_EDITOR
    public const string ValidationKey = "HerosManager.ValidationStorage.";
    public static string EditorProfile => Path.GetFullPath(Path.Combine(Application.dataPath, "../Library/EditorSaveData"));
    public static string ValidationRoot => Path.GetFullPath(Path.Combine(Application.dataPath, "../Library/ValidationSaves"));
    public static bool HasValidationSession => UnityEditor.SessionState.GetString(ValidationKey + "State", "") != "";
    static bool IsAutomation => Application.isBatchMode || Array.IndexOf(Environment.GetCommandLineArgs(), "-executeMethod") >= 0;

    // SessionState survives domain reloads; no nullable override can fall back to player saves.
    public static string BeginValidation()
    {
        if (Application.isPlaying) throw new InvalidOperationException("Start validation outside Play mode.");
        UnityEditor.SessionState.SetString(ValidationKey + "State", "initializing");
        UnityEditor.SessionState.SetString(ValidationKey + "Directory", "");
        string token = Guid.NewGuid().ToString("N");
        string directory = Path.Combine(ValidationRoot, token);
        RequireUnlinkedPath(directory);
        System.IO.Directory.CreateDirectory(directory);
        File.WriteAllText(Path.Combine(directory, ".validation-owner"), token);
        UnityEditor.SessionState.SetString(ValidationKey + "Directory", directory);
        UnityEditor.SessionState.SetString(ValidationKey + "State", "active");
        return directory;
    }

    public static void SealValidation() => UnityEditor.SessionState.SetString(ValidationKey + "State", "sealed");

    public static string RequireValidationDirectory()
    {
        if (UnityEditor.SessionState.GetString(ValidationKey + "State", "") != "active")
            throw new InvalidOperationException("Validation storage is not active; normal saves are never a fallback.");
        string directory = Canonical(UnityEditor.SessionState.GetString(ValidationKey + "Directory", ""));
        string token = Path.GetFileName(directory);
        if (!Guid.TryParseExact(token, "N", out _) || !SamePath(Path.GetDirectoryName(directory), ValidationRoot))
            throw new InvalidOperationException("Validation directory is not owned by this project.");
        RequireUnlinkedPath(directory);
        RequireUnlinkedPath(Path.Combine(directory, ".validation-owner"));
        if (!File.Exists(Path.Combine(directory, ".validation-owner")) ||
            File.ReadAllText(Path.Combine(directory, ".validation-owner")) != token)
            throw new InvalidOperationException("Validation ownership marker is missing or invalid.");
        return directory;
    }
#endif
    public static string Directory
    {
        get
        {
#if UNITY_EDITOR
            if (HasValidationSession || IsAutomation) return RequireValidationDirectory();
#endif
            string[] args = Environment.GetCommandLineArgs();
            for (int i = 0; i < args.Length; i++)
                if (args[i] == "--save-directory")
                {
                    if (i + 1 >= args.Length || !Path.IsPathFullyQualified(args[i + 1]))
                        throw new InvalidOperationException("--save-directory requires an absolute path; refusing the default profile.");
                    return Canonical(args[i + 1]);
                }
#if UNITY_EDITOR
            return EditorProfile;
#else
            return Application.persistentDataPath;
#endif
        }
    }

    public static bool IsIsolated => !SamePath(Directory, Application.persistentDataPath);

    public static string Canonical(string path)
    {
        if (string.IsNullOrWhiteSpace(path) || !Path.IsPathFullyQualified(path))
            throw new ArgumentException("An absolute save path is required.");
        string full = Path.GetFullPath(path);
        string root = Path.GetPathRoot(full);
        return full.Length == root.Length ? root : full.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
    }

    public static bool SamePath(string left, string right) =>
        string.Equals(Canonical(left), Canonical(right), StringComparison.OrdinalIgnoreCase);

    public static bool IsWithin(string path, string directory) =>
        Canonical(path).StartsWith(Canonical(directory).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) +
            Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase);

    // Invoked by both reads and writes, before backups, temp files, migration or recovery I/O.
    public static bool TryAuthorizeFile(string path, out string error)
    {
        error = null;
#if UNITY_EDITOR
        try
        {
            string full = Canonical(path);
            if (SamePath(full, Application.persistentDataPath) || IsWithin(full, Application.persistentDataPath))
                throw new InvalidOperationException("Editor code cannot access the standalone player's save profile.");
            string root = Directory;
            if (!HasValidationSession && !IsAutomation && !Application.isPlaying)
                throw new InvalidOperationException("Edit-mode save access requires an explicit validation session.");
            if (!IsWithin(full, root)) throw new InvalidOperationException("Save path escapes the active profile.");
            RequireUnlinkedPath(full);
        }
        catch (Exception exception) when (exception is ArgumentException || exception is InvalidOperationException ||
            exception is IOException || exception is UnauthorizedAccessException || exception is NotSupportedException)
        { error = "Save access denied: " + exception.Message; return false; }
#endif
        return true;
    }

#if UNITY_EDITOR
    public static void RequireUnlinkedPath(string path)
    {
        string full = Canonical(path);
        string relative = full.Substring(Path.GetPathRoot(full).Length);
        foreach (string part in relative.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar))
            if (part.EndsWith(".") || part.EndsWith(" ") || part.Contains(":"))
                throw new InvalidOperationException("Ambiguous filesystem path is not allowed.");
        for (string node = full; !string.IsNullOrEmpty(node); node = Path.GetDirectoryName(node))
            if ((File.Exists(node) || System.IO.Directory.Exists(node)) &&
                (File.GetAttributes(node) & FileAttributes.ReparsePoint) != 0)
                throw new InvalidOperationException("Linked validation/save paths are not allowed.");
    }
#endif
}
