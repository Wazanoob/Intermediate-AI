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

    private FastFood currentFastFood;
    private int currentParkingSlot;
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

        navMeshAgent.speed = 5.0f;
    }

    public override void OnUpdate()
    {
        if (Vector3.Distance(taxisCar.transform.position, navMeshAgent.destination) <= navMeshAgent.stoppingDistance)
        {
            waitTimer += Time.deltaTime;

            if (!isEating && waitTimer >= TIME_TO_ORDER)
            {
                isEating = true;
                waitTimer = 0;
                navMeshAgent.SetDestination(parkingSlotPosition);
            }
            else if (isEating && waitTimer >= TIME_TO_EAT)
            {
                currentFastFood.isSlotAvailable[currentParkingSlot] = true;
                OnStateEnd();
            }
        }

        CheckCollision();
    }

    public override void OnStateEnd()
    {
        BaseState nextStep;

        if (taxisStateMachine.driveCount >= 5 && Random.Range(0, 2) < 1)
        {
            nextStep = new GoHomeState();
            taxisStateMachine.OnEnd(nextStep);
        }

        taxisStateMachine.driveCount++;
        nextStep = new GoToClientState();
        taxisStateMachine.OnEnd(nextStep);
    }

    private void GetRandomRestaurant()
    {
        bool succed = false;
        int attempt = 0;
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
                    currentFastFood = fastFood;
                    currentParkingSlot = i;
                    fastFood.isSlotAvailable[i] = false;
                    succed = true;
                    return;
                }
            }

            attempt++;
            if (attempt >= 10)
            {
                return;
            }
        } while(!succed);
    }

    private void CheckCollision()
    {
        RaycastHit hit;

        Vector3 positionOffset = new Vector3(taxisCar.transform.position.x, taxisCar.transform.position.y + 0.5f, taxisCar.transform.position.z);

        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(positionOffset, taxisCar.transform.TransformDirection(Vector3.forward), out hit, 4.0f))
        {
            Debug.DrawRay(positionOffset, taxisCar.transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);

            if (hit.transform.CompareTag("Car"))
            {
                Debug.Log("CARCRASH");
                Debug.DrawRay(positionOffset, taxisCar.transform.TransformDirection(Vector3.forward) * hit.distance, Color.red);

                if(hit.transform.GetComponent<NavMeshAgent>().velocity !=Vector3.zero) 
                {
                    navMeshAgent.speed = 0.5f;
                }else
                {
                    navMeshAgent.speed = 0.0f;
                }
            }
        }
        else
        {
            navMeshAgent.speed = 5.0f;
        }
    }
}
