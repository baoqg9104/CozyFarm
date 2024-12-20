using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FishingController : MonoBehaviour
{
    private bool isPlayer = false;
    private bool isFishing = false;
    public GameObject player;
    public GameObject playerFishing;
    public InventoryManager inventoryManager;
    public Item fishingRodItem;
    private float minFishingTime;
    private float maxFishingTime; 

    public Item[] fishItems;

    private Coroutine fishingCoroutine;

    private void Start() {
        minFishingTime = 30f;
        maxFishingTime = 60f;
    }

    public bool getIsFishing() {
        return isFishing;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            isPlayer = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            isPlayer = false;
        }
    }

    private void Update() {
        if (isPlayer && Input.GetKeyDown(KeyCode.E)) {
            if (!isFishing) {
                Item receivedItem = inventoryManager.GetSelectedItem(false);
                if (receivedItem != null && receivedItem == fishingRodItem) {
                    StartFishing();
                }
            } else {
                CancelFishing();
            }
        }
    }

    private void StartFishing() {
        isFishing = true;
        player.GetComponent<SpriteRenderer>().enabled = false;
        player.GetComponent<PlayerController>().enabled = false;
        playerFishing.gameObject.SetActive(true);
        // inventoryManager.GetComponent<InventoryManager>().enabled = false;

        fishingCoroutine = StartCoroutine(FishCoroutine());
    }

    private IEnumerator FishCoroutine() {

        InventoryItem fishingRod = inventoryManager.GetSelectedInventoryItem();

        while (isFishing) {
            // Nếu không tìm thấy công cụ câu cá, dừng câu cá
            if (fishingRod == null || fishingRod.item != fishingRodItem) {
                Debug.LogError("Fishing rod not found or mismatched.");
                CancelFishing();
                yield break;
            }


            // Thời gian ngẫu nhiên để câu thành công
            float fishingTime = Random.Range(minFishingTime, maxFishingTime);

            yield return new WaitForSeconds(fishingTime);

            // Giảm độ bền của công cụ câu cá
            fishingRod.ReduceDurability();

            // Kiểm tra nếu còn độ bền
            if (fishingRod.durability > 0)
            {
                Item randomFish = fishItems[Random.Range(0, fishItems.Length)];
                inventoryManager.AddItem(randomFish);

            }
            else
            {
                inventoryManager.RemoveItem(fishingRod);
                CancelFishing();
                yield break;
            }
        }
    }

    private void CancelFishing() {
        isFishing = false;
        player.GetComponent<SpriteRenderer>().enabled = true;
        player.GetComponent<PlayerController>().enabled = true;
        playerFishing.gameObject.SetActive(false);
        // inventoryManager.GetComponent<InventoryManager>().enabled = true;

        if (fishingCoroutine != null) {
            StopCoroutine(fishingCoroutine);
            fishingCoroutine = null;
        }
    }
}
