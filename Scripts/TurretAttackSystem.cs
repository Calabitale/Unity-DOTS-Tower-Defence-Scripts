using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Burst;
using Unity.Collections;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Physics.Extensions;
using Unity.Rendering;
using Drawing;

[DisableAutoCreation]
//[AlwaysUpdateSystem]
public partial class TurretAttackSystem : SystemBase
{
  
    EndSimulationEntityCommandBufferSystem endecbSystem;
    StepPhysicsWorld stepphysworld;

    EntityQuery TurretQuery;
    EntityQuery EnemyQuery;
    EntityQuery URPMaterialQuery;

    EndSimulationEntityCommandBufferSystem endsimBuffer;

    float firetimer;

    float HitFlashTimer;

    //float timeRemaining = 2;
    bool timerIsRunning = true;

    BuildPhysicsWorld m_BuildPhysicsWorld;

    GameObject GameManagerStuff;

    GameObjectRefs GameobjecREFS;

    GameObject BloodHitEffects;

    float nextTurretFire;

    //GlobalPhysicsFilters enemyFilter;

    float HitTime;

    //public CommandBuilder builder;


    protected override void OnCreate()
    {
        endecbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        stepphysworld = World.GetOrCreateSystem<StepPhysicsWorld>();

        TurretQuery = GetEntityQuery(ComponentType.ReadOnly<TurretTag>(), ComponentType.ReadOnly<Translation>(), typeof(TurretAttackRate), typeof(TurretTarget));
        EnemyQuery = GetEntityQuery(ComponentType.ReadOnly<EnemyDudeTag>(), ComponentType.ReadOnly<Translation>(), typeof(FlashHitTimer));

        URPMaterialQuery = GetEntityQuery(typeof(URPMaterialPropertyBaseColor), typeof(FlashHitTimer));

        m_BuildPhysicsWorld = World.GetExistingSystem<BuildPhysicsWorld>();

        GameManagerStuff = GameObject.Find("GameManagerStuff");

        endsimBuffer = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>(); //EndSimulationEntityCommandBufferSystem(Allocator.TempJob);

        //GameobjecREFS = GameManagerStuff.GetComponent<GameObjectRefs>();

        //BloodHitEffects = GameobjecREFS.BloodHitEffect;
        //Enabled = false;

        firetimer = 0.3f;

        HitFlashTimer = 0.1f;

        nextTurretFire = 1f;

        RequireSingletonForUpdate<PauseSystemDat>();

        //Enabled = false;
    }

    protected override void OnStartRunning()
    {
        

        GameobjecREFS = GameManagerStuff.GetComponent<GameObjectRefs>();
    }

