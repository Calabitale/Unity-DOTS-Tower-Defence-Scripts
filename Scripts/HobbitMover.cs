using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.Mathematics;

public class HobbitMover : MonoBehaviour
{
    public Transform Tagget;
    public NavMeshAgent agent;

    public Transform NearestBlockingObstacle;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    

    public void DisableHobbit()
    {
        StartCoroutine(Disable());
    }

    IEnumerator Disable()
    {
        //play your sound
        yield return new WaitForSeconds(3); //waits 3 seconds
        this.gameObject.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
       
        var isitonamesh = this.gameObject.GetComponent<NavMeshAgent>();
        if(isitonamesh.isOnNavMesh)
        {
            NavMeshPath path = new NavMeshPath();
            agent.CalculatePath(Tagget.position, path);
            if (path.status == NavMeshPathStatus.PathPartial)                
            {
                agent.SetDestination(NearestBlockingObstacle.position) ;
            }                
            else                
            {                    
                agent.SetDestination(Tagget.position);
                
            }
            
            

             
        }
        
       
        

        if(math.distance(this.transform.position, Tagget.position) < 3f)
        {
            if(this.gameObject.activeSelf)
            {
               
                
                this.gameObject.SetActive(false);
            }

        }
    }
}
