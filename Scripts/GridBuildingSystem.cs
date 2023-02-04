using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;
//using Drawing;

public partial class GridBuildingSystem : SystemBase
{

    public float GridCellsize;
    public int2 GridheightWidth;

    public float3 GridRootpos;

    public EntityQuery GridPositionQuery;

    public EntityArchetype gridarch;

    protected override void OnCreate()
    {
        Enabled = false; // base.OnCreate();
        GridCellsize = 3f;
        GridheightWidth.x = 33;
        GridheightWidth.y = 33;

        GridRootpos = new float3(-50, 0, -50);
        GridPositionQuery = EntityManager.CreateEntityQuery(ComponentType.ReadOnly<GridIndexBuilder>());

        gridarch = EntityManager.CreateArchetype(typeof(GridIndexBuilder),
            typeof(GridposBuilder),
            typeof(GridSpaceOccupied)
            );
         
        //RequireSingletonForUpdate<CreateNewGridEvent>();
        
    }

    protected override void OnStartRunning()
    {
        

    }

    protected override void OnUpdate()
    {
        if (GridPositionQuery.CalculateEntityCount() > 0)
        {
            Enabled = false;
            return;
        }

        int numbents = GridheightWidth.x * GridheightWidth.y;
        NativeArray<Entity> grdidudes = new NativeArray<Entity>(numbents, Allocator.Temp);

        EntityManager.CreateEntity(gridarch, grdidudes);

        int tempcount = 0;
        for(int i = 0; i < GridheightWidth.x; i++)
        {
            for (int j = 0; j < GridheightWidth.y; j++)
            {                
                GridIndexBuilder tempgrid = new GridIndexBuilder { int2Value = new int2(i, j) };

                //SetComponent<GridIndexBuilder>(grdidudes[i], tempgrid);

                EntityManager.SetComponentData<GridIndexBuilder>(grdidudes[tempcount], tempgrid);
                var currentval = new float3(GridCellsize * i + (GridCellsize / 2), 0, GridCellsize * j + (GridCellsize / 2));
                var ultval = currentval + GridRootpos;

                SetComponent<GridposBuilder>(grdidudes[tempcount], new GridposBuilder { float3val = ultval });

                SetComponent<GridSpaceOccupied>(grdidudes[tempcount], new GridSpaceOccupied { boolValue = false });

                tempcount++;
            }
        }
    }
}

public struct GridIndexBuilder : IComponentData
{
    public int2 int2Value;
}

public struct GridposBuilder : IComponentData
{
    public float3 float3val;
}

public struct GridSpaceOccupied : IComponentData 
{
    public bool boolValue;
} //Should I have this as a tag or a boolean will try as a tag for now When its occupied I just add this to 

public struct CreateNewGridEvent : IComponentData { }

public partial class GridViewerSystem : SystemBase
{
    public EntityQuery gridfoolsquery;

    protected override void OnCreate()
    {
        base.OnCreate();
        gridfoolsquery = GetEntityQuery(ComponentType.ReadOnly<GridposBuilder>());

        //Enabled = false;

    }


    protected override void OnUpdate()
    {
        var gridPosNumb = gridfoolsquery.CalculateEntityCount();
        //var builder = DrawingManager.GetBuilder(true);

        //builder.Preallocate(gridPosNumb);
        Entities.ForEach((in GridposBuilder gridpoos) =>
        {
            //builder.WireBox(gridpoos.float3val, 3);

            //builder.Label2D();


        }).ScheduleParallel();

        this.CompleteDependency();

        //builder.Dispose();    

    }
}