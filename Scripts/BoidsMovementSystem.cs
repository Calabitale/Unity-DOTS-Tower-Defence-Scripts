using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using Unity.Transforms;
using Unity.Mathematics;
//using Drawing;



[UpdateAfter(typeof(PlayerInputStuffSystem))]
public partial class CreateObstacleDudes : SystemBase
{    
    public PlayerInput tempinput;   

    protected override void OnCreate()
    {
        base.OnCreate();
        Enabled = false;
    }

    protected override void OnStartRunning()
    {
        //Prefabinator = HasSingleton<DotPrefabinator>() ? GetSingleton<DotPrefabinator>() : default;
        //var Rundomnumcreator = new NativeArray<Unity.Mathematics.Random>(JobsUtility.MaxJobThreadCount, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
        //var r = (uint)UnityEngine.Random.Range(int.MinValue, int.MaxValue);
        //for (int i = 0; i < JobsUtility.MaxJobThreadCount; i++)
        //{
        //    Rundomnumcreator[i] = new Unity.Mathematics.Random(r == 0 ? r + 1 : r);
        //}
    }

    protected override void OnUpdate()
    {        
        tempinput = HasSingleton<PlayerInput>() ? GetSingleton<PlayerInput>() : default;        
        
        if (tempinput.fireinput)
        {
            if(HasSingleton<StartSpawningSystemEvent>())
            {
                var destroyedent = GetSingletonEntity<StartSpawningSystemEvent>();
                EntityManager.DestroyEntity(destroyedent);
            }
            else
            {
                EntityManager.CreateEntity(typeof(StartSpawningSystemEvent));
            }
            
        }  
        //if (!HasSingleton<CalcintegrationFieldEvent>())
        //    {
        //        EntityManager.CreateEntity(typeof(CalcintegrationFieldEvent));
        //    }


        //    NativeArray<Entity> dudees = new NativeArray<Entity>(500, Allocator.Temp);
        //    EntityManager.Instantiate(tempobject, dudees);
        //    EntityManager.AddComponent<TestMoveojbectTag>(dudees);
        //    EntityManager.AddComponent<CellBDLayer>(dudees);
        //    for (int i = 0; i < 500; i++)
        //    {
        //        int xrandnumpos = UnityEngine.Random.Range(0, 5);
        //        int zrandnumpos = UnityEngine.Random.Range(0, 5);

        //        //var tudue = EntityManager.Instantiate(tempobject);
        //        //EntityManager.AddComponent<TestMoveojbectTag>(dudees);

        //        float3 thecreatepos = new float3(xrandnumpos, 0, zrandnumpos);

        //        EntityManager.SetComponentData<Translation>(dudees[i], new Translation { Value = thecreatepos });
        //        EntityManager.SetComponentData<CellBDLayer>(dudees[i], new CellBDLayer { intVal = 1 });

            
    }
}


public partial class PerformanceTestSystem : SystemBase
{

    public DotPrefabinator Prefabinator;
    
    public int NumbofEntitys;
    

    protected override void OnCreate()
    {
        base.OnCreate();
        NumbofEntitys = 100;

        Enabled = false;
    }



    protected override void OnStartRunning()
    {
        //var query0 = new EntityQueryDesc
        //{
        //    All = new ComponentType[] { typeof(StartSpawningSystemEvent) }
        //};

        //var query1 = new EntityQueryDesc
        //{
        //    All = new ComponentType[] { typeof(PlayerInput) }
        //};

        //EntityQuery m_Query = GetEntityQuery(new EntityQueryDesc[] { query0, query1 });

        Prefabinator = HasSingleton<DotPrefabinator>() ? GetSingleton<DotPrefabinator>() : default;
        RequireSingletonForUpdate<StartSpawningSystemEvent>();
        //RequireSingletonForUpdate(m_Query);


        //RequireForUpdate(GetEntityQuery(query0, query1));

    }

