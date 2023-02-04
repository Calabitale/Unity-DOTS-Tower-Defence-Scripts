using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Rendering;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Transforms;

public partial class StopMotionAnimTest : SystemBase
{

    RenderMesh currMesh;

    EntityQuery enemyQuery;

    DotPrefabinator prefabinator;

    EndSimulationEntityCommandBufferSystem endsimBuffer;

    float ChangeAnimeTimer;


    protected override void OnCreate()
    {

        endsimBuffer = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        Enabled = false;
    }

    protected override void OnStartRunning()
    {
        prefabinator = GetSingleton<DotPrefabinator>();

    }

    protected override void OnUpdate()
    {
        var ecb = endsimBuffer.CreateCommandBuffer();

        var tempmesh = prefabinator.EnemyTestMesh;
        bool changeAnimbool = false;
        if ((float)Time.ElapsedTime >= ChangeAnimeTimer)
        {
            ChangeAnimeTimer = (float)Time.ElapsedTime + 0.5f;
           
            changeAnimbool = true;

        }


            //var newmesh = GetComponent<RenderMesh>(tempmesh);

            Entities.
            WithAny<EnemyModelOne>().
            WithAll<EnemyDudeTag>().
            WithAny<EnemyModelTwo>().        
            WithName("ChangeRendermesh").
            ForEach((Entity e, ref FlashHitTimer animtime) =>
            {
                
                if (animtime.fltVal > 0)
                    return;


                //changeAnimbool = false;

                //var isitenabled = HasComponent<Disabled>(e);

                //if(HasComponent<RenderBounds>(e))

               // if (isitenabled)
                {
                    
                   // ecb.RemoveComponent<Disabled>(e);
                }
               // else
               
                {
                    //ecb.AddComponent<DisableRendering>(e);
                    ecb.AddComponent<Disabled>(e);
                }

                
                
            }).WithoutBurst().Run();


        endsimBuffer.AddJobHandleForProducer(this.Dependency);
    }

    public void SetWholeHierarchy(EntityCommandBuffer commandBuffer, Entity entity, bool enabled)
    {
        // We traverse the whole hierarchy and set all the rendering states using RenderBounds to determine if it's a rendering component
        //if (this.renderBounds.HasComponent(entity))
        //{
        //    var renderingDisabled = this.disableRenderings.HasComponent(entity);
        //    if (enabled && renderingDisabled)
        //    {
        //        commandBuffer.RemoveComponent<DisableRendering>(entity);
        //    }
        //    else if (!enabled && !renderingDisabled)
        //    {
        //        commandBuffer.AddComponent<DisableRendering>(entity);
        //    }
        //}

        //if (!this.childrens.HasBuffer(entity))
        //{
        //    return;
        //}

        //var children = this.childrens[entity];
        //for (var i = 0; i < children.Length; i++)
        //{
        //    this.SetWholeHierarchy(commandBuffer, children[i].Value, enabled);
        //}
    }


}

//public struct DisableRenderingHelper
//{
//    [ReadOnly]
//    private ComponentLookup<DisableRendering> disableRenderings;

//    [ReadOnly]
//    private BufferLookup<Child> childrens;

//    [ReadOnly]
//    private ComponentLookup<RenderBounds> renderBounds;

//    public DisableRenderingHelper(
//        ComponentLookup<DisableRendering> disableRenderings,
//        BufferLookup<Child> childrens,
//        ComponentLookup<RenderBounds> renderBounds)
//    {
//        this.disableRenderings = disableRenderings;
//        this.childrens = childrens;
//        this.renderBounds = renderBounds;
//    }

//    /// <summary> Toggles the <see cref="DisableRendering"/> component for all entities in a hierarchy. </summary>
//    /// <param name="commandBuffer"> Command buffer. </param>
//    /// <param name="entity"> The parent entity. </param>
//    /// <param name="enabled"> When true adds <see cref="DisableRendering"/> otherwise removes it. </param>
//    public void SetWholeHierarchy(EntityCommandBuffer commandBuffer, Entity entity, bool enabled)
//    {
//        // We traverse the whole hierarchy and set all the rendering states using RenderBounds to determine if it's a rendering component
//        if (this.renderBounds.HasComponent(entity))
//        {
//            var renderingDisabled = this.disableRenderings.HasComponent(entity);
//            if (enabled && renderingDisabled)
//            {
//                commandBuffer.RemoveComponent<DisableRendering>(entity);
//            }
//            else if (!enabled && !renderingDisabled)
//            {
//                commandBuffer.AddComponent<DisableRendering>(entity);
//            }
//        }

//        if (!this.childrens.HasBuffer(entity))
//        {
//            return;
//        }

//        var children = this.childrens[entity];
//        for (var i = 0; i < children.Length; i++)
//        {
//            this.SetWholeHierarchy(commandBuffer, children[i].Value, enabled);
//        }
//    }

//    /// <summary> Toggles the <see cref="DisableRendering"/> component for all entities in a hierarchy. </summary>
//    /// <param name="commandBuffer"> Parallel command buffer. </param>
//    /// <param name="sortIndex"> Index for the parallel command buffer. </param>
//    /// <param name="entity"> The parent entity. </param>
//    /// <param name="enabled"> When true adds <see cref="DisableRendering"/> otherwise removes it. </param>
//    public void SetWholeHierarchy(EntityCommandBuffer.ParallelWriter commandBuffer, int sortIndex, Entity entity, bool enabled)
//    {
//        // We traverse the whole hierarchy and set all the rendering states using RenderBounds to determine if it's a rendering component
//        if (this.renderBounds.HasComponent(entity))
//        {
//            var renderingDisabled = this.disableRenderings.HasComponent(entity);
//            if (enabled && renderingDisabled)
//            {
//                commandBuffer.RemoveComponent<DisableRendering>(sortIndex, entity);
//            }
//            else if (!enabled && !renderingDisabled)
//            {
//                commandBuffer.AddComponent<DisableRendering>(sortIndex, entity);
//            }
//        }

//        if (!this.childrens.HasBuffer(entity))
//        {
//            return;
//        }

//        var children = this.childrens[entity];
//        for (var i = 0; i < children.Length; i++)
//        {
//            this.SetWholeHierarchy(commandBuffer, sortIndex, children[i].Value, enabled);
//        }
//    }
//}