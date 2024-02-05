using Facebook.Unity;
using System;
using UnityEngine;

public class FacebookManager : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (!FB.IsInitialized)
        {
            FB.Init(InitCallback, OnHideUnity);
        }
    }

    private void InitCallback()
    {
        if (FB.IsInitialized)
        {
            FB.ActivateApp();
            if(AppLoadManager.FirstLaunch) FB.LogPurchase((decimal)1, currency: "USD");
        }
        else
        {
            Debug.Log("failed to init Facebook");
        }
    }

    private void OnHideUnity(bool isGameShown)
    {
        if (!isGameShown)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }
}
