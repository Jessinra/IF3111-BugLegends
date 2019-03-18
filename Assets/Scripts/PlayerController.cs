using System.Collections;
using System.Collections.Generic;
using BirdDogGames.PaperDoll;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    [SerializeField] private DollInstance player = null;
    [SerializeField] private float moveSpeed = 15F;
    [SerializeField] private float jumpSpeed = 15F;
    [SerializeField] private int airboneTime = 100;

    [SerializeField] private BugConfig bugConfig = null;
    [SerializeField] private PlatformConfig platformConfig = null;

    private Rigidbody2D rigidBody;
    private Transform transform;
    private PlayerAnimator animator;
    private string direction = "right";
    private int timeBeforeLand;

    private float nextBug = 0.0F;
    private float nextWall = 0.0F;
    private float nextFloor = 0.0F;
    private float nextRoof = 0.0F;
    private float nextStair = 0.0F;

    void Start() {
        rigidBody = player.GetComponent<Rigidbody2D>();
        transform = player.GetComponent<Transform>();
        animator = new PlayerAnimator(player);

        timeBeforeLand = airboneTime;
        StartCoroutine(getInput());

    }

    void OnTriggerEnter2D(Collider2D other) {
        Debug.Log("trigger");
        timeBeforeLand = airboneTime;
    }

    void FixedUpdate() { }

    IEnumerator getInput() {

        while (true) {
            if (timeBeforeLand > 0) {

                if (Input.GetKey("t")) {
                    jump();
                    timeBeforeLand--;

                } else if (Input.GetKey("f") || Input.GetKey("h")) {
                    move();

                } else if (Input.GetKeyUp("t") || Input.GetKeyUp("f") || Input.GetKeyUp("h")) {
                    idle();
                }
            }

            if (timeBeforeLand == 0) {
                Debug.Log("out");
                timeBeforeLand = airboneTime;
                idle();
                yield return new WaitForSeconds(1.5F);
                continue;
            }

            if (Input.GetKeyDown("1")) {
                buildRoof();

            } else if (Input.GetKeyDown("2")) {
                buildFloor();

            } else if (Input.GetKeyDown("3")) {
                buildWall();

            } else if (Input.GetKeyDown("4")) {
                buildStair();

            } else if (Input.GetKey("space") && ableToAttack()) {
                attack();
            } else if (Input.GetKey(KeyCode.LeftShift) && ableToAttack()) {
                ultiAttack();
            }

            yield return new WaitForFixedUpdate();
        }

    }
    private void move() {

        float moveHorizontal = Input.GetAxis("Horizontal");
        Vector2 movement = new Vector2(moveHorizontal, -0.8F);
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

        nextBug = Time.time + bugConfig.shotDelay;

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

    private void ultiAttack() {
        animator.AnimateAttack();
        noMovement();

        nextBug = Time.time + bugConfig.shotDelay;

        for (int i = 0; i < 10; i++) {

            if (direction == "right") {
                Instantiate(bugConfig.bugR,
                    bugConfig.bugSpawnPoint.position + new Vector3(i * 3, i * 3, 0),
                    bugConfig.bugSpawnPoint.rotation);
            } else {
                Instantiate(bugConfig.bugL,
                    bugConfig.bugSpawnPoint.position + new Vector3(i * -3, i * 3, 0),
                    bugConfig.bugSpawnPoint.rotation);
            }
        }
    }

    private void buildRoof() {
        animator.AnimateAttack();
        noMovement();
        nextRoof = Time.time + platformConfig.spawnRoofDelay;

        Instantiate(platformConfig.platformFlat,
            platformConfig.platformRoofSpawnPoint.position,
            platformConfig.platformRoofSpawnPoint.rotation);
    }

    private void buildFloor() {
        animator.AnimateAttack();
        noMovement();
        nextFloor = Time.time + platformConfig.spawnFloorDelay;

        Instantiate(platformConfig.platformFlat,
            platformConfig.platformFloorSpawnPoint.position,
            platformConfig.platformFloorSpawnPoint.rotation);
    }

    private void buildWall() {
        animator.AnimateAttack();
        noMovement();
        nextWall = Time.time + platformConfig.spawnWallDelay;

        Instantiate(platformConfig.platformWall,
            platformConfig.platformWallSpawnPoint.position,
            platformConfig.platformWallSpawnPoint.rotation);
    }

    private void buildStair() {
        animator.AnimateAttack();
        noMovement();
        nextStair = Time.time + platformConfig.spawnStairDelay;

        if (direction == "right") {

            Instantiate(platformConfig.platformStairR,
                platformConfig.platformStairSpawnPoint.position,
                platformConfig.platformStairSpawnPoint.rotation);
        } else {
            Instantiate(platformConfig.platformStairL,
                platformConfig.platformStairSpawnPoint.position,
                platformConfig.platformStairSpawnPoint.rotation);
        }

    }

    private bool ableToAttack() {
        return Time.time > nextBug;
    }
    private bool ableToBuildRoof() {
        return Time.time > nextRoof;
    }
    private bool ableToBuildFloor() {
        return Time.time > nextFloor;
    }
    private bool ableToBuildWall() {
        return Time.time > nextWall;
    }
    private bool ableToBuildStair() {
        return Time.time > nextStair;
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
    public float shotDelay = 0.2F;
}

[System.Serializable]
public class PlatformConfig {
    public GameObject platformFlat;
    public GameObject platformWall;
    public GameObject platformStairR;
    public GameObject platformStairL;

    public Transform platformRoofSpawnPoint;
    public Transform platformFloorSpawnPoint;
    public Transform platformWallSpawnPoint;
    public Transform platformStairSpawnPoint;

    public float spawnRoofDelay = 0.3F;
    public float spawnFloorDelay = 0.3F;
    public float spawnWallDelay = 0.3F;
    public float spawnStairDelay = 0.3F;
}

public class PlayerAnimator {
    private string playerIdle = "dummy_idle_001";
    private string playerWalk = "dummy_run_001";
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