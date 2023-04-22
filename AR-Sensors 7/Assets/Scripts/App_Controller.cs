using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.WorldLocking.Core;
using UnityEngine;

public class App_Controller : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        CoreServices.InputSystem.EyeGazeProvider.GazeTarget.GetComponentInChildren<Sensor_Update>().UpdateSensor();
    }

    //private void OnApplicationQuit()
    //{
    //    WorldLockingManager.GetInstance().Reset();
    //}
}
