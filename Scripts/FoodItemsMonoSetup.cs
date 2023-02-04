using System.Collections;
using System.Collections.Generic;
using Unity.Physics.Authoring;
using UnityEngine;
using Unity.Physics;

public class FoodItemsMonoSetup : MonoBehaviour
{
    void OnValidate()
    {
        var physicsshappe = GetComponent<PhysicsBodyAuthoring>();
        //physicsshappe.InitialLinearVelocity.z = Random.RandomRange(-100, 100);
        //physicsshappe.

        var currphys = physicsshappe.InitialLinearVelocity;//TODO This method is not going to likely work at all when I turn it into a prefab
        currphys.z = Random.Range(-100, 100);

        physicsshappe.InitialLinearVelocity = currphys;

    }

    void Awake()
    {
       
        
        

    }


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
