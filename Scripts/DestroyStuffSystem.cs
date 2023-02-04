using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Physics.Systems;
using Unity.Physics;
using Unity.Physics.Extensions;

public partial class DestroyStuffSystem : SystemBase
{
    PhysicsWorld world;
    CollisionWorld collisionWorld;

    protected override void OnCreate()
    {
        var m_BuildPhysicsWorld = World.GetExistingSystem<BuildPhysicsWorld>();
        collisionWorld = m_BuildPhysicsWorld.PhysicsWorld.CollisionWorld;
        world = m_BuildPhysicsWorld.PhysicsWorld;
    }

    protected override void OnUpdate()
    {
        //var DestroyStuffCollFillEntity = HasSingleton<DestroyShelvesCollFilterTag>() ? GetSingletonEntity<DestroyShelvesCollFilterTag>() : default;
        //var destroystuffrigid = world.GetRigidBodyIndex(DestroyStuffCollFillEntity);
        //CollisionFilter DestroyStuffCollFilter = world.GetCollisionFilter(destroystuffrigid);





    }
}
