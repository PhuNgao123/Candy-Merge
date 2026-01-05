using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("Audio Source")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    [Header("Audio Clip:")]
    //music
    [Header("music")]
    public AudioClip background;

    //input
    [Header("input")]
    public AudioClip mouseClick;

    //sfx
    [Header("sfx")]
    //1.Candy
    public AudioClip candySpawn;
    public AudioClip candyDrop;
    public AudioClip candyCollision;
    public AudioClip candyMerge;

    //2.Combo
    public AudioClip Combo1;
    public AudioClip Combo2;
    public AudioClip Combo3;
    public AudioClip Combo4;
    public AudioClip Combo5;
    public AudioClip Combo6;

    public static AudioManager Instance;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public void Start(){
        musicSource.clip= background;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip){
        SFXSource.PlayOneShot(clip);
    }

    public void PlayComboSFX(int comboLevel)
    {
        AudioClip comboClip = null;
        
        switch (comboLevel)
        {
            case 1: comboClip = Combo1; break;
            case 2: comboClip = Combo2; break;
            case 3: comboClip = Combo3; break;
            case 4: comboClip = Combo4; break;
            case 5: comboClip = Combo5; break;
            default: return; 
        }
        
        if (comboClip != null)
        {
            PlaySFX(comboClip);
        }
    }

}
