﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player_cubele : MonoBehaviour
{
    [SerializeField] float moveSpeed = 1f;
    [SerializeField] float rotateSpeed = 1f;
    [SerializeField] float jumpSpeed = 10f;

    [SerializeField] GameObject explosion;
    [SerializeField] GameObject[] dustPrefabs;
    [SerializeField] GameObject jumpEffectPrefab;
    [SerializeField] GameObject wallCollisionPrefab;

    bool moveRight;
    bool isDead;

    Rigidbody2D rb2d;
    SpriteRenderer spriteRenderer;
    SoundsManager_cubele soundsManager;
    Animator animator;
    Animator cameraAnimator;

    public bool IsBouncing { get; set; }
    bool ignoreCollisionsAndControlls;

    void Awake() {
        rb2d = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        cameraAnimator = Camera.main.GetComponent<Animator>();

        soundsManager = FindObjectOfType<SoundsManager_cubele>();
    }

    void Start() {
        EventManager.AddListener("StartGame", OnStartGame);
        EventManager.AddListener("Collision", OnCollision);
    }

    void Update() {
        if (!LevelManager_cubele.IsGameRunning)
            return;

        if (isDead)
            return;

        transform.RotateAround(
            transform.position,
            moveRight ? Vector3.back : Vector3.forward,
            rotateSpeed * Time.deltaTime
        );

        if (!ignoreCollisionsAndControlls && (
            Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)
        ))
            Jump();
    }

    void OnTriggerEnter2D(Collider2D collision) {
        if (isDead)
            return;

        if (collision.gameObject.CompareTag("Wall") && !ignoreCollisionsAndControlls) {
            if (IsBouncing)
                BounceFromWall();
            else
                EventManager.TriggerEvent("Collision");

            MakeCollisionEffect();
        }
        else if (collision.gameObject.CompareTag("DeadZone")) {
            // dead zones are on the top and on the bottom of play area
            EventManager.TriggerEvent("Collision");

            MakeCollisionEffect();
        }
    }

    void OnDestroy() {
        EventManager.RemoveListener("StartGame", OnStartGame);
        EventManager.RemoveListener("Collision", OnCollision);
    }

    void OnStartGame() {
        animator.SetBool("IsGameRunning", true);

        rb2d.bodyType = RigidbodyType2D.Dynamic;
        rb2d.velocity = Vector2.zero;

        rb2d.AddForce(new Vector2(
            0,
            jumpSpeed
        ));

        soundsManager.Jump();

        MakeDust();
    }

    void Jump() {
        // change the direction
        moveRight = !moveRight;

        rb2d.velocity = Vector2.zero;

        rb2d.AddForce(new Vector2(
            moveRight ? moveSpeed : -moveSpeed,
            jumpSpeed
        ));

        soundsManager.Jump();

        MakeDust();
        MakeJumpEffect();
    }

    void OnCollision() {
        if (isDead)
            return;

        isDead = true;

        cameraAnimator.SetTrigger("Shake");

        spriteRenderer.enabled = false;

        GameObject explosionObject = Instantiate(explosion);
        explosionObject.transform.position = new Vector3(
            transform.position.x, transform.position.y, 0
        );

        PlayerStats.Distance += transform.position.y;

        EventManager.TriggerEvent("PlayerDie");

        soundsManager.Explosion();

        StartCoroutine(GameOver());
    }

    IEnumerator GameOver() {
        yield return new WaitForSeconds(1.5f);

        // panel will be shown
        PlayerStats.ShowGameOverPanel = true;

        SceneManager.LoadSceneAsync(0 , LoadSceneMode.Single);
    }

    void BounceFromWall() {
        cameraAnimator.SetTrigger("Shake");

        // bounce to oposit side
        moveRight = transform.position.x < 0;

        rb2d.velocity = Vector2.zero;

        rb2d.AddForce(new Vector2(
            (moveRight ? moveSpeed : -moveSpeed) * 2f,
            jumpSpeed / 2f
        ));

        soundsManager.WallCollision();

        StartCoroutine(IgnoreCollision());
    }

    IEnumerator IgnoreCollision() {
        ignoreCollisionsAndControlls = true;

        yield return new WaitForSeconds(0.05f);

        ignoreCollisionsAndControlls = false;
    }

    void MakeDust() {
        foreach (GameObject dust in dustPrefabs) {
            GameObject dustObject = Instantiate(dust);
            dustObject.transform.position = new Vector3(
                transform.position.x, transform.position.y - 0.15f, 0
            );
        }
    }

    void MakeJumpEffect() {
        GameObject jumpEffect = Instantiate(jumpEffectPrefab);
        jumpEffect.transform.position = new Vector3(
            transform.position.x, transform.position.y - 0.15f, 0
        );
        jumpEffect.GetComponent<PlayerJumpEffect>().SetRotation(transform.localRotation);
    }

    void MakeCollisionEffect() {
        GameObject effect = Instantiate(wallCollisionPrefab);
        effect.transform.position = transform.position;
        effect.GetComponent<PlayerJumpEffect>().SetRotation(transform.localRotation);
    }
}
