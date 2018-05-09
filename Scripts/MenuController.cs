using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour {

    private Animator animator;
    public GameObject startImage;
	// Use this for initialization
	void Awake () {
        animator = GetComponent<Animator>();
	}

    public void RestartGame()
    {
        print("RESTART GAME");
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }
	
	public void StartGame()
    {
        animator.SetTrigger("Start");
        GameController.Instance.StartGame();
    }

    public void ShowUi()
    {
        if(animator != null)
        {
            animator.SetTrigger("GameOver");
        } else if(startImage != null)
        {
            startImage.SetActive(true);
        }
    }

    public void hideUI()
    {
        if (startImage != null)
        {
            startImage.SetActive(false);
        }
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
