using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Dialog : PanelBase
{
    [SerializeField]
    private Text titleText;

    [SerializeField]
    private Text bodyText;

    [SerializeField]
    private Button leftButton;

    [SerializeField]
    private Button rightButton;

    [SerializeField]
    private Sprite[] buttonSprites;

    public Button LeftButton { get => leftButton; set => leftButton = value; }
    public Button RightButton { get => rightButton; set => rightButton = value; }

    //public event EventHandler LeftClick;
    //public event EventHandler RightClick;

    public void Show(string title, string body, string leftStr, string rightStr, 
                     int leftTheme, int rightTheme, UnityAction leftCall, UnityAction rightCall)
    {
        titleText.text = title;
        bodyText.text = body;
        LeftButton.GetComponentInChildren<Text>().text = leftStr;
        if(leftTheme > 0)
            LeftButton.GetComponent<Image>().sprite = buttonSprites[leftTheme];
        RightButton.GetComponentInChildren<Text>().text = rightStr;
        if(rightTheme > 0)
            RightButton.GetComponent<Image>().sprite = buttonSprites[rightTheme];
        leftButton.onClick.RemoveAllListeners();
        rightButton.onClick.RemoveAllListeners();
        if(leftCall!=null)
            leftButton.onClick.AddListener(leftCall);
        leftButton.onClick.AddListener(new UnityAction(CloseDialog));
        if(rightCall!=null)
            rightButton.onClick.AddListener(rightCall);
        rightButton.onClick.AddListener(new UnityAction(CloseDialog));
        this.Open();
    }

    private void CloseDialog()
    {
        this.Close();
    }

    /*
    private void OnLeftClick()
    {
        if(LeftClick!= null)
            LeftClick(this, EventArgs.Empty);
        else
        {
            this.Close();
        }
    }

    private void OnRightClick()
    {
        if(RightClick!= null)
            RightClick(this, EventArgs.Empty);
        else
            this.Close();
    }
    */

}
