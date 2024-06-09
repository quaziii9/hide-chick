using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventLibrary;
using EnumTypes;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject BackgroundUI;
    [SerializeField] GameObject WinnderUI;

    public void OnEnable()
    {
        EventManager<UIEvents>.StartListening(UIEvents.BackGroundUION, BackGroundUION);
        EventManager<UIEvents>.StartListening(UIEvents.WinnerUION, WinnerUION);
    }

    public void BackGroundUION()
    {
        BackgroundUI.SetActive(true);
    }

    public void WinnerUION()
    {
        WinnderUI.SetActive(true);
    }
}
