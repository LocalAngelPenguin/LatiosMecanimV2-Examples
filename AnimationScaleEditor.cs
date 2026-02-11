using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public static class AnimationScaleFixer
{
    [MenuItem("Tools/Fix Scale")]
    private static void FixScaleInSelectedClips()
    {
        var selected = Selection.GetFiltered<AnimationClip>(SelectionMode.Assets);

        if (selected.Length == 0)
        {
            Debug.Log("No AnimationClips selected in Project window.");
            return;
        }

        int totalModified = 0;
        foreach (var clip in selected)
            totalModified += ProcessClip(clip);

        Debug.Log($"Processed {selected.Length} clip(s). Modified {totalModified} scale curve(s).");
    }

   private static int ProcessClip(AnimationClip clip)
{
    if (clip == null) return 0;

    var bindings = AnimationUtility.GetCurveBindings(clip);

    var scaleBindings = new List<EditorCurveBinding>();
    foreach (var binding in bindings)
    {
        if (binding.propertyName.ToLower().Contains("scale"))
            scaleBindings.Add(binding);
    }

    if (scaleBindings.Count == 0)
        return 0;

    scaleBindings.Sort((a, b) =>
    {
        int depthCompare = a.path.Split('/').Length.CompareTo(b.path.Split('/').Length);
        if (depthCompare != 0)
            return depthCompare;

        return string.Compare(a.path, b.path, System.StringComparison.Ordinal);
    });

    var referenceBinding = scaleBindings[0];
    var referenceCurve = AnimationUtility.GetEditorCurve(clip, referenceBinding);

    if (referenceCurve == null || referenceCurve.keys.Length == 0)
        return 0;

    float referenceScale = referenceCurve.keys[0].value;
    int modifiedCount = 0;

    foreach (var binding in scaleBindings)
    {
        var curve = AnimationUtility.GetEditorCurve(clip, binding);
        if (curve == null || curve.keys.Length == 0)
            continue;

        var keys = curve.keys;
        bool anyChange = false;

        for (int i = 0; i < keys.Length; i++)
        {
            if (keys[i].value != referenceScale)
            {
                keys[i].value = referenceScale;
                anyChange = true;
            }
        }

        if (anyChange)
        {
            curve.keys = keys;
            AnimationUtility.SetEditorCurve(clip, binding, curve);
            modifiedCount++;
        }
    }

    if (modifiedCount > 0)
        EditorUtility.SetDirty(clip);

    return modifiedCount;
}
}