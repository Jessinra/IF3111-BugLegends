using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {
    public string endSceneName;
    public float delayBeforeStart = 5F;
    public float delayBeforeEnd = 5F;

    public static bool GameOver = false;
    public static bool focusedOnAll = false;
    public static bool focusedOn01 = false;
    public static bool focusedOn02 = false;

    private float timerStart = 0F;
    private float timerEnd = 0F;

    void Start() {
        timerStart = 0F;
        timerEnd = 0F;
    }

    void Update() {

        if (!focusedOnAll) {
            StartCoroutine(startGame());
        }

        if (GameOver) {
            StartCoroutine(endGame());
        }
    }

    IEnumerator startGame() {

        this.timerStart += Time.deltaTime;

        if (this.timerStart >= 6 * delayBeforeStart && !focusedOnAll) {
            Debug.Log("focusing 0");
            CameraController.focusOn(0);
            focusedOnAll = true;

        } else if (this.timerStart >= 3 * delayBeforeStart && !focusedOn02) {
            Debug.Log("focusing 2");
            CameraController.focusOn(2);
            focusedOn02 = true;

        } else if (this.timerStart >= delayBeforeStart && !focusedOn01) {
            Debug.Log("focusing 1");
            CameraController.focusOn(1);
            focusedOn01 = true;
        }

        yield return new WaitForSeconds(1);
    }

    IEnumerator endGame() {
        Debug.Log("ending");

        this.timerEnd += Time.deltaTime;

        // check if it's time to switch scenes
        if (this.timerEnd >= delayBeforeEnd) {
            SceneManager.LoadScene(endSceneName);
        }

        yield return new WaitForSeconds(1);
    }
}