    protected override void OnUpdate()
    {
        
        var tempinput = HasSingleton<PlayerInput>() ? GetSingleton<PlayerInput>() : default;
        NativeArray<Entity> tempunts = new NativeArray<Entity>(NumbofEntitys, Allocator.Temp);
        //EntityManager.Instantiate(Prefabinator.TestObjectPrefab, tempunts);
        

        if (HasSingleton<TimeTickEvent>())        
        {
            EntityManager.Instantiate(Prefabinator.TestObjectPrefab, tempunts);
            EntityManager.AddComponent<MoveTargetChoice>(tempunts);

            for (int i = 0; i < NumbofEntitys; i++)
            {

                var tempra1 = new float(); //UnityEngine.Random.Range(-20, 21);
                var tempra2 = new float();

                //tempra1 = UnityEngine.Random.Range(0, 80);
                //tempra2 = UnityEngine.Random.Range(0, 80);
                //var randdirect = UnityEngine.Random.Range(0, 2);
                if (i >= NumbofEntitys / 2)
                {

                    tempra2 = -40;
                    tempra1 = (i - (NumbofEntitys / 2)) * 2f;
                    //tempra1 = (NumbofEntitys / 2) i * 1.5f;
                    EntityManager.SetComponentData<MoveTargetChoice>(tempunts[i], new MoveTargetChoice { intVal = 0 });

                }
                else
                {
                    tempra1 = i * 2f;
                    tempra2 = 40;
                    EntityManager.SetComponentData<MoveTargetChoice>(tempunts[i], new MoveTargetChoice { intVal = 1 });
                }

                float3 thecreatepos = new float3(tempra1, 0, tempra2);
                EntityManager.SetComponentData<Translation>(tempunts[i], new Translation { Value = thecreatepos });
            }
        }



    }
}

public partial class ObstacleAvoidanceSystem : SystemBase//TODO This is a simple system that avoids each other somewhat but doesn't work in certain ways like when a bunch of entities are going the same direction and cramped up against one another as they 
{
    public EntityQuery TestObjectQuery;

    protected override void OnCreate()
    {
        base.OnCreate();
        TestObjectQuery = GetEntityQuery(typeof(Translation), ComponentType.ReadOnly<TestEntAuthorTag>());
        Enabled = false;
    }

    protected override void OnUpdate()
    {
        var targ1 = new float3(35, 0, 70);
        var targ2 = new float3(35, 0, -70);
        var Timmydelta = Time.DeltaTime;

        var TranslutionEnts = TestObjectQuery.ToComponentDataArray<Translation>(Allocator.TempJob);

        Entities.WithName("SimpleAvoidanceJob").WithReadOnly(TranslutionEnts).WithDisposeOnCompletion(TranslutionEnts).WithAll<TestEntAuthorTag>().ForEach((ref Translation currPos, ref Rotation rotaval, in MoveTargetChoice gohere) =>
        {            
            var tempdirect = math.select(targ2, targ1, gohere.intVal == 0);

            var avoidirection = math.normalize(tempdirect - currPos.Value);

            int total = 0;
            for (int i = 0; i < TranslutionEnts.Length; i++)
            {

                float currentDistance = 1.5f; //Must be 1.5f or as close to for some reason othewise it does not work 1f also seems a pretty good value

                float3 currentLocationToCheck = TranslutionEnts[i].Value;

                var temdistval = math.distance(currPos.Value, currentLocationToCheck);

                if (!currPos.Value.Equals(currentLocationToCheck))//I think this rules out it mistaking itself and trying to avoid colliding with itself and getting confused
                {
                    if (currentDistance > temdistval)
                    {
                        //Debug.Log("The mathsqur" + )

                        currentDistance = math.distance(currPos.Value, currentLocationToCheck);
                        float3 distanceFromTo = currPos.Value - currentLocationToCheck;
                        avoidirection = math.normalize(distanceFromTo / currentDistance);

                        //Debug.Log($"This is after{tempavoidbefore}::::{avoidirection}");
                        total++;
                    }
                }
            }


            avoidirection = math.select(avoidirection, avoidirection / total, total > 0);         

            var waypodirection = math.normalize(tempdirect - currPos.Value);
            waypodirection = waypodirection + avoidirection;

            currPos.Value += waypodirection * 5 * Timmydelta;
            rotaval.Value = math.slerp(rotaval.Value, quaternion.LookRotation(waypodirection, math.up()), Timmydelta * 5);

        }).ScheduleParallel();

      


    }
}

