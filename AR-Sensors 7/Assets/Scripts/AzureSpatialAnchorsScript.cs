using Microsoft.Azure.SpatialAnchors;
using Microsoft.Azure.SpatialAnchors.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.XR;


[RequireComponent(typeof(SpatialAnchorManager))]
public class AzureSpatialAnchorsScript : MonoBehaviour
{
    public GameObject sensor;

    /// <summary>
    /// Used to distinguish short taps and long taps
    /// </summary>
    private float[] _tappingTimer = { 0, 0 };

    /// <summary>
    /// Main interface to anything Spatial Anchors related
    /// </summary>
    private SpatialAnchorManager _spatialAnchorManager = null;

    /// <summary>
    /// Used to keep track of all GameObjects that represent a found or created anchor
    /// </summary>
    private Dictionary<GameObject, string> _foundAnchorGameObjects = new Dictionary<GameObject, string>();

    /// <summary>
    /// Details for creating a sensor hologram
    /// </summary>
    struct SensorInformation
    {
        public Vector3 position;
        public Quaternion rotation;
        public string deviceId;
        public int deviceClass;
        public SensorInformation(Vector3 position, Quaternion rotation, string deviceId, int deviceClass)
        {
            this.position = position;
            this.rotation = rotation;
            this.deviceId = deviceId;
            this.deviceClass = deviceClass;
        }
    }

