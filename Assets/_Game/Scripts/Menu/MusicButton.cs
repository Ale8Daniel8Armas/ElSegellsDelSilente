using UnityEngine;

public class MusicButton : MonoBehaviour
{
    private AudioSource music;
    public AudioClip clickAudio;
    public AudioClip hoverAudio;

    void Start()
    {
        music = GetComponent<AudioSource>();
	}

	public void ClickAudioOn()
	{
		music.PlayOneShot(clickAudio);
	}

    public void HoverAudioOn()  
    {
        music.PlayOneShot(hoverAudio);
	}
}
