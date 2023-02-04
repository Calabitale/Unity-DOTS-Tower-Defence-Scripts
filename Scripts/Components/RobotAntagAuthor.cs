using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Physics.Stateful;
using Unity.Mathematics;


public class RobotAntagAuthor : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddBuffer<StatefulTriggerEvent>(entity);
        dstManager.AddComponent<RobotAntagonistTag>(entity);
        dstManager.AddComponent<RobotAntStates>(entity);
        dstManager.AddComponent<RobotTargetEnt>(entity);
        dstManager.SetComponentData<RobotAntStates>(entity, new RobotAntStates { currStat = RobotAntEnum.Patrolling });
        //dstManager.AddComponent<IsAlive>(entity);
        //dstManager.AddComponent<DeathAnimeTimer>(entity);
        //dstManager.SetComponentData<DeathAnimeTimer>(entity, new DeathAnimeTimer { fltVal = 5f });
        //dstManager.SetComponentData<IsAlive>(entity, new IsAlive { booVal = true });
    }
}

public struct RobotAntagonistTag : IComponentData { }

public struct RobotAntStates : IComponentData
{
    public RobotAntEnum currStat;
}

public struct RobotTargetEnt : IComponentData
{
    public Entity entVal;
    public float3 flt3Val;
}

public enum RobotAntEnum
{
    Patrolling,
    MoveToTurret,
    RotateTowardsObj,
    DestroyingTurret,
    Standing,
    FixShelves,

}