using UnityEngine;
using System.Collections;
using System.Runtime.Serialization;

public class PlaySoundFXContinuously : MonoBehaviour
{
    [SerializeField] private AudioClip clip;
    [SerializeField] private float volume = 0.5f;
    [SerializeField] private bool volumeIncreases;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(PlaySoundContinuously());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator PlaySoundContinuously()
    {
        StartCoroutine(increaseVolumeOverTime());
        while (true)
        {
            SoundFXManager.instance.PlaySoundFXCLip(clip, transform, volume);
            yield return new WaitForSeconds(clip.length);
        }
    }
    
    IEnumerator increaseVolumeOverTime()
    {
        while (volumeIncreases && volume < 1.0f)
        {
            volume += 0.05f;
            yield return new WaitForSeconds(2f);
        }
    }
}
