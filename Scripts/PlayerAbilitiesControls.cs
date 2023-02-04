using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Physics;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics.Extensions;
using Unity.Physics.Systems;
using Unity.Collections;
using DigitalRuby.LightningBolt;
using UnityEngine.UI;
using Unity.Burst;
using Unity.Jobs.LowLevel.Unsafe;
using Unity.Physics.Stateful;
using Unity.Rendering;
using UnityEngine.Rendering;
using Microsoft.Cci;
using Unity.Physics.Authoring;

//[AlwaysUpdateSystem]
//[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]//TODO I may need to separate the physics actions from teh input actions
[DisableAutoCreation]
[UpdateAfter(typeof(PlayerInputStuffSystem))]
public partial class PlayerAbilities : SystemBase
{
    public PlayerInput playInput;

    BuildPhysicsWorld m_BuildPhysicsWorld;
    StepPhysicsWorld stepphysworld;

    CollisionFilter currentcoll;

    EntityQuery CursorPosQuery;
    EntityQuery EnemyQuery;
    EntityQuery PlayerCompQuery;
    EntityQuery CursorSquareQuery;
    EntityQuery ShelfItemsQuery;

    GameObject DefaultExplosion;
    GameObject GameObjStoof;

    GameObject PlaceTurretGO;
    GameObject DestroyTurretGO;
    GameObject SlipHazGO;
    GameObject DestroyStuffGO;
    GameObject CursorMoveGO;
    GameObject IngameUIButtons;
    GameObject ChaosButtonGO;
    GameObject ExitScreenGO;

    Detonator ExplosionScript;

    bool MainAbilityExpBool;

    GameObject LightningBoltGO;

    LightningBoltScript lightningscript;

    float LightBoltTimer;
    float mainabilitytimer;
    float nextLighttimer;
    float CursorMoveSpeed;
    float lerptime;
    float nextFireinput;

    public int PricetoPlace;//NOTE THis is how much it costs to place objects and do things in the environment

    float2 currentinput;

    Camera mainCam;

    DotPrefabinator theprefabinator;

    int ChaosButtonCost;
    bool ChaosButtonEnable;

    protected override void OnCreate()
    {
        base.OnCreate();

        m_BuildPhysicsWorld = World.GetExistingSystem<BuildPhysicsWorld>();

        stepphysworld = World.GetOrCreateSystem<StepPhysicsWorld>();

        CursorPosQuery = GetEntityQuery(typeof(Translation), ComponentType.ReadOnly<CursorTag>());
        EnemyQuery = GetEntityQuery(ComponentType.ReadOnly<EnemyDudeTag>(), typeof(LocalToWorld), typeof(PhysicsMass), typeof(PhysicsVelocity));
        CursorSquareQuery = GetEntityQuery(typeof(Translation), ComponentType.ReadOnly<CursorSquareTag>());
        PlayerCompQuery = GetEntityQuery(typeof(ControlModesDat), typeof(MouseClickModDat));

        ShelfItemsQuery = GetEntityQuery(ComponentType.ReadOnly<ShelfItemsTag>());//, ComponentType.)

        RequireSingletonForUpdate<PauseSystemDat>();//NOT sure If I want this in here I still need the player to be able to control things when the game is paused

        GameObjStoof = GameObjectRefs.UIGameObjRefs;//GameObject.Find("GameManagerStuff");

        var gameobjrefs = GameObjStoof.GetComponent<GameObjectRefs>();

        ExitScreenGO = gameobjrefs.ExitScreenGO;
        //DefaultExplosion = gameobjrefs.DefaultExplosion;

        //ExplosionScript = DefaultExplosion.GetComponent<Detonator>();        

        //LightningBoltGO = gameobjrefs.LightningBolt;

        //lightningscript = LightningBoltGO.GetComponent<LightningBoltScript>();        

        PlaceTurretGO = gameobjrefs.PlaceTurret;
        DestroyTurretGO = gameobjrefs.DestroyTurret;
        SlipHazGO = gameobjrefs.PlaceSlipHazard;
        DestroyStuffGO = gameobjrefs.DestroyStuffGO;
        CursorMoveGO = gameobjrefs.MoveCursorGO;
        ChaosButtonGO = gameobjrefs.ChaosButtonGO;
        var ExitButtonGO = gameobjrefs.NoExitButtGO;

        var currbotton = DestroyTurretGO.GetComponentInChildren<Button>();
        currbotton.onClick.AddListener(delegate { DestroyTurretMeth(); });

        var otherbooton = PlaceTurretGO.GetComponentInChildren<Button>();
        otherbooton.onClick.AddListener(delegate { PlaceTurretMeth(); });

        var nothbootn = SlipHazGO.GetComponentInChildren<Button>();
        nothbootn.onClick.AddListener(delegate { PlaceSlipHazard(); });

        var tothbooton = DestroyStuffGO.GetComponentInChildren<Button>();
        tothbooton.onClick.AddListener(delegate { DestroyStuffMeth(); });

        var boothbooton = CursorMoveGO.GetComponentInChildren<Button>();
        boothbooton.onClick.AddListener(delegate { CursorMovedMeth(); });

        var sithbooton = ChaosButtonGO.GetComponentInChildren<Button>();
        sithbooton.onClick.AddListener(delegate { ChaoshappensButtMeth(); });

        var pythbooton = ExitButtonGO.GetComponentInChildren<Button>();
        pythbooton.onClick.AddListener(delegate { NoExitButton(); });



        LightBoltTimer = 1f;
        mainabilitytimer = 0.5f;
        CursorMoveSpeed = 20f;

        mainCam = Camera.main;

        PricetoPlace = 100;

        ChaosButtonCost = 10000;
        ChaosButtonEnable = true;

    }