    protected override void OnUpdate()
    {
        var ecb = endsimBuffer.CreateCommandBuffer();        

        var currTurret = TurretQuery.ToEntityArray(Allocator.Temp);//TurretQuery.ToComponentDataArray<TurretAttackRate>(Allocator.Temp);
        var firerate = TurretQuery.ToComponentDataArray<TurretAttackRate>(Allocator.Temp);

        var TurretTranslates = TurretQuery.ToComponentDataArray<Translation>(Allocator.Temp);
        var EnemyTrans = EnemyQuery.ToComponentDataArray<Translation>(Allocator.Temp);
        var TurretTargEnt = TurretQuery.ToComponentDataArray<TurretTarget>(Allocator.Temp);


        var currEnemyEntity = EnemyQuery.ToEntityArray(Allocator.Temp);
        var currEnenmyAnim = EnemyQuery.ToComponentDataArray<FlashHitTimer>(Allocator.Temp);

        var URPMatEntities = URPMaterialQuery.ToEntityArray(Allocator.Temp);
        var URPMatFlashTime = URPMaterialQuery.ToComponentDataArray<FlashHitTimer>(Allocator.Temp);

        CollisionWorld collisionWorld = m_BuildPhysicsWorld.PhysicsWorld.CollisionWorld;

        PhysicsWorld world = m_BuildPhysicsWorld.PhysicsWorld;

        var enemyCollFilter = HasSingleton<EnemyCollFilterTag>() ? GetSingletonEntity<EnemyCollFilterTag>() : default;

        var enemycollfilterenty = world.GetRigidBodyIndex(enemyCollFilter); //new CollisionFilter { BelongsTo = 4, CollidesWith = 4};        

        CollisionFilter enemycollfilter = world.GetCollisionFilter(enemycollfilterenty);

        var jobFlashTimeMAX = HitFlashTimer;

        //var builder = DrawingManager.GetBuilder(true);

        //var builder = DrawingManager.GetBuilder(true);

        //builder.Preallocate();
       
        //if ((float)Time.ElapsedTime >= nextLighttimer)
        //{
        //    //nextLighttimer = (float)Time.ElapsedTime + LightBoltTimer;

        //    if (LightningBoltGO.activeSelf == true)
        //    {
        //        LightningBoltGO.SetActive(false);
        //    }
        //}
        var turrTimerMax = firetimer;
        //var timeRemaining = Time.DeltaTime;
        //(float)Time.ElapsedTime >= nextFireinput
        //timeRemaining -= 

        for (int i = 0; i < firerate.Length; i++)
        {
            var currval = firerate[i];//GetComponent<TurretAttackRate>(currTurret[i]);//TODO I need to iterate through all the turrets and do this for each of them            
            var otherval = currval.fltVal -= Time.DeltaTime;            
            SetComponent<TurretAttackRate>(currTurret[i], new TurretAttackRate { fltVal = otherval });
        }

        for (int i = 0; i < URPMatEntities.Length; i++)
        {
            if (URPMatFlashTime[i].fltVal > 0)
            {
                var currEnemva = URPMatFlashTime[i];
                var newval = currEnemva.fltVal -= Time.DeltaTime;
                SetComponent<FlashHitTimer>(URPMatEntities[i], new FlashHitTimer { fltVal = newval });            

                continue;
            }

            EntityManager.RemoveComponent<URPMaterialPropertyBaseColor>(URPMatEntities);

        }

        for (int p = 0; p < TurretTargEnt.Length; p++)
        {

            //Debug.Log("Is it going into here");
            if (TurretTargEnt[p].entVal == Entity.Null)
                continue;
            using (Draw.WithColor(Color.red))
            {
                //Draw.Line(TurretTranslates[p].Value, HasComponent<Translation>(TurretTargEnt[p].entVal) ? GetComponent<Translation>(TurretTargEnt[p].entVal).Value : default);
                Draw.ingame.Line(TurretTranslates[p].Value, HasComponent<Translation>(TurretTargEnt[p].entVal) ? GetComponent<Translation>(TurretTargEnt[p].entVal).Value : default);//TODO This should really be changed to a particle system or shader or whatever
            }
            //Draw.Line(new float3(0, 0, 0), TurretTranslates[p].Value);
            //Debug.Log("It sometimes gos here");
        }

        //Draw.Line(new float3(0, 0, 0), new float3(50, 3, 50));
        //builder.Dispose();

        var ENemyPHysfilter = GlobalPhysicsFilters.CollFilterAttackEnemy;

        NativeList<Entity> Hitentitys = new NativeList<Entity>(Allocator.Temp);

        //Debug.Log("Inside the job");

        Entities.WithAll<TurretTag>().
            WithStructuralChanges().
            WithName("Jobthatlocksonandattacks").
            ForEach((ref TurretTarget entTarget, ref TurretAttackRate timetoattack, ref Rotation currrotate, in Translation turrpos, in DamageGiven damamount) =>
            {  
                if (timetoattack.fltVal > 0)
                    return;             

                if (entTarget.entVal != Entity.Null)
                {
                    var currhealth = GetComponent<CurrHealth>(entTarget.entVal);
                    if (currhealth.intVal > 0)
                    {
                        SetComponent<DamageTaken>(entTarget.entVal, new DamageTaken { intVal = damamount.intVal });
                        timetoattack.fltVal = turrTimerMax;

                        var inboofer = GetBuffer<Child>(entTarget.entVal);
                        var inotherboofer = GetBuffer<Child>(inboofer[0].Value);
                        Entity inchild1 = inotherboofer[0].Value;
                        Entity inchild2 = inotherboofer[1].Value;

                        ecb.AddComponent(inchild1, new URPMaterialPropertyBaseColor { Value = new float4(25, 255, 0, 255) });
                        ecb.AddComponent(inchild2, new URPMaterialPropertyBaseColor { Value = new float4(25, 255, 0, 255) });

                        ecb.AddComponent(inchild1, new FlashHitTimer { fltVal = jobFlashTimeMAX });
                        ecb.AddComponent(inchild2, new FlashHitTimer { fltVal = jobFlashTimeMAX });                        
                        return;
                    }

                    entTarget.entVal = Entity.Null;
                    timetoattack.fltVal = turrTimerMax;
                    return;

                }


                NativeList<DistanceHit> thehits = new NativeList<DistanceHit>(Allocator.Temp);
                float spheredist = 10f;
                float prevdist = spheredist;
                Entity CurrEntity = Entity.Null;

                var isCollision = collisionWorld.OverlapSphere(turrpos.Value, spheredist, ref thehits, enemycollfilter);               

                if (!isCollision)
                    return;

                //Debug.Log("Inside the job");

                for (int i = 0; i < thehits.Length; i++)
                {
                    var currdist = thehits[i].Distance;
                    if (currdist < prevdist)
                    {
                        prevdist = currdist;
                        CurrEntity = thehits[i].Entity;//I may change this to an int later if its too slow
                    }

                }

                if (CurrEntity == Entity.Null)
                    return;
               
                Hitentitys.Add(CurrEntity);

                var boofer = GetBuffer<Child>(CurrEntity);
                var otherboofer = GetBuffer<Child>(boofer[0].Value);
                Entity child1 = otherboofer[0].Value;
                Entity child2 = otherboofer[1].Value;
              
                ecb.AddComponent(child1, new URPMaterialPropertyBaseColor { Value = new float4(25, 255, 0, 255) });
                ecb.AddComponent(child2, new URPMaterialPropertyBaseColor { Value = new float4(25, 255, 0, 255) });

                ecb.AddComponent(child1, new FlashHitTimer { fltVal = jobFlashTimeMAX });
                ecb.AddComponent(child2, new FlashHitTimer { fltVal = jobFlashTimeMAX });

                SetComponent<DamageTaken>(CurrEntity, new DamageTaken { intVal = damamount.intVal });
                //TODO I may need to get rid of this SetComponent as it may slow it down a lot

                timetoattack.fltVal = turrTimerMax;                                         

                entTarget.entVal = CurrEntity;

                var lookdir = math.normalize(turrpos.Value - GetComponent<Translation>(entTarget.entVal).Value);

                
                currrotate.Value = quaternion.LookRotation(lookdir, math.up());
                //currRotat.Value = Quaternion.Slerp(currRotat.Value, lookrotate, timDelta * 3);
                //currrotate.Value = Quaternion.Slerp(currrotate.Value, newrotate, Time.DeltaTime * 4);


            }).Run();

        //for (int i = 0; i < Hitentitys.Length; i++)
        //{
        //    //SystemAPI.HasBuffer

        //    var boofer = GetBuffer<Child>(Hitentitys[i]);
        //    var otherboofer = GetBuffer<Child>(boofer[0].Value);
        //    Entity child1 = otherboofer[0].Value;//TODO I should probably do this with a list
        //    Entity child2 = otherboofer[1].Value;

        //    //Debug.Log("How many children " + boofer.Length);
        //    //for (int j = 0; j < otherboofer.Length; j++)
        //    {
        //       // boofer = GetBuffer<Child>(Hitentitys[i]);
        //        //otherboofer = GetBuffer<Child>(boofer[0].Value);
        //        EntityManager.AddComponentData(child1, new URPMaterialPropertyBaseColor { Value = new float4(25, 255, 0, 255) });
        //        EntityManager.AddComponentData(child2, new URPMaterialPropertyBaseColor { Value = new float4(25, 255, 0, 255) });

        //    }

        //    EntityManager.AddComponentData<FlashHitTimer>(child1, new FlashHitTimer { fltVal = HitFlashTimer });
        //    EntityManager.AddComponentData<FlashHitTimer>(child2, new FlashHitTimer { fltVal = HitFlashTimer });
          

        //}

        //builder.Dispose();

        TurretTranslates.Dispose();

        EnemyTrans.Dispose();

        TurretTargEnt.Dispose();

        currEnemyEntity.Dispose();

        currEnenmyAnim.Dispose();

        URPMatEntities.Dispose();
        URPMatFlashTime.Dispose();

        //if((float)Time.ElapsedTime >= HitFlashTimer)//TODO Need to just countdown until it reaches
        //{
        //    HitFlashTimer = (float)Time.ElapsedTime + HitFlashTimer;

        //    Debug.Log("Something happened");
        //}
        Hitentitys.Dispose();

        endsimBuffer.AddJobHandleForProducer(this.Dependency);
        //if()
        //CompleteDependency();
        //collisionWorld.OverlapSphere(RaycastHits[0].Position, 2f, ref thehits, enemycollfilter);


        //var triggerjob = new TriggerJob
        //{
        //    enemydudes = GetComponentDataFromEntity<EnemyDudeTag>(),
        //    turretdude = GetComponentDataFromEntity<TurretTag>(),
        //    firerate = GetComponentDataFromEntity<TurretAttackRate>(),
        //    Transpos = GetComponentDataFromEntity<Translation>(true),           
        //    ecb = endecbSystem.CreateCommandBuffer(),
        //    currTurret = currTurret[0]

        //};

        //stepphysworld.Simulation

        //Dependency = triggerjob.Schedule(stepphysworld.Simulation, Dependency);
        //endecbSystem.AddJobHandleForProducer(Dependency);


        //Entities.ForEach((in FlowFieldData flooydata) =>
        //{


        //}).Schedule();


    }

