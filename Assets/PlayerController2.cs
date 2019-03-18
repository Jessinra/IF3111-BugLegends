using System.Collections;
using System.Collections.Generic;
using BirdDogGames.PaperDoll;
using UnityEngine;

public class PlayerController2 : MonoBehaviour {

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
        Debug.Log("trigger2");
        timeBeforeLand = airboneTime;
    }

    void FixedUpdate() { }

    IEnumerator getInput() {

        while (true) {
            if (timeBeforeLand > 0) {

                if (Input.GetKey(KeyCode.UpArrow)) {
                    jump();
                    timeBeforeLand--;

                } else if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow)) {
                    move();

                } else if (Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow)) {
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

            if (Input.GetKeyDown("k")) {
                buildRoof();
            } else if (Input.GetKeyDown("l")) {
                buildFloor();

            } else if (Input.GetKeyDown(";")) {
                buildWall();

            } else if (Input.GetKeyDown("'")) {
                buildStair();

            } else if (Input.GetKey(KeyCode.RightControl) && ableToAttack()) {
                attack();
            } else if (Input.GetKey(KeyCode.RightShift) && ableToAttack()) {
                ultiAttack();
            }

            yield return new WaitForFixedUpdate();
        }

    }
    private void move() {

        float moveHorizontal = Input.GetAxis("Horizontal2");
        Vector2 movement = new Vector2(moveHorizontal, -0.8F);
        rigidBody.velocity = movement * moveSpeed;

        animator.AnimateWalk();
        setFaceDirection(movement);
    }

    private void jump() {

        // Set movement
        float moveHorizontal = Input.GetAxis("Horizontal2");
        float moveVertical = Input.GetAxis("Vertical2");
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
