using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;

public class EndPageController : MonoBehaviour {
    // Start is called before the first frame update

    public Text winnerText;
    public Text winnerNameText;
    public Button submitButton;

    void Start() {
        string winner = PlayerPrefs.GetString("winner");
        winnerText.text = winner;

        submitButton.onClick.AddListener(submitHistory);
    }

    void submitHistory() {
        string winnerName = winnerNameText.text;
        StartCoroutine(logHistory(winnerName));
    }

    IEnumerator logHistory(string name) {

        UnityWebRequest logger = UnityWebRequest.Get("https://us-central1-if3111-smartdoor.cloudfunctions.net/bugLegendHistoryLogger?name=" + name+"&score=100");
        yield return logger.SendWebRequest();

        Debug.Log("sendig to firebase " + name);
        SceneManager.LoadScene(sceneName: "TitleScene");

        yield return new WaitForSeconds(0);
    } 

}