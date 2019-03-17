using System.Collections;
using System.Collections.Generic;
using BirdDogGames.PaperDoll;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    [SerializeField] private DollInstance player = null;
    [SerializeField] private float moveSpeed = 3F;
    [SerializeField] private float jumpSpeed = 3F;
    [SerializeField] private BugConfig bugConfig = null;

    private Rigidbody2D rigidBody;
    private Transform transform;
    private PlayerAnimator animator;
    private string direction = "right";
    private float nextShot = 0.0F;

    void Start() {
        rigidBody = player.GetComponent<Rigidbody2D>();
        transform = player.GetComponent<Transform>();
        animator = new PlayerAnimator(player);
    }

    void FixedUpdate() {

        if (Input.GetKey("w")) {
            jump();

        } else if (Input.GetKey("a") || Input.GetKey("d")) {
            move();

        } else if (Input.GetKey("space") && ableToAttack()) {
            attack();

        } else if (Input.GetKeyDown("1")) {
            print("build 1");

        } else if (Input.GetKeyDown("2")) {
            print("build 2");

        } else if (Input.GetKeyDown("3")) {
            print("build 3");

        } else if (Input.GetKeyUp("w") || Input.GetKeyUp("a") || Input.GetKeyUp("d")) {
            idle();
        }
    }

    private void move() {

        float moveHorizontal = Input.GetAxis("Horizontal");
        Vector2 movement = new Vector2(moveHorizontal, 0);
        rigidBody.velocity = movement * moveSpeed;

        animator.AnimateWalk();
        setFaceDirection(movement);
    }

    private void jump() {

        // Set movement
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        Vector2 movement = new Vector2(moveHorizontal, moveVertical);
        rigidBody.velocity = movement * jumpSpeed;

        animator.AnimateJump();
        setFaceDirection(movement);
    }

    private void idle() {
        animator.AnimateIdle();
        noMovement();
    }

    private void attack() {
        animator.AnimateAttack();
        noMovement();

        nextShot = Time.time + bugConfig.shotDelay;

        if (direction == "right") {
            Instantiate(bugConfig.bugR,
                bugConfig.bugSpawnPoint.position,
                bugConfig.bugSpawnPoint.rotation);
        } else {
            Instantiate(bugConfig.bugL,
                bugConfig.bugSpawnPoint.position,
                bugConfig.bugSpawnPoint.rotation);
        }
    }

    private bool ableToAttack() {
        return Time.time > nextShot;
    }

    private void crouch() {
        animator.AnimateCrouch();
        noMovement();
    }

    private void setFaceDirection(Vector2 movement) {
        if (movement.x < 0) {
            transform.transform.localScale = new Vector2(-1.0f, 1.0f);
            direction = "left";
        } else if (movement.x > 0) {
            transform.transform.localScale = new Vector2(1.0f, 1.0f);
            direction = "right";
        }
    }

    private void noMovement() {
        rigidBody.velocity = new Vector2(0, 0);
    }
}

[System.Serializable]
public class BugConfig {
    public GameObject bugR;
    public GameObject bugL;
    public Transform bugSpawnPoint;
    public float shotDelay = 0.0F;
}

public class PlayerAnimator {
    private string playerIdle = "dummy_idle_001";
    private string playerWalk = "dummy_walk_001";
    private string playerJump = "dummy_jump_001";
    private string playerFall = "dummy_fall_001";
    private string playerAttack = "dummy_atk_punch_fast_001";
    private string playerCrouch = "dummy_crouch_001";
    private string playerImpact = "dummy_impact_001";

    private DollInstance player;

    public PlayerAnimator(DollInstance player) {
        this.player = player;
    }

    public void AnimateIdle() {
        this.player.view.AnimationName = playerIdle;
    }

    public void AnimateWalk() {
        this.player.view.AnimationName = playerWalk;
    }

    public void AnimateJump() {
        this.player.view.AnimationName = playerJump;
    }

    public void AnimateFall() {
        this.player.view.AnimationName = playerFall;
    }

    public void AnimateAttack() {
        this.player.view.AnimationName = playerAttack;
    }

    public void AnimateCrouch() {
        this.player.view.AnimationName = playerCrouch;
    }

    public void AnimateImpact() {
        this.player.view.AnimationName = playerImpact;
    }
}