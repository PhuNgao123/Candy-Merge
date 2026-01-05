using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private static bool gameIsPaused = false;
    public GameObject PauseMenuUI;

    // Update is called once per frame

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(gameIsPaused){
                Resume();
            }else 
            {
                Pause();
            }

        }
        
        }
        private void Resume(){
            PauseMenuUI.SetActive(false);
            Time.timeScale= 1f;
            gameIsPaused = false;
        }

        private void Pause(){
            PauseMenuUI.SetActive(true);
            Time.timeScale= 0f;
            gameIsPaused = true;
    }
}

