using System;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class ChestInteractable : MonoBehaviour, IInteractable
{
    //Setting the chest animation variables for DOTween, with assistance from lab instructors
    [SerializeField] private Ease ease;

    [Header("Chest Opening/Close Settings")]
    [SerializeField] private Transform chestLid;
    [SerializeField] private Vector3 chestLidRotation;
    [SerializeField] private float chestLidDuration;
    
    [Header("Audio Feedback")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip chestClip;
    
    private Tween _collectTween;
   private Tween _openLidTween;
    private Tween _loopTween;
    private int _isOpenHash;
    
    public static event Action<int> ChestDestroyed; //event to allow score to increase in GameManager
    public int Score = 1; //value to be added to the score in GameManager

    private void Start()
    {
        //Chest pulse animation
        _loopTween = transform.DOScale(1.6f, .2f).SetLoops(-1, LoopType.Yoyo).SetEase(ease)
            .SetDelay(Random.Range(0.5f, 2.5f));
    }

    private void OnDestroy()
    {
        //kill Tween once object destroyed
        DOTween.Kill(gameObject);
    }

    public void OnHoverIn()
    { 
        //open chest via DOTween
        chestLid.DOLocalRotate(chestLidRotation, chestLidDuration).SetEase(ease);

        //enable toast text when player close
        Toast.Instance?.ShowToast("Press \"E\" to Interact"); 
    }

    public void OnHoverOut()
    {
        //close chest via DOTween
      chestLid.DOLocalRotate(Vector3.zero, chestLidDuration).SetEase(ease);

        //hide toast text when player moves away
        Toast.Instance?.HideToast(); 
    }

    public void OnInteract()
    {
        if(audioSource != null && chestClip != null)
            audioSource.PlayOneShot(chestClip);
            
        //Change scale then destroy chest once interacted with
        _collectTween = transform.DOScale(0, .5f).SetEase(Ease.InBack).OnComplete(() => { Destroy(gameObject); });
        ChestDestroyed?.Invoke(Score);
    }
    
}