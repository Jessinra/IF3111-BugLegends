using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyWhenOffScreen : MonoBehaviour {
	// Update is called once per frame
	void Update () {
		// Once we've gone off the left side of the screen, destroy!
		if(Camera.main.WorldToViewportPoint(transform.position).x < -.1f)
		{
			Destroy(gameObject);
		}
	}
}
