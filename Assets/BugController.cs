using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BugController : MonoBehaviour {

    [SerializeField] private float lifetime = 3.0F;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(this.gameObject, lifetime);
    }

    // Destroy everything that enters the trigger
    void OnTriggerEnter2D(Collider2D other) {

        Collider2D ownCollider = this.GetComponent<Collider2D>();

        // Scoring
        if (other.CompareTag("code") && ownCollider.CompareTag("bug") || other.CompareTag("bug") && ownCollider.CompareTag("code")) {
            Destroy(other.gameObject);
            Destroy(this.gameObject);
        }

        if (other.CompareTag("Player") && ownCollider.CompareTag("bug") || other.CompareTag("bug") && ownCollider.CompareTag("Player")) {
            Destroy(other.gameObject);
            Destroy(this.gameObject);

            print("game over bitch");
        }
    }
}