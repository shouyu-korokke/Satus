using System.Collections.Generic;
using UnityEngine;

public class AnimatorFunctions : MonoBehaviour
{
    [SerializeField] MenuButtonController menuButtonController;
    //[SerializeField] selectCharacter_scroll selectCharacter_scroll;
    public bool disableOnce;

    void PlaySound(AudioClip whichSound)
    {
        if (!disableOnce)
        {
            menuButtonController.audioSource.PlayOneShot(whichSound);
            
        }
        else
        {
            disableOnce = false;
        }
    }
}
