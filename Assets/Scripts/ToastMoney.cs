using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ToastMoney : MonoBehaviour
{
    public static ToastMoney Instance { get; private set; }
    public TextMeshProUGUI messageText;
    private CanvasGroup canvasGroup;

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
        gameObject.SetActive(false);
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Show(string moneyText)
    {
        Instance.gameObject.SetActive(true);
        messageText.text = moneyText;
        StartCoroutine(FadeIn());
        StartCoroutine(Hide());
    }

    private IEnumerator FadeIn()
    {
        float fadeDuration = 0.25f;
        float time = 0;

        while (time < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(0, 1, time / fadeDuration);
            time += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = 1;
    }

    private IEnumerator Hide()
    {
        yield return new WaitForSeconds(1);
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        float fadeDuration = 0.25f;
        float time = 0;

        while (time < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(1, 0, time / fadeDuration);
            time += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = 0;
        gameObject.SetActive(false);
    }
}