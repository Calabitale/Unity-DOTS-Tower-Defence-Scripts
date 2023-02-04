using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;



[AlwaysUpdateSystem]
public partial class TimeTickSystem : SystemBase
{
    //1.0f is about equal to 1 second 0.5 is about half a second
    public float TICK_TIMER_MAX; // = 2f;
    public float Longer_TickMax;

    private int tick;
    private float tickTimer;

    protected override void OnCreate()
    {
        TICK_TIMER_MAX = 0.5f;
        Longer_TickMax = 1f;
        //this conflicts with AlwaysupdateSystem
        //RequireSingletonForUpdate<PauseSystemsCompTag>();
    }

    protected override void OnStartRunning()
    {
        tick = 0;
        tickTimer = 0.0f;
    }

    //To get rid of the inconsistent spawning at the beginning I can delay spawning or this for a couple of seconds perhaps
    protected override void OnUpdate()
    {
        if (HasSingleton<TimeTickEvent>())
        {
            EntityManager.DestroyEntity(GetSingletonEntity<TimeTickEvent>());
            
        }
        
        if (HasSingleton<TimeTickPartialEvent>())
        {
            
            EntityManager.DestroyEntity(GetSingletonEntity<TimeTickPartialEvent>());
        }




        tickTimer += Time.DeltaTime;
        if (tickTimer >= TICK_TIMER_MAX)
        {
            tickTimer -= TICK_TIMER_MAX;
            tick++;
            
            if (!HasSingleton<TimeTickEvent>())
            {
                
                EntityManager.CreateEntity(typeof(TimeTickEvent));

            }

            //if(tick % Longer_TickMax == 0)//TODO I donn't need another timer currently
            //{
            //    if(!HasSingleton<TimeTickPartialEvent>())
            //    {
            //        EntityManager.CreateEntity(typeof(TimeTickPartialEvent));
            //  
            //    }
            //}


        }

    }

}
public struct TimeTickEvent : IComponentData { }
public struct TimeTickPartialEvent : IComponentData { }
