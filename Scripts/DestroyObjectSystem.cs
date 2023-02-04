using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using static UnityEngine.EventSystems.EventTrigger;

public partial class DestroyObjectSystem : SystemBase
{
    EndSimulationEntityCommandBufferSystem endsimBuffer;
    public GameObject GameobjRefs;
    public GameObject SparksPrefab;
    public GameObject ExplosPrefab;

    public DotPrefabinator Prefabinator;

    protected override void OnCreate()
    {
        //endsimBuffer = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnStartRunning()
    {
        GameobjRefs = GameObject.Find("GameManagerStuff");
        var gameobjscripts = GameobjRefs.GetComponent<GameObjectRefs>();
        SparksPrefab = gameobjscripts.SparksParticGO;
        ExplosPrefab = gameobjscripts.DefaultExplosion;

        Prefabinator = HasSingleton<DotPrefabinator>() ? GetSingleton<DotPrefabinator>() : default;
    }

    protected override void OnUpdate()
    {
        Prefabinator = HasSingleton<DotPrefabinator>() ? GetSingleton<DotPrefabinator>() : default;
        var timmydelta = Time.DeltaTime;


        //new DestroyObjectsJob { 
        //    ecb  = ecb,
        //    TimmyDelta = timmydelta
        //}.Schedule();

        Entities.            
            WithStructuralChanges().           
            WithAny<SlipHazardTag>().
            WithAny<TurretTag>().
            WithAny<ShelfDestroyedTag>().
            ForEach((Entity medude, ref DeathAnimeTimer deathtime, in IsAlive Imalive) =>           
            {
                if (Imalive.booVal)
                    return;

                if (deathtime.fltVal <= 0)
                {
                    var theposition = GetComponent<Translation>(medude);
                    theposition.Value.y += 2f;
                    //EntityManager.DestroyEntity(medude);

                    if(HasComponent<TurretTag>(medude)) 
                        GameObject.Instantiate(ExplosPrefab, theposition.Value, Quaternion.identity);//TODO I maybe need to put a conditional here for cases where I maybe don't want to have the explosion for certain objects
                    else if(HasComponent<ShelfDestroyedTag>(medude))
                    {
                                               
                        var thenew = EntityManager.Instantiate(Prefabinator.ShelfNewPrefab);
                        theposition.Value.y = 0f;
                        
                        SetComponent<Translation>(thenew, new Translation { Value = theposition.Value });                        /
                        //var thechildboof = GetBuffer<Child>(thenew); //: default;

                        //for(int i = 0; i < thechildboof.Length; i++)
                        //{
                        //    var newboof = GetBuffer<Child>(thenew);
                        //    SetComponent<Translation>(thechildboof[i].Value, new Translation { Value = theposition.Value });
                        //}

                      
                    }

                    EntityManager.DestroyEntity(medude);

                }
                
                if(HasComponent<TurretTag>(medude) && deathtime.fltVal == 2f)
                {
                    var theposition = GetComponent<Translation>(medude);                    
                    theposition.Value.y += 2f;
                    GameObject.Instantiate(SparksPrefab, theposition.Value, Quaternion.identity);


                }

              


                deathtime.fltVal = deathtime.fltVal - timmydelta;


            }).Run();


        //endsimBuffer.AddJobHandleForProducer(this.Dependency);
    }

    


    //public partial struct DestroyObjectsJob : IJobEntity
    //{
    //    public EntityCommandBuffer ecb;
    //    public float TimmyDelta;
        
    //    void Execute(Entity enty, ref DeathAnimeTimer deathtime, in SlipHazardTag metag, in IsAlive Imalive)//TODO I should make this object agnostic
    //    {
    //        if(deathtime.fltVal <= 0)
    //        {
    //            ecb.DestroyEntity(enty);
    //            Debug.Log("Why isint this");
    //        }

    //        deathtime.fltVal = deathtime.fltVal - TimmyDelta;
            
    //    }
    //}


}

[WithAll]
public partial struct DestroyObjectsJob : IJobEntity
{
    public EntityCommandBuffer ecb;
    public float TimmyDelta;

    public void Execute(Entity enty, ref DeathAnimeTimer deathtime, in IsAlive Imalive)//TODO I should make this object agnostic
    {        
        if (deathtime.fltVal <= 0)
        {
            ecb.DestroyEntity(enty);            
        }

        deathtime.fltVal = deathtime.fltVal - TimmyDelta;

    }
}
