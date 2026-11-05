using System;
using System.IO;
using System.Security;
using System.Text;
using UnityEngine;

/// <summary>Validated JSON snapshots with atomic replacement and one known-good backup.</summary>
public static class JsonSaveStore
{
    private static readonly object Sync = new object();

    public static bool TryLoad<T>(string path, Func<T, bool> validate, out T value, out string error)
        where T : class, new()
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            value = null;
            error = "A save path is required.";
            return false;
        }
        if (validate == null) throw new ArgumentNullException(nameof(validate));
        if (!SaveStoragePaths.TryAuthorizeFile(path, out error)) { value = null; return false; }
        if (!SaveStoragePaths.TryAuthorizeFile(path + ".bak", out error)) { value = null; return false; }
        lock (Sync)
        {
            if (TryRead(path, validate, out value, out error))
                return true;
            if (TryRead(path + ".bak", validate, out value, out string backupError))
            {
                error = null;
                return true;
            }
            if (string.IsNullOrEmpty(error))
                error = backupError;
            return false;
        }
    }

    public static bool TrySave<T>(string path, T value, Func<T, bool> validate, out string error)
        where T : class, new()
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            error = "A save path is required.";
            return false;
        }
        if (validate == null) throw new ArgumentNullException(nameof(validate));
        if (!SaveStoragePaths.TryAuthorizeFile(path, out error) ||
            !SaveStoragePaths.TryAuthorizeFile(path + ".bak", out error)) return false;
        lock (Sync)
        {
            error = null;
            if (value == null || !validate(value))
            {
                error = $"Cannot save invalid {typeof(T).Name} to '{path}'.";
                return false;
            }

            string temporaryPath = path + ".tmp-" + Guid.NewGuid().ToString("N");
            try
            {
                string directory = Path.GetDirectoryName(path);
                if (!string.IsNullOrEmpty(directory))
                    Directory.CreateDirectory(directory);

                byte[] json = new UTF8Encoding(false).GetBytes(JsonUtility.ToJson(value, true));
                using (var stream = new FileStream(temporaryPath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    stream.Write(json, 0, json.Length);
                    stream.Flush(true);
                }

                if (File.Exists(path))
                {
                    bool currentIsValid = TryRead(path, validate, out T previous, out _);
                    if (!currentIsValid)
                    {
                        // Preserve damaged evidence and do not replace a valid backup with it.
                        File.Copy(path, path + ".corrupt-" + Guid.NewGuid().ToString("N"));
                    }
                    File.Replace(temporaryPath, path, currentIsValid ? path + ".bak" : null);
                }
                else
                {
                    File.Move(temporaryPath, path);
                }
                return true;
            }
            catch (Exception exception) when (IsStorageException(exception))
            {
                error = $"Could not save '{path}': {exception.Message}";
                return false;
            }
            finally
            {
                try
                {
                    if (File.Exists(temporaryPath))
                        File.Delete(temporaryPath);
                }
                catch (Exception exception) when (IsStorageException(exception))
                {
                    // The original save is intact; a stale temporary file is safe to ignore.
                }
            }
        }
    }

    private static bool TryRead<T>(string path, Func<T, bool> validate, out T value, out string error)
        where T : class, new()
    {
        value = null;
        error = null;
        if (!File.Exists(path))
            return false;

        try
        {
            string json = File.ReadAllText(path).Trim();
            if (json.Length < 2 || json[0] != '{' || json[json.Length - 1] != '}')
                throw new InvalidDataException("Expected a JSON object.");

            var loaded = new T();
            JsonUtility.FromJsonOverwrite(json, loaded);
            if (!validate(loaded))
                throw new InvalidDataException($"Invalid {typeof(T).Name} snapshot.");
            value = loaded;
            return true;
        }
        catch (Exception exception) when (IsStorageException(exception))
        {
            error = $"Could not load '{path}': {exception.Message}";
            return false;
        }
    }

    private static bool IsStorageException(Exception exception)
    {
        return exception is IOException || exception is InvalidDataException ||
               exception is UnauthorizedAccessException ||
               exception is ArgumentException || exception is NotSupportedException ||
               exception is SecurityException;
    }
}
