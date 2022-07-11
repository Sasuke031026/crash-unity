using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class betAmount : MonoBehaviour
{
    public InputField myBet;
    public Text myBalance;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void Max()
    {
        if (float.Parse(myBalance.text) > 10000)
        {
            myBet.GetComponent<InputField>().text = "10000";
        }
        else if (float.Parse(myBalance.text) < 10)
        {
            myBet.GetComponent<InputField>().text = "10";
        }
        else
        {
            myBet.GetComponent<InputField>().text = myBalance.text;
        }
    }
    public void Min()
    {
        myBet.GetComponent<InputField>().text = "10";
    }
    public void Half()
    {
        if (float.Parse(myBet.GetComponent<InputField>().text) / 2 < 10)
        {
            myBet.GetComponent<InputField>().text = "10";
        }
        else
        {
            myBet.GetComponent<InputField>().text = (float.Parse(myBet.GetComponent<InputField>().text) / 2).ToString();
        }
    }
    public void Times()
    {
        if (float.Parse(myBet.GetComponent<InputField>().text) * 2 > float.Parse(myBalance.text))
        {
            myBet.GetComponent<InputField>().text = myBalance.text;
        }
        else if (float.Parse(myBet.GetComponent<InputField>().text) * 2 > 10000)
        {
            myBet.GetComponent<InputField>().text = "10000";
        }
        else
        {
            myBet.GetComponent<InputField>().text = (float.Parse(myBet.GetComponent<InputField>().text) * 2).ToString();
        }
    }
    public void betAmount_change()
    {
        if (myBet.GetComponent<InputField>().text == "")
        {
            myBet.GetComponent<InputField>().text = "10";
        }
        else
        {
            float num = float.Parse(myBet.GetComponent<InputField>().text);
            if (num > 10000)
            {
                myBet.GetComponent<InputField>().text = "10000";
            }
            else if (num < 10)
            {
                myBet.GetComponent<InputField>().text = "10";
            }
        }
    }
}
