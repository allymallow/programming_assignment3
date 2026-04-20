using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip backgroundMusicClip;
  
      void Awake()
      {
          DontDestroyOnLoad(gameObject); //ensuring the audio source stays active even in different scenes 
      }

      void Start()
      {
          //null check in case audio source and background music clip are missing 
          if (audioSource != null && backgroundMusicClip != null)
          {
              audioSource.clip = backgroundMusicClip;
              audioSource.loop = true; //keep the music looping throughout the game
              audioSource.Play();

          }
      }
}
