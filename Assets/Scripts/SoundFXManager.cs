using UnityEngine;
using UnityEngine.UI;

public class SoundFXManager : MonoBehaviour
{
    [SerializeField] private AudioSource soundFXObject;
    [SerializeField] private float globalVolume = 1.0f;
    public static SoundFXManager instance;
    [SerializeField] private Slider volumeSlider;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    } 

    private void Start()
    {
        if (volumeSlider != null)
        {
            volumeSlider.onValueChanged.AddListener((v) => SetGlobalVolume(v));
        }
    }

    public void SetGlobalVolume(float volume)
    {
        globalVolume = volume;
    }

    public void PlaySoundFXCLip(AudioClip clip, Transform spawnTransform, float volume = 1.0f)
    {
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);

        audioSource.clip = clip;
        audioSource.volume = volume * globalVolume;
        audioSource.Play();
        float clipLength = audioSource.clip.length;
        Destroy(audioSource.gameObject, clipLength);
    }
}
