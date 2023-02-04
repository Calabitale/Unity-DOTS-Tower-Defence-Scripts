using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using UnityEngine.UI;

[DisableAutoCreation]
public partial class StartGameSystem : SystemBase
{
    GameObject GameObjectManStuff;
    GameObjectRefs GameRefs;

    GameObject StartButton;
    GameObject StartMenu;

    EntityQuery PlayerCompQuery;

    protected override void OnCreate()
    {
        GameObjectManStuff = GameObject.Find("GameManagerStuff");//TODO I know this is bad but its a prototype init
        GameRefs = GameObjectManStuff.GetComponent<GameObjectRefs>();

        StartButton = GameRefs.StartButton;
        StartMenu = GameRefs.StartMenu;

        PlayerCompQuery = GetEntityQuery(typeof(ControlModesDat));    
       
        //var currbutton = StartButton.GetComponentInChildren<Button>();
        
    }

    protected override void OnStartRunning()
    {
        //base.OnStartRunning();

        var currbotton = StartButton.GetComponentInChildren<Button>();
        currbotton.onClick.AddListener(delegate { BeginGame(); });

    }



    protected override void OnUpdate()
    {
       

    }

    void BeginGame()
    {

        EntityManager.CreateEntity(typeof(PauseSystemDat));
        //StartButton.active = false;
        StartMenu.SetActive(false);// = false;
        var contromde = PlayerCompQuery.GetSingleton<ControlModesDat>();
        contromde.currMode = ControlModes.EnvCursorMode;
        PlayerCompQuery.SetSingleton<ControlModesDat>(contromde);

        ProfitsDat startprofits = new ProfitsDat();
        startprofits.intVal = 500;
        SetSingleton<ProfitsDat>(startprofits);

    }

}
