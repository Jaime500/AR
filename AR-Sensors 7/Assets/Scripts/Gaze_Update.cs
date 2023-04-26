using Microsoft.MixedReality.Toolkit;
using UnityEngine;

public class Gaze_Update : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        CoreServices.InputSystem.EyeGazeProvider.GazeTarget.GetComponentInChildren<Sensor_Update>().UpdateSensor();
    }
}
