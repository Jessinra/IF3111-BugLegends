using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {
    public string endSceneName;
    public const float delayBeforeEnd = 5F;

    public static bool GameOver = false;
    private float timer = 0F;


    void Update() {
        if (GameOver) {
            StartCoroutine(endGame());
        }
    }

    IEnumerator endGame() {
        Debug.Log("ending");

        this.timer += Time.deltaTime;

        // check if it's time to switch scenes
        if (this.timer >= delayBeforeEnd) {
            SceneManager.LoadScene(endSceneName);
        }

        yield return new WaitForSeconds(1);
    }
}