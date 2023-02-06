using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class FastFood : MonoBehaviour
{
    [SerializeField] private Transform driveThrough;
    [SerializeField] private GameObject[] parkingSlotsIndex;

    [HideInInspector] public bool[] isSlotAvailable;

    public GameObject GetParkingSlot(int i) { return parkingSlotsIndex[i]; }
    public Transform GetDriveThrough() { return driveThrough; }

    // Start is called before the first frame update
    void Start()
    {
        isSlotAvailable = new bool[parkingSlotsIndex.Length];

        for (int i = 0; i < parkingSlotsIndex.Length; i++)
        {
            isSlotAvailable[i] = true;
        }
    }
}
