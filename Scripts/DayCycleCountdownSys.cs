using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using UnityEngine.UI;
using TMPro;

[UpdateAfter(typeof(LossTrackingSystem))]//I need to just create this after all the other systems but will just do this way for now
public partial class DayCycleCountdownSys : SystemBase
{
    public float timeRemaining = 300;
    public bool timerIsRunning = true;
    public GameObject TimerDisplayGO;

    public TextMeshProUGUI TimeDisplayText;

    float minutes;
    float seconds;


    protected override void OnCreate()
    {
        var GameManagerStuff = GameObject.Find("GameManagerStuff");

        var gameobjrefs = GameManagerStuff.GetComponent<GameObjectRefs>();
        TimerDisplayGO = gameobjrefs.TimerDisplayGO;

        TimeDisplayText = TimerDisplayGO.GetComponent<TextMeshProUGUI>();

        RequireSingletonForUpdate<PauseSystemDat>();
        //TODO I need to make this run when the Start button has been pressed

    }

    protected override void OnUpdate()
    {
        //TimeDisplayText.text = minutes.ToString() + seconds.ToString();
        //DisplayTime(timeRemaining);

        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.DeltaTime;
                DisplayTime(timeRemaining);
            }
            else
            {
                //Debug.Log("Time has run out!");
                timeRemaining = 0;
                timerIsRunning = false;
                EntityManager.CreateEntity(typeof(TimerCompleteEvent));
                EntityManager.DestroyEntity(GetSingletonEntity<PauseSystemDat>());//TODO Should I destroy the PauseSystemDat here or perhaps somewhere else?
            }
        }



    }

    void DisplayTime(float timeToDisplay)
    {
        timeToDisplay += 1;
        minutes = Mathf.FloorToInt(timeToDisplay / 60);
        seconds = Mathf.FloorToInt(timeToDisplay % 60);
        TimeDisplayText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}

public struct StartTimerEvent : IComponentData { }
public struct TimerCompleteEvent : IComponentData { }