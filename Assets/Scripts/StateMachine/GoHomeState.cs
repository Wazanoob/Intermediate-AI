using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GoHomeState : BaseState
{
    private TaxisStateMachine taxisStateMachine;
    private GameObject taxisCar;
    private GameObject taxisDriver;
    private NavMeshAgent navMeshAgent;

    bool isWalking = false;
    Vector3 homePosition;

    public override void OnStart(TaxisStateMachine fsm)
    {
        taxisStateMachine = fsm;
        taxisDriver = taxisStateMachine.taxisDriver;
        taxisCar = taxisDriver.GetComponent<TaxisDriver>().taxisCar;
        navMeshAgent = taxisCar.GetComponent<NavMeshAgent>();


        homePosition = taxisStateMachine.taxisDriver.GetComponent<TaxisDriver>().homeLocation.position;
        Vector3 point;

        if (RandomPoint(homePosition, 6.0f, out point))
        {
            Debug.DrawRay(point, Vector3.up, Color.blue, 1.0f);
            navMeshAgent.SetDestination(point);
        }
    }

    public override void OnUpdate()
    {
        if (!isWalking && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            taxisDriver.transform.position = taxisDriver.GetComponent<TaxisDriver>().driverSit.position;
            navMeshAgent = taxisDriver.GetComponent<NavMeshAgent>();

            taxisDriver.GetComponent<MeshRenderer>().enabled = true;
            isWalking = true;
            navMeshAgent.SetDestination(homePosition);
        }
        else if (isWalking && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            isWalking = false;
            OnStateEnd();
        }
    }

    public override void OnStateEnd()
    {
        BaseState nextStep;
        nextStep = new LeaveHomeState();

        taxisStateMachine.OnEnd(nextStep);
    }

    bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {
        for (int i = 0; i < 30; i++)
        {
            Vector3 randomPoint = center + Random.insideUnitSphere * range;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }
        result = Vector3.zero;
        return false;
    }
}
