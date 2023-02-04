using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public struct EnemyDudeTag : IComponentData{}

public struct FlashHitTimer : IComponentData//A time that countsdown teh start of death 
{
    public float fltVal;
}

public struct DeathAnimeTimer : IComponentData
{
    public float fltVal;
}

public struct IsHitTag : IComponentData { }