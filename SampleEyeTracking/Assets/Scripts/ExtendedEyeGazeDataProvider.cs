// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using UnityEngine;
using System;
using Microsoft.MixedReality.OpenXR;
using Microsoft.MixedReality.EyeTracking;

/// <summary>
/// This class provides access to the Extended Eye Gaze Tracking API 
/// Values are given in Unity world space or relative to the main camera
/// </summary>
[DisallowMultipleComponent]
public class ExtendedEyeGazeDataProvider : MonoBehaviour
{
    public enum GazeType
    {
        Left,
        Right,
        Combined
    }

    public class GazeReading
    {
        public Vector3 EyePosition;
        public Vector3 GazeDirection;
        public GazeReading() { }
        public GazeReading(Vector3 position, Vector3 direction)
        {
            EyePosition = position;
            GazeDirection = direction;
        }
    }

    private Camera _mainCamera;
    private EyeGazeTrackerWatcher _watcher;
    private EyeGazeTracker _eyeGazeTracker;
    private EyeGazeTrackerReading _eyeGazeTrackerReading;
    private System.Numerics.Vector3 _trackerSpaceGazeOrigin;
    private System.Numerics.Vector3 _trackerSpaceGazeDirection;
    private GazeReading _gazeReading = new GazeReading();
    private GazeReading _transformedGazeReading = new GazeReading();
    private bool _gazePermissionEnabled;
    private bool _readingSucceeded;
    private SpatialGraphNode _eyeGazeTrackerNode;
    private Pose _eyeGazeTrackerPose;
    private Matrix4x4 _eyeGazeTrackerSpaceToPlayspace = new Matrix4x4();
    private Matrix4x4 _eyeGazeTrackerSpaceToWorld = new Matrix4x4();
    private Transform _mixedRealityPlayspace;

    /// <summary>
    /// Get the current reading for the requested GazeType, relative to the main camera
    /// Will return null if unable to return a valid reading
    /// </summary>
    /// <param name="gazeType"></param>
    /// <returns></returns>
    public GazeReading GetCameraSpaceGazeReading(GazeType gazeType)
    {
        return GetCameraSpaceGazeReading(gazeType, DateTime.Now);
    }

    /// <summary>
    /// Get the reading for the requested GazeType at the given TimeStamp, relative to the main camera
    /// Will return null if unable to return a valid reading
    /// </summary>
    /// <param name="gazeType"></param>
    /// <param name="timestamp"></param>
    /// <returns></returns>
    public GazeReading GetCameraSpaceGazeReading(GazeType gazeType, DateTime timestamp)
    {
        if (GetWorldSpaceGazeReading(gazeType, timestamp) == null)
        {
            return null;
        }

        _transformedGazeReading.EyePosition = _mainCamera.transform.InverseTransformPoint(_gazeReading.EyePosition);
        _transformedGazeReading.GazeDirection = _mainCamera.transform.InverseTransformDirection(_gazeReading.GazeDirection).normalized;

        return _transformedGazeReading;
    }

    /// <summary>
    /// Get the current reading for the requested GazeType
    /// Will return null if unable to return a valid reading
    /// </summary>
    /// <param name="gazeType"></param>
    /// <returns></returns>
    public GazeReading GetWorldSpaceGazeReading(GazeType gazeType)
    {
        return GetWorldSpaceGazeReading(gazeType, DateTime.Now);
    }

