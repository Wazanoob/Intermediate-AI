using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GoToClientState : BaseState
{
    private TaxisStateMachine taxisStateMachine;
    private GameObject taxisCar;
    private GameObject taxisDriver;
    private NavMeshAgent navMeshAgent;

    private GameObject client;

    public override void OnStart(TaxisStateMachine fsm)
    {
        taxisStateMachine = fsm;
        taxisDriver = taxisStateMachine.taxisDriver;
        taxisCar = taxisDriver.GetComponent<TaxisDriver>().taxisCar;
        navMeshAgent = taxisCar.GetComponent<NavMeshAgent>();

        GetRandomClient();

        navMeshAgent = taxisCar.GetComponent<NavMeshAgent>();
        navMeshAgent.SetDestination(client.transform.position);
    }

    public override void OnUpdate()
    {
        if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            OnStateEnd();
        }
    }

    public override void OnStateEnd()
    {
        BaseState nextStep;
        nextStep = new GoToDriveState();

        taxisStateMachine.driveCount++;
        taxisStateMachine.OnEnd(nextStep);
    }

    private void GetRandomClient()
    {
        GameObject[] Clients = GameObject.FindGameObjectsWithTag("Client");
        int random = Random.Range(0, Clients.Length);

        client = Clients[random];
    }
}
