using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public abstract class BaseState
{
    abstract public void OnStart(TaxisStateMachine fsm);
    abstract public void OnUpdate();
    abstract public void OnStateEnd();
}
