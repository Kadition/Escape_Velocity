using UnityEngine;

public class MainMenuMusicPlayer : MonoBehaviour
{
    [SerializeField] private AudioClip mainMenuMusic;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SoundFXManager.instance.PlaySoundFXCLip(mainMenuMusic, transform, 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
