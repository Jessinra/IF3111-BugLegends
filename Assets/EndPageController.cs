using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndPageController : MonoBehaviour
{
    // Start is called before the first frame update

    public Text winnerText;

    void Start()
    {
        string winner = PlayerPrefs.GetString("winner");
        winnerText.text = winner;
    }
}
