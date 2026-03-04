using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal; // Change to UnityEngine.Rendering.HighDefinition if using HDRP

// This creates a custom block in the Unity Inspector for each resolution tier
[System.Serializable]
public struct ResolutionDoFProfile
{
    [Tooltip("The vertical resolution to target (e.g., 720, 1080, 1440, 2160).")]
    public int targetScreenHeight;

    [Tooltip("The exact aperture value for this resolution.")]
    public float aperture;

    [Tooltip("The exact focus distance for this resolution.")]
    public float focusDistance;
}

[RequireComponent(typeof(Volume))]
public class ManualResolutionDoF : MonoBehaviour
{
    private Volume postProcessVolume;
    private DepthOfField dofComponent;

    [Header("Fallback Settings")]
    [Tooltip("These are used if your manual profile list is empty.")]
    public float fallbackAperture = 5.6f;
    public float fallbackFocusDistance = 10f;

    [Header("Resolution Profiles")]
    [Tooltip("Add your specific settings for each resolution height here. The script will automatically pick the closest match.")]
    public List<ResolutionDoFProfile> profiles = new List<ResolutionDoFProfile>();

    private int lastScreenHeight;

    void Start()
    {
        postProcessVolume = GetComponent<Volume>();

        if (postProcessVolume.profile.TryGet(out dofComponent))
        {
            lastScreenHeight = Screen.height;
            ApplyClosestProfile();
        }
        else
        {
            Debug.LogWarning("ManualResolutionDoF: No Depth of Field override found in the Volume Profile.");
        }
    }

    void Update()
    {
        // We only check vertical height, as that's what drives post-processing scale
        if (Screen.height != lastScreenHeight)
        {
            if (dofComponent != null)
            {
                ApplyClosestProfile();
            }
            lastScreenHeight = Screen.height;
        }
    }

    private void ApplyClosestProfile()
    {
        // If the user forgot to set up profiles, use the fallback settings
        if (profiles.Count == 0)
        {
            ApplySettings(fallbackAperture, fallbackFocusDistance);
            return;
        }

        ResolutionDoFProfile closestProfile = profiles[0];
        int minDifference = int.MaxValue;

        // Loop through all defined profiles to find the closest match to the current screen height
        foreach (var profile in profiles)
        {
            int difference = Mathf.Abs(profile.targetScreenHeight - Screen.height);

            if (difference < minDifference)
            {
                minDifference = difference;
                closestProfile = profile;
            }
        }

        // Apply the settings from the closest match we found
        ApplySettings(closestProfile.aperture, closestProfile.focusDistance);

        // Debug.Log($"Applied DoF Profile for ~{closestProfile.targetScreenHeight}p. Aperture: {closestProfile.aperture}");
    }

    private void ApplySettings(float newAperture, float newFocusDistance)
    {
        // Clamp to physically plausible boundaries
        dofComponent.aperture.value = Mathf.Clamp(newAperture, 1.0f, 32.0f);
        dofComponent.focusDistance.value = Mathf.Max(0.1f, newFocusDistance);
    }
}