using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Physics.Stateful;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Transforms;
using Unity.Core;

public partial class SharkBehaveSystem : SystemBase
{
    public GameObject FrankShark;
    NativeArray<Translation> RobAntWaypoints;

    EntityQuery RobAntWaypointQuery;
    NativeReference<int> CurrWaypoint;

    public float sharkspeed;
    protected override void OnCreate()
    {
        FrankShark = GameObject.Find("FranktheShark");
        RobAntWaypointQuery = GetEntityQuery(ComponentType.ReadOnly<RobAntWaypTag>(), ComponentType.ReadOnly<Translation>());

        sharkspeed = 10;

        RequireSingletonForUpdate<SharkHappensEvent>();
    }

    protected override void OnStartRunning()
    {
        RobAntWaypoints = RobAntWaypointQuery.ToComponentDataArray<Translation>(Allocator.Persistent);
        CurrWaypoint = new NativeReference<int>(Allocator.Persistent);

        CurrWaypoint.Value = RobAntWaypointQuery.CalculateEntityCount() - 1;
       
    }

    protected override void OnUpdate()
    {
        //TODO I need to figure out if I can synchronise a physicsbody with a gameobject and have them both move together, YES I CAN        

        if (math.distance(FrankShark.transform.position, RobAntWaypoints[CurrWaypoint.Value].Value) > 1f)
        {
            var anothefuckingvar = RobAntWaypoints[CurrWaypoint.Value];
            anothefuckingvar.Value.y = 0.03f;
            RobAntWaypoints[CurrWaypoint.Value] = anothefuckingvar;
            Vector3 currMovDir = math.normalize(RobAntWaypoints[CurrWaypoint.Value].Value - (float3)FrankShark.transform.position);

            //currMovDir.y = -0.03f;            
            FrankShark.transform.position += currMovDir * sharkspeed * Time.DeltaTime;//Normal speed equals 10
            //var temppos = FrankShark.transform.position;
            //temppos.y = 0.03f;
            //FrankShark.transform.position = temppos;
            var newrotate = quaternion.LookRotation(currMovDir, math.up());
            //currRotat.Value = Quaternion.Slerp(currRotat.Value, lookrotate, timDelta * 3);
            FrankShark.transform.rotation = Quaternion.Slerp(FrankShark.transform.rotation, newrotate, Time.DeltaTime * 4);
        }
        else
        {
            int tempwaylimit = RobAntWaypoints.Length - 1;
           
            if (CurrWaypoint.Value <= tempwaylimit)
            {
                CurrWaypoint.Value -= 1;
                if (CurrWaypoint.Value <= 0)
                    CurrWaypoint.Value = 0;
            }

            //if (CurrWaypoint.Value == tempwaylimit)
                //CurrWaypoint.Value = RobAntWaypoints.Length - 1;

        }

        Entities
           .WithName("FrankSharkTriggerJob")
           .WithAll<FrankSharkTag>()
           //.WithBurst()
           .WithoutBurst()
           .WithStructuralChanges()
           .ForEach((Entity e, ref DynamicBuffer<StatefulTriggerEvent> triggerEventBuffer, ref Translation currTrans, ref Rotation currRotate, in LocalToWorld currworld) =>
           {
               if (math.distance(currTrans.Value, RobAntWaypoints[CurrWaypoint.Value].Value) > 0.5f)
               {
                   var anothefrikingvar = RobAntWaypoints[CurrWaypoint.Value];
                   
                   RobAntWaypoints[CurrWaypoint.Value] = anothefrikingvar;
                   float3 currMovDir = math.normalize(RobAntWaypoints[CurrWaypoint.Value].Value - currTrans.Value);

                   currTrans.Value += currMovDir * sharkspeed * Time.DeltaTime;
                   //var directionfloat = new float3(1, 0, 0);
                   //var newrotate = quaternion.LookRotation(currMovDir, directionfloat);

                   //currRotate.Value = newrotate;//Todo 
                   
                   //currRotate.Value = Quaternion.Slerp(FrankShark.transform.rotation, newrotate, Time.DeltaTime * 4);
               }
               else
               {
                   int tempwaylimit = RobAntWaypoints.Length - 1;
                  
                   //if (CurrWaypoint.Value <= tempwaylimit)
                   {
                      
                       CurrWaypoint.Value -= 1;
                      
                       if (CurrWaypoint.Value <= 0)
                       {
                           CurrWaypoint.Value = 0;
                           if (math.distance(currTrans.Value, RobAntWaypoints[0].Value) <= 1f)
                           {
                              
                               if (!HasSingleton<ChaosEndsEvent>())//TODO I need this because for some reason it goes into this twice even though I only have one shark
                               {
                                   EntityManager.CreateEntity(typeof(ChaosEndsEvent));
                               }
                               EntityManager.DestroyEntity(HasSingleton<SharkHappensEvent>() ?  GetSingletonEntity<SharkHappensEvent>() : default);
                              
                               return;
                           }
                       }
                   }
                   //if (CurrWaypoint.Value == tempwaylimit)
                   //CurrWaypoint.Value = RobAntWaypoints.Length - 1;

               }

               

               for (int i = 0; i < triggerEventBuffer.Length; i++)
               {
                   var triggerEvent = triggerEventBuffer[i];
                   if (!EntityManager.Exists(triggerEvent.GetOtherEntity(e)))
                       return;

                   var otherEntity = triggerEvent.GetOtherEntity(e);

                   if(triggerEvent.State == StatefulEventState.Enter)
                   {
                       
                       
                       SetComponent<EnemyStatesDat>(otherEntity, new EnemyStatesDat { EnemyState = EnemyStates.leavingentrance });
                   }

               }



           }).Run();
           
    
    }



}


public struct SharkHappensEvent : IComponentData { }