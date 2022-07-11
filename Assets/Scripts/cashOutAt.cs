using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class cashOutAt : MonoBehaviour
{
    public Button Minus;
    public Button Plus;
    public Button Check;
    public Image check_Img;
    public InputField cashOutAt_Amount;

    public bool checkState = false;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void minusBtn_Click()
    {
        float cashAmount = float.Parse(cashOutAt_Amount.GetComponent<InputField>().text);
        if (cashAmount - 1 >= 2)
        {
            cashOutAt_Amount.GetComponent<InputField>().text = (cashAmount - 1).ToString();
        }
        else
        {
            cashOutAt_Amount.GetComponent<InputField>().text = "2";
        }
    }
    public void plusBtn_Click()
    {
        cashOutAt_Amount.GetComponent<InputField>().text = (float.Parse(cashOutAt_Amount.GetComponent<InputField>().text) + 1).ToString();
    }
    public void CheckBtn_Click()
    {
        checkState = !checkState;
        check_Img.enabled = checkState;
        Minus.GetComponent<Button>().interactable = checkState;
        Plus.GetComponent<Button>().interactable = checkState;
        cashOutAt_Amount.GetComponent<InputField>().interactable = checkState;
    }
    public void inputChange()
    {
        if (cashOutAt_Amount.GetComponent<InputField>().text == "")
        {
            cashOutAt_Amount.GetComponent<InputField>().text = "2";
        }
        else
        {
            float num = float.Parse(cashOutAt_Amount.GetComponent<InputField>().text);
            if (num < 2)
            {
                cashOutAt_Amount.GetComponent<InputField>().text = "2";
            }
        }
    }
}
