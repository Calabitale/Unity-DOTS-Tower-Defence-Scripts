using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using Unity.Transforms;
using Unity.Mathematics;

[UpdateAfter(typeof(PlayerDamageSystem))]
[UpdateAfter(typeof(SharkBehaveSystem))]
public partial class DestroyEnemySystem : SystemBase//TODO I need to be aware of errors coming from this system I need to ensure it runs at the end of all other systems
{

    //EndSimulationEntityCommandBufferSystem endsimBuffer;
    protected override void OnCreate()
    {
        base.OnCreate();
        //Enabled = false;
        //ecb = new EntityCommandBuffer(Allocator.TempJob);
    }
    protected override void OnUpdate()
    {
        //endsimBuffer = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>(); //EndSimulationEntityCommandBufferSystem(Allocator.TempJob)
        //var ecb = endsimBuffer.CreateCommandBuffer();
        Entities.
            WithStructuralChanges().
            //WithReadOnly(BestDirectionCells).
            //WithReadOnly(TranslutionEnts). 
            //WithDisposeOnCompletion(TranslutionEnts).
            WithAll<EnemyDudeTag>().
            ForEach((Entity medude, ref DamageTaken enemdamage, ref CurrHealth curryheath, ref IsAlive alive) =>
            {
                if (alive.booVal == true)
                    return;

                //ecb.DestroyEntity(medude);
                EntityManager.DestroyEntity(medude);

            }).Run();

        //endsimBuffer.AddJobHandleForProducer(this.Dependency);
    }

}

[UpdateAfter(typeof(PlayerDamageSystem))]
public partial class MoveTowardsEntranceDestroy : SystemBase
{
    EntityQuery CelldataQuery;
    EndSimulationEntityCommandBufferSystem endsimBuffer;

    float walkTimer;
    float deathTimerMax;

    protected override void OnCreate()
    {
        CelldataQuery = GetEntityQuery(ComponentType.ReadOnly<CellsBestDirection>());

        endsimBuffer = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();

        deathTimerMax = 3f;
    }


    protected override void OnUpdate()
    {
        //TODO This system will make the enemies move towards the opposite direction for a time as an indicator that they are angry and leaving the store
        

        //var ecb = endsimBuffer.CreateCommandBuffer();
        var BestDirectionCells = CelldataQuery.ToComponentDataArray<CellsBestDirection>(Allocator.TempJob);
        var Timedelt = Time.DeltaTime;

        FlowFieldData tempflowfield = HasSingleton<FlowFieldData>() ? GetSingleton<FlowFieldData>() : default;

        var deathTimeMax = deathTimerMax;
        //walkTimer += Time.DeltaTime;
        //TODO Do I use a buffer on each enemy or not to store all the timers
        var tempflatus = new ToFlatIndex();

        Entities.
            WithReadOnly(BestDirectionCells).
            //WithReadOnly(TranslutionEnts).
            WithDisposeOnCompletion(BestDirectionCells).
            WithAll<EnemyDudeTag>().
            ForEach((Entity endDude, ref Translation currPos, ref LocalToWorld myworld, ref Rotation rotor, ref DeathAnimeTimer deathTimer, ref IsAlive isLive, ref EnemyStatesDat meStates) =>
            {
                if (isLive.booVal == false)
                    return;

                if (meStates.EnemyState == EnemyStates.leavingentrance)
                {


                    if (deathTimer.fltVal >= deathTimeMax)
                    {                        
                        isLive.booVal = false;
                        return;
                    }

                    //var currcellpos = cellindexworldpos.Execute(currPos.Value, tempflowfield.gridSize, tempflowfield.cellRadius * 2);
                    var currcellPOS = GridCellHelperStuff.GetGridXZ(currPos.Value, tempflowfield.GridOrigin, tempflowfield.cellRadius * 2);
                    //var tempgridPOS = math.abs(tempflatus.Execute(currcellPOS, tempflowfield.gridSize.y));
                    var tempgridPOS = math.abs(GridCellHelperStuff.ToFlatIndex(currcellPOS, tempflowfield.gridSize.y));
                    if (tempgridPOS < 0 || tempgridPOS > BestDirectionCells.Length)
                    {
                        tempgridPOS = 0;  
                    }

                    //var tempcurrentdirection = BestDirectionCells[tempgridpos];
                    var tempcurrDirection = BestDirectionCells[tempgridPOS];                   

                    var targetDirection = new float3(tempcurrDirection.bestDirection.x, 0, tempcurrDirection.bestDirection.y);

                    currPos.Value += -targetDirection * Timedelt * 10f;
                    //math.slerp(rotor.Value, quaternion.LookRotation(targetDirection, math.up()), Timmydelta * 5);
                    rotor.Value = quaternion.LookRotation(-targetDirection, math.up());

                    deathTimer.fltVal += Timedelt;
                }

            }).Schedule();
       
    }
}