using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using DotsFlowField;
using Unity.Transforms;

[DisableAutoCreation]
public partial class InitialiseFieldBoidSystem : SystemBase
{
    public EntityQuery EnemyMovTargQuery;

    protected override void OnCreate()
    {
        EntityManager.CreateEntity(typeof(NewFlowFieldEvent));

        EnemyMovTargQuery = GetEntityQuery(ComponentType.ReadOnly<EnemyTargetTag>(), ComponentType.ReadOnly<Translation>());


    }

    protected override void OnStartRunning()
    {

       

    }

    // Start is called before the first frame update
    protected override void OnUpdate()
    {
        if (!HasSingleton<FlowFieldData>())
            return;

        var enemymovetarg = EnemyMovTargQuery.GetSingleton<Translation>();
        var Flowfieddat = GetSingleton<FlowFieldData>();

        var Gridtarget = GridCellHelperStuff.GetGridXZ(enemymovetarg.Value, Flowfieddat.GridOrigin, Flowfieddat.cellRadius * 2);
        base.OnStartRunning();
        //EntityManager.CreateEntity(typeof(NewFlowFieldEvent));
        if (!HasSingleton<CalcintegrationFieldEvent>())
        {
            EntityManager.CreateEntity(typeof(CalcintegrationFieldEvent));
        }

        var DestinEnt = GetSingletonEntity<CellDestinationsBuffer>();

        var DestinBuffer = EntityManager.GetBuffer<CellDestinationsBuffer>(DestinEnt);//TODO I should probably make some safety checks for these just in case 

        var soemval = DestinBuffer[0];

        soemval.Destination = Gridtarget;

        DestinBuffer[0] = soemval;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        
        EntityManager.CreateArchetype(typeof(InitialiseEnemysTag));

        //Debug.Log("How many times did this run ");
        Enabled = false;

    }
}
