using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Physics.Authoring;


public class ShelfItemsAuthor : MonoBehaviour, IConvertGameObjectToEntity
{

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent<ShelfItemsTag>(entity);
        //var physicsshappe = GetComponent<PhysicsBodyAuthoring>();     

        //var currphys = physicsshappe.InitialLinearVelocity;
        //var direction = Random.insideUnitSphere.normalized;
        //currphys = direction * 5;
        //physicsshappe.InitialLinearVelocity = currphys;

    }

}
 

public struct ShelfItemsTag : IComponentData
{
}
