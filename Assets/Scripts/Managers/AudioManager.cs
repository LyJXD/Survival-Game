using UnityEngine;

public class AudioManager : MonoSingleton<AudioManager>
{
    [SerializeField] private float sfxMinimumDistance;
    [SerializeField] private AudioSource[] bgm;
    [SerializeField] private AudioSource[] sfx;

    public int lastSfxIndex;
    public bool playBgm;
    private int bgmIndex;

    private void Update()
    {
        if (!playBgm)
        {
            StopAllBGM();
        }
        else
        {
            if (!bgm[bgmIndex].isPlaying)
            {
                PlayBGM(bgmIndex);
            }
        }
    }
    public void PlaySFX(int _sfxIndex, Transform _source)
    {
        // if (sfx[_sfxIndex].isPlaying) return;

        if (_sfxIndex < 0)
        {
            return;
        }

        // 限制可听见的声音距离
        if (_source != null && Vector2.Distance(PlayerManager.Instance.Player.transform.position, _source.position) > sfxMinimumDistance)
        {
            return;
        }

        if (_sfxIndex < sfx.Length)
        {
            sfx[_sfxIndex].pitch = Random.Range(0.9f, 1.1f);
            sfx[_sfxIndex].Play();
        }
    }

    public void StopSFX(int _index)
    {
        if (_index < 0)
        {
            return;
        }
        sfx[_index].Stop();
    }

    public void PlayBGM(int _bgmIndex)
    {
        bgmIndex = _bgmIndex;

        StopAllBGM();

        bgm[_bgmIndex].Play();
    }

    public void StopAllBGM()
    {
        for (int i = 0; i < bgm.Length; i++)
        {
            bgm[i].Stop();
        }
    }
}
