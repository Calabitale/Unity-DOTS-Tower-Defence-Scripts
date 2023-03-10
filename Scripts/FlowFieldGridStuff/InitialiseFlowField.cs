using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using DotsFlowField;
using Unity.Collections;


namespace DotsFlowField
{

    //[UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class InitialiseFlowField : SystemBase
    {
        //public int FieldGridsize;
        public int2 FieldGridsize;

        public float CellDiameter;
        public float CellRadii;
              
        public Entity FlowfieldEntity;
        
        public float3 FieldOriginPosition;

        public EntityArchetype FlowFieldDataType;

        public EntityQuery Flowdfieldquery;

        public EntityQuery TestObstaclequery;

        public int NumbofDestinations;

        public int PrelimLayers;

        protected override void OnCreate()
        {
            TestObstaclequery = GetEntityQuery(ComponentType.ReadOnly<ObstacleCollisionVerts>());

            //TODO This minimum size should be just a bit larger than the smallest entity that uses the flowfield, So if an entity size is 1 then this should probably be 1.1 or something like that, so I need to automate this somehow
            CellDiameter = 2f;
            //TODO This should be big enough to cover the whole screen I will need a better way of calculating this, like defining the player and fitting this within that somehow but will just do this like this for now, also need to automate this
            //FieldGridsize = 50;  
            FieldGridsize.x = 70; //TODO Need to keep these values under 255 otherwise it will not work 255 is the max byte value, nope it needs to be way under 255, I'll stick to max values of 100 either way
            FieldGridsize.y = 70;

            NumbofDestinations = 10;

            PrelimLayers = 4;
           
            //TODO The flowfield needs to be at the origin currently need to figure out how to make it work any where in the world
            FieldOriginPosition = new float3(0, 1, 0);
         

            FlowFieldDataType = EntityManager.CreateArchetype(
                typeof(FlowfieldMemberOf),
                typeof(CellData),
                typeof(FlowfieldVertPointsBuff),
                typeof(CellsBestDirection),
                typeof(CellBestDirectionBuff),                
                typeof(StaticOptimizeEntity)//TODO This seems to a new component that does exactly what it says opmtimises should probably on all static environment objects things that arent ever going to move
                );
            RequireSingletonForUpdate<NewFlowFieldEvent>();
        }

        protected override void OnStartRunning()
        {
           
            
          
        }

        protected override void OnUpdate()
        {

            if(HasSingleton<FlowFieldData>())//A check to make sure there isn't already a flowfield just in chase I didn't already do a check elsewhere
            {
                return;
            }
           
            CellRadii = CellDiameter / 2;
        
            var tempfieldgridpos = FieldOriginPosition;

            //Batch creating entitys like this is way
            NativeArray<Entity> celldataents = new NativeArray<Entity>(FieldGridsize.x * FieldGridsize.y, Allocator.Temp);
            EntityManager.CreateEntity(FlowFieldDataType, celldataents);

            float3 firstgridpoint = new float3();
            int tempcount = 0;
            for (int x = 0; x < FieldGridsize.x; x++)
            {
                for (int y = 0; y < FieldGridsize.y; y++)
                {                    
                    
                    float3 cellWorldPos = new float3(CellDiameter * x + CellRadii, 0, CellDiameter * y + CellRadii);
                    float3 othercellworldpos = cellWorldPos + tempfieldgridpos;

                    CellData newCelldata = new CellData
                    {
                        worldPos = othercellworldpos,
                        gridIndex = new int2(x, y),
                        cost = new byte(),
                        bestCost = ushort.MaxValue,
                        //bestDirection = int2.zero

                    };

                    CellsBestDirection celliac = new CellsBestDirection{ bestDirection = int2.zero};

                    EntityManager.SetComponentData<CellsBestDirection>(celldataents[tempcount], celliac);
                    EntityManager.SetComponentData<CellData>(celldataents[tempcount], newCelldata);

                    var BestDirectionBuff = EntityManager.GetBuffer<CellBestDirectionBuff>(celldataents[tempcount]);

                    int2 pew = new int2(int2.zero);

                  

                    //Todo I will just set this to one to start whenever adding to it later on I need to make sure it doesn't become larger than the destination otherwise bad things could happen
                    for (int p = 0; p < PrelimLayers; p++)
                    {
                        BestDirectionBuff.Add(pew);
                    }
              
                    var tempboff = EntityManager.GetBuffer<FlowfieldVertPointsBuff>(celldataents[tempcount]);

                    //TODO Need to just get the min and max points, does it matter which is the minimun and maximum as in does it matter in which order they are 

                    //tempboff.Add(new float3(othercellworldpos.x - CellRadii, othercellworldpos.y - CellRadii, othercellworldpos.z + CellRadii));
                    //tempboff.Add(new float3(othercellworldpos.x + CellRadii, othercellworldpos.y - CellRadii, othercellworldpos.z + CellRadii));
                    //tempboff.Add(new float3(othercellworldpos.x + CellRadii, othercellworldpos.y - CellRadii, othercellworldpos.z - CellRadii));
                    tempboff.Add(new float3(othercellworldpos.x - CellRadii, othercellworldpos.y - CellRadii, othercellworldpos.z - CellRadii));

                    //tempboff.Add(new float3(othercellworldpos.x - CellRadii, othercellworldpos.y + CellRadii, othercellworldpos.z + CellRadii));
                    tempboff.Add(new float3(othercellworldpos.x + CellRadii, othercellworldpos.y + CellRadii, othercellworldpos.z + CellRadii));
                    //tempboff.Add(new float3(othercellworldpos.x + CellRadii, othercellworldpos.y + CellRadii, othercellworldpos.z - CellRadii));
                    //tempboff.Add(new float3(othercellworldpos.x - CellRadii, othercellworldpos.y + CellRadii, othercellworldpos.z - CellRadii));

                    FlowfieldMemberOf tempflowfield = new FlowfieldMemberOf
                    {

                        myflowfeld = CurrentFlowfield.Flowfield1
                    };

                    if (tempcount == 0)
                    {
                        firstgridpoint = othercellworldpos;

                    }

                    EntityManager.SetComponentData<FlowfieldMemberOf>(celldataents[tempcount], tempflowfield);

                    tempcount++;
                }
            }            

            celldataents.Dispose();

            var tempdude = EntityManager.CreateEntity(typeof(FlowFieldData));

            FlowFieldData realflowfield = new FlowFieldData
            {
                cellRadius = CellRadii,
                currenfield = CurrentFlowfield.Flowfield1,
                gridSize = FieldGridsize,
                Destination = new float3(0, 0, 0),
                MaxFlowLayers = 10,
                GridOrigin = firstgridpoint
            };

            EntityManager.SetComponentData<FlowFieldData>(tempdude, realflowfield);

            if (!HasSingleton<DestinationCellData>())//TODO Not sure if this a good place to create it but will leave this here for now may move it somewhere else later
            {
                EntityManager.CreateEntity(typeof(DestinationCellData));
            }   
            
            if(!HasSingleton<CellBDLayerToCalc>())
            {
                EntityManager.CreateEntity(typeof(CellBDLayerToCalc));                
            }

            EntityQuery tempentityquery = GetEntityQuery(typeof(CellDestinationsBuffer));
            
            if(tempentityquery.CalculateEntityCount() == 0)
            {
                var dude = EntityManager.CreateEntity(typeof(CellDestinationsBuffer));
                var daboof = GetBuffer<CellDestinationsBuffer>(dude);
                var startval = new int2(0, 0);
                for (int i = 0; i < PrelimLayers; i++)
                {
                    daboof.Add(startval);
                }
            } 

            

            if (HasSingleton<NewFlowFieldEvent>())
            {
                var tempent = GetSingletonEntity<NewFlowFieldEvent>();

                EntityManager.DestroyEntity(tempent);
            }

            if (!HasSingleton<CalculateCellCostEventTag>())
            {
                
                EntityManager.CreateEntity(typeof(CalculateCellCostEventTag));
            }

            //if(!HasSingleton<CalcintegrationFieldEvent>())
            //{
            //    EntityManager.CreateEntity(typeof(CalcintegrationFieldEvent));
            //}
        }
    }
}

