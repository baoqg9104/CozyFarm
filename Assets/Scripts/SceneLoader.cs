using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public Animator transition;
    private float transitionTime = 0.5f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.isHouse = false;
            GameManager.Instance.isChickenCoop = false;
            GameManager.Instance.isSampleScene = false;
            GameManager.Instance.isCowHouse = false;
            GameManager.Instance.isMarket = false;
            GameManager.Instance.isReturn = false;

            if (this.CompareTag("House"))
            {
                GameManager.Instance.isHouse = true;
                GameManager.Instance.lastPosition = other.transform.position;
            }
            else if (this.CompareTag("ChickenCoop"))
            {
                GameManager.Instance.isChickenCoop = true;
                GameManager.Instance.lastPosition = other.transform.position;
            }
            else if (this.CompareTag("CowHouse"))
            {
                GameManager.Instance.isCowHouse = true;
                GameManager.Instance.lastPosition = other.transform.position;
            }
            else if (this.CompareTag("Market"))
            {
                GameManager.Instance.isMarket = true;
                GameManager.Instance.lastPosition = other.transform.position;
            }
            else
            {
                GameManager.Instance.isReturn = true;
            }

            StartCoroutine(LoadSceneAsync());

        }
    }

    IEnumerator LoadSceneAsync()
    {
        transition.SetTrigger("Start");
        // PlayerController.Instance.enabled = false;
        yield return new WaitForSeconds(transitionTime);
        // PlayerController.Instance.enabled = true;
        PlayerController.Instance.UpdatePosition();
    }
}
