using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FarmLand : MonoBehaviour
{
    [Serializable]
    private class TileData
    {
        public Vector3Int position;
        public string tileName;
    }

    [Serializable]
    private class GameData
    {
        public List<TileData> farmlandTiles = new List<TileData>();
    }

    public Tilemap tilemap;
    public Collider2D triggerArea;
    public TileBase[] tileFarmLand;
    public TileBase[] tilePotato;
    public TileBase[] tileTomato;
    public Animator animator;
    public Transform player;
    public SpriteRenderer playerRenderer;
    public PlayerController playerController;
    public Item hoeItem;
    public Item wateringBucketItem;
    public Item potatoSeedsItem;
    public Item tomatoSeedsItem;
    public Item potatoItem;
    public Item tomatoItem;
    public InventoryManager inventoryManager;

    private float maxDistance = 1.25f;
    // private float timeToGrowOfPotato = 144f;
    // private float timeToGrowOfTomato = 120f;
    private float timeToGrowOfPotato = 2f;
    private float timeToGrowOfTomato = 2f;

    private string saveFilePath;


    private void Start()
    {
        saveFilePath = Path.Combine(Application.persistentDataPath, "farm_land.json");
        LoadGame();
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Item receivedItem = inventoryManager.GetSelectedItem(false);

            if (receivedItem == hoeItem)
            {
                PlaceTile();
            }
            else if (receivedItem != null && receivedItem.type == ItemType.Seed)
            {
                Sow(receivedItem);
            }
            else if (receivedItem == wateringBucketItem)
            {
                Watering(receivedItem);
            }
            else
            {
                Collect();
            }

        }


    }

    void PlaceTile()
    {
        Vector3 mouseScreenPosition = Input.mousePosition;
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
        Vector3Int cellPosition = tilemap.WorldToCell(worldPosition);

        if (IsWithinTriggerArea(cellPosition) && IsWithinDistance(cellPosition) && CheckState(cellPosition))
        {
            if (tilemap.GetTile(cellPosition) == null)
            {
                animator.SetTrigger("Hoe");

                tilemap.SetTile(cellPosition, tileFarmLand[0]);

                // Reduce durability Hoe
                InventoryItem hoeTool = inventoryManager.GetSelectedInventoryItem();
                hoeTool.ReduceDurability();

                if (hoeTool.durability == 0)
                {
                    inventoryManager.RemoveItem(hoeTool);
                }

                StartCoroutine(DisablePlayerController(1));

            }
            else if (tilemap.GetTile(cellPosition).name == tilePotato[tilePotato.Length - 1].name)
            {
                inventoryManager.AddItem(potatoItem);

                tilemap.SetTile(cellPosition, null);
            }
            else if (tilemap.GetTile(cellPosition).name == tileTomato[tileTomato.Length - 1].name)
            {
                inventoryManager.AddItem(tomatoItem);

                tilemap.SetTile(cellPosition, null);
            }
        }
    }

    void Sow(Item seedItem)
    {
        Vector3 mouseScreenPosition = Input.mousePosition;
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
        Vector3Int cellPosition = tilemap.WorldToCell(worldPosition);

        TileBase[] tileSeed = tilePotato;
        Item itemAdd = potatoItem;

        if (seedItem.image.name == potatoSeedsItem.image.name)
        {
            tileSeed = tilePotato;
            itemAdd = potatoItem;
        }
        else if (seedItem.image.name == tomatoSeedsItem.image.name)
        {
            tileSeed = tileTomato;
            itemAdd = tomatoItem;
        }

        if (IsWithinTriggerArea(cellPosition) && IsWithinDistance(cellPosition) && CheckState(cellPosition))
        {
            if (tilemap == null || tilemap.GetTile(cellPosition) == null)
            {
                return;
            }

            if (tilemap.GetTile(cellPosition).name == tileFarmLand[0].name || tilemap.GetTile(cellPosition).name == tileFarmLand[1].name)
            {

                animator.SetTrigger("Sow");

                // If the land has not been watering
                if (tilemap.GetTile(cellPosition).name == tileFarmLand[0].name)
                {
                    tilemap.SetTile(cellPosition, tileSeed[0]);
                }
                else
                {  // If the land has been watering 
                    tilemap.SetTile(cellPosition, tileSeed[1]);

                    // Bắt đầu phát triển cây
                    StartCoroutine(GrowPlant(cellPosition, tileSeed, tileSeed[1]));
                }

                // Reduce count Seeds
                InventoryItem seedInventoryItem = inventoryManager.GetSelectedInventoryItem();
                seedInventoryItem.ReduceCount();

                if (seedInventoryItem.count == 0)
                {
                    inventoryManager.RemoveItem(seedInventoryItem);
                }

                StartCoroutine(DisablePlayerController(0.9f));

            }
            else if (tilemap.GetTile(cellPosition).name == tilePotato[tilePotato.Length - 1].name)
            {
                inventoryManager.AddItem(potatoItem);

                tilemap.SetTile(cellPosition, null);
            }
            else if (tilemap.GetTile(cellPosition).name == tileTomato[tileTomato.Length - 1].name)
            {
                inventoryManager.AddItem(tomatoItem);

                tilemap.SetTile(cellPosition, null);
            }
        }
    }

    void Watering(Item seedItem)
    {
        Vector3 mouseScreenPosition = Input.mousePosition;
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
        Vector3Int cellPosition = tilemap.WorldToCell(worldPosition);

        if (IsWithinTriggerArea(cellPosition) && IsWithinDistance(cellPosition) && CheckState(cellPosition))
        {
            if (tilemap == null || tilemap.GetTile(cellPosition) == null)
            {
                return;
            }

            if (tilemap.GetTile(cellPosition).name == tileFarmLand[0].name ||
            tilemap.GetTile(cellPosition).name == tilePotato[0].name ||
            tilemap.GetTile(cellPosition).name == tileTomato[0].name)
            {
                animator.SetTrigger("Watering");

                if (tilemap.GetTile(cellPosition).name == tileFarmLand[0].name)
                {
                    tilemap.SetTile(cellPosition, tileFarmLand[1]);
                }
                else if (tilemap.GetTile(cellPosition).name == tilePotato[0].name)
                {
                    tilemap.SetTile(cellPosition, tilePotato[1]);

                    // Bắt đầu phát triển cây
                    StartCoroutine(GrowPlant(cellPosition, tilePotato, tilePotato[1]));
                }
                else if (tilemap.GetTile(cellPosition).name == tileTomato[0].name)
                {
                    tilemap.SetTile(cellPosition, tileTomato[1]);

                    // Bắt đầu phát triển cây
                    StartCoroutine(GrowPlant(cellPosition, tileTomato, tileTomato[1]));
                }

                // Reduce durability Watering
                InventoryItem wateringTool = inventoryManager.GetSelectedInventoryItem();
                wateringTool.ReduceDurability();

                if (wateringTool.durability == 0)
                {
                    inventoryManager.RemoveItem(wateringTool);
                }

                StartCoroutine(DisablePlayerController(0.9f));

            }
            else if (tilemap.GetTile(cellPosition).name == tilePotato[tilePotato.Length - 1].name)
            {
                inventoryManager.AddItem(potatoItem);

                tilemap.SetTile(cellPosition, null);
            }
            else if (tilemap.GetTile(cellPosition).name == tileTomato[tileTomato.Length - 1].name)
            {
                inventoryManager.AddItem(tomatoItem);

                tilemap.SetTile(cellPosition, null);
            }
        }
    }

    void Collect()
    {
        Vector3 mouseScreenPosition = Input.mousePosition;
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
        Vector3Int cellPosition = tilemap.WorldToCell(worldPosition);

        if (IsWithinTriggerArea(cellPosition) && IsWithinDistance(cellPosition) && CheckState(cellPosition))
        {
            if (tilemap == null || tilemap.GetTile(cellPosition) == null)
            {
                return;
            }

            if (tilemap.GetTile(cellPosition).name == tilePotato[tilePotato.Length - 1].name)
            {
                if (tilemap == null || tilemap.GetTile(cellPosition) == null)
                {
                    return;
                }

                if (tilemap.GetTile(cellPosition).name == tilePotato[tilePotato.Length - 1].name)
                {
                    inventoryManager.AddItem(potatoItem);
                }

                tilemap.SetTile(cellPosition, null);

            }
            else if (tilemap.GetTile(cellPosition).name == tileTomato[tileTomato.Length - 1].name)
            {
                if (tilemap.GetTile(cellPosition).name == tileTomato[tileTomato.Length - 1].name)
                {
                    inventoryManager.AddItem(tomatoItem);
                }

                tilemap.SetTile(cellPosition, null);
            }
        }
    }

    IEnumerator DisablePlayerController(float delayTime)
    {
        playerController.enabled = false;


        yield return new WaitForSeconds(delayTime); // Chờ 1 giây

        playerController.enabled = true;
    }

    bool IsWithinTriggerArea(Vector3Int cellPosition)
    {
        // Chuyển đổi từ vị trí ô tile sang vị trí thế giới
        Vector3 cellWorldPosition = tilemap.GetCellCenterWorld(cellPosition);

        // Kiểm tra xem vị trí thế giới có nằm trong vùng trigger không
        return triggerArea.OverlapPoint(cellWorldPosition);
    }

    bool IsWithinDistance(Vector3Int cellPosition)
    {
        Vector3 cellWorldPosition = tilemap.GetCellCenterWorld(cellPosition);
        float distance = Vector3.Distance(player.position, cellWorldPosition);
        return distance <= maxDistance;
    }

    bool CheckState(Vector3Int cellPosition)
    {
        Vector3 cellWorldPosition = tilemap.GetCellCenterWorld(cellPosition);
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("player-idle-side") && !playerRenderer.flipX && cellWorldPosition.x >= player.position.x && cellWorldPosition.y <= player.position.y)
        {
            return true;
        }
        else if (stateInfo.IsName("player-idle-side") && playerRenderer.flipX && cellWorldPosition.x < player.position.x && cellWorldPosition.y <= player.position.y)
        {
            return true;
        }
        else if (stateInfo.IsName("player-idle-front") && cellWorldPosition.y <= player.position.y - 0.5f && cellWorldPosition.x >= player.position.x - 0.5f)
        {
            return true;
        }
        else if (stateInfo.IsName("player-idle-back") && cellWorldPosition.y > player.position.y)
        {
            return true;
        }
        return false;
    }

    IEnumerator GrowPlant(Vector3Int cellPosition, TileBase[] tileFarmLand, TileBase currentStage)
    {
        if (tileFarmLand == null)
        {
            yield break;
        }

        int k = 0;
        for (int i = 0; i < tileFarmLand.Length; i++)
        {
            if (tileFarmLand[i].name.Equals(currentStage.name))
            {
                k = i;
                break;
            }
        }

        for (int i = k + 1; i < tileFarmLand.Length; i++)
        {
            if (tileFarmLand == tilePotato)
                yield return new WaitForSeconds(timeToGrowOfPotato);
            else if (tileFarmLand == tileTomato)
                yield return new WaitForSeconds(timeToGrowOfTomato);

            tilemap.SetTile(cellPosition, tileFarmLand[i]);
        }
    }

    public void SaveGame()
    {
        GameData data = new GameData();
        foreach (var pos in tilemap.cellBounds.allPositionsWithin)
        {
            if (tilemap.HasTile(pos))
            {
                TileBase tile = tilemap.GetTile(pos);
                if (tile != null)
                {
                    // if (tileFarmLand.Contains(tile))
                    // {
                    data.farmlandTiles.Add(new TileData { position = pos, tileName = tile.name });
                    // }
                }
            }
        }

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(saveFilePath, json);
    }

    public void LoadGame()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            GameData data = JsonUtility.FromJson<GameData>(json);

            tilemap.ClearAllTiles();

            foreach (var tileData in data.farmlandTiles)
            {
                TileBase tile = tileFarmLand.FirstOrDefault(t => t.name == tileData.tileName);

                if (tile == null)
                    tile = tilePotato.FirstOrDefault(t => t.name == tileData.tileName);

                if (tile == null)
                    tile = tileTomato.FirstOrDefault(t => t.name == tileData.tileName);

                if (tile != null)
                {
                    tilemap.SetTile(tileData.position, tile);

                    TileBase[] tiles = null;
                    if (tilePotato.Contains(tile) && tile != tilePotato[0])
                    {
                        tiles = tilePotato;
                    }
                    else if (tileTomato.Contains(tile) && tile != tileTomato[0])
                    {
                        tiles = tileTomato;
                    }

                    if (tiles != null)
                        StartCoroutine(GrowPlant(tileData.position, tiles, tile));
                }
            }

        }
    }
}