    protected override void OnStartRunning()
    {

        var gameobjrefs = GameObjStoof.GetComponent<GameObjectRefs>();

        DefaultExplosion = gameobjrefs.DefaultExplosion;

        ExplosionScript = DefaultExplosion.GetComponent<Detonator>();

        LightningBoltGO = gameobjrefs.LightningBolt;

        lightningscript = LightningBoltGO.GetComponent<LightningBoltScript>();

        IngameUIButtons = gameobjrefs.InGameUIbuttons;

        theprefabinator = HasSingleton<DotPrefabinator>() ? GetSingleton<DotPrefabinator>() : default;
    }

    protected override void OnUpdate()
    {
        //var Rundomnumcreator = new NativeArray<Unity.Mathematics.Random>(JobsUtility.MaxJobThreadCount, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
        //var r = (uint)UnityEngine.Random.Range(int.MinValue, int.MaxValue);
        //for (int i = 0; i < JobsUtility.MaxJobThreadCount; i++)
        //{
        //    Rundomnumcreator[i] = new Unity.Mathematics.Random(r == 0 ? r + 1 : r);
        //}

        //var rndum = Rundomnumcreator[0];//[nativeThreadIndex]; I only need one currently;
        //var ultimaterand = rndum.NextInt(-10, 10);
        var CurrentProfits = HasSingleton<ProfitsDat>() ? GetSingleton<ProfitsDat>() : default;

        playInput = HasSingleton<PlayerInput>() ? GetSingleton<PlayerInput>() : default;
        var currplayerMode = PlayerCompQuery.GetSingleton<ControlModesDat>();

        var currmouseMode = PlayerCompQuery.GetSingleton<MouseClickModDat>();

        var Cameramovebool = GetSingleton<CameraMoveDat>();

        var CursorSquare = CursorSquareQuery.GetSingleton<Translation>();

        var timmydelt = Time.DeltaTime;

        CollisionWorld collisionWorld = m_BuildPhysicsWorld.PhysicsWorld.CollisionWorld;

        PhysicsWorld world = m_BuildPhysicsWorld.PhysicsWorld;

        var tempcurrentinput = new Vector3(playInput.CursorPos.x, playInput.CursorPos.y, 0);

        var theground = HasSingleton<GroundTag>() ? GetSingletonEntity<GroundTag>() : default;

        var enemyCollFilter = HasSingleton<EnemyCollFilterTag>() ? GetSingletonEntity<EnemyCollFilterTag>() : default;
        var EnvironCollFilter = HasSingleton<EnvironCollFilterTag>() ? GetSingletonEntity<EnvironCollFilterTag>() : default;//TODO These do not need to be here they should be put into ONstartRunning
        var TurretCollFilterEnt = HasSingleton<TurretCollFilterTag>() ? GetSingletonEntity<TurretCollFilterTag>() : default;
        var TurretPlatformEnt = HasSingleton<TurretPlatformCollFilTag>() ? GetSingletonEntity<TurretPlatformCollFilTag>() : default;

        var DestroyStuffCollFillEntity = HasSingleton<DestroyShelvesCollFilterTag>() ? GetSingletonEntity<DestroyShelvesCollFilterTag>() : default;
        var destroystuffrigid = world.GetRigidBodyIndex(DestroyStuffCollFillEntity);
        CollisionFilter DestroyStuffCollFilter = world.GetCollisionFilter(destroystuffrigid);

        Entity enemyent = new Entity();
        if (EnemyQuery.CalculateEntityCount() > 0)
        {

            var nativeents = EnemyQuery.ToEntityArray(Allocator.Temp);
            enemyent = nativeents[0];
        }

        Vector2 mousePosition = playInput.CursorPos;

        int ceIdx = world.GetRigidBodyIndex(EnvironCollFilter);

        CollisionFilter groundfilter = world.GetCollisionFilter(ceIdx);

        var enemycollfilterenty = world.GetRigidBodyIndex(enemyCollFilter); //new CollisionFilter { BelongsTo = 4, CollidesWith = 4};

        CollisionFilter enemycollfilter = world.GetCollisionFilter(enemycollfilterenty);

        var TurrCollFilterRigid = world.GetRigidBodyIndex(TurretCollFilterEnt);

        CollisionFilter TurretCollFilter = world.GetCollisionFilter(TurrCollFilterRigid);

        var TurretPlatformRigid = world.GetRigidBodyIndex(TurretPlatformEnt);

        CollisionFilter TurretPlatformCollFilter = world.GetCollisionFilter(TurretPlatformRigid);

        UnityEngine.Ray unityRay = Camera.main.ScreenPointToRay(mousePosition);

        var FoodItemsRayInput = new RaycastInput
        {
            Start = unityRay.origin,
            End = unityRay.origin + unityRay.direction * 100f,
            Filter = DestroyStuffCollFilter,
        };

        var EnvironRayInput = new RaycastInput
        {
            Start = unityRay.origin,
            End = unityRay.origin + unityRay.direction * 100f,
            Filter = groundfilter,
        };

        var TurretRayInput = new RaycastInput
        {
            Start = unityRay.origin,
            End = unityRay.origin + unityRay.direction * 100f,
            Filter = TurretCollFilter,//GlobalPhysicsFilters.CollFilterTurret,
        };

        var TurretPlatRayInpu = new RaycastInput
        {
            Start = unityRay.origin,
            End = unityRay.origin + unityRay.direction * 100f,
            Filter = TurretPlatformCollFilter,
        };

        var sphere = new SphereGeometry
        {
            Center = new float3(-8.45f, 9.65f, -0.10f),
            Radius = 0.98f
        };

        var CursorPosTrans = CursorPosQuery.GetSingletonEntity();

        var enemyphysmas = EnemyQuery.ToComponentDataArray<PhysicsMass>(Allocator.Temp);
        var enemyvelocity = EnemyQuery.ToComponentDataArray<PhysicsVelocity>(Allocator.Temp);
        var enemylocalto = EnemyQuery.ToComponentDataArray<LocalToWorld>(Allocator.Temp);

        float2 currentmovpos = new float2();

        currentmovpos = playInput.DirectionInput * CursorMoveSpeed * Time.DeltaTime;       

        if (math.length(playInput.DirectionInput) > 0.01)//This is the controller controls
        {
            float3 tempcurr = new float3(currentmovpos.x, 0, currentmovpos.y);
            CursorSquare.Value += tempcurr;
            CursorSquareQuery.SetSingleton<Translation>(CursorSquare);
            //currentmovpos = float2.zero;

        }

        if (playInput.ShowinGameButtons)
        {
            if (!IngameUIButtons.activeSelf)
            {
                IngameUIButtons.SetActive(true);
                SetSingleton<CameraMoveDat>(new CameraMoveDat { boolVal = false });
            }
            else
            {
                IngameUIButtons.SetActive(false);
                SetSingleton<CameraMoveDat>(new CameraMoveDat { boolVal = true });
            }
        }

        NativeList<Unity.Physics.RaycastHit> RaycastHits = new NativeList<Unity.Physics.RaycastHit>(Allocator.Temp);
        switch (currplayerMode.currMode)
        {
            case ControlModes.EnvCursorMode:                /
                //NativeList<Unity.Physics.RaycastHit> RaycastHits = new NativeList<Unity.Physics.RaycastHit>(Allocator.Temp);
                if (collisionWorld.CastRay(EnvironRayInput, ref RaycastHits))//TODO need to figure out if I can just have this return a single hit
                {
                    //TODO I could change this so it just makes the position equal to the max value but then I would have to do separate ifs to figure but it would make it slide up and down the walls better instead of being locked in position
                    //But this will do fine for now
                    if (RaycastHits[0].Position.x <= 0 || RaycastHits[0].Position.z <= 0 || RaycastHits[0].Position.x > 80 || RaycastHits[0].Position.z > 85)
                        return;

                    CursorSquareQuery.SetSingleton<Translation>(new Translation { Value = RaycastHits[0].Position });

                }

                break;
            case ControlModes.DestroyTowerMode://TODO I don't think I really need this

                break;
        }


        //var SlipHazTriggJob = new SlipHazTriggJob
        //{
        //    enemydudes = GetComponentDataFromEntity<EnemyDudeTag>(),
        //    slipHazard = GetComponentDataFromEntity<SlipHazardTag>(),
        //    enemstates = GetComponentDataFromEntity<EnemyStatesDat>(),
        //    rundomNumb = Rundomnumcreator[0]


        //};

        //Dependency = SlipHazTriggJob.Schedule(stepphysworld.Simulation, Dependency);


        NativeList<DistanceHit> thehits = new NativeList<DistanceHit>(Allocator.Temp);

        #region Main AbilityButton, The left click button abilities

        if (playInput.fireinput && (float)Time.ElapsedTime >= nextFireinput && currmouseMode.mousMode == MouseClickMode.LightningAbility)//I need to figure out this method so I don't get a blob error when there is no spawning enemies
        {
            //var timeLeft = nextJump - Time.time;
            nextFireinput = (float)Time.ElapsedTime + mainabilitytimer;
            nextLighttimer = (float)Time.ElapsedTime + LightBoltTimer;

            //NativeList<Unity.Physics.RaycastHit> RaycastHits = new NativeList<Unity.Physics.RaycastHit>(Allocator.Temp);

            if (enemyent == Entity.Null)
                return;

            int enemdix = world.GetRigidBodyIndex(enemyent);
            CollisionFilter enemyfilter = world.GetCollisionFilter(enemdix);//TODO THeres is a moment right at the beginning where there is a blob error from this sometimes

            LightningBoltGO.SetActive(true);
            
            if (collisionWorld.CastRay(EnvironRayInput, ref RaycastHits))
            {                
                SetComponent<Translation>(CursorPosTrans, new Translation { Value = RaycastHits[0].Position });

                var dude = collisionWorld.OverlapSphere(RaycastHits[0].Position, 2f, ref thehits, enemycollfilter);

                var theexplon = GameObject.Instantiate(DefaultExplosion);

                var explosionpos = RaycastHits[0].Position;
                explosionpos.y = 2;
                theexplon.transform.position = explosionpos;

                MainAbilityExpBool = true;

                explosionpos.y = 0;
                LightningBoltGO.transform.position = explosionpos;

                if (dude)//thehits.Length > 0)
                {
                    for (int i = 0; i < thehits.Length; i++)
                    {
                        var theent = thehits[i].Entity;

                        var itsmeworld = GetComponent<LocalToWorld>(theent);
                        var physmas = GetComponent<PhysicsMass>(theent);
                        var physveloc = GetComponent<PhysicsVelocity>(theent);

                        var farcevect = itsmeworld.Up * 3000 * timmydelt;

                        physveloc.ApplyLinearImpulse(physmas, farcevect);

                        SetComponent<DamageTaken>(theent, new DamageTaken { intVal = 100 });

                    }

                }
            }
        }

        if (playInput.fireinput)
        {
            ProfitsDat newprofsval = new ProfitsDat();
            switch (currmouseMode.mousMode)
            {
                case MouseClickMode.DestroyTurret:
                    if (collisionWorld.CastRay(TurretRayInput, ref RaycastHits))//TODO need to figure out if I can just have this return a single hit
                    {
                        CursorSquareQuery.SetSingleton<Translation>(new Translation { Value = RaycastHits[0].Position });
                        if (!RaycastHits.IsEmpty)
                        {
                            EntityManager.DestroyEntity(RaycastHits[0].Entity);
                        }
                    }

                    break;
                case MouseClickMode.PlaceTurret:

                    if (CurrentProfits.intVal < PricetoPlace)
                        return;

                    if (collisionWorld.CastRay(TurretPlatRayInpu, ref RaycastHits))
                    {
                        float3 thePlatformPos = new float3();
                        for (int i = 0; i < RaycastHits.Length; i++)
                        {
                            if (HasComponent<TurretTag>(RaycastHits[i].Entity))
                                return;

                            if(HasComponent<TurretPlaceTag>(RaycastHits[i].Entity))
                            {
                                thePlatformPos = GetComponent<Translation>(RaycastHits[i].Entity).Value;
                            }

                        }

                        var prefabdude = EntityManager.Instantiate(theprefabinator.TurretPrefab);
                       
                        SetComponent<Translation>(prefabdude, new Translation { Value = thePlatformPos });

                        newprofsval.intVal = CurrentProfits.intVal - PricetoPlace;
                        SetSingleton<ProfitsDat>(newprofsval);
                    }
                    break;
                case MouseClickMode.PlaceSlipHazard:
                    if (CurrentProfits.intVal < PricetoPlace)
                        return;

                    if (collisionWorld.CastRay(EnvironRayInput, ref RaycastHits))
                    {
                        if (RaycastHits.IsEmpty)
                            return;

                        var prefabdude = EntityManager.Instantiate(theprefabinator.SlipHazPrefab);
                        SetComponent<Translation>(prefabdude, new Translation { Value = RaycastHits[0].Position });
                        SetComponent<IsAlive>(prefabdude, new IsAlive { booVal = false });

                    }
                    
                    newprofsval.intVal = CurrentProfits.intVal - PricetoPlace;
                    SetSingleton<ProfitsDat>(newprofsval);

                    break;
                case MouseClickMode.DestroyStuff:
                    if (collisionWorld.CastRay(FoodItemsRayInput, ref RaycastHits))
                    {
                        if (RaycastHits.IsEmpty)
                            return;

                        for (int i = 0; i < RaycastHits.Length; i++)
                        {
                          
                            if (HasComponent<ShelfDestructionTag>(RaycastHits[i].Entity))
                            {
                                var currpos = GetComponent<Translation>(RaycastHits[i].Entity);
                                EntityManager.DestroyEntity(RaycastHits[i].Entity);
                                var current = EntityManager.Instantiate(theprefabinator.ShelfDestructPrefab);
                             
                                SetComponent<Translation>(current, new Translation { Value = currpos.Value });

                                var shelfitemsquery = GetEntityQuery(ComponentType.ReadOnly<ShelfItemsTag>(), ComponentType.ReadWrite<Translation>());
                                var theshelfitems = shelfitemsquery.ToEntityArray(Allocator.Temp);

                                for(int u = 0; u < theshelfitems.Length; u++)
                                {
                                    SetComponent<Translation>(theshelfitems[u], new Translation { Value = currpos.Value });
                                    //var thephysics = GetComponent<PhysicsBodyAuthoring>(theshelfitems[u]);
                                }

                                break;
                            }
                        }

                    }
                    break;

            }
        }

        if(playInput.EscapeButton)
        {
            //ExitScreenGO.SetActive(true);
            if (!IngameUIButtons.activeSelf)
            {
                ExitScreenGO.SetActive(true);
                IngameUIButtons.SetActive(true);
                SetSingleton<CameraMoveDat>(new CameraMoveDat { boolVal = false });
            }
            else
            {
                ExitScreenGO.SetActive(false);
                IngameUIButtons.SetActive(false);
                SetSingleton<CameraMoveDat>(new CameraMoveDat { boolVal = true });
            }
        }
      

        #endregion;


        if ((float)Time.ElapsedTime >= nextLighttimer)
        {
            nextLighttimer = (float)Time.ElapsedTime + LightBoltTimer;

            if (LightningBoltGO.activeSelf == true)
            {
                LightningBoltGO.SetActive(false);
            }
        }

        if (MainAbilityExpBool)
        {
            //MainAbilityExplosion();
        }

        //Rundomnumcreator.Dispose();
    }

