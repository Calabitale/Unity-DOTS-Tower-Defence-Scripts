using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Physics;

public static class GlobalPhysicsFilters
{
    public const uint EnvironmentLayer = 1 << 0; //<< 3;//THis should equal layer zero
    public const uint EnemyLayer = 1 << 1; //THis should equal layer 1
    public const uint Layer_Raycast = 2;
    public const uint TurretLayer = 8;

    public static CollisionFilter CollFilter_Environment = new  CollisionFilter() { BelongsTo = EnvironmentLayer, CollidesWith = EnemyLayer | EnvironmentLayer };
    public static CollisionFilter CollFilterAttackEnemy = new CollisionFilter() { BelongsTo = EnvironmentLayer, CollidesWith = EnemyLayer };
    public static CollisionFilter CollFilterTurret = new CollisionFilter() { BelongsTo = TurretLayer, CollidesWith = TurretLayer };//TODO Need to figure out this I don't understand it at all, OK I do kind of understand it now



  
}
