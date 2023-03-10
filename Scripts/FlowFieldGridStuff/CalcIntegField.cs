using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Jobs;
using DotsFlowField;

namespace DotsFlowField
{
   
    //[UpdateAfter(typeof(PlayerTestInput))]
    [UpdateAfter(typeof(CalculateCellCostSystem))]
    public partial class CalcIntegrationFieldSys : SystemBase
    {
        public EntityQuery NeighboursQuery;
        public EntityQuery CellDataQuery;
        public NativeQueue<int2> nubbyQueue;

        public EntityQuery DestinationBoofQuery;
        protected override void OnCreate()
        {
            //base.OnCreate();
            NeighboursQuery = GetEntityQuery(ComponentType.ReadOnly<FlowfieldNeigborEntities>());
            CellDataQuery = GetEntityQuery(typeof(CellData), typeof(CellsBestDirection));
            DestinationBoofQuery = GetEntityQuery(typeof(CellDestinationsBuffer));
            RequireSingletonForUpdate<CalcintegrationFieldEvent>();
            //Enabled = false;
        }

        protected override void OnStartRunning()
        {

        }

        protected override void OnUpdate()
        {
            if (HasSingleton<CalcintegrationFieldEvent>())
            {
                var tempevent = GetSingletonEntity<CalcintegrationFieldEvent>();
                EntityManager.DestroyEntity(tempevent);
            }

            CellBDLayerToCalc LayertoCalc = HasSingleton<CellBDLayerToCalc>() ? GetSingleton<CellBDLayerToCalc>() : default;
            int layer = LayertoCalc.intVal;
         

            var flooydata = GetSingleton<FlowFieldData>();

            Entities.WithName("ResetCostandBestCost").ForEach((ref CellData celldudes) =>
            {
                if (celldudes.cost != 255)
                {
                    celldudes.cost = 1;
                }

                celldudes.bestCost = byte.MaxValue;
                
            }).ScheduleParallel();

            var celldatacount = CellDataQuery.CalculateEntityCount();
            NativeArray<CellData> TempCelldata = new NativeArray<CellData>(celldatacount, Allocator.TempJob);

            NativeArray<CellsBestDirection> TempCellBestDirect = new NativeArray<CellsBestDirection>(celldatacount, Allocator.TempJob);

            Entities.WithName("CollectAllCellData").ForEach((int entityInQueryIndex, ref CellData celldudes) =>
            {

                TempCelldata[entityInQueryIndex] = celldudes;

            }).ScheduleParallel();

            var tempNeigbourindices = new GetNeighborIndices();

            var TempFlatIndex = new ToFlatIndex();

            NativeQueue<int2> indicesToCheck = new NativeQueue<int2>(Allocator.TempJob);
     
            var Tembufferent = GetSingletonEntity<CellDestinationsBuffer>();

            var TempBufferDestinate = GetBufferFromEntity<CellDestinationsBuffer>(true);
            
            var Tempbuffedestinations = TempBufferDestinate[Tembufferent];

            Entities.WithName("CalculateIntegrationField").WithDisposeOnCompletion(indicesToCheck).ForEach((in FlowFieldData flooydata) =>
            {
              
                indicesToCheck.Clear();
                 
                    
                int flatdestindex = TempFlatIndex.Execute(Tempbuffedestinations[layer], flooydata.gridSize.y);
                                
                var tempcelldestinate = TempCelldata[flatdestindex];               

                tempcelldestinate.cost = 0;
                tempcelldestinate.bestCost = 0;

                TempCelldata[flatdestindex] = tempcelldestinate;

                indicesToCheck.Enqueue(Tempbuffedestinations[layer]);                
                while (indicesToCheck.Count > 0)
                {
                    int2 cellindex = indicesToCheck.Dequeue();
                    var cellfatindex = TempFlatIndex.Execute(cellindex, flooydata.gridSize.y);
                    var curCelldata = TempCelldata[cellfatindex];
                    FixedList128Bytes<int2> tempint2list = new FixedList128Bytes<int2>();
                    tempNeigbourindices.Execute(cellindex, flooydata.gridSize, true, ref tempint2list);

                    foreach (int2 neeborindex in tempint2list)
                    {
                        int flatneigbourindex = TempFlatIndex.Execute(neeborindex, flooydata.gridSize.y);

                        var neigbourcelldata = TempCelldata[flatneigbourindex];

                        if (neigbourcelldata.cost == byte.MaxValue)
                        {
                            
                            continue;
                        }

                        if (neigbourcelldata.cost + curCelldata.bestCost < neigbourcelldata.bestCost)
                        {
                            neigbourcelldata.bestCost = (ushort)(neigbourcelldata.cost + curCelldata.bestCost);
                            TempCelldata[flatneigbourindex] = neigbourcelldata;
                            indicesToCheck.Enqueue(neeborindex);
                           
                        }
                    }
                }
                    
                    for (int i = 0; i < TempCelldata.Length; i++)//NOTE Comment out this code and replace with below job code if you have lots of layers and want to multithread it and speed things up quite a bit
                    {
                        CellData curCullData = TempCelldata[i];

                        ushort currbestCost = curCullData.bestCost;
                        int2 currbestDirection = int2.zero;
                        FixedList128Bytes<int2> tempint2list = new FixedList128Bytes<int2>();
                        tempNeigbourindices.Execute(curCullData.gridIndex, flooydata.gridSize, false, ref tempint2list);
                        foreach (int2 neebor in tempint2list)
                        {
                            var flatusindicus = TempFlatIndex.Execute(neebor, flooydata.gridSize.y);
                            CellData neeborcellindex = TempCelldata[flatusindicus];
                            if (neeborcellindex.bestCost < currbestCost)
                            {
                                currbestCost = neeborcellindex.bestCost;
                                currbestDirection = neeborcellindex.gridIndex - curCullData.gridIndex;
                               

                            }

                        }
                       

                        var tempbesty = TempCellBestDirect[i];
                        tempbesty.bestDirection = currbestDirection;
                        TempCellBestDirect[i] = tempbesty;
                        //TempCelldata[i] = curCullData;


                    }
                //}

            }).Schedule();


            //Entities.WithReadOnly(TempCelldata).WithName("AltDirectionCalcTest").ForEach((int entityInQueryIndex, DynamicBuffer<CellBestDirectionBuff> dudedirection) =>//TODO Replace the for loop above with this for a speedup
            //{
            //    CellData curCullData = TempCelldata[entityInQueryIndex];

            //    ushort currbestcost = curCullData.bestCost;
            //    int2 currbestDirection = int2.zero;
            //    FixedList128Bytes<int2> tempint2list = new FixedList128Bytes<int2>();
            //    tempNeigbourindices.Execute(curCullData.gridIndex, flooydata.gridSize, false, ref tempint2list);
            //    foreach (int2 neebor in tempint2list)
            //    {
            //        var flatusindicus = TempFlatIndex.Execute(neebor, flooydata.gridSize.y);
            //        CellData neeborcellindex = TempCelldata[flatusindicus];
            //        if (neeborcellindex.bestCost < currbestcost)
            //        {
            //            currbestcost = neeborcellindex.bestCost;
            //            currbestDirection = neeborcellindex.gridIndex - curCullData.gridIndex;
            //            //currbestDirection = curCullData.gridIndex - neeborcellindex.gridIndex;
            //            //Debug.Log($"The currbest cost is: {neeborcellindex.gridIndex} {curCullData.gridIndex}");

            //        }

            //    }               
            //    dudedirection[layer] = currbestDirection;

            //}).ScheduleParallel();


            #region SeparateCodeidea stuff 
            //Entities.WithName("SeperationTestJob").ForEach((in FlowFieldData floofdata) =>
            //{
            //    var gridSize = floofdata.gridSize;

            //    for (int i = 0; i < TempCelldata.Length; i++)
            //    {
            //        CellData currcelldoot = TempCelldata[i];


            //        //var alldirections = new int2[]            
            //        //{            
            //        //    new int2(0, 0), //The current Cell            
            //        //    new int2(0, 1), //Supposedly North             
            //        //    new int2(0, -1), //South?            
            //        //    new int2(1, 0), //East             
            //        //    new int2(-1, 0), //West            
            //        //    new int2(1, 1),  //NorthEast            
            //        //    new int2(-1, 1),  //NorthWest            
            //        //    new int2(1, -1),  // SouthEast            
            //        //    new int2 (-1, -1)  //SouthWest            
            //        //};

            //        var leftrightdirections = new int2[]
            //        {
            //            new int2(1, 0), //East
            //            new int2(-1, 0), //West
            //            //new int2(1, 1),//NorthEast
            //            //new int2(-1, 1),//NorthWest
            //            //new int2(1, -1),
            //            //new int2(-1, -1)

            //        };

            //        var originindexs = new int2[]
            //        {
            //            new int2(0, 0),
            //            new int2(0, 1),
            //            new int2(0, -1)

            //        };

            //        //I'm tempted to just get all the Celldata for all the surrounding NeigbourCells maybe see if that's more efficient or in a loop
            //        //TODO See if getting all neigbour cells first and putting them into an array helps to speed up this process
            //        var originindex = TempCelldata[i].gridIndex;
            //        ushort currbestCost = TempCelldata[i].bestCost;



            //        foreach (int2 upanddowns in originindexs)
            //        {
            //            int2 neeborindex;
            //            //neeborindex = originindex + deerections;
            //            //if(neeborindex.x < 0 || neeborindex.x >= gridSize.x || neeborindex.y < 0 || neeborindex.y >= gridSize.y)                        
            //                //continue;

            //            //var flatusindicus = TempFlatIndex.Execute(neeborindex, gridSize.y);
            //            //CellData neeborcellindex = TempCelldata[flatusindicus];
            //            //if(neeborcellindex.bestCost < )



            //        }

            //        //foreach (int2 curDirection in currentdirections)            //might have to use for with burst
            //        //{
            //        //    int2 neighborIndex;
            //        //    neighborIndex = originIndex + curDirection;
            //        //    neighborIndex = neighborIndex.x < 0 || neighborIndex.x >= gridSize.x || neighborIndex.y < 0 || neighborIndex.y >= gridSize.y ? new int2(-1, -1) : neighborIndex;
            //        //    if (neighborIndex.x >= 0)
            //        //    {
            //        //        results.Add(neighborIndex);//just discard the -1,-1 in the calling method

            //        //    }
            //        //}

            //        //TODO and Notes I feel like this may be a simpler way of creating or finding the grid index
            //        var tempxind = i / gridSize.x;
            //        var tempyind = i % gridSize.y;
            //        Debug.Log("The index values are " + i + "," + tempxind + "," + tempyind);


            //    }

            //}).WithoutBurst().Run();
            #endregion

            Entities.WithName("CopyBackTempCelldata").WithDisposeOnCompletion(TempCelldata).WithDisposeOnCompletion(TempCellBestDirect).ForEach((int entityInQueryIndex, ref CellData celldudes, ref CellsBestDirection tempcellbesty) =>
            {
                celldudes = TempCelldata[entityInQueryIndex];
                tempcellbesty = TempCellBestDirect[entityInQueryIndex];

            }).ScheduleParallel();
         
        }
    }

    public struct ToFlatIndex
    {
        public int Execute(int2 index2D, int height)
        {
            return height * index2D.x + index2D.y;
        }
    }

    public struct CalcintegrationFieldEvent : IComponentData { }


}