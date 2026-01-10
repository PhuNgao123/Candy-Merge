using UnityEngine;

public class GameOverTrigger : MonoBehaviour
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // Update is called once per frame

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Touched: " + collision.name);
        if (collision.CompareTag(collision.tag) && collision.tag.StartsWith("C"))
        {
            CandyMergeManager.gameOver = true;
            Debug.Log("Game Over!");
        }
    }
}
