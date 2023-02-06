using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TaxisStateMachine : MonoBehaviour
{
    // EnumState
    // 1_LeaveHome
    // -> Go to the taxi

    // 2_Eat
    // -> Go to one of the restaurant's drive
    // -> Eat in the car in the parking of the restaurant

    // 2_TakeClient
    // -> Go to the client
    // -> Bring the client to destination (Crazy taxis [fast] or normal taxis [normal speed]

    // 3_GoHome
    // -> Go home

    BaseState currentState;

    [HideInInspector] public GameObject taxisDriver;
    public int driveCount = 0;


    private void Start()
    {
        taxisDriver = this.gameObject;

        currentState = new LeaveHomeState();
        currentState.OnStart(this);
    }

    private void Update()
    {
        currentState.OnUpdate();
    }

    public void OnEnd(BaseState nextStateP)
    {
        currentState = nextStateP;
        currentState.OnStart(this);
    }

    public void RestartDay()
    {
        driveCount = 0;
        currentState = new LeaveHomeState();
        currentState.OnStart(this);
    }
}