public partial class BoidsSystem : SystemBase
{
    public EntityQuery TestObjectQuery;
    public float Entspeed;
    public float Distoclose;

    public EntityQuery TargetsQuery;

    public EntityQuery CelldataQuery;

    public float Timertoadd;

    public bool changetarget;

    public float timelapsed;
    protected override void OnCreate()
    {
        base.OnCreate();
        CelldataQuery = GetEntityQuery(ComponentType.ReadOnly<CellsBestDirection>());
        TestObjectQuery = GetEntityQuery(typeof(Translation), typeof(LocalToWorld), ComponentType.ReadOnly<EnemyDudeTag>());
        TargetsQuery = GetEntityQuery(ComponentType.ReadOnly<TargetAuthoring>(), ComponentType.ReadOnly<Translation>());
        Entspeed = 10f;
        Distoclose = 1.5f;
        Timertoadd = 20f; //Number of seconds in timer
        changetarget = true;
        timelapsed = 0f;

        RequireSingletonForUpdate<CellsBestDirection>();
        RequireSingletonForUpdate<PauseSystemDat>();

        //Enabled = false;
    }
    
    protected override void OnUpdate()
    {
        var targ1 = new float3(35, 0, 70);
        var targ2 = new float3(35, 0, -70);
        var Timmydelta = Time.DeltaTime;

        float separationDistance = 2f;
        var localEntspeed = Entspeed;
        var cohesionbias = 5f;
        var alignmentbias = 2.5f;
        var separatebias = 25f;

        var step = 1.25f;

        //var builder = DrawingManager.GetBuilder(true);

        //builder.Preallocate(flowfielddudat.gridSize.x * flowfielddudat.gridSize.y);


        //var staydistance = new float();
        //staydistance = 2.0f;

        //var currspeed = Entspeed;
        //var toclosedist = Distoclose;
        

        FlowFieldData tempflowfield = HasSingleton<FlowFieldData>() ?  GetSingleton<FlowFieldData>() : default;

        //var targets = TargetsQuery.ToComponentDataArray<Translation>(Allocator.TempJob);

        var TranslutionEnts = TestObjectQuery.ToComponentDataArray<Translation>(Allocator.TempJob);
        //var Localtowoldents = TestObjectQuery.ToComponentDataArray<LocalToWorld>(Allocator.TempJob);
        var BestDirectionCells = CelldataQuery.ToComponentDataArray<CellsBestDirection>(Allocator.TempJob);

        var cellindexworldpos = new GetCellIndexFromWorldPos();

        var tempflatus = new ToFlatIndex();


        Entities.
            WithReadOnly(BestDirectionCells).
            WithReadOnly(TranslutionEnts).
            WithDisposeOnCompletion(TranslutionEnts).
            WithAll<EnemyDudeTag>().
            ForEach((ref Translation currPos, ref LocalToWorld myworld, ref Rotation rotor, ref EnemyStatesDat currState, ref IsAlive Imalive) =>
        {
            if (Imalive.booVal == false || currState.EnemyState == EnemyStates.leavingentrance)
                return;

            //if (currState.EnemyState == EnemyStates.leavingentrance)
                //return;

            var prevpos = currPos.Value;

            var currcellpos = cellindexworldpos.Execute(currPos.Value, tempflowfield.gridSize, tempflowfield.cellRadius * 2);

            var tempgridpos = tempflatus.Execute(currcellpos, tempflowfield.gridSize.y);

            var tempcurrentdirection = BestDirectionCells[tempgridpos];

            var targetDirection = new float3(tempcurrentdirection.bestDirection.x, 0, tempcurrentdirection.bestDirection.y);

            int total = 0;

            float3 separation = float3.zero;
            float3 alignment = float3.zero;
            float3 coheshion = float3.zero;

            float3 acceleration = float3.zero;

            FixedList64Bytes<float3> fookedlist = new FixedList64Bytes<float3>();//TODO The max number this can currently store is 4

            float3 thisvelocity = myworld.Forward * localEntspeed;

            for (int i = 0; i < TranslutionEnts.Length; i++)
            {
                var otherentscurrpos = TranslutionEnts[i].Value;
                if (!currPos.Value.Equals(otherentscurrpos))
                {
                    float distToOtherBoid = math.distance(currPos.Value, otherentscurrpos);
                    if (distToOtherBoid < separationDistance)
                    {
                        //separation += -(otherentscurrpos - currPos.Value) * (1f / math.max(distToOtherBoid, .0001f));
                        float3 difference = currPos.Value - otherentscurrpos;
                        math.normalize(difference);
                        difference /= distToOtherBoid;
                        separation += difference;
                        coheshion += otherentscurrpos;
                        //positionSum += otherLocalToWorld.Position;
                        //separation += (distanceFromTo / math.distance(trans.Value, neighbour.currentPosition));
                        //steer.linear += other.transform.position;
                        if (total < 5)
                        {
                            fookedlist.Add(otherentscurrpos);                            
                        }
                        total++;
                    }
                }
            }
        
            if (total > 0)        
            {
            
                acceleration += separation; //TODO Need to figure if I need this in here or not
                coheshion /= total;
                coheshion = coheshion - currPos.Value;
            }

            targetDirection = targetDirection + acceleration + coheshion;
            targetDirection.y = 0;

            currPos.Value += targetDirection * localEntspeed * Timmydelta;
            foreach (float3 sully in fookedlist)
            {
                var checkdist = math.distance(currPos.Value, sully);//TODO I need a view cone
                if (checkdist < 0.5f)
                {
                    currPos.Value = prevpos;                    
                }

            }

            if (currState.EnemyState == EnemyStates.slipping)
            {
                //float3 falldirection = new float3()
                rotor.Value = quaternion.LookRotation(targetDirection, math.left());
            }
            else if(currState.EnemyState == EnemyStates.leavingentrance)
            {
                //Imalive.booVal = false;
            }

            rotor.Value = quaternion.LookRotation(targetDirection, math.up());

            //rotor.Value = math.slerp(rotor.Value, quaternion.LookRotation(targetDirection, math.up()), Timmydelta * 5);

        }).Schedule();

        //targets.Dispose();

        //BestDirectionCells.Dispose();
        #region just old code that stopped the entity if it came too close to another entity
        //int count = 0;
        //float3 diff = new float3();

        //var currycurrspeed = currspeed;

        //var tempdirect = math.select(targ2, targ1, gohere.intVal == 0);
        //var avoidirection = math.normalize(tempdirect - currPos.Value);

        //for (int i = 0; i < TranslutionEnts.Length; i++)
        //{
        //    float distbetween = math.distance(currPos.Value, TranslutionEnts[i].Value);
        //    if (!currPos.Value.Equals(TranslutionEnts[i].Value))

        //        if (distbetween < toclosedist)
        //        {



        //            currycurrspeed = 0;

        //        }

        //}


        //currPos.Value += avoidirection * currycurrspeed * Timmydelta;

        #endregion

        this.CompleteDependency();
        BestDirectionCells.Dispose();
        //builder.Dispose();
    }
}

