# AR Sensors
 A Hololens 2 app that allows a user to walk around the Link Lab at UVA and view digital twins of smart sensors.
 Demo Video: https://www.youtube.com/watch?v=dAbrqEMBI2A

Installation and Setup:
 To use the unity project, you must have Unity version 2020.3.47f1 with Universal Windows Platform Build Support and Windows Build Support (IL2CPP).  You also need Visual Studio with .NET desktop development, Desktop development with C++, Universal Windows Platform (UWP) development, and Game development with Unity.  Then you can download this project into the root directory of your machine.  The project has to be in the root directory (or very near to the root directory) because Windows has a maximum path length of 255 characters, which some of the libraries in this project are close to exceeding.

Developer Mode:
 In the Unity project, you can set the app to developer mode by viewing the "AzureSpatialAnchors" object in the main scene, activating the "Develop Azure Spatial Anchors Script" script, and deactivating the "Azure Spatial Anchors Script".  This allows you to create new Azure Spatial Anchors so that you can add devices in new locations to the app.  You can also select "Show_anchors" in the "Azure Spatial Anchors Script" to have the app show where all the Azure Spacial Anchors are placed.  For more details on how to use the the project, view this video: https://www.youtube.com/watch?v=UTv_cNzbPP8

App Usage:
 While in the app, the Hololens will automatically recognize areas around the Link Lab and populate sensor holograms.  Once a sensor hologram is populated, it will automatically update while the user is gazing at it.  The user is also able to use three lengths of air taps in order to correct errors that the app may have with placing sensors.  A tap of 0.5 to 2 seconds will delete any spacial anchor (along with the associated sensor holograms) within 2 meters of the user's hand.  A 2 to 10 second tap will once more query Azure for the Azure Spatial Anchors that have not yet been found, including any that have been deleted.  A 10+ second tap will clear all of the sensors found in the app and query for the Azure Spatial Anchors again.
