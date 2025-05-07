using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Class extending simple hinge to make a door
/// </summary>
public class DoorInteractable : SimpleHingeInteractable
{  
    public UnityEvent OnOpen; // Even for when it's opened

    [SerializeField] ComboLock ComboLock;   // The keypad for unlocking the door
    [SerializeField] Transform DoorObject;  // The transform for the door
    [SerializeField] Vector3 RotationLimits;  // Limits for the door rotation

    [SerializeField] Collider ClosedCollider;   // Colliders used when the door is closed or open
    [SerializeField] Collider OpenCollider;

    [SerializeField] private bool IsClosed; // Are we closed
    [SerializeField] private Vector3 StartRotation; // Starting values for the rotations
    [SerializeField]private float StartAngleX;
    
    [SerializeField] private bool IsOpen;   // Are we open
    [SerializeField] private Vector3 EndRotation;   // Ending values for the rotations
    [SerializeField] private float EndAngleX;

    protected override void Start()
    {
        base.Start();

        StartRotation = transform.localEulerAngles; // Get the starting values based on what the default values in the scene are
        StartAngleX = GetAngle(StartRotation.x);
        
        if (ComboLock != null)
        {   // IF we have a valud combo lock set up the callbacks
            ComboLock.UnlockAction += OnUnlocked;
            ComboLock.LockAction += OnLocked;
        }
        else
        {
            // Debug.LogError("ERROR: No ComboLock in DoorInteractable.cs");
        }
    }

    /// <summary>
    /// Called when the door is unlocked
    /// </summary>
    private void OnUnlocked()
    {
        UnlockHinge(); // Unlock our hinge
    }

    /// <summary>
    /// Called when the door is locked
    /// </summary>
    private void OnLocked()
    {
        LockHinge(); // Lock our hinge
    }

    protected override void Update()
    {
        base.Update();

        if (DoorObject != null)
        {   // Adjust the door's y value based on the grab transform but leave x and z alone
            DoorObject.localEulerAngles = new Vector3(
                DoorObject.localEulerAngles.x,
                transform.localEulerAngles.y,
                DoorObject.localEulerAngles.z
            );
        }

        if (isSelected)
        {
            CheckLimits(); // Make sure we're not going too far in either direction
        }
    }

    /// <summary>
    /// Function to make sure the doors don't go where they're not supposed to
    /// </summary>
    private void CheckLimits()
    {   // if we're here then the interactable is selected and moving the hinge
        IsClosed = false;
        IsOpen = false;
        float localAngleX = GetAngle(transform.localEulerAngles.x);
        if (localAngleX >= StartAngleX + RotationLimits.x ||
           localAngleX <= StartAngleX - RotationLimits.x)
        {
            ReleaseHinge();

        }
    } 

    /// <summary>
    /// Helper function for adjusting the angle to work with Unity's system
    /// </summary>
    /// <param name="angle"></param>
    /// <returns></returns>
    private float GetAngle(float angle)
    {
        if (angle >= 180f)
        {
            angle -= 360f;
        }
        return angle;
    }

    /// <summary>
    /// Resets the hinge to it's starting point
    /// </summary>
    protected override void ResetHinge()
    {
        if (IsClosed == true)
        {   // IF we're closed then go back to the starting rotation
            transform.localEulerAngles = StartRotation;
        }
        else if(IsOpen == true)
        {
            // If we're open then go to the end rotation and invoke the callback
            transform.localEulerAngles = EndRotation;
            OnOpen?.Invoke();
        }
        else
        {   // Reset to start
            transform.localEulerAngles = new Vector3(StartAngleX,
                    transform.localEulerAngles.y, transform.localEulerAngles.z);
        }

    }

    /// <summary>
    /// Called when one of the triggers used in the hinge system is entered
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other == ClosedCollider)
        {   // We've entered the close collider trigger so let the game know we're 
            // closed and release the hinge
            IsClosed = true;
            ReleaseHinge(); // this will eventually call ResetHinge()
        }
        else if(other == OpenCollider)
        {   // We've hit the make open collider so let the game know
            // and release the hinge
            IsOpen = true;
            Debug.Log("Set IsOpen to true name: " + this.name);
            ReleaseHinge(); // this will eventually call ResetHinge()
        }
    }

    // some debug code
    // string s = "Pre localAngleX: " + localAngleX + ", ";
    // s += "Post localAngleX: " + localAngleX;
    //Debug.Log(s);

    /*
    #if false
    bool HasPrinted = false;
    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (DoorObject != null)
        {
            string s = "before: " + DoorObject.localEulerAngles.ToString("F3") + ", ";
            DoorObject.localEulerAngles = new Vector3(DoorObject.localEulerAngles.x,
                transform.localEulerAngles.y, DoorObject.localEulerAngles.z);
            s += "after: " + DoorObject.localEulerAngles.ToString("F3") + ", ";
            Debug.Log("s: " + s);
           /* if(HasPrinted == false) 
            {
                HasPrinted = true;
                Debug.Log("s: " + s);
            }*/
        }

      /*  if (isSelected == true)
        {
            CheckLimits();
        }
    }
    #endif
    */
