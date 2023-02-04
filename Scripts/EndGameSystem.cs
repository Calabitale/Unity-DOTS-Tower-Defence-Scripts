using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using TMPro;

[UpdateAfter(typeof(LossTrackingSystem))]
//[UpdateAfter(typeof(ProfitTrackingSystem))]
public partial class EndGameSystem : SystemBase
{
    GameObject GameManagerStuff;
    GameObject EndScreenUI;

    TextMeshProUGUI ProfitDisTex;
    TextMeshProUGUI LossDisTex;
    TextMeshProUGUI ProfLossDisTex;

    protected override void OnCreate()
    {
        GameManagerStuff = GameObject.Find("GameManagerStuff");

        var gameobjrefs = GameManagerStuff.GetComponent<GameObjectRefs>();
        EndScreenUI = gameobjrefs.EndScreenUI;

        ProfitDisTex = gameobjrefs.EndProfitDisGO.GetComponent<TextMeshProUGUI>();
        LossDisTex = gameobjrefs.EndLossDisGO.GetComponent<TextMeshProUGUI>();
        ProfLossDisTex = gameobjrefs.TotProfLossDis.GetComponent<TextMeshProUGUI>();

        RequireSingletonForUpdate<TimerCompleteEvent>();

    }

    protected override void OnUpdate()
    {
        EndScreenUI.SetActive(true);

        EntityManager.DestroyEntity(GetSingletonEntity<TimerCompleteEvent>());

        var tempLoss = GetSingleton<LossDat>();
        var tempProf = GetSingleton<ProfitsDat>();

        LossDisTex.text = tempLoss.intVal.ToString();
        ProfitDisTex.text = tempProf.intVal.ToString();

        var tempresult = (tempProf.intVal - tempLoss.intVal);
        ProfLossDisTex.text = tempresult.ToString();

        
    }

}
