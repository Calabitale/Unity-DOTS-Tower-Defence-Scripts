using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class CameraFollowMono : MonoBehaviour//TODO I should change this all into a SystemBase and get references from the camera in a ComponentObject
{
    public EntityQuery GridCursorQuery;
    public EntityManager entMananger;
    EntityQuery PlayerQuery;

    public float smoothspid; //TODO If I want this to lerp really slowly I should use a value something like 0.0125f
    public float3 offset;

    // Start is called before the first frame updateUpdat
    void Start()
    {
        
       //TODO DO I want the camera to follow the cursor I could I separate it from the cursor and just have it controlled manually with the cursor keys or analog
    
        entMananger = World.DefaultGameObjectInjectionWorld.EntityManager;
        GridCursorQuery = entMananger.CreateEntityQuery(ComponentType.ReadOnly<CursorSquareTag>(), typeof(Translation));
        PlayerQuery = entMananger.CreateEntityQuery(ComponentType.ReadOnly<CameraMoveDat>());
        
        Cursor.lockState = CursorLockMode.Confined;
    }


    void Update()//TODO Should I change this to LateUpdate or FixedUpdate it seems to wortk ok so far but maybe it will not always and because of the order will this update fire before the cursor is update but that is in a system so which goes first this or the systembase
    {//
        if (PlayerQuery.CalculateEntityCount() == 0)
            return;

        var cameramovebool = PlayerQuery.GetSingleton<CameraMoveDat>();//TODO I need to fix this error I need to either slow down the running of this script or something
        if (!cameramovebool.boolVal)
            return;

        var GridcursorTarg = GridCursorQuery.CalculateEntityCount() > 0 ? GridCursorQuery.GetSingleton<Translation>() : default;

        float3 desiredpos = GridcursorTarg.Value + offset;
        float3 smoothpos = math.lerp(transform.position, desiredpos, smoothspid);

        transform.position = smoothpos;


    }
}
