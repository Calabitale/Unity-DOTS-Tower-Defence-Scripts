using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;
//using Unity.Physics;
using System;
using Unity.Collections;
using DotsFlowField;
using Unity.Jobs.LowLevel.Unsafe;


public partial class MyMovementSystem : SystemBase
{

    public float3 Target;
    public float Speed;
    public EntityQuery CelldataQuery;
    public EntityQuery flowfielddata;
    public Unity.Mathematics.Random Rundomnumcreator;
    protected override void OnCreate()
    {
        base.OnCreate();
        Target = new float3(-5, 0, 5);
        Speed = 15f;
        
        CelldataQuery = GetEntityQuery(ComponentType.ReadOnly<CellsBestDirection>());        
        RequireSingletonForUpdate<FlowFieldData>();
        Rundomnumcreator = new Unity.Mathematics.Random();

        Enabled = false;
    }

    protected override void OnStartRunning()
    {

    }

    protected override void OnDestroy()
    {
        
    }

    protected override void OnUpdate()
    {
        var Rundomnumcreator = new NativeArray<Unity.Mathematics.Random>(JobsUtility.MaxJobThreadCount, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
        var r = (uint)UnityEngine.Random.Range(int.MinValue, int.MaxValue);
        for (int i = 0; i < JobsUtility.MaxJobThreadCount; i++)
        {
            Rundomnumcreator[i] = new Unity.Mathematics.Random(r == 0 ? r + 1 : r);
        }
        
        //Rundomnumcreator.Length;        
        var tempcelldata = CelldataQuery.ToComponentDataArray<CellsBestDirection>(Allocator.TempJob);
        //var cellentitys = CelldataQuery.ToEntityArray(Allocator.TempJob);
        
        var tempflowfield = GetSingleton<FlowFieldData>();
        var spood = Speed;
        var temptarget = Target;

        var Timmydelta = Time.DeltaTime;       

        //Entity tempvent = GetSingletonEntity<TargetAuthoring>();

        //var temptargpos = GetComponent<Translation>(tempvent);
        //float3 temptargfloat = temptargpos.Value;
        //temptargfloat.y = 0;

        //var currfielddata = GetSingleton<FlowFieldData>();
        var cellindexworldpos = new GetCellIndexFromWorldPos();

        var tempflatus = new ToFlatIndex();
      
        //TODO It might be best to create all these layers as soon as they are required
        //Entities.WithName("CollectSpecificLayer").ForEach((DynamicBuffer<CellBestDirectionBuff> dudedirection) =>
        //{


        //}).ScheduleParallel();        

        //var rndum = Rundomnumcreator[0];
        //var ultimaterand = rndum.NextInt(0, 2);
        //var TempBufferDirections = GetBufferFromEntity<CellBestDirectionBuff>(true);
        Entities//.WithReadOnly(TempBufferDirections)
            //.WithReadOnly(cellentitys)
            .WithNativeDisableParallelForRestriction(Rundomnumcreator)
            .WithAll<EnemyDudeTag>()
            .WithDisposeOnCompletion(Rundomnumcreator)
            .WithDisposeOnCompletion(tempcelldata)
            .ForEach((int nativeThreadIndex, ref Translation trunslation, ref Rotation rutate, in CellBDLayer dooby) =>        
            {

            
            var rndum = Rundomnumcreator[nativeThreadIndex];
            var ultimaterand = rndum.NextInt(0, 2);
            //float3 movedir = math.normalize(temptargfloat - trunslation.Value);
            var currcellpos = cellindexworldpos.Execute(trunslation.Value, tempflowfield.gridSize, tempflowfield.cellRadius * 2);

            var tempgridpos = tempflatus.Execute(currcellpos, tempflowfield.gridSize.y);
            float2 moveDirection = tempcelldata[tempgridpos].bestDirection;
            //var currententity = cellentitys[tempgridpos];
            //var idonevenknow = TempBufferDirections[currententity];
            //float2 moveDirection = idonevenknow[dooby.intVal].bestDirection;
            float3 movedir = new float3(moveDirection.x, 0, moveDirection.y);


            //float3 temptargetheight = temptargfloat;
            //temptargetheight.y = 0;

            float3 tempentheight = trunslation.Value;
            tempentheight.y = 0;

            

            trunslation.Value += movedir * spood * Timmydelta;// * math.forward(rutate.Value);
            Rundomnumcreator[0] = rndum;

        }).Run();//ScheduleParallel();


    }
}

public static class BoidMovementClass
{
    public static float3 CalculateBoidPosition(int2 gridpos, float3 firstgridpos, float gridcellsize)
    {
        return new float3(gridpos.x, 0, gridpos.y) * gridcellsize + firstgridpos;
    }

}


    
public partial class DestroyEntitysatDest : SystemBase 
{
    //Todo There's one problem with this maybe more, it has to check every loop whether the entitys are in the destination square the more entity's there are the more cycles this uses and its just a waste it would be best to tag entity's
    public EndSimulationEntityCommandBufferSystem endcommbuff;
    EntityQuery ExitTargetQuery;

    protected override void OnCreate()
    {
        //base.OnCreate();
        endcommbuff = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        //Enabled = false;

        ExitTargetQuery = GetEntityQuery(ComponentType.ReadOnly<EnemyTargetTag>(), ComponentType.ReadWrite<Translation>());
        //celldestbuffQuery = GetEntityQuery(ComponentType.ReadOnly<CellDestinationsBuffer>());
    }

    protected override void OnUpdate()
    {
        var GetCellindpos = new GetCellIndexFromWorldPos();

        var ecb = endcommbuff.CreateCommandBuffer().AsParallelWriter();

        var PaidExitTarget = ExitTargetQuery.GetSingleton<Translation>();

        Entities.
            WithAll<EnemyDudeTag>().
            ForEach((Entity entity, int entityInQueryIndex, in Translation currentpos) =>        
            {
           
            //var curcelpos = GetCellindpos.Execute(currentpos.Value, 100, 1);

            if (math.distance(currentpos.Value, PaidExitTarget.Value) < 2)
            {
                //Debug.Log($"Its not woring correctly {curcelpos }");
                ecb.DestroyEntity(entityInQueryIndex, entity);
            }
          

        }).Run();

        

        endcommbuff.AddJobHandleForProducer(this.Dependency);
    }
}

public struct GetCellIndexFromWorldPos
{

    public int2 Execute(float3 worldPos, int2 gridSize, float cellDiameter)
    {
        float percentX = worldPos.x / (gridSize.x * cellDiameter);
        float percentY = worldPos.z / (gridSize.y * cellDiameter);

        percentX = math.clamp(percentX, 0f, 1f);
        percentY = math.clamp(percentY, 0f, 1f);

        int2 cellIndex = new int2
        {
            x = math.clamp((int)math.floor((gridSize.x) * percentX), 0, gridSize.x - 1),
            y = math.clamp((int)math.floor((gridSize.y) * percentY), 0, gridSize.y - 1)
        };

        return cellIndex;
    }
}

public struct ToFlatIndex
{
    public int Execute(int2 index2D, int height)
    {
        return height * index2D.x + index2D.y;
    }
}
