using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class ButtonController : MonoBehaviour
{
    public Button startButton;
    public string onClickPlayScene = "GameScene01";
    
    void Start()
    {
        startButton.onClick.AddListener(StartGame);
    }

    void StartGame(){
        Debug.Log("starting game");
        SceneManager.LoadScene (sceneName:onClickPlayScene);
    }
}
