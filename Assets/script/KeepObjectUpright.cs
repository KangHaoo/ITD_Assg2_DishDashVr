using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class KeepObjectUpright : MonoBehaviour
{
    private Quaternion initialRotation;
    private Vector3 offset;
    private Transform controllerTransform;

    private void Start()
    {
        // Store the initial rotation of the object
        initialRotation = transform.rotation;
    }

    // Called when the object is picked up
    public void OnSelectEnter(SelectEnterEventArgs args)
    {
        UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor interactor = args.interactorObject as UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor;
        if (interactor != null)
        {
            // Get the controller's transform
            controllerTransform = interactor.transform;

            // Calculate the initial offset between the controller and the object
            offset = transform.position - controllerTransform.position;

            // Align the object to face the desired rotation (270 degrees on Y-axis)
            AlignToTargetRotation();
        }
    }

    // Called every frame when the object is being held
    private void Update()
    {
        if (controllerTransform != null)
        {
            // Update the object's position to stay relative to the controller
            transform.position = controllerTransform.position + offset;

            // Maintain the desired rotation
            AlignToTargetRotation();
        }
    }

    // Align the object to face the desired rotation (270 degrees on Y-axis)
    private void AlignToTargetRotation()
    {
        // Set the desired rotation (facing 270 degrees around Y-axis)
        Quaternion targetRotation = Quaternion.Euler(270, 0, 0);
        transform.rotation = targetRotation;
    }
}
