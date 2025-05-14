using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class PlayerAnimationTriggers : MonoBehaviour
{
    private Player player => GetComponentInParent<Player>();
    private AttackSense AttackSense => GetComponent<AttackSense>();


    public AudioClip LandingAudioClip;
    public AudioClip[] FootstepAudioClips;
    [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

    private void AttackTrigger()
    {
        if (player.comboCounter == 0)
        {
            //AudioManager.Instance.PlaySFX(3, null);
        }
        else
        {
            //AudioManager.Instance.PlaySFX(4, null);
        }

        player.WeaponHolder.GetComponentInChildren<Collider>().enabled = true;
    }

    private void AttackFinishTrigger()
    {
        player.AttackFinishTrigger();
    }

    private void OnFootstep(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            if (FootstepAudioClips.Length > 0)
            {
                var index = Random.Range(0, FootstepAudioClips.Length);
                AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(player.transform.position), FootstepAudioVolume);
            }
        }
    }

    private void OnLand(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(player.transform.position), FootstepAudioVolume);
        }
    }
}
