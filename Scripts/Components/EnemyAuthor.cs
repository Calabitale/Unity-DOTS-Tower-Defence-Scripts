using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class EnemyAuthor : MonoBehaviour , IConvertGameObjectToEntity
{
    public int baseHealth;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {

        dstManager.AddComponent<EnemyDudeTag>(entity);
        dstManager.AddComponent<CellBDLayer>(entity);
        dstManager.AddComponent<BaseHealth>(entity);
        dstManager.AddComponent<CurrHealth>(entity);
        dstManager.AddComponent<DamageTaken>(entity);//NOTE This is not the amount of damage it does its the amount of damage its taking I need to rename this
        dstManager.AddComponent<IsAlive>(entity);//TODO I need to setup some of these values somewhere either here or somewhere else but I only have one enemy currently so should be fine
        dstManager.AddComponent<FlashHitTimer>(entity);
        dstManager.AddComponent<DeathAnimeTimer>(entity);
        dstManager.AddComponent<EnemyStatesDat>(entity);
       

        dstManager.SetComponentData(entity, new IsAlive { booVal = true });
        dstManager.SetComponentData(entity, new BaseHealth { intVal = baseHealth });
        dstManager.SetComponentData(entity, new CurrHealth { intVal = baseHealth });
        dstManager.SetComponentData(entity, new EnemyStatesDat { EnemyState = EnemyStates.shopping });

    }

}

//[DisallowMultipleComponent]
//[RequiresEntityConversion]
//public class CameraTagAuthoring : MonoBehaviour, IConvertGameObjectToEntity
//{
//    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
//    {
//
//        //dstManager.AddComponentData(entity, new CameraTag());
//        dstManager.AddComponent(entity, typeof(CopyTransformToGameObject));
//    }
//}
