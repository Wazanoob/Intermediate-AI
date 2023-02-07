using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.AI;

public class GoToClientState : BaseState
{
    private TaxisStateMachine taxisStateMachine;
    private GameObject taxisCar;
    private GameObject taxisDriver;
    private NavMeshAgent navMeshAgent;

    private Vector3 clientPosition;
    private GameObject myClient;
    private bool clientOnBoard = false;

    private float maxSpeed = 5.0f;

    public override void OnStart(TaxisStateMachine fsm)
    {
        taxisStateMachine = fsm;
        taxisDriver = taxisStateMachine.taxisDriver;
        taxisCar = taxisDriver.GetComponent<TaxisDriver>().taxisCar;
        navMeshAgent = taxisCar.GetComponent<NavMeshAgent>();
        
        GetRandomClient();

        navMeshAgent = taxisCar.GetComponent<NavMeshAgent>();
        navMeshAgent.SetDestination(clientPosition);

        if (UnityEngine.Random.Range(0, 2) < 1)
        {
            maxSpeed = 7.5f;
            navMeshAgent.speed = maxSpeed;
        }
    }

    public override void OnUpdate()
    {
        if (!clientOnBoard && Vector3.Distance(taxisCar.transform.position, clientPosition) <= 10.0f)
        {
            if (navMeshAgent.velocity.magnitude < 0.2f) 
            {
                TaxisDriver taxis = taxisDriver.GetComponent<TaxisDriver>();
                Client client = myClient.GetComponent<Client>();

                client.SetClientDestination(taxis.passengerSit.position);

                if (Vector3.Distance(taxis.passengerSit.position, myClient.transform.position) <= 1.25f)
                {
                    Vector3 newDestination;
                    RandomPoint(client.randomDestination, 6.0f, out newDestination);
                    navMeshAgent.SetDestination(newDestination);

                    clientOnBoard = true;
                }
            }
        }

        if (clientOnBoard && Vector3.Distance(taxisCar.transform.position, navMeshAgent.destination) <= 10.0f)
        {
            if (navMeshAgent.velocity.magnitude < 0.1f)
            {
                TaxisDriver taxis = taxisDriver.GetComponent<TaxisDriver>();
                Client client = myClient.GetComponent<Client>();

                client.DropClient(taxis.passengerSit.position);
                clientOnBoard = false;
                OnStateEnd(); 
            }
        }
    }

    public override void OnStateEnd()
    {
        navMeshAgent.speed = 5.0f;

        BaseState nextStep;

        if (UnityEngine.Random.Range(0, 3) < 2)
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

    private void GetRandomClient()
    {
        bool succed = false;
        int attempt = 0;
        do
        {
            GameObject[] clients = GameObject.FindGameObjectsWithTag("Client");
  
            int random = UnityEngine.Random.Range(0, clients.Length);

            Client client = clients[random].GetComponent<Client>();

            for (int i = 0; i < clients.Length; i++)
            {
                if (client.clientAvailable)
                {
                    RandomPoint(clients[random].transform.position, 6.0f, out clientPosition);
                    myClient = clients[random];

                    client.RandomDestination();

                    succed = true;
                    return;
                }
            }

            attempt++;
            if (attempt >= 10)
            {
                return;
            }
        } while (!succed);
    }

    bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {
        for (int i = 0; i < 30; i++)
        {
            Vector3 randomPoint = center + UnityEngine.Random.insideUnitSphere * range;
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