    void DestroyStuffMeth()
    {
        IngameUIButtons.SetActive(false);
        var currplayerMode = PlayerCompQuery.GetSingletonEntity();
        SetComponent<MouseClickModDat>(currplayerMode, new MouseClickModDat { mousMode = MouseClickMode.DestroyStuff });


    }

    void DestroyTurretMeth()
    {
        var currplayerMode = PlayerCompQuery.GetSingletonEntity();
        SetComponent<ControlModesDat>(currplayerMode, new ControlModesDat { currMode = ControlModes.DestroyTowerMode });
        SetComponent<MouseClickModDat>(currplayerMode, new MouseClickModDat { mousMode = MouseClickMode.DestroyTurret });

        IngameUIButtons.SetActive(false);

    }

    void PlaceTurretMeth()
    {
        IngameUIButtons.SetActive(false);

        var currplaymode = PlayerCompQuery.GetSingletonEntity();
        SetComponent<MouseClickModDat>(currplaymode, new MouseClickModDat { mousMode = MouseClickMode.PlaceTurret });

    }

    void PlaceSlipHazard()
    {
        IngameUIButtons.SetActive(false);
        var currplaymode = PlayerCompQuery.GetSingletonEntity();
        SetComponent<MouseClickModDat>(currplaymode, new MouseClickModDat { mousMode = MouseClickMode.PlaceSlipHazard });

    }

