using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public GameObject[] cameraList;

    private static GameObject[] staticCameraList;
    private static int currentCamera = 0 ;

    // Start is called before the first frame update
    void Start() {

        CameraController.staticCameraList = cameraList;

        if (cameraList.Length > 0) {
            cameraList[0].gameObject.SetActive(true);
            currentCamera = 0;
        }
    }

    // Update is called once per frame
    void FixedUpdate() {
        if (Input.GetKeyDown(KeyCode.Mouse1)) {
            currentCamera++;
            if (currentCamera < cameraList.Length) {
                cameraList[currentCamera - 1].gameObject.SetActive(false);
                cameraList[currentCamera].gameObject.SetActive(true);
            } else {
                currentCamera = 0;
                cameraList[currentCamera].gameObject.SetActive(true);
            }
        }
    }

    public static void focusOn(int i){
        CameraController.staticCameraList[currentCamera].gameObject.SetActive(false);
        CameraController.staticCameraList[i].gameObject.SetActive(true);
    }
}