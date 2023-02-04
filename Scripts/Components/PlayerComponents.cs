using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public struct PlayerLevel : IComponentData
{
    public int intVal;
}

//TODO WHat stats should the player have as a God
//

public struct PowerDat : IComponentData//This effects the amount of damage you do as well as the level,its basically just the strength value
{
    public int intVal;

}

public struct ProfitsDat : IComponentData//The number of souls you currently have stored, this could function both as a resource and your health zero you are dead
{
    public int intVal;
}

public struct LossDat : IComponentData
{
    public int intVal;
}

public enum ControlModes
{
    EnvCursorMode,
    UICursorMode,
    DestroyTowerMode,
    CreateTowerMode

}

public enum MouseClickMode
{
    LightningAbility,
    DestroyTurret,
    PlaceTurret,
    PlaceSlipHazard,
    DestroyStuff,
    MoveAround
}


public struct ControlModesDat : IComponentData
{
    public ControlModes currMode;
}

public struct MouseClickModDat : IComponentData
{
    public MouseClickMode mousMode;
}


public struct CameraMoveDat : IComponentData
{
    public bool boolVal;

}