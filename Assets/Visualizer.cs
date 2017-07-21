using UnityEngine;
using UnityEngine.UI;

public class Visualizer : MonoBehaviour
{
    Slider[] sliders;
    float[] spectrum = new float[1024];
    float[] bands;

    void Awake()
    {
        sliders = GetComponentsInChildren<Slider>();
        foreach (var slider in sliders)
        {
            slider.value = 0;
        }

        bands = new float[sliders.Length];
    }

    // TODO: 整理する
    void Update()
    {
        var baseHz = (float)AudioSettings.outputSampleRate / spectrum.Length / 2;

        AudioListener.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);

        var specI = 0;
        var interval = (Mathf.Log((spectrum.Length - 1) * baseHz, 2) - Mathf.Log(1 * baseHz, 2)) / bands.Length / 4f; // 4?

        for (var i = 0; i < bands.Length; i++)
        {
            var v = 0f;
            var startSpecI = specI;
            while (specI < spectrum.Length)
            {
                if (Mathf.Log(specI * baseHz, 2) - Mathf.Log(startSpecI * baseHz, 2) >= interval) break;
                v += spectrum[specI];
                specI++;
            }

            v /= specI - startSpecI;
            bands[i] = v;
        }

        for (var i = 0; i < sliders.Length; i++)
        {
            var slider = sliders[i];
            slider.value = bands[i] * 4f;
        }
    }
}
