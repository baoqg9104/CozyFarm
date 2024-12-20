using UnityEngine;

public class GameManager : MonoBehaviour
{
  public static GameManager Instance;
  public bool isSampleScene = true; 
  public bool isHouse = false;
  public bool isChickenCoop = false;
  public bool isCowHouse = false;
  public bool isMarket = false;
  public bool isReturn = false; 
  public Vector3 lastPosition;

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
}
