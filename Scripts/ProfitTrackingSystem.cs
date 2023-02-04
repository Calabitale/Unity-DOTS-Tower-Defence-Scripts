using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Physics.Stateful;
using Unity.Physics.Systems;
using Unity.Burst;
using Unity.Collections;
using UnityEngine.UI;
using TMPro;

[DisableAutoCreation]
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(StatefulTriggerEventBufferSystem))]
public partial class ProfitTrackingSystem : SystemBase
{
    public GameObject GameManagerStuff;
    public GameObject ProfitsDisplayGO;
    public GameObject LossDisplayGO;

    TextMeshProUGUI ProfitsDisText;

    BuildPhysicsWorld m_BuildPhysicsWorld;
    StepPhysicsWorld stepphysworld;

    EntityQuery ProfitsQuery;

    protected override void OnCreate()
    {
        GameManagerStuff = GameObject.Find("GameManagerStuff");

        var gameobjrefs = GameManagerStuff.GetComponent<GameObjectRefs>();
        ProfitsDisplayGO = gameobjrefs.ProfitsDisplay;
        //LossDisplayGO = gameobjrefs.LossDisplay;

        //ProfitsDisText = ProfitsDisplayGO.GetComponentInChildren<TextMeshProUGUI>();

        m_BuildPhysicsWorld = World.GetExistingSystem<BuildPhysicsWorld>();
        stepphysworld = World.GetOrCreateSystem<StepPhysicsWorld>();

        ProfitsQuery = GetEntityQuery(ComponentType.ReadWrite<ProfitsDat>());

        RequireSingletonForUpdate<ProfitsDat>();
    }

    protected override void OnStartRunning()
    {
        ProfitsDisText = ProfitsDisplayGO.GetComponent<TextMeshProUGUI>();
    }

    protected override void OnUpdate()
    {
        var currProfitsDatVal = GetSingleton<ProfitsDat>();


        ProfitsDisText.text = currProfitsDatVal.intVal.ToString();

        var ProfitsEnt = GetSingletonEntity<ProfitsDat>();
      
        NativeReference<ProfitsDat> ProfitsRef = new NativeReference<ProfitsDat>(Allocator.TempJob);
        ProfitsRef.Value = new ProfitsDat { intVal = currProfitsDatVal.intVal };

        var currjob = new ProfitTriggerJob()
        {
          
            dudes = GetComponentDataFromEntity<EnemyDudeTag>(),
            payTrigger = GetComponentDataFromEntity<PaymentTriggerTag>(),          
            Profitref = ProfitsRef//new NativeReference<ProfitsDat>(Allocator.TempJob)

        };       
        
        currjob.Schedule(this.Dependency).Complete();
     
        SetComponent<ProfitsDat>(ProfitsEnt, currjob.Profitref.Value);//new ProfitsDat { intVal = currjob.CurrProfits.intVal });// Profitref.Value });

        currjob.Profitref.Dispose();


    }
}

//[BurstCompile]
public partial struct ProfitTriggerJob : IJobEntity
{
    public ComponentDataFromEntity<EnemyDudeTag> dudes;
    public ComponentDataFromEntity<PaymentTriggerTag> payTrigger;   

    public ProfitsDat CurrProfits;
    public NativeReference<ProfitsDat> Profitref;

    public void Execute(Entity ent, ref DynamicBuffer<StatefulTriggerEvent> triggerEventBuffer)
    {
        //Profitref = new NativeReference<int> { Value = 0 };
        if (!payTrigger.HasComponent(ent))
            return;

        //Profitref = new NativeReference<int> { Value = 0 };

        //Profitref.Value = CurrProfits.intVal;

        for (int i = 0; i < triggerEventBuffer.Length; i++)
        {
            var triggerEvent = triggerEventBuffer[i];
            var otherEntity = triggerEvent.GetOtherEntity(ent);

            if (triggerEvent.State == StatefulEventState.Stay)// || !nonTriggerMask.Matches(otherEntity))
            {
                
                    
                continue;               
            }

            if (triggerEvent.State == StatefulEventState.Enter)
            {
               
                continue;
            }
            else//State == PhysicsEventState.Exit
            {
               
                var tempProfit = Profitref.Value;
                tempProfit.intVal += 100;
                Profitref.Value = tempProfit;


                continue;
            }
        }




    }
}