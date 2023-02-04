using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Physics.Stateful;

public class PaymentTriggerAuthor : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddBuffer<StatefulTriggerEvent>(entity);
        dstManager.AddComponent<PaymentTriggerTag>(entity);
        
    }
}

public struct PaymentTriggerTag : IComponentData
{
    
}
