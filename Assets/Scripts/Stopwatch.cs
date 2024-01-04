using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Stopwatch : MonoBehaviour
{
    [SerializeField] private GameObject startScreen;

    [SerializeField] private GameObject mainScreen;
    [SerializeField] private Button stopButton;
    [SerializeField] private Button playButton;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button recordButton;

    [SerializeField] private Text timerLable;
    [SerializeField] private VerticalLayoutGroup verticalLayoutGroup;
    [SerializeField] private List<ResultRecord> records = new List<ResultRecord>();

    bool isPlaying = false;
    double timeInMilisecs;   
    // Start is called before the first frame update
    void Start()
    {
        stopButton.onClick.AddListener(Stop);
        playButton.onClick.AddListener(Play);
        recordButton.onClick.AddListener(Record);
        pauseButton.onClick.AddListener(Pause);
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlaying) timeInMilisecs += Time.deltaTime * 1000;

        timerLable.text = TimeSpan.FromMilliseconds(timeInMilisecs).ToString(@"mm\:ss\.ff");
    }

    public void OpenPrivacy()
    {

    }

    public void Next()
    {
        startScreen.SetActive(false);
        mainScreen.SetActive(true);
        
    }

    private void Stop()
    {
        timeInMilisecs = 0;
        foreach(var record in records) record.gameObject.SetActive(false);

        verticalLayoutGroup.childAlignment = TextAnchor.MiddleCenter;
        ChangeActiveButtons(false, true, false, false);
    }

    private void Play()
    {
        isPlaying = true;

        verticalLayoutGroup.childAlignment = TextAnchor.UpperCenter;
        ChangeActiveButtons(false, false, true, records.Count(rec => rec.gameObject.activeSelf) < 8);
    }

    private void Record()
    {
        for(var i = 0; i < 9; i++)
        {
            if(!records[i].gameObject.activeSelf)
            {
                records[i].gameObject.SetActive(true);
                records[i].UpdateView((i + 1).ToString(), TimeSpan.FromMilliseconds(timeInMilisecs).ToString(@"mm\:ss\.ff"));
                break;
            }
        }

        ChangeActiveButtons(false, false, true, records.Count(rec => rec.gameObject.activeSelf) < 8);
    }

    private void Pause()
    {
        isPlaying = false;
        ChangeActiveButtons(true, true, false, false);
    }

    private void ChangeActiveButtons(bool _stop, bool _play, bool _pause, bool _record)
    {
        stopButton.gameObject.SetActive(_stop);
        playButton.gameObject.SetActive(_play);
        pauseButton.gameObject.SetActive(_pause);
        recordButton.gameObject.SetActive(_record);
    }
}
