// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using UnityEngine;

public class EETDataProviderTest : MonoBehaviour
{
    [SerializeField]
    private GameObject LeftGazeObject;
    [SerializeField]
    private GameObject RightGazeObject;
    [SerializeField]
    private GameObject CombinedGazeObject;
    [SerializeField]
    private GameObject CameraRelativeCombinedGazeObject;
    [SerializeField]
    private ExtendedEyeGazeDataProvider extendedEyeGazeDataProvider;

    private DateTime timestamp;
    private ExtendedEyeGazeDataProvider.GazeReading gazeReading;


    void Update()
    {
        timestamp = DateTime.Now;

        // positioning for left gaze object
        gazeReading = extendedEyeGazeDataProvider.GetWorldSpaceGazeReading(ExtendedEyeGazeDataProvider.GazeType.Left, timestamp);
        if (gazeReading != null)
        {
            // position gaze object 1.5 meters out from the gaze origin along the gaze direction
            LeftGazeObject.transform.position = gazeReading.EyePosition + 1.5f * gazeReading.GazeDirection;
            LeftGazeObject.SetActive(true);
        }
        else
        {
            LeftGazeObject.SetActive(false);
        }

        // positioning for right gaze object
        gazeReading = extendedEyeGazeDataProvider.GetWorldSpaceGazeReading(ExtendedEyeGazeDataProvider.GazeType.Right, timestamp);
        if (gazeReading != null)
        {
            // position gaze object 1.5 meters out from the gaze origin along the gaze direction
            RightGazeObject.transform.position = gazeReading.EyePosition + 1.5f * gazeReading.GazeDirection;
            RightGazeObject.SetActive(true);
        }
        else
        {
            RightGazeObject.SetActive(false);
        }

        // positioning for combined gaze object
        gazeReading = extendedEyeGazeDataProvider.GetWorldSpaceGazeReading(ExtendedEyeGazeDataProvider.GazeType.Combined, timestamp);
        if (gazeReading != null)
        {
            // position gaze object 1.5 meters out from the gaze origin along the gaze direction
            CombinedGazeObject.transform.position = gazeReading.EyePosition + 1.5f * gazeReading.GazeDirection;
            CombinedGazeObject.SetActive(true);
        }
        else
        {
            CombinedGazeObject.SetActive(false);
        }

        // positioning for camera relative gaze cube
        gazeReading = extendedEyeGazeDataProvider.GetCameraSpaceGazeReading(ExtendedEyeGazeDataProvider.GazeType.Combined, timestamp);
        if (gazeReading != null)
        {
            // position gaze object 1.5 meters out from the gaze origin along the gaze direction
            CameraRelativeCombinedGazeObject.transform.localPosition = gazeReading.EyePosition + 1.5f * gazeReading.GazeDirection;
            CameraRelativeCombinedGazeObject.SetActive(true);
        }
        else
        {
            CameraRelativeCombinedGazeObject.SetActive(false);
        }

    }
}