#region Latest Dudes code
//if (cellVsEntityPositionsForJob.TryGetFirstValue(key, out neighbour, out nmhKeyIterator))
//{
//    do
//    {
//        if (!trans.Value.Equals(neighbour.currentPosition) && math.distance(trans.Value, neighbour.currentPosition) < bc.perceptionRadius)
//        {
//            float3 distanceFromTo = trans.Value - neighbour.currentPosition;
//            separation += (distanceFromTo / math.distance(trans.Value, neighbour.currentPosition));
//            coheshion += neighbour.currentPosition;
//            alignment += neighbour.velocity;
//            total++;
//        }
//    } while (cellVsEntityPositionsForJob.TryGetNextValue(out neighbour, ref nmhKeyIterator));
//    if (total > 0)
//    {
//        coheshion = coheshion / total;
//        coheshion = coheshion - (trans.Value + bc.velocity);
//        coheshion = math.normalize(coheshion) * bc.cohesionBias;

//        separation = separation / total;
//        separation = separation - bc.velocity;
//        separation = math.normalize(separation) * bc.separationBias;

//        alignment = alignment / total;
//        alignment = alignment - bc.velocity;
//        alignment = math.normalize(alignment) * bc.alignmentBias;

//    }

//    bc.acceleration += (coheshion + alignment + separation);
//    rot.Value = math.slerp(rot.Value, quaternion.LookRotation(math.normalize(bc.velocity), math.up()), deltaTime * 10);
//    bc.velocity = bc.velocity + bc.acceleration;
//    bc.velocity = math.normalize(bc.velocity) * bc.speed;
//    trans.Value = math.lerp(trans.Value, (trans.Value + bc.velocity), deltaTime * bc.step);
//    bc.acceleration = math.normalize(bc.target - trans.Value) * bc.targetBias;
//}
#endregion

