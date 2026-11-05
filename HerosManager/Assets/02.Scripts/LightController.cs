using UnityEngine;
using UnityEngine.Rendering;

/// <summary>Clock-driven forest light. Visual time follows gameplay time, including pauses.</summary>
public class LightController : MonoBehaviour
{
    // Preserve the authored scene references and serialized controls.
    public Light sunLight;
    public Light moonLight;
    public float maxMoonLightIntensity;
    public float maxSunLightIntensity;
    public Color nightAmbientLight;
    public Color dayAmbientLight;
    public float sunsetTime;
    public AnimationCurve lightChangeCurve;

    [Header("Forest atmosphere")]
    public Color sunsetColor = new Color(1f, 0.62f, 0.38f);
    public Color moonColor = new Color(0.61f, 0.73f, 1f);
    [Range(0, 1)] public float daylightShadowStrength = 0.7f;
    [Min(1)] public float fireRange = 18f;
    [Min(0)] public float fireIntensity = 32f;

    Clock clock;
    Light fireLight;
    bool initialized;
    AmbientMode previousAmbientMode;
    Color previousSky, previousEquator, previousGround, previousFog;
    bool previousFogEnabled;
    FogMode previousFogMode;
    float previousFogStart, previousFogEnd;
    Light previousSun;

    void Start()
    {
        clock = Clock.Instance;
        if (InGameMgr.Instance.stage == 1 && GetComponent<MeadowSurfaceTreatment>() == null)
            gameObject.AddComponent<MeadowSurfaceTreatment>();
        if (sunLight == null || moonLight == null) { enabled = false; return; }
        previousAmbientMode = RenderSettings.ambientMode;
        previousSky = RenderSettings.ambientSkyColor;
        previousEquator = RenderSettings.ambientEquatorColor;
        previousGround = RenderSettings.ambientGroundColor;
        previousFogEnabled = RenderSettings.fog;
        previousFogMode = RenderSettings.fogMode;
        previousFog = RenderSettings.fogColor;
        previousFogStart = RenderSettings.fogStartDistance;
        previousFogEnd = RenderSettings.fogEndDistance;
        previousSun = RenderSettings.sun;
        initialized = true;
        ConfigureDirectional(sunLight);
        ConfigureDirectional(moonLight);
        var fire = FindAnyObjectByType<FireMgr>();
        if (fire != null && fire.FireObj != null)
        {
            fireLight = fire.FireObj.GetComponentInChildren<Light>(true);
            if (fireLight != null)
            {
                // The particle parent is rotated: offset in world space, not local Y.
                fireLight.transform.position += Vector3.up * 2.1f;
                fireLight.color = new Color(1f, 0.43f, 0.13f);
                fireLight.range = fireRange;
                fireLight.shadows = LightShadows.None;
                fireLight.cullingMask &= ~(1 << LayerMask.NameToLayer("UI"));
            }
        }
        Update();
    }

    static void ConfigureDirectional(Light light)
    {
        light.shadows = LightShadows.Soft;
        light.shadowBias = 0.035f;
        light.shadowNormalBias = 0.3f;
        light.shadowNearPlane = 0.2f;
    }

    void Update()
    {
        if (!initialized || clock == null) return;
        float time = Mathf.Clamp01(clock.nowTime / clock.DayDurationSeconds);
        sunsetTime = clock.DayDurationSeconds * 0.5f;
        // Twilight straddles the game's half-day boundary instead of teleporting the sun at it.
        float night = Mathf.SmoothStep(0, 1, Mathf.InverseLerp(0.38f, 0.58f, time));
        float daylight = 1 - night;
        float sunset = Mathf.SmoothStep(0, 1, Mathf.InverseLerp(0.27f, 0.48f, time));
        float elevation = time < 0.2f
            ? Mathf.Lerp(42, 65, Mathf.SmoothStep(0, 1, time / 0.2f))
            : Mathf.Lerp(65, -12, Mathf.SmoothStep(0, 1, Mathf.InverseLerp(0.2f, 0.58f, time)));
        sunLight.transform.rotation = Quaternion.Euler(elevation, -145, 0);
        moonLight.transform.rotation = Quaternion.Euler(48, -115, 0);
        sunLight.color = Color.Lerp(new Color(1f, 0.95f, 0.83f), sunsetColor, sunset);
        moonLight.color = moonColor;
        sunLight.intensity = maxSunLightIntensity * daylight;
        moonLight.intensity = maxMoonLightIntensity * night;
        sunLight.shadowStrength = daylightShadowStrength * daylight;
        moonLight.shadowStrength = 0.45f * night;
        RenderSettings.sun = daylight > 0.25f ? sunLight : moonLight;
        RenderSettings.ambientMode = AmbientMode.Trilight;
        RenderSettings.ambientSkyColor = Color.Lerp(dayAmbientLight, nightAmbientLight, night);
        RenderSettings.ambientEquatorColor = Color.Lerp(new Color(0.48f, 0.50f, 0.44f), new Color(0.22f, 0.28f, 0.38f), night);
        RenderSettings.ambientGroundColor = Color.Lerp(new Color(0.25f, 0.28f, 0.21f), new Color(0.12f, 0.16f, 0.23f), night);
        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.Linear;
        RenderSettings.fogStartDistance = 45;
        RenderSettings.fogEndDistance = 135;
        RenderSettings.fogColor = Color.Lerp(new Color(0.54f, 0.63f, 0.62f), new Color(0.13f, 0.19f, 0.29f), night);
        if (fireLight != null)
        {
            float flicker = 1 + 0.035f * Mathf.Sin(Time.time * 6.1f) + 0.02f * Mathf.Sin(Time.time * 9.7f);
            fireLight.intensity = fireIntensity * Mathf.Lerp(0.65f, 1, night) * flicker;
        }
    }

    void OnDisable()
    {
        if (!initialized) return;
        RenderSettings.ambientMode = previousAmbientMode;
        RenderSettings.ambientSkyColor = previousSky;
        RenderSettings.ambientEquatorColor = previousEquator;
        RenderSettings.ambientGroundColor = previousGround;
        RenderSettings.fog = previousFogEnabled;
        RenderSettings.fogMode = previousFogMode;
        RenderSettings.fogColor = previousFog;
        RenderSettings.fogStartDistance = previousFogStart;
        RenderSettings.fogEndDistance = previousFogEnd;
        RenderSettings.sun = previousSun;
    }
}
