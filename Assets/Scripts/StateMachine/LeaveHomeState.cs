using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LeaveHomeState : BaseState
{
    private TaxisStateMachine taxisStateMachine;
    private NavMeshAgent navMeshAgent;
    private GameObject taxisDriver;

    private Vector3 carPosition;
    private const float SLEEPING_TIME = 50;
    private float timer = 0;
    private bool isSleeping = true;

    public override void OnStart(TaxisStateMachine fsm)
    {
        taxisStateMachine = fsm;
        taxisDriver = taxisStateMachine.taxisDriver;

        navMeshAgent = taxisDriver.GetComponent<NavMeshAgent>();
        carPosition = taxisDriver.GetComponent<TaxisDriver>().driverSit.position;

        timer = Random.Range(0, SLEEPING_TIME);
        taxisDriver.GetComponent<MeshRenderer>().enabled = false;
        navMeshAgent.SetDestination(carPosition);
        navMeshAgent.isStopped = true;
    }

    public override void OnUpdate()
    {
        timer += Time.deltaTime;

        if (timer >= SLEEPING_TIME && isSleeping)
        {
            taxisDriver.GetComponent<MeshRenderer>().enabled = true;
            isSleeping = false;
            navMeshAgent.isStopped = false;
        }

        if (!isSleeping && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            taxisDriver.GetComponent<MeshRenderer>().enabled = false;
            OnStateEnd();
        }
    }

    public override void OnStateEnd()
    {
        int random = Random.Range(0, 3);

        BaseState nextStep;

        if (Random.Range(0, 3) < 2)
        {
            nextStep = new GoToClientState();
        }
        else
        {
            nextStep = new GoToDriveState();
        }

        taxisStateMachine.driveCount++;
        taxisStateMachine.OnEnd(nextStep);
    }
}
