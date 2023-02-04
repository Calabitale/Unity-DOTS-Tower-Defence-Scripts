using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class TurretAuthor : MonoBehaviour, IConvertGameObjectToEntity
{
    [SerializeField]
    private int TurretDamage; 

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {

        dstManager.AddComponent<TurretTag>(entity);
        dstManager.AddComponentData<DamageGiven>(entity, new DamageGiven { intVal = TurretDamage });
        dstManager.AddComponent<TurretAttackRate>(entity);
        dstManager.AddComponent<TurretTarget>(entity);
        dstManager.AddComponent<DeathAnimeTimer>(entity);
        dstManager.AddComponent<IsAlive>(entity);

        dstManager.SetComponentData(entity, new DeathAnimeTimer { fltVal = 2f });
        dstManager.SetComponentData(entity, new IsAlive { booVal = true });
    }

}

public struct TurretTag : IComponentData {}

public struct TurretAttackRate : IComponentData
{
    public float fltVal;
}

public struct StartFiring : IComponentData//NOTE Not sure if I will use this yet but will create it just in case, Will just have Turret Firing immediately on creation
{
    public bool bolVal;
}

public struct TurretTarget : IComponentData
{
    public Entity entVal;
}

public struct IDIED : IComponentData { }