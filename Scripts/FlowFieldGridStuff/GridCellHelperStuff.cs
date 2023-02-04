using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.Entities;
using Unity.Collections;

public static class GridCellHelperStuff
{

    public static float3  GetWorldPosition(int2 gridpos, float3 firstgridpos, float gridcellsize)
    {
        return new float3(gridpos.x, 0, gridpos.y) * gridcellsize + firstgridpos;
    }

    public static int2 GetGridXZ(float3 worldPosition, float3 firstgridpos, float gridcellsize)
    {
        
        var x = (int)math.round((worldPosition.x - firstgridpos.x) / gridcellsize);
        var z = (int)math.round((worldPosition.z - firstgridpos.z) / gridcellsize);

        return new int2(x, z);
    }

    public static int ToFlatIndex(int2 index2D, int height)
    {       
        return height * index2D.x + index2D.y;   
    }

    public static FixedList64Bytes<int2> GetNeighbours (int2 gridpos, int2 gridsize)//This gets the neigboours that surround in the not diagonals//TODO Need to check how many this list can store
    {
        int x, y;
        FixedList64Bytes<int2> theneebors = new FixedList64Bytes<int2>();
        y = 0;
        
        for(x = -1; x <= 1; x++)
        {
            int2 current = new int2(x, y);//I may need to sort
            if (current.Equals(int2.zero) || (current + gridpos).x < 0 || (current + gridpos).x >= gridsize.x || (current + gridpos).y < 0 || (current + gridpos).y >= gridsize.y)
                continue;

            theneebors.Add(current + gridpos);
          
        }

        x = 0;
        for(y = -1; y <= 1; y++)
        {
            int2 current = new int2(x, y);
            if (current.Equals(int2.zero) || (current + gridpos).x < 0 || (current + gridpos).x >= gridsize.x || (current + gridpos).y < 0 || (current + gridpos).y >= gridsize.y)
                continue;

            theneebors.Add(current + gridpos);
        } 

        return theneebors;
    }
        


 

}

//public static int2 GetCellIndexFromWorldPos(in float3 worldpos, in int2 gridSize, ref float cellDiameter)
//{

//    float percentX = worldpos.x / (gridSize.x * cellDiameter);
//    float percentY = worldpos.z / (gridSize.y * cellDiameter);

//    percentX = math.clamp(percentX, 0f, 1f);
//    percentY = math.clamp(percentY, 0f, 1f);

//    int2 cellIndex = new int2
//    {
//        x = math.clamp((int)math.floor((gridSize.x) * percentX), 0, gridSize.x - 1),
//        y = math.clamp((int)math.floor((gridSize.y) * percentY), 0, gridSize.y - 1)
//    };


//    return cellIndex;

//}


//public void GetXZ(Vector3 worldPosition, out int x, out int z)
//{
//x = Mathf.RoundToInt((worldPosition - originPosition).x / cellSize);
//z = Mathf.RoundToInt((worldPosition - originPosition).z / cellSize);
//}