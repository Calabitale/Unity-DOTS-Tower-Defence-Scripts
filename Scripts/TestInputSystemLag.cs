using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Entities;
using Unity.Mathematics;

public class TestInputSystemLag : MonoBehaviour
{
    private PlayerInputActions _inputMapping;

//private void Awake() => _inputMapping = new PlayerInputActions();


    //private void OnEnable() => _inputMapping.Enable();
    //private void OnDisable() => _inputMapping.Disable();

    public void Update()
    {
        //Debug.Log("Is this even running");
    }

    public void TestJump(InputAction.CallbackContext context)
    {

        Debug.Log("The player has jumped" +
            context.performed.ToString() );
    }

   
}

public partial class TestInputSyst : SystemBase
{
    private PlayerInputActions inputstuff;
    GameObject TheTestInpObj;

    float CursorMoveSpeed;

    protected override void OnCreate()
    {
        inputstuff = new PlayerInputActions();
        inputstuff.Enable();
    }

    protected override void OnStartRunning()
    {
        TheTestInpObj = GameObject.Find("TestInputObject");
        CursorMoveSpeed = 20;
    }

    protected override void OnUpdate()
    {
        var someval = inputstuff.PlayerControls.DirectionalControls.ReadValue<Vector2>();

        var currentmovpos = someval * CursorMoveSpeed * Time.DeltaTime;

        if (someval.magnitude > 0)        
        {
            Vector3 tempcurr = new Vector3(currentmovpos.x, 0, currentmovpos.y);
            TheTestInpObj.transform.position += tempcurr;
        }
    }

}
