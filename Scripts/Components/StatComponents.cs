using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public struct BaseHealth : IComponentData
{
    public int intVal;
}

public struct CurrHealth : IComponentData
{
    public int intVal;
}

public struct DamageGiven : IComponentData
{
    public int intVal;
}

public struct DamageTaken : IComponentData//TODO I may need to change this for a buffer on each enemy entity later depending
{
    public int intVal;
}

public struct IsAlive : IComponentData
{
    public bool booVal;
}