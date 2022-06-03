using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour
{

    public Light sunLight;
    public Light moonLight;

    public float maxMoonLightIntensity;
    public float maxSunLightIntensity;
    public Color nightAmbientLight;
    public Color dayAmbientLight;

    public float sunsetTime;
    public AnimationCurve lightChangeCurve;

    // Start is called before the first frame update
    void Start()
    {
        SetUp();
        
    }

    // Update is called once per frame
    void Update()
    {
        RotateSun();
        UpdateLightSetting();
    }

    void SetUp()
    {
        sunsetTime = Clock.Instance.dayTime / 2 * 60;
    }

    private void RotateSun()
    {
        float sunLightRotation;
        float moonLightRotation;

        
        if (Clock.Instance.nowTime <= sunsetTime)
        {
            double percentage = Clock.Instance.nowTime / sunsetTime;

            sunLightRotation = Mathf.Lerp(50, 180, (float)percentage);

            moonLightRotation = Mathf.Lerp(-180, -50, (float)percentage);
        }
        else
        {
            double percentage = (Clock.Instance.nowTime-sunsetTime) / sunsetTime;

            sunLightRotation = Mathf.Lerp(230, 360, (float)percentage);

            moonLightRotation = Mathf.Lerp(-360, -230, (float)percentage);
        }

        //sunLightRotation = Math.Abs(Mathf.Sin(nowTime)) * sunLightRotation;
        //moonLightRotation = Math.Abs(Mathf.Sin(nowTime)) * moonLightRotation;

        sunLight.transform.rotation = Quaternion.AngleAxis(sunLightRotation, Vector3.right);
        moonLight.transform.rotation = Quaternion.AngleAxis(moonLightRotation, Vector3.right);
    }

    private void UpdateLightSetting()
    {
        float dotProduct = Vector3.Dot(sunLight.transform.forward, Vector3.down);

        sunLight.intensity = Mathf.Lerp(0, maxSunLightIntensity, lightChangeCurve.Evaluate(dotProduct));
        moonLight.intensity = Mathf.Lerp(maxMoonLightIntensity, 0, lightChangeCurve.Evaluate(dotProduct));

        RenderSettings.ambientLight = Color.Lerp(nightAmbientLight, dayAmbientLight, lightChangeCurve.Evaluate(dotProduct));
    }
}
