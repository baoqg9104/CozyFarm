using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Scriptable object/Item")]
public class Item : ScriptableObject
{

    [Header("Only gameplay")]
    public TileBase tile;
    public ItemType type;
    public ActionType actionType;
    public Vector2Int range = new Vector2Int(5, 4);


    [Header("Only UI")]
    public bool stackable = true;

    [Header("Both")]
    public Sprite image;


    public bool hasDurability;
    public int price;

}

public enum ItemType {
    Fish,
    Seed,
    Vegetable,
    Tool
}

public enum ActionType {
    Fishing,
    Farm
}


