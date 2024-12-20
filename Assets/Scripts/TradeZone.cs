using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TradeZone : MonoBehaviour
{
    public Canvas marketCanvas;
    public PlayerController playerController;
    public CameraZoom cameraZoom;
    private bool isTradeZone = false;
    private bool isTradePanelOpen = false;

    void Start() {
        marketCanvas.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isTradeZone = true;
        }
    }

    void Update() {
        if (isTradeZone) {
            if (Input.GetKeyDown(KeyCode.E)) {
                ToggleTradePanel();
            } else if (isTradePanelOpen && Input.GetKeyDown(KeyCode.Escape)) {
                ToggleTradePanel();
            }
        }
    }

    public void ToggleTradePanel()
    {
        isTradePanelOpen = !isTradePanelOpen;
        marketCanvas.enabled = isTradePanelOpen;
        playerController.enabled = !isTradePanelOpen;
        cameraZoom.enabled = !isTradePanelOpen;
    }
}
