using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;


public partial class ChaosHappensSystem : SystemBase
{
    //TODO If I ever get further with this prototype which I probably wont this can be used to decide between all the different chaos things that might happen

    public GameObject WaterBlockGO;
    float MaxWaterHeight;
    float WaterRiseSpeed;

    public EntityQuery ChaosQuery;
    protected override void OnCreate()
    {
        var singleupdate = new EntityQueryDesc
        {
            Any = new ComponentType[]
            {
                ComponentType.ReadOnly<ChaosHappensEvent>(),//TODO need to perhaps think about just reducing this to a single entity witha  boolean or something
                ComponentType.ReadOnly<ChaosEndsEvent>()
            }
            //None = new ComponentType[] { typeof(Static) },
            //All = new ComponentType[]{ typeof(RotationQuaternion),
            //               ComponentType.ReadOnly<RotationSpeed>() }
        };

        RequireForUpdate(GetEntityQuery(singleupdate));        
       
        WaterBlockGO = GameObject.Find("WaterBlockGO");//TODO I know this is a bad way of doing this just doing this this way for now for speed this is just a fricking prototypye ok

        MaxWaterHeight = 3.1f;
        WaterRiseSpeed = 0.5f;
    }

    protected override void OnUpdate()
    {

        if (HasSingleton<ChaosHappensEvent>())
        {
            var tempPos = WaterBlockGO.transform.position;
            tempPos.y += WaterRiseSpeed * Time.DeltaTime;
            WaterBlockGO.transform.position = tempPos;

            if (tempPos.y >= 3.1f)
            {
                EntityManager.DestroyEntity(GetSingletonEntity<ChaosHappensEvent>());
                if(!HasSingleton<SharkHappensEvent>())                                    
                    EntityManager.CreateEntity(typeof(SharkHappensEvent));//TODO ONe rule whenever creating a singleton entity I should always check if there isn't already one existing
            }
        }

        if (HasSingleton<ChaosEndsEvent>())
        {
            var tempPos = WaterBlockGO.transform.position;
            tempPos.y -= WaterRiseSpeed * Time.DeltaTime;
            WaterBlockGO.transform.position = tempPos;            
            if (tempPos.y <= -1f)
            {
                EntityManager.DestroyEntity(GetSingletonEntity<ChaosEndsEvent>());
                //EntityManager.DestroyEntity(GetSingletonEntity<SharkHappensEvent>());

            }
        }

    }
}

public struct ChaosHappensEvent : IComponentData { }
    
public struct ChaosEndsEvent : IComponentData { }