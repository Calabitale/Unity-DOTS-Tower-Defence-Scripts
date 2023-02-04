using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine.AI;

public class SpawningHobbits : MonoBehaviour
{
	public int PoolIndex;
    private ObjectPooler OP;
    public Transform[] SpawnPoints;

	public int NumbtoIncreaseSpawn;

	public EntityManager tempEntmanger;
	public EntityQuery Timetick;

	private float SpawnRatetimer;

	public float SpawnRateMaxTimer;  //TODO I have to make sure this value isn't less than the time tick otherwise it will sometimes skip it.  This just tells it how long to take before increasing the number to spawn. 

	public int[] NumbtoSpawnatPonts;//I should have it as an area like this I think maybe I could initialise it to the size of the Spawnpoints and have thos spawnpoints at zero instead of constantly changing the array size

	public bool CalcNumbtoSpawnIntenal;

	public int MaxNumbSpawnCycle;

    // Start is called before the first frame update

    private void Awake()
    {
		CalcNumbtoSpawnIntenal = true;
    }
    void Start()
    {
		MaxNumbSpawnCycle = 300;
        OP = ObjectPooler.SharedInstance;
        PoolIndex = 0;
        NumbtoIncreaseSpawn = 0;
		SpawnRateMaxTimer = 3;
		SpawnRatetimer = SpawnRateMaxTimer;
        tempEntmanger = World.DefaultGameObjectInjectionWorld.EntityManager;
        Timetick = tempEntmanger.CreateEntityQuery(ComponentType.ReadOnly<TimeTickPartialEvent>());
    }

   
    void Update()
    {
		SpawnRatetimer -= Time.deltaTime;
      
        if (SpawnRatetimer <= 0)
        {
			NumbtoIncreaseSpawn += 1;
			if (NumbtoIncreaseSpawn > MaxNumbSpawnCycle)
				return;
            
            SpawnRatetimer = SpawnRateMaxTimer;            
        }

        var tuddly = Timetick.CalculateEntityCount();

		if (tuddly == 0)
			return;

        UnityEngine.Random.InitState((int)System.DateTime.Now.Ticks);

		var numbofenabledHobbits = OP.GetAllPooledObjects(0);
		var activeobjsnumb = 0;
		for(int i = 0; i < numbofenabledHobbits.Count; i++)
        {
			if(numbofenabledHobbits[i].activeSelf)
            {
				activeobjsnumb++;
            }

        }
		#region The SpawnPointsArray Bit
		
		if (CalcNumbtoSpawnIntenal)
		{
			int numberpool = NumbtoIncreaseSpawn;//This limits 
			NumbtoSpawnatPonts = new int[SpawnPoints.Length];

			for (int p = 0; p < NumbtoSpawnatPonts.Length; p++)
			{
				NumbtoSpawnatPonts[p] = UnityEngine.Random.Range(0, numberpool);
				numberpool -= NumbtoSpawnatPonts[p];				
			}
		}		
		#endregion

		int totaltospawn = 0;
		for(int a = 0; a < NumbtoSpawnatPonts.Length; a++)
        {
			totaltospawn += NumbtoSpawnatPonts[a];
        }		

		if (activeobjsnumb + totaltospawn > OP.GetAllPooledObjects(0).Count)//TODO Stops it from erroring from trying to Spawn Stuff when there isn't enough stuff
			return;       

        for (int z = 0; z < SpawnPoints.Length; z++)
		{			
			for (int i = 0; i < NumbtoSpawnatPonts[z]; i++)
            {				
				//Random.InitState((int)System.DateTime.Now.Ticks);
				GameObject Hobbity = OP.GetPooledObject(PoolIndex);
				
				var nazmeesh = Hobbity.GetComponent<NavMeshAgent>();
				nazmeesh.enabled = true;

				float xPos = UnityEngine.Random.Range(-10f, 10f);
                float zPos = UnityEngine.Random.Range(-2f, 2f);
                Hobbity.transform.position = SpawnPoints[z].transform.position + xPos * Vector3.right + zPos * Vector3.forward;
                Hobbity.SetActive(true);
            }
        }
    }

	public Transform[] PicktheSpawnPoints(Transform [] spawnpoints, int []chosenSpawnPoints)
    {
		List<Transform> listspawns = new List<Transform>();
		
		foreach(int point in chosenSpawnPoints)
        {
			listspawns.Add(spawnpoints[point]);
        }

		Transform[] returnTrans = listspawns.ToArray();

		return returnTrans;
    }


}


public partial class SpawingHobbitSystem : SystemBase
{
	public int PoolIndex;
	private ObjectPooler OP;
	public Transform SpawnPoint;

	public float3 otherSpawnpoint;
	public int numbtospawn;


	protected override void OnStartRunning()
    {
		OP = ObjectPooler.SharedInstance;
		PoolIndex = 0;
		numbtospawn = 20;
		
		otherSpawnpoint = new float3(1f, 0f, 48f);
		Enabled = false;
	}

    protected override void OnUpdate()
    {
		UnityEngine.Random.InitState((int)System.DateTime.Now.Ticks);
		//OP = ObjectPooler.SharedInstance;
		if (Input.GetKeyDown(KeyCode.Space))
		{
			for (int i = 0; i < numbtospawn; i++)
			{
				//Random.InitState((int)System.DateTime.Now.Ticks);
				GameObject Hobbity = OP.GetPooledObject(PoolIndex);				
				//Ball.transform.rotation = SpawnPoint.transform.rotation;

				float xPos = UnityEngine.Random.Range(-10f, 10f);
				float zPos = UnityEngine.Random.Range(-10f, 10f);
				Hobbity.transform.position = SpawnPoint.transform.position + xPos * Vector3.right + zPos * Vector3.forward;
				Hobbity.SetActive(true);
			}
		}


	}

}
