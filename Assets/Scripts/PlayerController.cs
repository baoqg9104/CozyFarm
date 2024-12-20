using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }
    private float speed = 4f;
    private Rigidbody2D rb;
    private Vector2 movement;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    public Tilemap tilemap;
    public GameObject highlightPrefab;
    private GameObject highlightInstance;
    public TextMeshProUGUI moneyText;
    public int money;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Load game
        money = SaveSystem.GetInt("money", 1000);
        moneyText.text = money.ToString();

        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        highlightInstance = Instantiate(highlightPrefab);
        highlightInstance.SetActive(false);
        UpdatePosition();
    }

    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        movement = movement.normalized;

        if (movement.x != 0)
        {
            spriteRenderer.flipX = movement.x < 0;
        }

        animator.SetBool("IsMoveLeftOrRight", false);
        animator.SetBool("IsMoveTop", false);
        animator.SetBool("IsMoveBottom", false);

        if (movement.x != 0)
        {
            animator.SetBool("IsMoveLeftOrRight", true);
        }
        else if (movement.y > 0)
        {
            animator.SetBool("IsMoveTop", true);
        }
        else if (movement.y < 0)
        {
            animator.SetBool("IsMoveBottom", true);
        }

        UpdateHighlightPosition();
    }

    public void UpdatePosition()
    {
        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.isSampleScene)
            {
                transform.position = new Vector3(-10.63f, 2.77f, 0);
            }
            else if (GameManager.Instance.isHouse)
            {
                transform.position = new Vector3(-84.507f, -6.116f, 0);
            }
            else if (GameManager.Instance.isChickenCoop)
            {
                transform.position = new Vector3(-84.785f, -24.605f, 0);
            }
            else if (GameManager.Instance.isCowHouse)
            {
                transform.position = new Vector3(-85.325f, -46.147f, 0);
            }
            else if (GameManager.Instance.isMarket)
            {
                transform.position = new Vector3(-84.085f, 15.875f, 0);
            }
            else if (GameManager.Instance.isReturn)
            {
                transform.position = GameManager.Instance.lastPosition + new Vector3(0, -0.5f, 0);
            }
        }
    }

    void FixedUpdate()
    {
        // Di chuyển nhân vật
        rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);
    }

    void UpdateHighlightPosition()
    {
        if (GameManager.Instance != null && GameManager.Instance.isSampleScene)
        {
            Vector3 mouseScreenPosition = Input.mousePosition;
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
            Vector3Int cellPosition = tilemap.WorldToCell(worldPosition);
            Vector3 cellCenterPosition = tilemap.GetCellCenterWorld(cellPosition);
            highlightInstance.transform.position = cellCenterPosition;
            highlightInstance.SetActive(true);
        }
        else
        {
            highlightInstance.SetActive(false);
        }
    }

    private void OnApplicationQuit()
    {
        SaveSystem.SetInt("money", money);
    }

}
