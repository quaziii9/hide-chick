using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EventLibrary;
using EnumTypes;

public class AtkTime : MonoBehaviour
{
    public GameObject hideObj;
    public Image hideImage;
    private bool isHide;
    private float atkTime = 2;
    private float getatkTime = 0;

    void Start()
    {
        hideObj.SetActive(false);
    }

    private void OnEnable()
    {
        EventManager<UIEvents>.StartListening(UIEvents.atkTime,HideSkillSetting);
    }



    // Update is called once per frame
    void Update()
    {
        StartCoroutine(atkTimeChk());
    }


    public void HideSkillSetting()
    {
        hideObj.SetActive(true);
        getatkTime = atkTime;
        isHide = true;
    }

    IEnumerator atkTimeChk()
    {
        yield return null;

        if(getatkTime >0)
        {
            getatkTime -= Time.deltaTime;
            EventManager<PlayerEvents>.TriggerEvent(PlayerEvents.isAtkFalse);

            if (getatkTime <0)
            {
                EventManager<PlayerEvents>.TriggerEvent(PlayerEvents.isAtkTrue);

                isHide = false;
                hideObj.SetActive(false);
            }

            float time = getatkTime / atkTime;
            hideImage.fillAmount = time;
        }

    }
}
