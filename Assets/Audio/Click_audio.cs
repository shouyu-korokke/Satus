using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Click_audio : MonoBehaviour
{

    [SerializeField] MenuButtonController menuButtonController;
    public bool disableOnce;
    public AudioSource audioSource;


    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    void PlaySound(AudioClip whichSound)
    {
        if (!disableOnce)
        {
            menuButtonController.audioSource.PlayOneShot(whichSound);
            //selectCharacter_scroll.audioSource.PlayOneShot(whichSound);
        }
        else
        {
            disableOnce = false;
        }
    }

}
