using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// https://www.youtube.com/watch?v=L4t2c1_Szdk ezen tutorialvideóból merített ötelettek készült. 

public class TimeController : MonoBehaviour
{
    // Nap fények
    private float timeMultiplier = 1500;
    private float startHour = 5; 
    [SerializeField]
    private Light sun;
    private float sunriseHour = 10;
    private float sunsetHour = 18;

    // Környezõ fények
    [SerializeField]
    private Color morningAmbientLights;
    [SerializeField]  
    private Color ninghtAmbientLights;
    [SerializeField]
    private AnimationCurve ambientLightsChangeCurve;
    private float maxSunLight = 1;
    [SerializeField]
    private Light moonLight;
    private float maxMoonLight = 0.5f;

    private DateTime currentTime;
    private TimeSpan sunrise;
    private TimeSpan sunset;

    void Start()
    {
        currentTime = DateTime.Now.Date + TimeSpan.FromHours(startHour);

        sunrise = TimeSpan.FromHours(sunriseHour);
        sunset = TimeSpan.FromHours(sunsetHour);
    }

 
    void Update()
    {
        UpdateDayTime();
        RotateSunLight();
        UpdateLights();
    }

    private void UpdateDayTime()
    {
        currentTime = currentTime.AddSeconds(timeMultiplier * Time.deltaTime);
        
    }

    private TimeSpan CalculateTime(TimeSpan fromTime, TimeSpan toTime)
    {
        TimeSpan calculatedTime = toTime - fromTime;

        if(calculatedTime.TotalSeconds < 0)
        {
            calculatedTime += TimeSpan.FromHours(24);
        }

        return calculatedTime;
    }

    private void RotateSunLight()
    {
        float sunLightRotation;
        if(currentTime.TimeOfDay > sunrise && currentTime.TimeOfDay < sunset)
        {
            TimeSpan sunriseToSunset = CalculateTime(sunrise, sunset);
            TimeSpan timeAfterSunrise = CalculateTime(sunrise, currentTime.TimeOfDay);

            double percantageOfSunlightTime = timeAfterSunrise.TotalMinutes / sunriseToSunset.TotalMinutes;

            sunLightRotation = Mathf.Lerp(0, 180, (float)percantageOfSunlightTime);
        }
        else
        {
            TimeSpan sunsetToSunrise = CalculateTime(sunset, sunrise);
            TimeSpan timeAfterSunset = CalculateTime(sunset, currentTime.TimeOfDay);

            double percantageOfSunlightTime = timeAfterSunset.TotalMinutes / sunsetToSunrise.TotalMinutes;

            sunLightRotation = Mathf.Lerp(0,180,(float)percantageOfSunlightTime);
        }

        sun.transform.rotation = Quaternion.AngleAxis(sunLightRotation, Vector3.right);
    }

    private void UpdateLights()
    {
        float dotShine = Vector3.Dot(sun.transform.forward, Vector3.down);
        sun.intensity = Mathf.Lerp(0, maxSunLight, ambientLightsChangeCurve.Evaluate(dotShine));
        moonLight.intensity = Mathf.Lerp (maxMoonLight, 0, ambientLightsChangeCurve.Evaluate(dotShine));
        RenderSettings.ambientLight = Color.Lerp(ninghtAmbientLights, morningAmbientLights,
                                                 ambientLightsChangeCurve.Evaluate(dotShine));
    }

}
