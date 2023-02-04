using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
//using Drawing;
using Unity.Collections;

public partial class DebugDrawFieldSystem : SystemBase//NOTE This just draws the field ucomment and adjust 
{
    public EntityQuery celldataquery;
    
    protected override void OnCreate()
    {
        base.OnCreate();

        celldataquery = GetEntityQuery(ComponentType.ReadOnly<CellData>());
        Enabled = false;
    }

    protected override void OnUpdate()
    {
        
        var flowfielddudat = GetSingleton<FlowFieldData>();
        //var builder = DrawingManager.GetBuilder(true);

        //builder.Preallocate(flowfielddudat.gridSize.x * flowfielddudat.gridSize.y);

        Entities
            //WithDisposeOnCompletion(builder)
            .ForEach((DynamicBuffer<CellBestDirectionBuff> buffdood, in CellsBestDirection cellbesty, in CellData cells) =>
            {

                if (cells.cost == 255)
                {
                    //builder.PushColor(Color.red);
                    //builder.WireBox(cells.worldPos, flowfielddudat.cellRadius * 2);
                    //builder.PopColor();
                }
                else
                {
                    //builder.WireBox(cells.worldPos, flowfielddudat.cellRadius * 2);
                    //builder.Label2D(cells.worldPos, cells.bestCost.ToString(), 12f, LabelAlignment.Center, Color.white);
                }

                //builder.Label2D
                //cellbesty.



            }).Run();// Schedule();

        Dependency.Complete();
        //this.CompleteDependency();

        //builder.Dispose();
    }


    //TODO Can use static functions like this
    public static bool CheckIfComponentExists(Entity entity)
    {

        return true;
    }

}