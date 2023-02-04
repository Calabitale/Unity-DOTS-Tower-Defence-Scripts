using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine.UI;
using TMPro;


[UpdateBefore(typeof(DestroyEnemySystem))]
public partial class LossTrackingSystem : SystemBase
{
    GameObject GameManagerStuff;
    GameObject LossDisplayGO;

    TextMeshProUGUI LossDisText;

    protected override void OnCreate()
    {
        GameManagerStuff = GameObject.Find("GameManagerStuff");

        var gameobjrefs = GameManagerStuff.GetComponent<GameObjectRefs>();
        LossDisplayGO = gameobjrefs.LossDisplay;

        RequireSingletonForUpdate<LossDat>();
        RequireSingletonForUpdate<PauseSystemDat>();
    }

    protected override void OnStartRunning()
    {
        LossDisText = LossDisplayGO.GetComponent<TextMeshProUGUI>();
    }

    protected override void OnUpdate()
    {
        var currLossDatVal = GetSingleton<LossDat>();

        LossDisText.text = currLossDatVal.intVal.ToString();//currLossDatVal.intVal.ToString();

        var LossDisEnt = GetSingletonEntity<LossDat>();

        LossDat meloss = new LossDat { intVal = currLossDatVal.intVal };

        Entities.
            //WithReadOnly(BestDirectionCells).
            //WithReadOnly(TranslutionEnts).
            //WithDisposeOnCompletion(BestDirectionCells).
            WithAll<EnemyDudeTag>().
            ForEach((Entity endDude, ref IsAlive isLive) =>
            {
                if (isLive.booVal == true)
                    return;

                meloss.intVal += 50;


                SetComponent<LossDat>(LossDisEnt, meloss);
               

            }).Schedule();
    }
}