    void CursorMovedMeth()
    {
        IngameUIButtons.SetActive(false);
        var currplaymode = PlayerCompQuery.GetSingletonEntity();
        SetComponent<MouseClickModDat>(currplaymode, new MouseClickModDat { mousMode = MouseClickMode.MoveAround });
        var contromde = PlayerCompQuery.GetSingleton<ControlModesDat>();
        contromde.currMode = ControlModes.EnvCursorMode;
        PlayerCompQuery.SetSingleton<ControlModesDat>(contromde);
        //var cameramovebool = GetSingleton<CameraMoveDat>();
        SetSingleton<CameraMoveDat>(new CameraMoveDat { boolVal = true });
    }

    void MainAbilityExplosion(float3 spherepos, float spheresize, CollisionFilter collfilt)
    {


    }   

    void ChaoshappensButtMeth()//TODO I need to make sure multiple presses of this button don't break things I think I've fixed it with the hassingleton checks but need to make sure throughout
    {
        var CurrentProfits = HasSingleton<ProfitsDat>() ? GetSingleton<ProfitsDat>() : default;

        if (CurrentProfits.intVal < ChaosButtonCost)
            return;

        if (!ChaosButtonEnable)
            return;

        ChaosButtonEnable = false;

        ProfitsDat newprofts = new ProfitsDat();

        newprofts.intVal = CurrentProfits.intVal - ChaosButtonCost;
        SetSingleton<ProfitsDat>(newprofts);

        IngameUIButtons.SetActive(false);
        if(!HasSingleton<ChaosHappensEvent>())        
            EntityManager.CreateEntity(typeof(ChaosHappensEvent));//TODO I probably need to have single entity that stays existing until the entire event has completed
        

    }

    void YesExitButton()
    {
        Application.Quit();
    }

    void NoExitButton()
    {
        ExitScreenGO.SetActive(false);
        IngameUIButtons.SetActive(false);
        SetSingleton<CameraMoveDat>(new CameraMoveDat { boolVal = true });
    }


}

public partial struct ThrowObjectsEverywhere : IJobEntity
{

    public void Execute(Entity enty)//TODO I should make this object agnostic
    {


    }
}