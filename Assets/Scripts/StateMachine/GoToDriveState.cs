using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.AI;

public class GoToDriveState : BaseState
{
    private TaxisStateMachine taxisStateMachine;
    private GameObject taxisCar;
    private GameObject taxisDriver;
    private NavMeshAgent navMeshAgent;

    private Vector3 drivePosition;
    private Vector3 parkingSlotPosition;

    private float waitTimer = 0;
    private const float TIME_TO_ORDER = 5.0f;
    private const float TIME_TO_EAT = 9.0f;
    private bool isEating = false;

    public override void OnStart(TaxisStateMachine fsm)
    {
        taxisStateMachine = fsm;
        taxisDriver = taxisStateMachine.taxisDriver;
        taxisCar = taxisDriver.GetComponent<TaxisDriver>().taxisCar;
        navMeshAgent = taxisCar.GetComponent<NavMeshAgent>();

        isEating = false;
        waitTimer = 0;

        GetRandomRestaurant();

        navMeshAgent = taxisCar.GetComponent<NavMeshAgent>();
        navMeshAgent.SetDestination(drivePosition);
    }

    public override void OnUpdate()
    {
        if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            // Random Delay
            waitTimer += Time.deltaTime;

            if (!isEating && waitTimer >= TIME_TO_ORDER)
            {
                isEating = true;
                waitTimer = 0;
                navMeshAgent.SetDestination(parkingSlotPosition); 
            }
            else if (isEating && waitTimer >= TIME_TO_EAT)
            {
                OnStateEnd();
            }
        }
    }

    public override void OnStateEnd()
    {
        BaseState nextStep;

        if (taxisStateMachine.driveCount >= 5 && Random.Range(0, 2) < 1)
        {
            nextStep = new GoHomeState();
            taxisStateMachine.OnEnd(nextStep);
            return;
        }

        taxisStateMachine.driveCount++;
        nextStep = new GoToClientState();
        taxisStateMachine.OnEnd(nextStep);
    }

    private void GetRandomRestaurant()
    {
        bool succed = false;

        do
        {
            GameObject[] Restaurants = GameObject.FindGameObjectsWithTag("FastFood");
            int random = Random.Range(0, Restaurants.Length);
            
            FastFood fastFood = Restaurants[random].GetComponent<FastFood>();

            for (int i = 0; i < fastFood.isSlotAvailable.Length; i++)
            {
                if (fastFood.isSlotAvailable[i])
                {
                    drivePosition = fastFood.GetDriveThrough().position;
                    parkingSlotPosition = fastFood.GetParkingSlot(i).transform.position;
                    fastFood.isSlotAvailable[i] = false;
                    succed = true;
                }
            }

        } while(!succed);
    }
}