//var currentarg = TargetQuery.ToComponentDataArray<Translation>(Allocator.TempJob);

//// This runs only if there exists a BoidControllerECS instance.
//if (!controller)
//{
//    controller = BoidControllerECS.Instance;
//}
//if (controller)
//{
//    EntityQuery boidQuery = GetEntityQuery(ComponentType.ReadOnly<BoidECS>(), ComponentType.ReadOnly<LocalToWorld>());
//    NativeArray<float4x4> newBoidPositions = new NativeArray<float4x4>(boidQuery.CalculateEntityCount(), Allocator.Temp);

//    int boidIndex = 0;
//    Entities.WithAll<BoidECS>().ForEach((Entity boid, ref LocalToWorld localToWorld) => {
//        float3 boidPosition = localToWorld.Position;

//        float3 seperationSum = float3.zero;
//        float3 positionSum = float3.zero;
//        float3 headingSum = float3.zero;

//        int boidsNearby = 0;

//        Entities.WithAll<BoidECS>().ForEach((Entity otherBoid, ref LocalToWorld otherLocalToWorld) => {
//            if (boid != otherBoid)
//            {

//                float distToOtherBoid = math.length(boidPosition - otherLocalToWorld.Position);
//                if (distToOtherBoid < controller.boidPerceptionRadius)
//                {

//                    seperationSum += -(otherLocalToWorld.Position - boidPosition) * (1f / math.max(distToOtherBoid, .0001f));
//                    positionSum += otherLocalToWorld.Position;
//                    headingSum += otherLocalToWorld.Forward;

//                    boidsNearby++;
//                }
//            }
//        });

//        float3 force = float3.zero;

//        if (boidsNearby > 0)
//        {
//            force += (seperationSum / boidsNearby) * controller.separationWeight;
//            force += ((positionSum / boidsNearby) - boidPosition) * controller.cohesionWeight;
//            force += (headingSum / boidsNearby) * controller.alignmentWeight;
//        }
//        if (math.min(math.min(
//            (controller.cageSize / 2f) - math.abs(boidPosition.x),
//            (controller.cageSize / 2f) - math.abs(boidPosition.y)),
//            (controller.cageSize / 2f) - math.abs(boidPosition.z))
//                < controller.avoidWallsTurnDist)
//        {
//            force += -math.normalize(boidPosition) * controller.avoidWallsWeight;
//        }

//        float3 velocity = localToWorld.Forward * controller.boidSpeed;
//        velocity += force * Time.DeltaTime;
//        velocity = math.normalize(velocity) * controller.boidSpeed;

//        newBoidPositions[boidIndex] = float4x4.TRS(
//            localToWorld.Position + velocity * Time.DeltaTime,
//            quaternion.LookRotationSafe(velocity, localToWorld.Up),
//            new float3(1f)
//        );
//        boidIndex++;
//    });

//    boidIndex = 0;
//    Entities.WithAll<BoidECS>().ForEach((Entity boid, ref LocalToWorld localToWorld) => {
//        localToWorld.Value = newBoidPositions[boidIndex];
//        boidIndex++;
//    });
//}

public struct MoveTargetChoice : IComponentData
{
    public int intVal;
}