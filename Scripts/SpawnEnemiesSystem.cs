using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Jobs.LowLevel.Unsafe;
using Unity.Transforms;


[DisableAutoCreation]
[UpdateAfter(typeof(TimeTickSystem))]
//[AlwaysUpdateSystem]
public partial class SpawnEnemiesSystem : SystemBase
{
    public DotPrefabinator prefabinator;

    public EntityQuery SpawnPointsQuery;

    public Translation SpawnPoints;
    public int numbtospawn;

    public Unity.Mathematics.Random Rundomnumcreator;

    NativeArray<Translation> spawnpoints;
    protected override void OnCreate()
    {
        //base.OnCreate();

        Rundomnumcreator = new Unity.Mathematics.Random();
        //prefabinator = GetSingleton<DotPrefabinator>();
        //Debug.Log("Why is this not running ");
        numbtospawn =5; //Will probably stick to 5 max

        SpawnPointsQuery = GetEntityQuery(ComponentType.ReadOnly<SpawnPointTag>(), ComponentType.ReadOnly<Translation>());
        
        RequireSingletonForUpdate<TimeTickEvent>();
        RequireSingletonForUpdate<PauseSystemDat>();

        //Enabled = false;
    }

    protected override void OnStartRunning()
    {
        //base.OnStartRunning();

        spawnpoints = SpawnPointsQuery.ToComponentDataArray<Translation>(Allocator.Temp);

        prefabinator = GetSingleton<DotPrefabinator>();// !HasSingleton<DotPrefabinator>() ? GetSingleton<DotPrefabinator>() : default;
    }

    protected override void OnUpdate()
    {
        var Rundomnumcreator = new NativeArray<Unity.Mathematics.Random>(JobsUtility.MaxJobThreadCount, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
        var r = (uint)UnityEngine.Random.Range(int.MinValue, int.MaxValue);
        for (int i = 0; i < JobsUtility.MaxJobThreadCount; i++)
        {
            Rundomnumcreator[i] = new Unity.Mathematics.Random(r == 0 ? r + 1 : r);
        }

        var rndum = Rundomnumcreator[0];//[nativeThreadIndex]; I only need one currently;
        //var ultimaterand = rndum.NextInt(-10, 10);

        var spawnpoints = SpawnPointsQuery.ToComponentDataArray<Translation>(Allocator.Temp);

       
        //for(int )
        //for (int z = 0; z < SpawnPoints.Length; z++)
        //{
        //    for (int i = 0; i < NumbtoSpawnatPonts[z]; i++)
        //    {
        //        //Random.InitState((int)System.DateTime.Now.Ticks);
        //        GameObject Hobbity = OP.GetPooledObject(PoolIndex);

        //        var nazmeesh = Hobbity.GetComponent<NavMeshAgent>();
        //        nazmeesh.enabled = true;

        //        float xPos = UnityEngine.Random.Range(-10f, 10f);
        //        float zPos = UnityEngine.Random.Range(-2f, 2f);
        //        Hobbity.transform.position = SpawnPoints[z].transform.position + xPos * Vector3.right + zPos * Vector3.forward;
        //        Hobbity.SetActive(true);
        //    }
        //}

        //prefabinator = !HasSingleton<DotPrefabinator>() ? GetSingleton<DotPrefabinator>() : default;

        //NativeArray<Entity> output = new NativeArray<Entity>(numbtospawn, Allocator.Temp);//, NativeArrayOptions.UninitializedMemory);

        //EntityManager.Instantiate(prefabinator.TestObjectPrefab, output);
        float ultimaterand = 0;        
        for (int z = 0; z < spawnpoints.Length; z++)
        {
            NativeArray<Entity> output = new NativeArray<Entity>(numbtospawn, Allocator.Temp);
            EntityManager.Instantiate(prefabinator.TestObjectPrefab, output);
            for (int i = 0; i < output.Length; i++)//TO
            {
                //var rndum = Rundomnumcreator[0];//[nativeThreadIndex]; I only need one currently;
                //var ultimaterand = rndum.NextInt(-10, 10);
                //var ultimaterand = UnityEngine.Random.Range(-10, 10);//TODO Make it simpler don't spawn them at random positions just spawn them each at equidistant positions depending on how many have a random number that perhaps spawns but not the positions
                //Debug.Log("The ultimate" + ultimaterand);
                ultimaterand = i * 1.5f;
                var newpos = spawnpoints[z].Value;
                newpos.z = newpos.z + ultimaterand;
                newpos.y = 2;
                SetComponent<Translation>(output[i], new Translation { Value = newpos });
                //SetComponent<BaseHealth>(output[i], new BaseHealth { intVal = 100 });//TODO I need to not set so many values here I think I want to be able to set the base health and its base stats in the Editor
                //SetComponent<CurrHealth>(output[i], new CurrHealth { intVal = 100 });but then what if I want the enemies to increase in health and levels I need to set the values here based on the values already set
            }
        }

        Rundomnumcreator.Dispose();
        spawnpoints.Dispose();
        
    }

}





public partial class IntialiseEnemySystem : SystemBase//GameObjectConversionSystem//TODO I don't even know what this System is or why it exists
{
    EntityQuery enemyquery;

    protected override void OnCreate()
    {
        RequireSingletonForUpdate<InitialiseEnemysTag>();
        //enemyquery = GetEntityQuery(typeof(EnemyDudeTag));
        Enabled = false;
    }

    protected override void OnUpdate()
    {
        
        if(HasSingleton<InitialiseEnemysTag>())
        {
            var destroyme = GetSingletonEntity<InitialiseEnemysTag>();

            EntityManager.DestroyEntity(destroyme);
        }
           

        var enemydudeent = enemyquery.ToEntityArray(Allocator.Temp);




    }
}
  