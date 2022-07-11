using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
// using Firesplash.UnityAssets.SocketIO;
using SimpleJSON;
using System.Runtime.InteropServices;
using UnitySocketIO;
using UnitySocketIO.Events;

public class GameManager : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void GameController(string msg);
    public SocketIOController sioCom;
    public GameObject showMessage;
    public Text message;
    public Text Time;
    public TextMeshProUGUI[] history = new TextMeshProUGUI[10];
    public static APIForm _userInfo = new APIForm();
    public static Betted_User_Info _user;
    public Text totalBalance;
    public InputField betBalance;
    public Button betButton;
    public Text betButton_State;
    public GameObject Content;
    public Transform prefab;
    public InputField CashAmount;
    public GameObject prefab_tooltip;
    public GameObject tooltip_parent;
    public Sprite error;
    public Sprite success;
    double startTime = 0;
    double currentTime = 0;
    double cashOutTime = 0;
    float betTime;
    float restTime;
    float endPoint = 0;
    string gameStatus = "bet";
    bool betStatus = false;
    bool gameControl = false;
    bool networkState = false;
    bool betted = false;
    bool finishedGame = true;
    bool cashoutState = false;
    bool Checkbtn_status = false;
    private Transform show_Users;
    // Start is called before the first frame update
    void Start()
    {
        sioCom.Connect();
#if UNITY_WEBGL == true && UNITY_EDITOR == false
            GameController("Ready");
#endif
        sioCom.On("round end", (SocketIOEvent data) =>
        {
            JSONNode endGame = JSON.Parse(data.data);
            startTime = currentTime = (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;
            finishedGame = true;
            endPoint = endGame["end"];
            SpaceCruizerController.Instance.Crash();
            SpaceCruizerController.Instance.SetStarted(false);
            foreach (Transform child in Content.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
            gameStatus = "ready";
        });
        sioCom.On("broadcast", (SocketIOEvent data) =>
        {
            getHistory(data.data);
        });
        sioCom.On("betted_users", (SocketIOEvent data) =>
        {
            foreach (Transform child in Content.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
            JSONNode users = JSON.Parse(data.data);
            for (int i = 0; i < users["users"].Count; i++)
            {
                show_Users = Instantiate(prefab, Vector2.zero, Quaternion.identity);
                show_Users.SetParent(Content.transform);
                show_Users.name = "user";
                show_Users.GetComponent<RectTransform>().anchorMax = new Vector2(1, 0.99f - 0.1f * i);
                show_Users.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0.91f - 0.1f * i);
                show_Users.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);
                show_Users.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
                show_Users.GetChild(0).GetComponent<Text>().text = users["users"][i]["name"] + " : ";
                show_Users.GetChild(1).GetComponent<Text>().text = users["users"][i]["amount"];
                show_Users.localScale = new Vector2(1f, 1f);
            }
        });
        sioCom.On("status", (SocketIOEvent data) =>
        {
            JSONNode GameStatus = JSON.Parse(data.data);
            startTime = (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds - GameStatus["nowTime"];
            gameStatus = GameStatus["state"];
            if (gameStatus == "gaming")
            {
                SpaceCruizerController.Instance.SetStarted(true);
            }
        });
        sioCom.On("cashout success", (SocketIOEvent data) =>
        {
            JSONNode cashedAmount = JSON.Parse(data.data);
            string str = "You successful cash out " + float.Parse(cashedAmount["amount"]).ToString("f2") + "!";
            totalBalance.text = (float.Parse(totalBalance.text) + cashedAmount["amount"]).ToString("f2");
            StartCoroutine(Alert(str, 1));
            GameObject tooltip = Instantiate(prefab_tooltip, tooltip_parent.transform);
            tooltip.GetComponent<TooltipHandler>().SetVal(cashedAmount["name"] + " : " + float.Parse(cashedAmount["amount"]).ToString("0.00"));
        });
        sioCom.On("bet success", (SocketIOEvent data) =>
        {
            JSONNode success = JSON.Parse(data.data);
            string str = "You successful placed a bet of " + success["amount"];
            StartCoroutine(Alert(str, 1));
        });
        // sioCom.On("cashout", (string data) =>
        // {

        // });
        StartCoroutine(iStart());
    }
    void Update()
    {
        if (networkState)
        {
            currentTime = (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;
            if (gameStatus == "bet")
            {
                betButton_State.text = "BET";
                Time.text = "Start In " + ((startTime + 7000d - currentTime) / 1000).ToString("f2") + "s";
                if (currentTime > startTime + 7000d)
                {
                    cashOutTime = 0;
                    SpaceCruizerController.Instance.SetStarted(true);
                    gameStatus = "gaming";
                    finishedGame = false;
                }
            }
            else if (gameStatus == "gaming")
            {
                cashOutTime = 30000 / (30000 - ((DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds - startTime - 7000));
                Time.text = cashOutTime.ToString("f2") + "x";
                betButton_State.text = "CASHOUT";
            }
            else if (gameStatus == "ready")
            {
                betButton_State.text = "READY";
                Time.text = endPoint.ToString("f2") + "x";
                Time.color = new Color(255, 0, 0);
                if (currentTime > startTime + 2000d)
                {
                    SpaceCruizerController.Instance.SetStarted(false);
                    Time.color = new Color(255, 255, 255);
                    betted = false;
                    gameStatus = "bet";
                    cashoutState = false;
                }
            }
        }
        if (Checkbtn_status)
        {
            if (gameStatus == "gaming")
            {
                betButton.interactable = false;
                if (betted)
                {
                    if (!cashoutState)
                    {
                        if (cashOutTime > float.Parse(CashAmount.text))
                        {
                            Debug.Log("Success");
                            cashoutState = true;
                            _userInfo.cashoutAt = float.Parse(CashAmount.text);
                            sioCom.Emit("cashout", JsonUtility.ToJson(_userInfo));
                        }
                    }
                }
            }
            else
            {
                betButton.interactable = true;
            }
        }
        else
        {
            _userInfo.cashoutAt = 0;
            betButton.interactable = true;
        }
    }
    public void BetBtn_Click()
    {
        if (gameStatus == "bet")
        {
            if (betted)
            {
                StartCoroutine(Alert("You've already betted!", 0));
            }
            else
            {
                if (float.Parse(betBalance.text) > float.Parse(totalBalance.text))
                {
                    StartCoroutine(Alert("Your Balance is not enough", 0));
                }
                else
                {
                    totalBalance.text = (float.Parse(totalBalance.text) - float.Parse(betBalance.text)).ToString();
                    betted = true;
                    _userInfo.betAmount = float.Parse(betBalance.text);
                    sioCom.Emit("bet", JsonUtility.ToJson(_userInfo));
                }
            }
        }
        else if (gameStatus == "gaming")
        {
            if (cashoutState)
            {
                StartCoroutine(Alert("You already cashouted!", 0));
            }
            else
            {
                if (betted)
                {
                    if (!cashoutState)
                    {
                        cashoutState = true;
                        sioCom.Emit("cashout", JsonUtility.ToJson(_userInfo));
                    }
                }
                else
                {
                    StartCoroutine(Alert("You did not betted yet!", 0));
                }
            }
        }
    }
    IEnumerator iStart()
    {
        yield return new WaitForSeconds(0.5f);
        sioCom.Emit("getStatus", JsonUtility.ToJson(_userInfo));
        networkState = true;
    }
    public void RequestToken(string data)
    {
        JSONNode usersInfo = JSON.Parse(data);
        _userInfo.token = usersInfo["token"];
        _userInfo.name = usersInfo["userName"];
        totalBalance.text = usersInfo["amount"];
    }
    void getHistory(string str)
    {
        JSONNode History = JSON.Parse(str);
        for (int i = 0; i < 10; i++)
        {
            if (History["history"][i] > 0)
            {
                if (History["history"][i] < 2)
                    history[i].color = new Color(255, 0, 0);
                else if (History["history"][i] < 5)
                    history[i].color = new Color(0, 238, 255);
                else
                    history[i].color = new Color(255, 255, 0);
                history[i].text = (float.Parse(History["history"][i])).ToString("f2");
            }
            else
            {
                history[i].text = "";
            }
        }
    }
    IEnumerator Alert(string data, int a)
    {
        if (a == 0)
        {
            showMessage.GetComponent<Image>().sprite = error;
            message.color = new Color(255, 255, 255);
        }
        else
        {
            showMessage.GetComponent<Image>().sprite = success;
            message.color = new Color(0, 0, 255);
        }
        showMessage.SetActive(true);
        message.text = data;
        yield return new WaitForSeconds(2f);
        showMessage.SetActive(false);
    }
    public void CheckButton_onClick()
    {
        Checkbtn_status = !Checkbtn_status;
    }
}