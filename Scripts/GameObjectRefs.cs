using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GameObjectRefs : MonoBehaviour//TODO I need to fix this SO I don't have a massive list of elements like this I should use an array or maybe
{//I should separate the UI stuff from teh other stuff
    public GameObject StartButton;
    public GameObject DefaultExplosion;
    public GameObject LightningBolt;
    public GameObject StartMenu;
    public GameObject PlaceTurret;
    public GameObject DestroyTurret;
    public GameObject PlaceSlipHazard;
    public GameObject DestroyStuffGO;
    public GameObject InGameUIbuttons;
    public GameObject ProfitsDisplay;
    public GameObject LossDisplay;
    public GameObject TimerDisplayGO;
    public GameObject MoveCursorGO;
    public GameObject SparksParticGO;
    public GameObject EndScreenUI;
    public GameObject EndProfitDisGO;
    public GameObject EndLossDisGO;
    public GameObject TotProfLossDis;
    public GameObject ChaosButtonGO;
    public GameObject YesExitButtGO;
    public GameObject NoExitButtGO;
    public GameObject ExitScreenGO;
    //public GameObject BloodHitEffect;
    public static GameObject UIGameObjRefs;

    private void Awake()
    {
        UIGameObjRefs = this.gameObject;
    }
    // Start is called before the first frame update
    void Start()
    {
       
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
