using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultRecord : MonoBehaviour
{
    [SerializeField] private Text placeLable;
    [SerializeField] private Text timeLable;

    public void UpdateView(string _place, string _result)
    {
        placeLable.text = _place;
        timeLable.text = _result;
    }
}
