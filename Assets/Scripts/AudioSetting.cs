using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class AudioSetting : MonoBehaviour
{
    // Start is called before the first frame update

    public Slider volumnSlider;
    public AudioMixer audioMixer;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setVolumn(float value)
    {
        audioMixer.SetFloat("BGMVolumn", value);
    }
}
