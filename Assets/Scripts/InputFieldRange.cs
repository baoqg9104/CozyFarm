using TMPro;
using UnityEngine;

public class InputFieldRange : MonoBehaviour
{
    public TMP_InputField inputField;
    public int minValue = 1;
    public int maxValue = 999;
    public BuyPage buyPage;
    public SellPage sellPage;

    void Start()
    {
        inputField.onEndEdit.AddListener(ValidateOnEnd);
    }

    public void OnValueChanged(string input)
    {
        // Chuyển input thành số nguyên và giới hạn giá trị
        if (int.TryParse(input, out int value))
        {
            value = Mathf.Clamp(value, minValue, maxValue);
            inputField.text = value.ToString();
        }
        buyPage.CalculatePrice();
        sellPage.CalculatePrice();
    }

    public void ValidateOnEnd(string input)
    {
        // Chuyển input thành số nguyên và giới hạn giá trị
        if (int.TryParse(input, out int value))
        {
            value = Mathf.Clamp(value, minValue, maxValue);
            inputField.text = value.ToString();
        }
        else
        {
            inputField.text = minValue.ToString(); // Reset về min nếu input trống hoặc không hợp lệ
        }
        buyPage.CalculatePrice();
        sellPage.CalculatePrice();
    }
}