    /// <summary>
    /// Get the reading for the requested GazeType at the given TimeStamp
    /// Will return null if unable to return a valid reading
    /// </summary>
    /// <param name="gazeType"></param>
    /// <param name="timestamp"></param>
    /// <returns></returns>
    public GazeReading GetWorldSpaceGazeReading(GazeType gazeType, DateTime timestamp)
    {
        if (!_gazePermissionEnabled || _eyeGazeTracker == null)
        {
            return null;
        }

        _eyeGazeTrackerReading = _eyeGazeTracker.TryGetReadingAtTimestamp(timestamp);
        if (_eyeGazeTrackerReading == null)
        {
            Debug.LogWarning($"Unable to get eyeGazeTrackerReading at: {timestamp.ToLongTimeString()}");
            return null;
        }

        _readingSucceeded = false;
        switch (gazeType)
        {
            case GazeType.Left:
                {
                    _readingSucceeded = _eyeGazeTrackerReading.TryGetLeftEyeGazeInTrackerSpace(out _trackerSpaceGazeOrigin, out _trackerSpaceGazeDirection);
                    break;
                }
            case GazeType.Right:
                {
                    _readingSucceeded = _eyeGazeTrackerReading.TryGetRightEyeGazeInTrackerSpace(out _trackerSpaceGazeOrigin, out _trackerSpaceGazeDirection);
                    break;
                }
            case GazeType.Combined:
                {
                    _readingSucceeded = _eyeGazeTrackerReading.TryGetCombinedEyeGazeInTrackerSpace(out _trackerSpaceGazeOrigin, out _trackerSpaceGazeDirection);
                    break;
                }
        }
        if (!_readingSucceeded)
        {
            return null;
        }

        // Get tracker pose under _mixedRealityPlayspace, and construct the matrix to transform gaze data from tracker space to _mixedRealityPlayspace
        if (!_eyeGazeTrackerNode.TryLocate(_eyeGazeTrackerReading.SystemRelativeTime.Ticks, out _eyeGazeTrackerPose))
        {
            return null;
        }
        _eyeGazeTrackerSpaceToPlayspace.SetTRS(_eyeGazeTrackerPose.position, _eyeGazeTrackerPose.rotation, Vector3.one);

        // Construct the matrix to transform gaze data from tracker space to Unity world space
        _eyeGazeTrackerSpaceToWorld = (_mixedRealityPlayspace != null) ?
                _mixedRealityPlayspace.localToWorldMatrix * _eyeGazeTrackerSpaceToPlayspace :
                _eyeGazeTrackerSpaceToPlayspace;

        // Transform gaze data from tracker space to Unity world space 
        _gazeReading.EyePosition = _eyeGazeTrackerSpaceToWorld.MultiplyPoint3x4(ToUnity(_trackerSpaceGazeOrigin));
        _gazeReading.GazeDirection = _eyeGazeTrackerSpaceToWorld.MultiplyVector(ToUnity(_trackerSpaceGazeDirection));

        return _gazeReading;
    }

    private async void Start()
    {
        _mainCamera = Camera.main;
        _mixedRealityPlayspace = _mainCamera.transform.parent;

        Debug.Log("Initializing ExtendedEyeTracker");
#if ENABLE_WINMD_SUPPORT
        Debug.Log("Triggering eye gaze permission request");
        // This function call may not required if you already use MRTK in your project 
        _gazePermissionEnabled = await AskForEyePosePermission();
#else
        // Always enable when running in editor
        _gazePermissionEnabled = true;
#endif

        if (!_gazePermissionEnabled)
        {
            Debug.LogError("Gaze is disabled");
            return;
        }

        _watcher = new EyeGazeTrackerWatcher();
        _watcher.EyeGazeTrackerAdded += _watcher_EyeGazeTrackerAdded;
        _watcher.EyeGazeTrackerRemoved += _watcher_EyeGazeTrackerRemoved;
        await _watcher.StartAsync();
    }

    private void _watcher_EyeGazeTrackerRemoved(object sender, EyeGazeTracker e)
    {
        Debug.Log("EyeGazeTracker removed");
        _eyeGazeTracker = null;
    }

    private async void _watcher_EyeGazeTrackerAdded(object sender, EyeGazeTracker e)
    {
        Debug.Log("EyeGazeTracker added");
        try
        {
            await e.OpenAsync(true);
            _eyeGazeTracker = e;
            var supportedFrameRates = _eyeGazeTracker.SupportedTargetFrameRates;
            foreach (var frameRate in supportedFrameRates)
            {
                Debug.Log($"  supportedFrameRate: {frameRate.FramesPerSecond}");
            }

            // Set to highest framerate, it is 90FPS at this time
            _eyeGazeTracker.SetTargetFrameRate(supportedFrameRates[supportedFrameRates.Count - 1]);
            _eyeGazeTrackerNode = SpatialGraphNode.FromDynamicNodeId(e.TrackerSpaceLocatorNodeId);
        }
        catch (Exception ex)
        {
            Debug.LogError("Unable to open EyeGazeTracker\r\n" + ex.ToString());
        }
    }

#if ENABLE_WINMD_SUPPORT
    /// <summary>
    /// Triggers a prompt to let the user decide whether to permit using eye tracking 
    /// </summary>
    private async System.Threading.Tasks.Task<bool> AskForEyePosePermission()
    {
        var accessStatus = await Windows.Perception.People.EyesPose.RequestAccessAsync();
        Debug.Log("Eye gaze access status: " + accessStatus.ToString());
        return accessStatus == Windows.UI.Input.GazeInputAccessStatus.Allowed;
    }
#endif

    private static UnityEngine.Vector3 ToUnity(System.Numerics.Vector3 v) => new UnityEngine.Vector3(v.X, v.Y, -v.Z);
}
