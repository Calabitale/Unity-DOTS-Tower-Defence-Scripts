using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using UnityEngine.AI;
using Unity.Mathematics;

public class PlayerController : MonoBehaviour
{
    public Transform mouseposobject;

    public EntityManager entManger;
    public EntityQuery PlayerInput;

    [SerializeField] private LayerMask SelectLayer;

    public EntityQuery gridQuery;

    public GameObject NavmeeshObstakle;

    public GetCellIndexFromWorldPos GetCellindexpos;

    public ToFlatIndex flatusindex;
    // Start is called before the first frame update
    void Start()
    {
        entManger = World.DefaultGameObjectInjectionWorld.EntityManager;
        PlayerInput = entManger.CreateEntityQuery(ComponentType.ReadOnly<PlayerInput>());

        gridQuery = entManger.CreateEntityQuery(ComponentType.ReadOnly<GridposBuilder>(), typeof(GridSpaceOccupied));
        GetCellindexpos = new GetCellIndexFromWorldPos();
        flatusindex = new ToFlatIndex();
        
    }

    // Update is called once per frame
    void Update()
    {
        var CurrentInput = PlayerInput.CalculateEntityCount() > 0 ? PlayerInput.GetSingleton<PlayerInput>() : default;

        var tempcurrentinput = new Vector3(CurrentInput.CursorPos.x, CurrentInput.CursorPos.y, 0);

        var raydude = Camera.main.ScreenPointToRay(tempcurrentinput);

        var tempgridpos = gridQuery.ToComponentDataArray<GridposBuilder>(Allocator.Temp);

        var tempGridocced = gridQuery.ToComponentDataArray<GridSpaceOccupied>(Allocator.Temp);

        var tempgridentity = gridQuery.ToEntityArray(Allocator.Temp);

        //RaycastHit raycasthoot;

        if (CurrentInput.fireinput)
        {
            if(Physics.Raycast(raydude, out RaycastHit raycasthoot))            
            {
                int maxcolliders = 100;
                Collider[] hitcolluders = new Collider[maxcolliders];
                //TODO Perhaps I should try using the simple OverLapSphere method it seems when there is to many it kind of just makes them dissapear instantly or something not sure why that is will need to investigate
                var numcolliders = Physics.OverlapSphereNonAlloc(raycasthoot.point, 5, hitcolluders, SelectLayer);
                //var nomonolots = Physics.OverlapSphere(raycasthoot.point, 5, SelectLayer);
                for(int i = 0; i < numcolliders; i++)
                {
                    
                    var nazmeesh = hitcolluders[i].GetComponent<NavMeshAgent>();
                    nazmeesh.enabled = false;
                    Rigidbody riggdude = hitcolluders[i].GetComponent<Rigidbody>();
                    riggdude.AddExplosionForce(100f, raycasthoot.point, 5f, 20f, ForceMode.Impulse);
                   
                    var someink = hitcolluders[i].GetComponent<HobbitMover>();
                    someink.DisableHobbit();

                }                   
                    
                mouseposobject.position = raycasthoot.point;                    
                    
                //raycasthoot.transform.gameObject.SetActive(false);

                

            }
           

        }        

        if(CurrentInput.Calcotherlayer)
        {
            if (Physics.Raycast(raydude, out RaycastHit raycasthoot))
            {


                int2 tempgrid = new int2(33, 33);

                var tempgrdirotpos = new float3(-50, 0, -50);
                var tempminuspos = (float3)raycasthoot.point - tempgrdirotpos;

                var tempval = GetCellindexpos.Execute(tempminuspos, tempgrid, 3); //TODO Get This to work where the grid coordinates are not at 0,0,0 I just need to minus the rootpoint from the current point as seen here I should probably make this internal to the method
                var tompvool = flatusindex.Execute(tempval, 33);

                if(tempGridocced[tompvool].boolValue)
                {
                   
                    return;
                }
                else
                {
                  

                    GridSpaceOccupied temprgid = new GridSpaceOccupied();
                    temprgid = tempGridocced[tompvool];
                    temprgid.boolValue = true;
                    //tempGridocced[tompvool] = temprgid;



                    entManger.SetComponentData<GridSpaceOccupied>(tempgridentity[tompvool], temprgid);
                    Instantiate(NavmeeshObstakle, tempgridpos[tompvool].float3val, Quaternion.identity);
                }                
                //Debug.Log("the tompvool" + tompvool);
                //var isoccupido = entManger.HasComponent<GridSpaceOccupied>(tempgridentity[tompvool]);
                //Debug.Log("The is occupied vale is " + isoccupido);
                //Debug.Log("The tempval is " + raycasthoot.point + tempval);
                

                //entManger.AddComponent(tempgridentity[tompvool], typeof(GridSpaceOccupied));
                //entManger.AddComponent<GridSpaceOccupied>(tempgridentity[tompvool]);
                //Instantiate(NavmeeshObstakle, tempgridpos[tompvool].float3val, Quaternion.identity);
                //Debug.Log("It created the thingy");
            }
            //if (NavmeeshObstakle.activeSelf)
            //{
            //    NavmeeshObstakle.gameObject.SetActive(false);
            //}
            //else
            //{
            //    NavmeeshObstakle.gameObject.SetActive(true);
            //}



        }

        tempGridocced.Dispose();
        tempgridentity.Dispose();
        tempgridpos.Dispose();

        
    }
}

public partial class PlayerActionsSystem : SystemBase
{


    protected override void OnCreate()
    {
        base.OnCreate();
    }

    protected override void OnStartRunning()
    {
        
    }

    protected override void OnUpdate()
    {
        



    }
}