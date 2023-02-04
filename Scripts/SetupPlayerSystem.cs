using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public partial class SetupPlayerSystem : SystemBase
{
    EntityArchetype PlayerArch;

    protected override void OnCreate()
    {
        base.OnCreate();
        PlayerArch = EntityManager.CreateArchetype(
            typeof(PlayerLevel),
            typeof(ProfitsDat),
            typeof(PowerDat),
            typeof(LossDat),
            typeof(ControlModesDat),
            typeof(CameraMoveDat),
            typeof(MouseClickModDat)
            
            );
    }

    protected override void OnStartRunning()
    {
        if (!HasSingleton<PlayerLevel>())
        {
            var entdude = EntityManager.CreateEntity(PlayerArch);

            SetComponent<PlayerLevel>(entdude, new PlayerLevel { intVal = 1 });
            SetComponent<ProfitsDat>(entdude, new ProfitsDat { intVal = 0 });
            SetComponent<PowerDat>(entdude, new PowerDat { intVal = 1 });
            SetComponent<ControlModesDat>(entdude, new ControlModesDat { currMode = ControlModes.UICursorMode });
            SetComponent<CameraMoveDat>(entdude, new CameraMoveDat { boolVal = true });
            SetComponent<MouseClickModDat>(entdude, new MouseClickModDat { mousMode = MouseClickMode.MoveAround });
        }

        Enabled = false;
    }

    protected override void OnUpdate()
    {
        
    }

}