    /// <summary>
    /// Used to keep track of all the created Anchor IDs
    /// </summary>
    private readonly Dictionary<string, List<SensorInformation>> _AnchorIDs = new Dictionary<string, List<SensorInformation>>()
    {
        { "e4d6bf30-79e4-4ba9-a56d-7e65055f88a0", new List<SensorInformation>() {new SensorInformation (new Vector3(-1.25f, 0, 2), new Quaternion(0, 0, 0, 0), "awair-4019", 0) } }, // by sink (201)
        { "3348a9b0-c726-485e-9e22-4a962dada085", new List<SensorInformation>() {new SensorInformation (new Vector3(0, 0, 1), new Quaternion(0, 0, 0, 0), "70886b123507", 0) } },
        { "1eea0fd2-9d0a-4365-9b2e-83e406f68a86", new List<SensorInformation>() {new SensorInformation (new Vector3(-0.75f, 0, 1.5f), new Quaternion(0, 0, 0, 0), "70886b125d88", 0) } }, // arena north wall near TV (203)
        { "f0b94dd6-cf1a-426f-870c-b933924dfa8d", new List<SensorInformation>() { // kitchen (208)
            new SensorInformation (new Vector3(1.25f, 0, 0), Quaternion.AngleAxis(90, Vector3.up), "70886b12335b", 0),
            new SensorInformation (new Vector3(0, 0, -1.5f), Quaternion.AngleAxis(180, Vector3.up), "awair-3446", 0),
        } },
        { "51213bfc-3e7e-434c-bcf6-ca08a4a37e16", new List<SensorInformation>() {new SensorInformation (new Vector3(0, 0, 1), new Quaternion(0, 0, 0, 0), "70886b123507", 0) } },
        { "1c9d29da-1b17-4d43-8d8f-38a1d837a556", new List<SensorInformation>() {new SensorInformation (new Vector3(0, 0, 1), new Quaternion(0, 0, 0, 0), "70886b123507", 0) } },
        { "9843ccca-397e-4b28-b500-ee40571822e7", new List<SensorInformation>() {new SensorInformation (new Vector3(0, 0, 1), new Quaternion(0, 0, 0, 0), "70886b123507", 0) } },
        { "09ce0a2a-6ed9-4bb8-8754-9811df9d7592", new List<SensorInformation>() {new SensorInformation (new Vector3(0, 0, 1), new Quaternion(0, 0, 0, 0), "70886b123507", 0) } },
        { "f0408c3c-59b6-4fe6-a163-f612574558c5", new List<SensorInformation>() {new SensorInformation (new Vector3(0, 0, 1), new Quaternion(0, 0, 0, 0), "70886b123507", 0) } },
        { "a0d2bf22-fe2f-4640-9a6b-61b437336739", new List<SensorInformation>() { // 217
            new SensorInformation (new Vector3(2, 0, 1.8f), new Quaternion(0, 0, 0, 0), "70886b123507", 0),
            new SensorInformation (new Vector3(0.25f, 0, -1.75f), Quaternion.AngleAxis(180, Vector3.up), "018a2c81", 1),
            new SensorInformation (new Vector3(-1.25f, 0, -2), new Quaternion(0, 0, 0, 0), "018169f7", 3),
            new SensorInformation (new Vector3(-.2f, 1, 0), new Quaternion(0, 0, 0, 0), "050621c6", 4),
            new SensorInformation (new Vector3(1.2f, 1, 0), new Quaternion(0, 0, 0, 0), "050d68bf", 5),
        } },
        { "59aff259-44e9-4b8e-9935-28a21770eccc", new List<SensorInformation>() {
            new SensorInformation (new Vector3(0, 0.3f, -2.6f), Quaternion.AngleAxis(180, Vector3.up), "70886b123992", 0), // Entrance 1
            new SensorInformation (new Vector3(0, -0.3f, -2.6f), Quaternion.AngleAxis(180, Vector3.up), "70886b1268c6", 0), // Entrance 2
            new SensorInformation (new Vector3(-2.3f, -0.3f, 0.25f), Quaternion.AngleAxis(270, Vector3.up), "70886b1275bc", 0), // Wall 3
            new SensorInformation (new Vector3(-2.3f, -0.3f, 1.25f), Quaternion.AngleAxis(270, Vector3.up), "70886b12778c", 0), // Wall 4
            new SensorInformation (new Vector3(-2.3f, 0.3f, 1.25f), Quaternion.AngleAxis(270, Vector3.up), "70886b12762f", 0), // Wall 5
            new SensorInformation (new Vector3(-2.3f, 0.3f, 0.25f), Quaternion.AngleAxis(270, Vector3.up), "70886b126a04", 0), // Wall 6
            new SensorInformation (new Vector3(0.3f, -0.5f, 0), new Quaternion(0, 0, 0, 0), "70886b123735", 0), // Table 1
        } }, // (211)
        { "44f586e3-2271-408d-a0e6-b8057e2a6757", new List<SensorInformation>() {
            new SensorInformation (new Vector3(0,  0.3f, 3), new Quaternion(0, 0, 0, 0), "70886b1234a0", 0), // Wall 1
            new SensorInformation (new Vector3(0,  -0.3f, 3), new Quaternion(0, 0, 0, 0), "70886b123b81", 0), // Back Wall (wall 2)
            new SensorInformation (new Vector3(1.5f,  0.3f, 1.5f), Quaternion.AngleAxis(90, Vector3.up), "70886b127730", 0), // TV 1
            new SensorInformation (new Vector3(1.5f,  0.3f, -1.5f), Quaternion.AngleAxis(90, Vector3.up), "70886b1238b5", 0), // TV 2
            new SensorInformation (new Vector3(1.5f,  -0.3f, -1.5f), Quaternion.AngleAxis(90, Vector3.up), "70886b12395b", 0), // TV 3
            new SensorInformation (new Vector3(1.5f,  -0.3f, 1.5f), Quaternion.AngleAxis(90, Vector3.up), "70886b127639", 0), // TV 4
            new SensorInformation (new Vector3(-1.5f, -0.5f, 0), new Quaternion(0, 0, 0, 0), "70886b123b83", 0), // Table
            new SensorInformation (new Vector3(-0.5f, -0.5f, 0), new Quaternion(0, 0, 0, 0), "70886b123962", 0), // Table 3
        } }, // (211)
        { "d7a52e8e-0bc1-4707-899e-a80efdfc7e3a", new List<SensorInformation>() {new SensorInformation (new Vector3(0, 0, 0.5f), new Quaternion(0, 0, 0, 0), "70886b123c0b", 0) } }, // elevator (210)
        { "f19def10-eca3-4efd-9fac-0da4c7f4e09d", new List<SensorInformation>() {new SensorInformation (new Vector3(0, 0, 1), new Quaternion(0, 0, 0, 0), "70886b123507", 0) } },
        { "0074e2e5-1f5b-46f6-bb22-2bc26d3f7556", new List<SensorInformation>() {new SensorInformation (new Vector3(-0.25f, 0, -0.75f), Quaternion.AngleAxis(180, Vector3.up), "70886b122593", 0) } }, // team room (217)
        { "e22df7d4-0f27-4738-888f-908096daa38d", new List<SensorInformation>() {
            new SensorInformation (new Vector3(-1.25f, 0, -.3f), Quaternion.AngleAxis(270, Vector3.up), "70886b125f1e", 0), // awair
            new SensorInformation (new Vector3(1.3f, 0, 1.8f), new Quaternion(0, 0, 0, 0), "018a2167", 1), // temp/humidity
        } }, // Heydarian office (281)
        { "16ed98d4-8846-4cf0-ada7-779843f0cbbe", new List<SensorInformation>() {
            new SensorInformation (new Vector3(0.3f, 0, 1.3f), new Quaternion(0, 0, 0, 0), "70886b123039", 0), // awair
            new SensorInformation (new Vector3(-1.2f, 0, -1.25f), Quaternion.AngleAxis(180, Vector3.up), "018984f9", 1), // temp/humidity
            new SensorInformation (new Vector3(0.5f, 1.25f, 0), new Quaternion(0, 0, 0, 0), "050621a8", 4), // light
            new SensorInformation (new Vector3(0.1f, 1.25f, 0), new Quaternion(0, 0, 0, 0), "050d69ce", 5), // dual motion
        } }, // Campbell office (281)
    };

