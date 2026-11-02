using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CandyMergeManager : MonoBehaviour
{
    private int score =0;

    private Coroutine addScoreCoroutine;

    private Coroutine multiplierCoroutine;

    public static GameObject[] Prefabs;

    public static bool gameOver = false;

    [Header("Game Object Reference")]
    public TMP_Text scoreText;

    public TMP_Text addScoreText;

    public TMP_Text multiplierText;

    public Image multiplierImg;

    public TMP_Text shakeCountText;

    [SerializeField] private Image comboFillImage;
    [SerializeField] private JarFunction jarFunction;

    [Header("JarShakeLogic")]
    private int lastRewardedCombo = 1;

    [SerializeField] private int maxShakeCount = 5;

    [Header("Combo-Multiplier Settings")]
    private float comboTimeLeft = 0f;   
    private int comboCount = 0;
    private bool comboActive = false;
    private Coroutine comboCoroutine;
    private float comboMultiplier = 1f;
    [SerializeField] private float comboWindow = 5f;
    [SerializeField] private float comboStep = 0.1f;   
    [SerializeField] private float tripleMaxBonus = 1f;

    [Header("Combo SFX Settings")]
    [SerializeField] private float multiplierThreshold = 1f; 
    [SerializeField] private float firstComboAt = 2f; 

private float lastComboSFXMultiplier = 1f; 
    

AudioManager audioManager;
 
 private void Awake(){
     audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
     if (Prefabs == null || Prefabs.Length == 0)
            Prefabs = Resources.LoadAll<GameObject>("Prefab");

 }
    private void Update()
{
    if (!comboActive) return;

    comboTimeLeft -= Time.deltaTime;

    float t = comboTimeLeft / comboWindow;
    comboFillImage.fillAmount = Mathf.Clamp01(t);

    
}
    private void OnEnable() {
        Candy.OnCandyMerged += HandleCandyMerge;
        Candy.OnTripleMaxLevelDetected += HandleTripleMaxLevel;   
        }

    private void OnDisable() {
        Candy.OnCandyMerged -= HandleCandyMerge;
        Candy.OnTripleMaxLevelDetected -= HandleTripleMaxLevel;
    }
    
    
    private void HandleCandyMerge(int level, Vector3 position)
{
        if (CandyMergeManager.gameOver) return;

        RegisterCombo();

    int baseScore = level * 10;
    int finalScore = CalculateScore(baseScore);

    score += finalScore;
    scoreText.text = score.ToString();
    audioManager.PlaySFX(audioManager.candyMerge);

    ShowAddScore(finalScore);
}
    
    
  private void HandleTripleMaxLevel(List<Candy> candies, int mergeCount)
{
        if (CandyMergeManager.gameOver) return;
        foreach (Candy candy in candies)
        if (candy != null) Destroy(candy.gameObject);

    RegisterCombo();

    comboMultiplier += tripleMaxBonus;
    
    CheckAndPlayComboSFX();
    audioManager.PlaySFX(audioManager.candyMerge);

    int baseScore = Candy.maxLevel * 100;
    float specialMultiplier = mergeCount;

    int finalScore = CalculateScore(baseScore, specialMultiplier);

    score += finalScore;
    scoreText.text = score.ToString();

    ShowAddScore(finalScore);
    ShowMultiplier();

    Debug.Log($"MAX MERGE x{specialMultiplier} | Combo x{comboMultiplier} | +{finalScore}");
}

private int CalculateScore(int baseScore, float extraMultiplier = 1f)
{
    float finalMultiplier = comboMultiplier * extraMultiplier;
    return Mathf.CeilToInt(baseScore * finalMultiplier);
}
    private void RegisterCombo()
{
    if (!comboActive)
    {
        comboActive = true;
        comboCount = 0;
        comboMultiplier = 1f;
        lastComboSFXMultiplier = 1f; // Reset khi bắt đầu combo mới
    }
        comboCount++;
        comboMultiplier += comboStep;
        CheckAndRewardShake();

        // Kiểm tra và phát combo SFX
        CheckAndPlayComboSFX();

        comboTimeLeft = comboWindow;
        if (comboCoroutine != null)
        StopCoroutine(comboCoroutine);

        comboCoroutine = StartCoroutine(DelayAction(comboWindow, ResetCombo));

        ShowMultiplier();
    ShowComboBar();
}   


private void ResetCombo()
{
    comboActive = false;
    comboCount = 0;
    comboMultiplier = 1f;
    comboCoroutine = null;
    lastComboSFXMultiplier = 1f;
    lastRewardedCombo = 1;

        multiplierText.gameObject.SetActive(false);
    HideComboBar();
    
}

private void CheckAndPlayComboSFX()
{
    
    if (comboMultiplier < firstComboAt) return;
    
    // cal combo base on multiplier and threshold
    int currentComboLevel = Mathf.FloorToInt((comboMultiplier - firstComboAt) / multiplierThreshold) + 1;
    int lastComboLevel = Mathf.FloorToInt((lastComboSFXMultiplier - firstComboAt) / multiplierThreshold) + 1;
    
    // Check combo level below 5
    if (currentComboLevel > lastComboLevel && currentComboLevel <= 5)
    {
        audioManager.PlayComboSFX(currentComboLevel);
        lastComboSFXMultiplier = comboMultiplier;
        
        Debug.Log($"Combo SFX {currentComboLevel} played at multiplier {comboMultiplier:F1}x");
    }
}
    IEnumerator DelayAction(float delay, Action action)
{
    yield return new WaitForSeconds(delay);
    action?.Invoke();
}
private void ShowAddScore(int value)
{
    addScoreText.text = "+" + value;
    addScoreText.gameObject.SetActive(true);

    if (addScoreCoroutine != null)
        StopCoroutine(addScoreCoroutine);

    addScoreCoroutine = StartCoroutine(
        DelayAction(5f, () => addScoreText.gameObject.SetActive(false))
    );
}

private void ShowMultiplier(){
    multiplierText.text= "x" + comboMultiplier.ToString("0.0");
    multiplierText.gameObject.SetActive(true);

    if(multiplierCoroutine != null){
        StopCoroutine(multiplierCoroutine);
    }
    multiplierCoroutine = StartCoroutine(
        DelayAction(5f, () => multiplierText.gameObject.SetActive(false))
    );
}

private void ShowMultiplierSpecial()
    {
        // Show special multiplier img effect
    }
    private void ShowComboBar()
{
    comboFillImage.transform.parent.gameObject.SetActive(true);
    comboFillImage.fillAmount = 1f;
}

private void HideComboBar()
{
    comboFillImage.transform.parent.gameObject.SetActive(false);
}

    private void CheckAndRewardShake()
    {
        int currentComboInt = Mathf.FloorToInt(comboMultiplier);

        if (currentComboInt > lastRewardedCombo && jarFunction != null)
        {
            int reward = currentComboInt - lastRewardedCombo;
            lastRewardedCombo = currentComboInt;

            int before = jarFunction.shakeCount;
            jarFunction.shakeCount = Mathf.Min(
                jarFunction.shakeCount + reward,
                maxShakeCount
            );

            int gained = jarFunction.shakeCount - before;
            if (gained > 0)
                Debug.Log($"+{gained} Shake (Combo x{currentComboInt})");
        }
    }


    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    public void GameOverScreen()
    {
        // Show Game Over Screen
    }
}
