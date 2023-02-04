using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class ShelfDestroyedAuthor : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent<ShelfDestroyedTag>(entity);
        dstManager.AddComponent<IsAlive>(entity);
        dstManager.AddComponent<DeathAnimeTimer>(entity);
        dstManager.SetComponentData<DeathAnimeTimer>(entity, new DeathAnimeTimer { fltVal = 4f });
        dstManager.SetComponentData<IsAlive>(entity, new IsAlive { booVal = true });
    }
}


public struct ShelfDestroyedTag : IComponentData { }