    // <Start>
    // Start is called before the first frame update
    async void Start()
    {
        _spatialAnchorManager = GetComponent<SpatialAnchorManager>();
        _spatialAnchorManager.LogDebug += (sender, args) => Debug.Log($"ASA - Debug: {args.Message}");
        _spatialAnchorManager.Error += (sender, args) => Debug.LogError($"ASA - Error: {args.ErrorMessage}");
        _spatialAnchorManager.AnchorLocated += SpatialAnchorManager_AnchorLocated;

        await _spatialAnchorManager.StartSessionAsync();
        LocateAnchor();
    }
    // </Start>

    // <Update>
    // Update is called once per frame
    async void Update()
    {

        //Check for any air taps from either hand
        for (int i = 0; i < 2; i++)
        {
            InputDevice device = InputDevices.GetDeviceAtXRNode((i == 0) ? XRNode.RightHand : XRNode.LeftHand);
            if (device.TryGetFeatureValue(CommonUsages.primaryButton, out bool isTapping))
            {
                if (!isTapping)
                {
                    //Stopped Tapping or wasn't tapping
                    if (.5f < _tappingTimer[i] && _tappingTimer[i] < 2f)
                    {
                        //User has been tapping for less than 2 sec. Get hand position and call ShortTap
                        if (device.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 handPosition))
                        {
                            ShortTap(handPosition);
                        }
                    }
                    else if (2f < _tappingTimer[i] && _tappingTimer[i] < 10f)
                    {
                        //User has been tapping for less than 8 sec. Get hand position and call ShortTap
                        await MediumTap();
                    }
                    _tappingTimer[i] = 0;
                }
                else
                {
                    _tappingTimer[i] += Time.deltaTime;
                    if (_tappingTimer[i] >= 10f)
                    {
                        //User has been air tapping for at least 10sec. Get hand position and call LongTap
                        LongTap();
                        _tappingTimer[i] = -float.MaxValue; // reset the timer, to avoid retriggering if user is still holding tap
                    }
                }
            }

        }
    }
    // </Update>

    // <ShortTap>
    /// <summary>
    /// Called when a user is air tapping for a short time 
    /// </summary>
    /// <param name="handPosition">Location where tap was registered</param>
    private void ShortTap(Vector3 handPosition)
    {
        if (IsAnchorNearby(handPosition, out GameObject anchorGameObject))
        {
            //Delete nearby Anchor
            DeleteAnchor(anchorGameObject);
        }
    }
    // </ShortTap>

    // <DeleteAnchor>
    /// <summary>
    /// Deleting the anchor GameObject
    /// </summary>
    /// <param name="anchorGameObject">Anchor GameObject that is to be deleted</param>
    private async void DeleteAnchor(GameObject anchorGameObject)
    {
        await MediumTap();
        //Remove reference
        _foundAnchorGameObjects.Remove(anchorGameObject);
        Destroy(anchorGameObject);

    }
    // </DeleteAnchor>

    // <IsAnchorNearby>
    /// <summary>
    /// Returns true if an Anchor GameObject is within 2m of the received reference position
    /// </summary>
    /// <param name="position">Reference position</param>
    /// <param name="anchorGameObject">Anchor GameObject within 2m of received position. Not necessarily the nearest to this position. If no AnchorObject is within 2m, this value will be null</param>
    /// <returns>True if a Anchor GameObject is within 2m</returns>
    private bool IsAnchorNearby(Vector3 position, out GameObject anchorGameObject)
    {
        anchorGameObject = null;

        if (_foundAnchorGameObjects.Count <= 0)
        {
            return false;
        }

        //Iterate over existing anchor gameobjects to find the nearest
        var (distance, closestObject) = _foundAnchorGameObjects.Keys.Aggregate(
            new Tuple<float, GameObject>(Mathf.Infinity, null),
            (minPair, gameobject) =>
            {
                Vector3 gameObjectPosition = gameobject.transform.position;
                float distance = (position - gameObjectPosition).magnitude;
                return distance < minPair.Item1 ? new Tuple<float, GameObject>(distance, gameobject) : minPair;
            });

        if (distance <= 2)
        {
            //Found an anchor within 2m
            anchorGameObject = closestObject;
            return true;
        }
        else
        {
            return false;
        }
    }
    // </IsAnchorNearby>

    // <MediumTap>
    /// <summary>
    /// Called when a user is air tapping for a long time (>=4 sec)
    /// </summary>
    private async Task MediumTap()
    {
        if (_spatialAnchorManager.IsSessionStarted)
        {
            _spatialAnchorManager.DestroySession();
            
        }
        //Start session and search for all Anchors
        await _spatialAnchorManager.StartSessionAsync();
        LocateAnchor();
    }
    // </MediumTap>

    // <LongTap>
    /// <summary>
    /// Called when a user is air tapping for a long time (>=4 sec)
    /// </summary>
    private async void LongTap()
    {
        RemoveAllAnchorGameObjects();
        if (_spatialAnchorManager.IsSessionStarted)
        {
            _spatialAnchorManager.DestroySession();

        }
        //Start session and search for all Anchors
        await _spatialAnchorManager.StartSessionAsync();
        LocateAnchor();
    }
    // </LongTap>

    // <RemoveAllAnchorGameObjects>
    /// <summary>
    /// Destroys all Anchor GameObjects
    /// </summary>
    private void RemoveAllAnchorGameObjects()
    {
        foreach (var anchorGameObject in _foundAnchorGameObjects.Keys)
        {
            Destroy(anchorGameObject);
        }
        _foundAnchorGameObjects.Clear();
    }
    // </RemoveAllAnchorGameObjects>

    // <LocateAnchor>
    /// <summary>
    /// Looking for anchors with ID in _createdAnchorIDs
    /// </summary>
    private void LocateAnchor()
    {
        if (_AnchorIDs.Count > _foundAnchorGameObjects.Count)
        {
            //Create watcher to look for all stored anchor IDs
            Debug.Log($"ASA - Creating watcher to look for {_AnchorIDs.Count} spatial anchors");
            AnchorLocateCriteria anchorLocateCriteria = new AnchorLocateCriteria();
            List<string> ids = _AnchorIDs.Keys.ToList();
            foreach ( string id in _foundAnchorGameObjects.Values ) 
            {
                ids.Remove( id );
            }
            anchorLocateCriteria.Identifiers = ids.ToArray();
            _spatialAnchorManager.Session.CreateWatcher(anchorLocateCriteria);
            Debug.Log($"ASA - Watcher created!");
        }
    }
    // </LocateAnchor>

    // <SpatialAnchorManagerAnchorLocated>
    /// <summary>
    /// Callback when an anchor is located
    /// </summary>
    /// <param name="sender">Callback sender</param>
    /// <param name="args">Callback AnchorLocatedEventArgs</param>
    private void SpatialAnchorManager_AnchorLocated(object sender, AnchorLocatedEventArgs args)
    {
        Debug.Log($"ASA - Anchor recognized as a possible anchor {args.Identifier} {args.Status}");

        if (args.Status == LocateAnchorStatus.Located)
        {
            //Creating and adjusting GameObjects have to run on the main thread. We are using the UnityDispatcher to make sure this happens.
            UnityDispatcher.InvokeOnAppThread(() =>
            {
                // Read out Cloud Anchor values
                CloudSpatialAnchor cloudSpatialAnchor = args.Anchor;

                if (IsAnchorNearby(cloudSpatialAnchor.GetPose().position, out GameObject anchorObject))
                {
                    Debug.Log($"Anchor too close to an existing anchor {args.Identifier}");
                    return;
                }

                //Create GameObject
                GameObject anchorGameObject = Instantiate(new GameObject());

                // Link to Cloud Anchor
                anchorGameObject.AddComponent<CloudNativeAnchor>().CloudToNative(cloudSpatialAnchor);
                _foundAnchorGameObjects.Add(anchorGameObject, args.Identifier);

                Debug.Log("Sensor list length: " + _AnchorIDs[args.Identifier].Count);
                foreach (SensorInformation sensorInfo in _AnchorIDs[args.Identifier])
                {
                    Debug.Log("Creating sensor");
                    GameObject sensorObject = Instantiate(sensor, anchorGameObject.transform);
                    sensorObject.GetComponentInChildren<Sensor_Update>().deviceId = sensorInfo.deviceId;
                    sensorObject.GetComponentInChildren<Sensor_Update>().deviceClass = sensorInfo.deviceClass;
                    Debug.Log("Sensor ID: " + sensorObject.GetComponentInChildren<Sensor_Update>().deviceId);
                    Debug.Log("Sensor type: " + sensorObject.GetComponentInChildren<Sensor_Update>().deviceClass);
                    sensorObject.transform.localPosition = sensorInfo.position;
                    sensorObject.transform.localRotation = sensorInfo.rotation;
                }
                Debug.Log("Sensors Created");

            });
        }
    }
    // </SpatialAnchorManagerAnchorLocated>
}