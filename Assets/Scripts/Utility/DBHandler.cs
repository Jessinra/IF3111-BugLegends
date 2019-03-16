using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
 
public class DBHandler : MonoBehaviour {
 
    public IEnumerator addHistoryToDB(string name, string score) {

        UnityWebRequest www = UnityWebRequest.Get("https://us-central1-if3111-smartdoor.cloudfunctions.net/bugLegendHistoryLogger?name="+name+"&score="+score);
        yield return www.SendWebRequest();
 
        if(www.isNetworkError || www.isHttpError) {
            Debug.Log(www.error);
        }
        else {
            // Show results as text
            Debug.Log(www.downloadHandler.text);
 
            // Or retrieve results as binary data
            byte[] results = www.downloadHandler.data;
        }

    }

    public IEnumerator fetchHistory() {

        UnityWebRequest www = UnityWebRequest.Get("https://us-central1-if3111-smartdoor.cloudfunctions.net/bugLegendHistory");
        yield return www.SendWebRequest();
 
        if(www.isNetworkError || www.isHttpError) {
            Debug.Log(www.error);
        }
        else {
            // Show results as text
            Debug.Log(www.downloadHandler.text);
 
            // Or retrieve results as binary data
            byte[] results = www.downloadHandler.data;
        }

    }
}