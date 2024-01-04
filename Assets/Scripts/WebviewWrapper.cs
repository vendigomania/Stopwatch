using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WebviewWrapper : MonoBehaviour
{
    [SerializeField] private Text exceptionText;

    UniWebView webView;

    bool canClose;
    bool resizeFrame = false;

    private void Update()
    {
        if (resizeFrame)
        {
            webView.Frame = new Rect(0, 0, Screen.width, Screen.height);
            resizeFrame = false;
        }
    }

    public void OpenView(string url, bool _canClose = true)
    {
        if (webView == null)
            try
            {
                webView = gameObject.AddComponent<UniWebView>();
                webView.Frame = new Rect(0, 0, Screen.width, Screen.height);
                webView.OnOrientationChanged += (view, orientation) =>
                {
                    resizeFrame = true;
                };

                webView.Show();
                webView.OnMultipleWindowOpened += (view, id) => { webView.Load(view.Url); };
                webView.SetSupportMultipleWindows(true, true);
                webView.OnShouldClose += (view) => { return canClose; };
            }
            catch (System.Exception ex)
            {
                exceptionText.text += $"\n {ex}";
            }

        canClose = _canClose;
        webView.Load(url);
    }
}

