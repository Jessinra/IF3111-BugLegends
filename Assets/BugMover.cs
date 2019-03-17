using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BugMover : MonoBehaviour {

    [SerializeField] private float speed = 0.0F;
    private Rigidbody2D rigidBody;

    public int power = 500;
    public int isRight = 1;

    private int thisPower;

    // Start is called before the first frame update
    void Start() {
        rigidBody = GetComponent<Rigidbody2D>();
        rigidBody.velocity = transform.forward * speed;

        thisPower = power;
    }

    // Update is called once per frame
    void Update() {

        if (thisPower > 0) {
            transform.Translate(Vector2.right * Time.deltaTime * speed * thisPower / power * isRight);
            transform.Translate(Vector2.up * Time.deltaTime * speed * thisPower / power);
            thisPower--;
        }
    }
}