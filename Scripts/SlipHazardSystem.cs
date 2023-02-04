using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Physics.Stateful;

public partial class SlipHazardSystem : SystemBase
{
    protected override void OnCreate()
    {
        base.OnCreate();
    }

    protected override void OnUpdate()
    {

        Entities
           .WithName("SlippingHazardJob")//TODO I need to make this work with a random chance to slip up the enemies and also cause an animation and other stuff
           .WithBurst()
           .ForEach((Entity e, ref DynamicBuffer<StatefulTriggerEvent> triggerEventBuffer) =>//I could possibly combine this into a single job with all the other trigger events that's if I really need the performance though
           {
               
               for (int i = 0; i < triggerEventBuffer.Length; i++)
               {
                   var triggerEvent = triggerEventBuffer[i];
                   var otherEntity = triggerEvent.GetOtherEntity(e);

                   if (!HasComponent<SlipHazardTag>(e))
                       return;

                   // exclude other triggers and processed events
                   if (triggerEvent.State == StatefulEventState.Stay)// || !nonTriggerMask.Matches(otherEntity))
                   {
                       //SetComponent<EnemyStatesDat>(otherEntity, new EnemyStatesDat { EnemyState = EnemyStates.slipping });
                       continue;
                       //return;
                   }

                   if (triggerEvent.State == StatefulEventState.Enter)
                   {                       
                       //SetComponent<EnemyStatesDat>(otherEntity, new EnemyStatesDat { EnemyState = EnemyStates.slipping });//TODO I need to setup some kind of animation system
                       //FOR Now just set it to leaving entrance
                       //currSTate.EnemyState = EnemyStates.slipping;

                       continue;
                   }
                   else//State == PhysicsEventState.Exit
                   {                       
                       SetComponent<EnemyStatesDat>(otherEntity, new EnemyStatesDat { EnemyState = EnemyStates.leavingentrance });

                       continue;
                   }
               }
           }).Run();

    }
}
