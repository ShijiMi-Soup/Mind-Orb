using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;

public class ControlOrb : MonoBehaviour
{
    public float SizeChangePerSec = 0.1f;
    public float IntensityChangePerSec = 0.1f;
    public float VolumeChangePerSec = 1f;
    public float BaseVolume = 0.5f;
    public ExposedProperty sizeProp = "Size";
    public ExposedProperty intensityProp = "Intensity";
    public AudioSource OrbSound;

    private VisualEffect orb;
    private float size = 0;
    private float intensity = 0;
    private float threshold = 35;
    private float maxScore = 100;
    private float lastMedScore = -1;

    // Start is called before the first frame update
    void Start()
    {
        orb = GetComponent<VisualEffect>();

        orb.Stop();
        threshold = (float)Variables.ActiveScene.Get("VFXActiveThreshold");
        maxScore = (float)Variables.ActiveScene.Get("MaxBrainScore");

        OrbSound.volume = 0;
    }

    // Update is called once per frame
    void Update()
    {
        float medScore = (float)Variables.ActiveScene.Get("MeditationScore");
        float largerScore = Math.Max(lastMedScore, medScore);
        float smallerScore = Math.Min(lastMedScore, medScore);
        bool crossed = smallerScore < threshold && threshold < largerScore;

        if (medScore > threshold) {
            if (crossed) orb.Play();
        }
        else orb.Stop();
        

        float refSize = medScore * 0.03f;
        float deltaSize = SizeChangePerSec * Time.deltaTime;
        if (size < refSize) size += deltaSize;
        if (size > refSize) size -= deltaSize;

        orb.SetFloat(sizeProp, size);
        lastMedScore = medScore;

        float attScore = (float)Variables.ActiveScene.Get("AttentionScore");
        attScore = Math.Max(attScore, 1);
        float refIntensity = attScore * 0.1f;
        float deltaIntensity = IntensityChangePerSec * Time.deltaTime;
        if (intensity < refIntensity) intensity += deltaIntensity;
        if (intensity > refIntensity) intensity -= deltaIntensity;

        orb.SetFloat(intensityProp, intensity);

        float refVolume = medScore / maxScore + BaseVolume;
        if (medScore < threshold) refVolume = 0;
        float newVolume = OrbSound.volume;
        float deltaVolume = VolumeChangePerSec * Time.deltaTime;
        if (newVolume < refVolume) newVolume += deltaVolume;
        if (newVolume > refVolume) newVolume -= deltaVolume;

        OrbSound.volume = Math.Max(0, Math.Min(1, newVolume));
    }

}
