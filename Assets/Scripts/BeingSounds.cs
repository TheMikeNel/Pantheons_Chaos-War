using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BeingSounds : MonoBehaviour
{
    [Header("Idle")]
    [SerializeField] private bool enableIdleClips = false;
    [SerializeField] private AudioClip[] idleClips;
    [SerializeField] private float baseRandomTime = 3;

    [Header("Events")]
    [SerializeField] private AudioClip[] stepClips;
    [SerializeField] private AudioClip[] attackClips;
    [SerializeField] private AudioClip[] specialAttackClips;
    [SerializeField] private AudioClip[] attackVoiceClips;
    [SerializeField] private AudioClip[] takeHitClips;
    [SerializeField] private AudioClip[] takeHitVoiceClips;
    [SerializeField] private AudioClip dieClip;

    private AudioSource _aud;

    private void Start()
    {
        _aud = GetComponent<AudioSource>();

        if (enableIdleClips && idleClips.Length > 0) 
            StartCoroutine(IdlePlaying());
    }

    private IEnumerator IdlePlaying()
    {
        while(gameObject.activeSelf)
        {
            yield return new WaitForSeconds(GetRandomTime());

            if (!_aud.isPlaying) 
                _aud.PlayOneShot(idleClips[Random.Range(0, idleClips.Length)]);
        }
    }

    public void PlaySound(SoundsType soundType)
    {
        AudioClip clip = null;

        switch (soundType)
        {
            case SoundsType.Step: clip = GetRandomClip(stepClips);
                break;
            case SoundsType.Attack: clip = GetRandomClip(attackClips);
                break;
            case SoundsType.SpecialAttack: clip = GetRandomClip(specialAttackClips);
                break;
            case SoundsType.AttackVoice: clip = GetRandomClip(attackVoiceClips);
                break;
            case SoundsType.TakeHit: clip = GetRandomClip(takeHitClips);
                break;
            case SoundsType.TakeHitVoice: clip = GetRandomClip(takeHitVoiceClips);
                break;
            case SoundsType.Die: clip = dieClip;
                break;
        }

        if (clip != null) _aud.PlayOneShot(clip);
    }

    public void PlayOneShotClip(AudioClip clip)
    {
        if (clip != null) _aud.PlayOneShot(clip);
    }

    private AudioClip GetRandomClip(AudioClip[] clipsArray)
    {
        if (clipsArray.Length > 0)
            return clipsArray[Random.Range(0, clipsArray.Length - 1)];
        return null;
    }

    private float GetRandomTime() => Random.Range(baseRandomTime / 2, baseRandomTime * 2);

    public enum SoundsType
    {
        Step,
        Attack,
        SpecialAttack,
        AttackVoice,
        TakeHit,
        TakeHitVoice,
        Die
    }
}
