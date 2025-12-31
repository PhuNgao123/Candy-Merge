using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System;
using System.Collections;

public class CandyMergeManager : MonoBehaviour
{
    private int score =0;
    public TMP_Text scoreText;

    public TMP_Text addScoreText;
    private float comboMultiplier = 1f;
private int comboCount = 0;
private bool comboActive = false;
private Coroutine comboCoroutine;

private Coroutine addScoreCoroutine;

[SerializeField] private float comboWindow = 5f;
[SerializeField] private float comboStep = 0.1f;   // mỗi merge +0.1x
    
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
    RegisterCombo();

    int baseScore = level * 10;
    int finalScore = CalculateScore(baseScore);

    score += finalScore;
    scoreText.text = score.ToString();

    ShowAddScore(finalScore);
}
    
    
  private void HandleTripleMaxLevel(List<Candy> candies, int mergeCount)
{
    foreach (Candy candy in candies)
        if (candy != null) Destroy(candy.gameObject);

    RegisterCombo();

    int baseScore = Candy.maxLevel * 100;
    float specialMultiplier = mergeCount;

    int finalScore = CalculateScore(baseScore, specialMultiplier);

    score += finalScore;
    scoreText.text = score.ToString();

    ShowAddScore(finalScore);

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

        if (comboCoroutine != null)
            StopCoroutine(comboCoroutine);

        comboCoroutine = StartCoroutine(DelayAction(comboWindow, ResetCombo));
    }

    comboCount++;
    comboMultiplier += comboStep;
}

private void ResetCombo()
{
    comboActive = false;
    comboCount = 0;
    comboMultiplier = 1f;
    comboCoroutine = null;
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
        DelayAction(3f, () => addScoreText.gameObject.SetActive(false))
    );
}
}
