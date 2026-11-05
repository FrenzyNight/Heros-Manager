using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>Resolve dependencies within the current minigame rather than by global object names.</summary>
public static class MiniGameReferences
{
    /// <summary>Open only the selected game's ancestors inside an already open panel.</summary>
    public static void ActivateWithin(GameObject game, Transform panel, List<GameObject> activatedAncestors)
    {
        if (game == null || panel == null || game.transform == panel || !game.transform.IsChildOf(panel))
            throw new InvalidOperationException("Minigame roots must be descendants of their minigame panel.");
        if (!panel.gameObject.activeInHierarchy)
            throw new InvalidOperationException("The minigame panel must be active before opening a game.");
        if (activatedAncestors == null)
            throw new ArgumentNullException(nameof(activatedAncestors));

        var path = new Stack<Transform>();
        for (Transform parent = game.transform.parent; parent != panel; parent = parent.parent)
            path.Push(parent);
        while (path.Count > 0)
        {
            GameObject ancestor = path.Pop().gameObject;
            if (ancestor.activeSelf)
                continue;
            activatedAncestors.Add(ancestor);
            ancestor.SetActive(true);
        }
        game.SetActive(true);
    }

    public static T Manager<T>(Component source, GameObject assigned = null) where T : Component
    {
        if (assigned != null && assigned.TryGetComponent(out T explicitManager))
            return explicitManager;
        for (Transform branch = source.transform; branch != null; branch = branch.parent)
        {
            T[] matches = branch.GetComponentsInChildren<T>(true);
            if (matches.Length == 1)
                return matches[0];
            if (matches.Length > 1)
                throw new InvalidOperationException($"{source.name} has multiple {typeof(T).Name} controllers in {branch.name}.");
        }
        throw new MissingReferenceException($"{source.name} requires a {typeof(T).Name} in its minigame hierarchy.");
    }

    public static Transform Container(Component source, string name, GameObject assigned = null)
    {
        if (assigned != null)
        {
            Transform assignedMatch = null;
            foreach (Transform child in assigned.GetComponentsInChildren<Transform>(true))
            {
                if (child.name != name)
                    continue;
                if (assignedMatch != null)
                    throw new InvalidOperationException($"Multiple '{name}' containers exist in {assigned.name}.");
                assignedMatch = child;
            }
            if (assignedMatch != null)
                return assignedMatch;
        }
        for (Transform branch = source.transform; branch != null; branch = branch.parent)
        {
            Transform found = null;
            foreach (Transform child in branch.GetComponentsInChildren<Transform>(true))
            {
                if (child.name != name)
                    continue;
                if (found != null)
                    throw new InvalidOperationException($"Multiple '{name}' containers exist in {branch.name}.");
                found = child;
            }
            if (found != null)
                return found;
        }
        throw new MissingReferenceException($"{source.name} requires a '{name}' container in its minigame hierarchy.");
    }
}
