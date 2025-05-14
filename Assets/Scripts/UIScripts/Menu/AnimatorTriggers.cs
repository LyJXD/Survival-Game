using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorTriggers : MonoBehaviour
{
    [SerializeField] private MenuButtonController menuButtonController;
    public bool disableOnce;

    private void PlaySound(AudioClip _whichSound)
    {
        if(!disableOnce)
        {
            menuButtonController.audioSource.PlayOneShot(_whichSound);
        }
        else
        {
            disableOnce = false;
        }
    }
}
