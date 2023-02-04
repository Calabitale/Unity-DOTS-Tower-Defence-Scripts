using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Entities.LowLevel.Unsafe;

public class RandomICollectedCode : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public struct MoveCamera : IComponentData
{
    public float3 origin;
}

//[UpdateInGroup(typeof(EarlySimulationSystemGroup))]
//[BurstCompile]
//public struct MouseViewHandler : ISystem, ISystemStartStop
//{
//    [UpdateInGroup(typeof(EarlySimulationSystemGroup))]
//    [UpdateBefore(typeof(Stop))]
//    [BurstCompile]
//    private struct Start : ISystem
//    {
//        private Entity _entity;
//        private Entity _inputEntity;

//        public void OnCreate(ref SystemState state)
//        {
//            state.RequireSingletonForUpdate<Performed<MiddleClick>>();
//            state.RequireSingletonForUpdate<SpaceOverlayClicked>();
//            _entity = state.EntityManager.GetOrCreateSingletonEntity<Singleton<MoveCamera>>();
//            _inputEntity = state.EntityManager.GetOrCreateSingletonEntity<PointerPosition>();
//        }

//        [BurstCompile]
//        public void OnUpdate(ref SystemState state)
//        {
//            state.EntityManager.AddComponentData(_entity,
//                new MoveCamera
//                {
//                    origin = state.EntityManager.GetComponentData<PointerPosition>(_inputEntity).worldPosition
//                });
//        }

//        public void OnDestroy(ref SystemState state) { }
//    }

//    [UpdateInGroup(typeof(EarlySimulationSystemGroup))]
//    [UpdateBefore(typeof(MouseViewHandler))]
//    [BurstCompile]
//    private struct Stop : ISystem
//    {
//        private Entity _entity;

//        public void OnCreate(ref SystemState state)
//        {
//            state.RequireSingletonForUpdate<Canceled<MiddleClick>>();
//            _entity = state.EntityManager.GetOrCreateSingletonEntity<Singleton<MoveCamera>>();
//        }

//        [BurstCompile]
//        public void OnUpdate(ref SystemState state) { state.EntityManager.RemoveComponent<MoveCamera>(_entity); }

//        public void OnDestroy(ref SystemState state) { }
//    }

//    private Entity _cameraEntity;
//    private Entity _inputEntity;
//    private Entity _entity;

//    private float3 _origin;
//    private EntityQuery _query;

//    public void OnCreate(ref SystemState state)
//    {
//        _cameraEntity = state.EntityManager.GetOrCreateSingletonEntity<SpaceCameraTag>();
//        _inputEntity = state.EntityManager.GetOrCreateSingletonEntity<PointerPosition>();
//        _entity = state.EntityManager.GetOrCreateSingletonEntity<Singleton<MoveCamera>>();

//        state.RequireSingletonForUpdate<MoveCamera>();
//        _query = state.GetEntityQuery(typeof(PointerPosition));
//        _query.AddChangedVersionFilter(typeof(PointerPosition));
//        state.RequireForUpdate(_query);
//    }

//    [BurstCompile]
//    public void OnStartRunning(ref SystemState state)
//    {
//        _origin = state.EntityManager.GetComponentData<MoveCamera>(_entity).origin;
//    }

//    [BurstCompile]
//    public void OnUpdate(ref SystemState state)
//    {
//        if (_query.IsEmpty) return;
//        var newPos = state.EntityManager.GetComponentData<PointerPosition>(_inputEntity).worldPosition -
//                     state.EntityManager.GetComponentData<LocalToWorld>(_cameraEntity).Position;
//        newPos = _origin - newPos;
//        state.EntityManager.SetCameraPosition(_cameraEntity, newPos);
//    }


//    public void OnDestroy(ref SystemState state) { }


//    public void OnStopRunning(ref SystemState state) { }
//}