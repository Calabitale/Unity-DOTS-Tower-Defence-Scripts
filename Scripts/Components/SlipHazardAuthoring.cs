using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Physics.Stateful;


public class SlipHazardAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddBuffer<StatefulTriggerEvent>(entity);
        dstManager.AddComponent<SlipHazardTag>(entity);
        dstManager.AddComponent<IsAlive>(entity);
        dstManager.AddComponent<DeathAnimeTimer>(entity);
        dstManager.SetComponentData<DeathAnimeTimer>(entity, new DeathAnimeTimer { fltVal = 5f });
        dstManager.SetComponentData<IsAlive>(entity, new IsAlive { booVal = true });
    }
}

public struct SlipHazardTag : IComponentData
{

}