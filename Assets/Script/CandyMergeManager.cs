using UnityEngine;

public class CandyMergeManager : MonoBehaviour
{
    private int score;

    private void OnEnable()
    {
        Candy.OnCandyMerged += HandleCandyMerged;
    }

    private void OnDisable()
    {
        Candy.OnCandyMerged -= HandleCandyMerged;
    }

    private void HandleCandyMerged(int level, Vector3 position)
    {
        AddScore(level);
        PlaySound();
        PlayEffect(position);
    }

    private void AddScore(int level)
    {
       //
    }

    private void PlaySound()
    {
        // AudioSource.PlayOneShot(...)
    }

    private void PlayEffect(Vector3 pos)
    {
        // Instantiate particle / effect
    }
}
