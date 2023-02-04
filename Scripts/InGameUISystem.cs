using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using UnityEngine.UI;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Collections;

[DisableAutoCreation]
public partial class InGameUISystem : SystemBase
{
    GameObject GameObjectManStuff;
    GameObjectRefs GameRefs;

    GameObject PlaceTurretGO;
    GameObject DestroyTurretGO;

    CollisionWorld collisionWorld;

    PlayerInput playInput;

    protected override void OnCreate()
    {
        GameObjectManStuff = GameObject.Find("GameManagerStuff");

        GameRefs = GameObjectManStuff.GetComponent<GameObjectRefs>();

        PlaceTurretGO = GameRefs.StartButton;
        DestroyTurretGO = GameRefs.StartMenu;

        var m_BuildPhysicsWorld = World.GetExistingSystem<BuildPhysicsWorld>();

        CollisionWorld collisionWorld = m_BuildPhysicsWorld.PhysicsWorld.CollisionWorld;

        var currbotton = DestroyTurretGO.GetComponentInChildren<Button>();
        currbotton.onClick.AddListener(delegate { DestroyTurretMeth(); });
    }

    protected override void OnStartRunning()
    {
        


    }

    protected override void OnUpdate()
    {
        playInput = HasSingleton<PlayerInput>() ? GetSingleton<PlayerInput>() : default;
    }

    void DestroyTurretMeth()
    {

        Vector2 mousePosition = playInput.CursorPos;

        UnityEngine.Ray unityRay = Camera.main.ScreenPointToRay(mousePosition);

        var rayInput = new RaycastInput
        {
            Start = unityRay.origin,
            End = unityRay.origin + unityRay.direction * 100f,
            Filter = GlobalPhysicsFilters.CollFilterAttackEnemy,
        };

        NativeList<Unity.Physics.RaycastHit> RaycastHits = new NativeList<Unity.Physics.RaycastHit>(Allocator.Temp);        

        if(collisionWorld.CastRay(rayInput, ref RaycastHits))
        {
            Debug.Log("It hit the turret");
        }
        //EntityManager.CreateEntity(typeof(PauseSystemDat));
        //StartButton.active = false;
        ///StartMenu.SetActive(false);// = false;
        //var contromde = PlayerCompQuery.GetSingleton<ControlModesDat>();
        //contromde.currMode = ControlModes.EnvCursorMode;
        //PlayerCompQuery.SetSingleton<ControlModesDat>(contromde);

    }

}
