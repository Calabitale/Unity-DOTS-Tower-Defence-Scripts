using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DotsFlowField;
using Unity.Entities;

public class FlowBoidMonoSystemCreator : MonoBehaviour
{
    //IEnumerator Start()    
    //{
    //yield return new WaitForSeconds(1);//NOTE Must be carful of this I cannot delay it because it breaks things like the flowfield and stuff for some reason
    // Start is called before the first frame update
    void Start()
    {
        var FlowfieldSyst = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<InitialiseFlowField>();
        var initialfieldboidsetupsy = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<InitialiseFieldBoidSystem>();
        var Stootgamsyst = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<StartGameSystem>();
        var IngameUIsyst = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<InGameUISystem>();
        var Spawnenemiessys = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<SpawnEnemiesSystem>();
        var TurretAttacksys = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<TurretAttackSystem>();
        var PlayerAbilitiessys = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<PlayerAbilities>();
        var ENemyDamageSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<PlayerDamageSystem>();
        var ProfitsTrackSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<ProfitTrackingSystem>();
        var AntagonRobotSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<AntagonistRobotSystem>();
        var EnemydamSystm = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<PlayerDamageSystem>();

        World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<SimulationSystemGroup>().AddSystemToUpdateList(FlowfieldSyst);
        World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<SimulationSystemGroup>().AddSystemToUpdateList(initialfieldboidsetupsy);
        World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<SimulationSystemGroup>().AddSystemToUpdateList(Stootgamsyst);
        World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<SimulationSystemGroup>().AddSystemToUpdateList(IngameUIsyst);
        World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<SimulationSystemGroup>().AddSystemToUpdateList(Spawnenemiessys);
        World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<SimulationSystemGroup>().AddSystemToUpdateList(TurretAttacksys);
        World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<SimulationSystemGroup>().AddSystemToUpdateList(PlayerAbilitiessys);
        World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<SimulationSystemGroup>().AddSystemToUpdateList(ENemyDamageSystem);
        World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<FixedStepSimulationSystemGroup>().AddSystemToUpdateList(ProfitsTrackSystem);
        World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<SimulationSystemGroup>().AddSystemToUpdateList(AntagonRobotSystem);
        World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<SimulationSystemGroup>().AddSystemToUpdateList(EnemydamSystm);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
