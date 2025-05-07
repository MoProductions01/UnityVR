using System.Collections;
using System.Collections.Generic;
//using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Class for implementing a hinge for a door
/// </summary>
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public abstract class SimpleHingeInteractable : XRSimpleInteractable
{   
    [SerializeField] Vector3 PositionLimits; // Limits for the door
    [SerializeField] private Transform GrabHand; // Our hand that will be doing the grabbing
    private Collider HingeCollider; // Collider for our hinge
    private Vector3 HingePosition; // Position of our hinge
    [SerializeField] bool IsLocked; // Whether or not we are locked
    private const string DEFAULT_LAYER = "Default"; // Strings for our different layers
    private const string GRAB_LAYER = "Grab"; 
   
    protected virtual void Start()
    {
        HingeCollider = GetComponent<Collider>(); // Get our collider reference
    }

    /// <summary>
    /// Tells us our hinge is unlocked from the keypad
    /// </summary>
    public void UnlockHinge()
    {
        IsLocked = false;
    }
    /// <summary>
    /// Locks our hinge
    /// </summary>
    public void LockHinge()
    {
        IsLocked = true;
    }

    protected virtual void Update()
    {
        if (GrabHand != null)
        {   // If we have a valid hand track it
            TrackHand();
        }
    }

    /// <summary>
    /// Callback for when you've grabbed something
    /// </summary>
    /// <param name="args"></param>
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        if (IsLocked == false)
        {   // Only grab if it's unlocked
            base.OnSelectEntered(args);

            GrabHand = args.interactorObject.transform; // Get transform from the arg passed in
            float localAngleY = GrabHand.transform.localEulerAngles.y; // Keep track of y
            if (localAngleY >= 180f)
            {   // Adjust Y value if necessary to work with the system
                localAngleY -= 360f;
            }
        }
    }

    /// <summary>
    /// Call back from when we've stopped grabbing something
    /// </summary>
    /// <param name="args"></param>
    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);

        GrabHand = null; // No longer grabbing so reset hand to null
        ChangeLayerMask(GRAB_LAYER); // Update layer so we can be grabbed
        ResetHinge();
    }

    /// <summary>
    /// Tracks our hand via the oculus controller
    /// </summary>
    private void TrackHand()
    {   
        transform.LookAt(GrabHand, transform.forward); // Update the hinge based on where our grab hand is
        HingePosition = HingeCollider.bounds.center;
        // Below are a bunch of checks to let go of the hinge if we've gone too far
        if (GrabHand.position.x >= HingePosition.x + PositionLimits.x ||
            GrabHand.position.x <= HingePosition.x - PositionLimits.x)
        {
            ReleaseHinge();
        }
        else if (GrabHand.position.y >= HingePosition.y + PositionLimits.y ||
            GrabHand.position.y <= HingePosition.y - PositionLimits.y)
        {
            ReleaseHinge();
        }
        else if (GrabHand.position.z >= HingePosition.z + PositionLimits.z ||
            GrabHand.position.z <= HingePosition.z - PositionLimits.z)
        {
            ReleaseHinge();
        }
    }

    /// <summary>
    ///  Releases the hinge from our grab hand
    /// </summary>
    public void ReleaseHinge()
    {
        ChangeLayerMask(DEFAULT_LAYER); // this gives us OnSelectExited
        if(GrabHand == null)
        {
            ResetHinge();
            ChangeLayerMask(GRAB_LAYER);
        }
    }

    protected abstract void ResetHinge();

    /// <summary>
    /// Helper function for changing the layer mask
    /// </summary>
    /// <param name="mask"></param>
    private void ChangeLayerMask(string mask)
    {
        interactionLayers = InteractionLayerMask.GetMask(mask);
    }




}
