using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class UiManager : MonoBehaviour
{
   
    

    void Start()
    {
        Time.timeScale = 1;
    }

  
    void Update()
    {
        
    }

    public void Home_panel()
    {
        SceneManager.LoadScene("Menu");
    }
    public void pause_game()
    {
        Time.timeScale = 0;
    }
    public void resume_game()
    {
        Time.timeScale = 1;
    }
}
