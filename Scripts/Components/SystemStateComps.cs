using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public struct PauseSystemDat : IComponentData { }//I'll use this as a basic pause system for now if this does not exist then the systems won't run so simple then

public struct InitialiseEnemysTag : IComponentData { }

public enum EnemyStates    
{
    shopping,
    slipping,
    leavingentrance,
    leavingexit,
    isDead

}

public struct EnemyStatesDat : IComponentData
{
    public EnemyStates EnemyState;
}