    [BurstCompile]
    struct TriggerJob : ITriggerEventsJob
    {

        public ComponentDataFromEntity<TurretTag> turretdude;
        public ComponentDataFromEntity<EnemyDudeTag> enemydudes;
        public EntityCommandBuffer ecb;
        public ComponentDataFromEntity<TurretAttackRate> firerate;
        public Entity currTurret;
        [ReadOnly]
        public ComponentDataFromEntity<Translation> Transpos;
        

        public void Execute(TriggerEvent triggevent)
        {            
            
            Entity itsame = triggevent.EntityA;
            Entity itsayou = triggevent.EntityB;

            if (enemydudes.HasComponent(itsame) && enemydudes.HasComponent(itsayou)) return;
           

            if(enemydudes.HasComponent(itsame) && turretdude.HasComponent(itsayou))
            {
                if (firerate[currTurret].fltVal <= 0)
                {                   

                    var fireturr = firerate[currTurret];
                    fireturr.fltVal = 2f;
                    firerate[currTurret] = fireturr;
                    
                }
            }
            else if(enemydudes.HasComponent(itsayou) && turretdude.HasComponent(itsame))
            {
                if (firerate[currTurret].fltVal <= 0)
                {
                    //ecb.AddComponent<IDIED>(itsayou);
                    //ecb.DestroyEntity(itsayou);
                    var fireturr = firerate[currTurret];
                    fireturr.fltVal = 2f;
                    firerate[currTurret] = fireturr;
                }
            }

        }
    }
    
}
