using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using Unity.Physics.Stateful;
using Unity.Transforms;
using Unity.Mathematics;
using System;
using System.Drawing;
using static UnityEngine.GraphicsBuffer;
using System.Linq;
using Unity.Physics;
using Unity.Entities.UniversalDelegates;


[DisableAutoCreation]
public partial class AntagonistRobotSystem : SystemBase
{
    EntityQuery RobotAntagonQuery;
    EntityQuery RobAntWaypointQuery;

    NativeArray<Translation> RobAntWaypoints;
    NativeReference<int> CurrWaypoint;//I'm going to store the Robots current waypoint in the system for now its a bad way, should be on the robot but this will suffice

    NativeReference<float> DestroyTimer;
    protected override void OnCreate()
    {
        RobotAntagonQuery = GetEntityQuery(ComponentType.ReadOnly<RobotAntagonistTag>(), ComponentType.ReadWrite<Translation>());
        RobAntWaypointQuery = GetEntityQuery(ComponentType.ReadOnly<RobAntWaypTag>(), ComponentType.ReadOnly<Translation>());

        //Enabled = false;
        
    }

    protected override void OnStartRunning()
    {

        var RobotantPos = RobotAntagonQuery.GetSingleton<Translation>();
        RobAntWaypoints = RobAntWaypointQuery.ToComponentDataArray<Translation>(Allocator.Persistent);

        RobotAntagonQuery.SetSingleton<Translation>(new Translation { Value = RobAntWaypoints[0].Value });
        //RobotantPos.Value =  RobAntWaypoints[0].Value;
        //Array.Reverse(RobAntWaypoints);

        CurrWaypoint = new NativeReference<int>(Allocator.Persistent);//RobAntWaypoints[].Value;

        RequireSingletonForUpdate<PauseSystemDat>();

        CurrWaypoint.Value = 1;//RobAntWaypoints[1].Value;

        DestroyTimer = new NativeReference<float>(Allocator.Persistent);

        DestroyTimer.Value = 0f;
        //DestroyTimer = 2f;
    }


    protected override void OnUpdate()
    {
        var Timmdelta = Time.DeltaTime;

        var RobantWaypJob = RobAntWaypoints;

        NativeReference<int> Currpwaypoint = CurrWaypoint;//TODO This probably needs to just be an int so as a reference to the element in the array

        NativeReference<float> destroyobjtim = DestroyTimer;

        //Debug.Log("The robotantwayp size is " + RobantWaypJob.Length);
        //Currpwaypoint.Value = RobAntWaypoints[2].Value;

        //var currjob = new RobotAntagonistJob()
        //{
        //    RobAntWaypoints = RobantWaypJob,
        //    timDelta = Timmdelta,
        //    currwaypint = CurrWaypoint,
        //    //turrDude = GetComponentDataFromEntity<Translation>(),
        //    //payTrigger = GetComponentDataFromEntity<PaymentTriggerTag>(),
        //    //Profitref = ProfitsRef//new NativeReference<ProfitsDat>(Allocator.TempJob)

        //}; // Schedule();
        var entstorage = GetStorageInfoFromEntity();

        new RobotAntagonistJob()
        {
            RobAntWaypoints = RobantWaypJob,
            timDelta = Timmdelta,
            currwaypint = Currpwaypoint,
            isObjAlive = GetComponentDataFromEntity<IsAlive>(),
            DestroyObjtime = destroyobjtim,
            theenty = entstorage,
            ShelfDestrTag = GetComponentDataFromEntity<ShelfDestroyedTag>(),
            Turrtag = GetComponentDataFromEntity<TurretTag>()
            //currTrans = GetComponentDataFromEntity<Translation>()


        }.Schedule();//.Complete();

        //currjob.Schedule(this.Dependency).Complete();
        //currjob.Schedule(this.Dependency);

        //Dependency.Complete();

        Entities
           .WithName("RobotAntagDestroyJob")
           .WithAll<RobotAntagonistTag>()
           .WithBurst()
           .ForEach((Entity e, ref DynamicBuffer<StatefulTriggerEvent> triggerEventBuffer, ref RobotAntStates currstate, ref RobotTargetEnt tarEnt) =>
           {     

               if (currstate.currStat != RobotAntEnum.Patrolling)
                   return;

               bool trigeventstayfound = false;
               float closestdist = 50f;
               int trigevenbuff = 0;
               Entity nearestEntity = Entity.Null;

               for (int i = 0; i < triggerEventBuffer.Length; i++)
               {
                   var triggerEvent = triggerEventBuffer[i];
                   var otherEntity = triggerEvent.GetOtherEntity(e);

                   
                   // exclude other triggers and processed events
                   if (triggerEvent.State == StatefulEventState.Stay)// || !nonTriggerMask.Matches(otherEntity))
                   {
                       var robotpos = GetComponent<Translation>(e);
                       var otherpos = HasComponent<Translation>(otherEntity) ? GetComponent<Translation>(otherEntity) : default;
                       var calceddist = math.distance(robotpos.Value, otherpos.Value);
                       
                       if (calceddist < closestdist)
                       {
                           closestdist = calceddist;
                           nearestEntity = otherEntity;
                           trigevenbuff = i;
                           
                       }

                       trigeventstayfound = true;
                       continue;
                   }

                   if (triggerEvent.State == StatefulEventState.Enter)
                   {

                       continue;
                   }
                   else//State == PhysicsEventState.Exit
                   {
                       
                       continue;
                   }


               }

               if(trigeventstayfound)
               {
                   //if (nearestEntity.Equals(Entity.Null))
                       //return;

                   if (HasComponent<TurretTag>(nearestEntity))
                   {

                       //currstate.currStat = RobotAntEnum.MoveToTurret;
                       tarEnt.entVal = nearestEntity;
                       tarEnt.flt3Val = GetComponent<Translation>(nearestEntity).Value;
                   }
                   else if (HasComponent<ShelfDestroyedTag>(nearestEntity))
                   {
                       //currstate.currStat = RobotAntEnum.MoveToTurret;
                       tarEnt.entVal = nearestEntity;
                       tarEnt.flt3Val = GetComponent<Translation>(nearestEntity).Value;
                       
                   }

               }
               else
               {
                   tarEnt.entVal = Entity.Null;
                   tarEnt.flt3Val = new float3(0, 0, 0);
               }

           }).Run();

    }

    protected override void OnStopRunning()
    {
        DestroyTimer.Dispose();
        RobAntWaypoints.Dispose();
        CurrWaypoint.Dispose();
    }

    protected override void OnDestroy()
    {
        //CurrWaypoint.Dispose();
        //DestroyTimer.Dispose();
    }
}


[WithAll(typeof(RobotAntagonistTag))]
public partial struct RobotAntagonistJob : IJobEntity
{
    public ComponentDataFromEntity<IsAlive> isObjAlive;
    public ComponentDataFromEntity<ShelfDestroyedTag> ShelfDestrTag;
    public ComponentDataFromEntity<TurretTag> Turrtag;
    //public ComponentDataFromEntity<PaymentTriggerTag> payTrigger;

    //public ProfitsDat CurrProfits;
    //public NativeReference<ProfitsDat> Profitref;

    public NativeArray<Translation> RobAntWaypoints;
    public NativeReference<int> currwaypint;
    public float timDelta;
    public NativeReference<float> DestroyObjtime;

    public StorageInfoFromEntity theenty;
    //public ComponentDataFromEntity<Translation> currTrans;
    public float RobSpeed;// ; = 20f;

    public void Execute(Entity ent, ref Translation currTrans, ref Rotation currRotat, ref RobotAntStates currState, ref RobotTargetEnt tarpos)//TODO I should make this object agnostic
    {
        switch (currState.currStat)
        {
            case RobotAntEnum.Patrolling:
                
                if (tarpos.entVal != Entity.Null)
                    currState.currStat = RobotAntEnum.MoveToTurret;

                
                if (math.distance(currTrans.Value, RobAntWaypoints[currwaypint.Value].Value) > 0.5f)
                {
                    float3 currMovDir = math.normalize(RobAntWaypoints[currwaypint.Value].Value - currTrans.Value);

                    //currTrans.Value += currMovDir * 10 * timDelta;
                    //float3 prevpos = currTrans.Value;
                    currTrans.Value += currMovDir * 5 * timDelta;
                    currRotat.Value = quaternion.LookRotation(currMovDir, math.up());
                }
                else
                {
                    int tempwaylimit = RobAntWaypoints.Length - 1;
                    if (currwaypint.Value < tempwaylimit)
                    {
                        currwaypint.Value += 1;
                        //if (currwaypint.Value == tempwaylimit)
                        //    currwaypint.Value = 0;
                    }
                    if (currwaypint.Value == tempwaylimit)
                        currwaypint.Value = 0;

                }
                            

                break;
            case RobotAntEnum.DestroyingTurret:
                
                if ((!theenty.Exists(tarpos.entVal)) || tarpos.entVal.Equals(Entity.Null)  || !isObjAlive.HasComponent(tarpos.entVal))
                {
                    
                    tarpos.entVal = Entity.Null;
                    tarpos.flt3Val = new float3();
                    currState.currStat = RobotAntEnum.Patrolling;
                    return;
                }

                //(!theenty.Exists(tarpos.entVal))

                //if (!(math.abs(currTrans.Value.x - tarpos.flt3Val.x) < 0.1))//TODO This is not always hitting when it should need to check the variance and accuracy of this, its probably due to the speed and its jumping over to far from the position
                //{//TODO I need to ensure that there is actually a turret to move towards
                //    Debug.Log("It went into the Destroy Turret second check"); 
                //    currState.currStat = RobotAntEnum.Patrolling;
                //    break;
                //}

                if (!isObjAlive[tarpos.entVal].booVal)//This just makes the robot stand still until the Turret is destroyed
                {                    
                    return;
                }
                //var thevalue = isObjAlive[tarpos.entVal];
                //if(thevalue.booVal)
                //{
                //    //var thevalue = isObjAlive[tarpos.entVal];
                //    thevalue.booVal = false;
                //    isObjAlive[tarpos.entVal] = thevalue;
                //    DestroyObjtime.Value = 1f;                   
                //}               

                //DestroyObjtime.Value -= timDelta;

                //if (DestroyObjtime.Value <= 0)
                //{
                //    tarpos.entVal = Entity.Null;
                //    currState.currStat = RobotAntEnum.Patrolling;
                //}

                var thevalue = isObjAlive[tarpos.entVal];
                thevalue.booVal = false;
                isObjAlive[tarpos.entVal] = thevalue;

                //tarpos.entVal = Entity.Null;
                //tarpos.flt3Val = new float3();

                //currState.currStat = RobotAntEnum.Patrolling;//TODO I'm not sure I understand exactly how this works but I need this code here for some reason otherwise it does not work

                break;

            case RobotAntEnum.MoveToTurret:
                
                //if (tarpos.entVal.Equals(Entity.Null))//TODO this is not a good idea this will just cause an infinite loop
                    //return;

                if (math.abs(currTrans.Value.x - tarpos.flt3Val.x) < 0.1)//TODO This is not always hitting when it should need to check the variance and accuracy of this, its probably due to the speed and its jumping over to far from the position
                {//TODO I need to ensure that there is actually a turret to move towards
                    
                    currState.currStat = RobotAntEnum.RotateTowardsObj;
                    break;
                }

                if (math.distance(currTrans.Value, RobAntWaypoints[currwaypint.Value].Value) > 0.5f)
                {
                    float3 currMovDir = math.normalize(RobAntWaypoints[currwaypint.Value].Value - currTrans.Value);

                    //currTrans.Value += currMovDir * 10 * timDelta;
                    //float3 prevpos = currTrans.Value;
                    currTrans.Value += currMovDir * 5 * timDelta;
                    currRotat.Value = quaternion.LookRotation(currMovDir, math.up());
                }
                else
                {
                    int tempwaylimit = RobAntWaypoints.Length - 1;
                    if (currwaypint.Value < tempwaylimit)
                    {
                        currwaypint.Value += 1;
                    }
                }
                
                break;
            case RobotAntEnum.RotateTowardsObj:
                //_direction = (Target.position - transform.position).normalized;
                //_lookRotation = Quaternion.LookRotation(_direction);
                //transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation, Time.deltaTime * RotationSpeed);

                //float dot = Vector3.Dot(transform.forward, (other.position - transform.position).normalized);
                //if (dot > 0.7f) { Debug.Log("Quite facing"); }
              

                var targenormalised = tarpos.flt3Val;
                targenormalised.y = currTrans.Value.y;
                var lookdir = math.normalize(targenormalised - currTrans.Value);
                //lookdir.y = currTrans.Value.y;

                //var float3lookdir = quaternion.Euler(currRotat.Value);
                var lookrotate = quaternion.LookRotation(lookdir, math.up());


                var thedot = math.dot(math.forward(currRotat.Value), math.normalize(targenormalised - currTrans.Value));

                if(thedot >= 0.999 && Turrtag.HasComponent(tarpos.entVal))//TODO I should probably move this towards the top
                {
                    currState.currStat = RobotAntEnum.DestroyingTurret;
                }
                else if(thedot >= 0.999 && ShelfDestrTag.HasComponent(tarpos.entVal))
                {
                    currState.currStat = RobotAntEnum.FixShelves;
                }
                else if(tarpos.entVal.Equals(Entity.Null))//NOTE I should not need this it should only be going into destroy Turret from here 
                {
                    //currState.currStat = RobotAntEnum.Patrolling;
                }

                currRotat.Value = Quaternion.Slerp(currRotat.Value, lookrotate, timDelta * 3);
                break;
            case RobotAntEnum.FixShelves://TODO JUst need to figure out how to do this part
                //TODO I think I might need some kind of timer for this

                if ((!theenty.Exists(tarpos.entVal)) || tarpos.entVal.Equals(Entity.Null) || !isObjAlive.HasComponent(tarpos.entVal))
                {
                    
                    tarpos.entVal = Entity.Null;
                    tarpos.flt3Val = new float3();
                    currState.currStat = RobotAntEnum.Patrolling;
                    return;
                }

                if (!isObjAlive[tarpos.entVal].booVal)//This just makes the robot stand still until the Turret is destroyed
                {

                    return;
                }

               
                var theothervalue = isObjAlive[tarpos.entVal];
                theothervalue.booVal = false;
                isObjAlive[tarpos.entVal] = theothervalue;

                break;

                
        }
            
     

      

    }

}