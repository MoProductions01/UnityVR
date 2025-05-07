
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;


// This class handles the key/drawer section of the world
public class DrawerInteractable : XRGrabInteractable
{
    [SerializeField] Transform DrawerTransform; // Transform that we move around when interacting
    [SerializeField] XRSocketInteractor KeySocket; // Interactor for key socket
    [SerializeField] bool IsLocked; // Keeps track of whether the drawer is locked

    private Transform ParentTransform; // The dresser
    private const string DEFAULT_LAYER = "Default"; // Different layers depending of it the drawer is grabbed or not
    private const string GRAB_LAYER = "Grab";
    private bool IsGrabbed; // Are we grabbed?
    private Vector3 LimitPosition; // Default position for the drawer
    [SerializeField] float DrawerLimitZ = .8f; // Max Z delta the drawer can move
    [SerializeField] private Vector3 LimitDistance = new Vector3(.02f, .02f, 0f); // x,y distance limits

    void Start()
    {
        if (KeySocket != null)
        {
            // If we have a key socket, we can add listeners for unlocking and locking 
            KeySocket.selectEntered.AddListener(OnDrawerUnlocked);
            KeySocket.selectExited.AddListener(OnDrawerLocked);
        }

        ParentTransform = transform.parent.transform; // set the parent to the dresser itself
        if (DrawerTransform != null)
        {
            LimitPosition = DrawerTransform.localPosition; // set the default position for the drawer
        }
        else
        {
            Debug.LogError("ERROR: null DrawerTransform");
        }
    }

    /// <summary>
    /// Listener function for when drawer is locked
    /// </summary>
    /// <param name="arg0"></param>
    private void OnDrawerLocked(SelectExitEventArgs arg0)
    {
        IsLocked = true;
    }

    /// <summary>
    /// Listener function for when drawer is unlocked
    /// </summary>
    /// <param name="arg0"></param>
    private void OnDrawerUnlocked(SelectEnterEventArgs arg0)
    {
        IsLocked = false;
    }

    /// <summary>
    /// Select Entered function inherited from XRGrabInteractable
    /// </summary>
    /// <param name="args"></param>
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args); // make sure to call the base function
        if (IsLocked == false)
        {
            // grab the object if it's not locked
            transform.SetParent(ParentTransform); //set the parent to the dresser
            IsGrabbed = true; // we are grabbed
        }
        else
        {
            // no longer on the grabbed layer
            ChangeLayerMask(DEFAULT_LAYER);
        }
    }

    /// <summary>
    ///  Select exited function inherited from XRGrabInteractable
    /// </summary>
    /// <param name="args"></param>
    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        ChangeLayerMask(GRAB_LAYER); // put the drawer back into the grab layer
        IsGrabbed = false; // we are no longer grabbed
        transform.localPosition = DrawerTransform.localPosition; // reset drawer interactable position to drawer position
    }

    void Update()
    {
        if (IsGrabbed == true && DrawerTransform != null)
        {
            // move the drawer based on drawer interactable movement but only along Z
            DrawerTransform.localPosition = new Vector3(DrawerTransform.localPosition.x,
               DrawerTransform.localPosition.y, transform.localPosition.z);

            CheckLimits(); // make sure the drawer isn't going somewhere it shouldn't
        }
    }

    /// <summary>
    /// Makes sure that the drawer isn't going somewhere it shouldn't go
    /// </summary>
    private void CheckLimits()
    {
        // Check drawer interactable's x and y position compared to the drawers x and y position
        if (transform.localPosition.x >= LimitPosition.x + LimitDistance.x ||
            transform.localPosition.x <= LimitPosition.x - LimitDistance.x ||
            transform.localPosition.y >= LimitPosition.y + LimitDistance.y ||
            transform.localPosition.y <= LimitPosition.y - LimitDistance.y)
        {   // Moved past x or y limit
            ChangeLayerMask(DEFAULT_LAYER);
        }
        else if (DrawerTransform.localPosition.z <= LimitPosition.z - LimitDistance.z)
        {   // pushed drawer in too far so reset to default position and let go
            IsGrabbed = false;
            DrawerTransform.localPosition = LimitPosition;
            ChangeLayerMask(DEFAULT_LAYER);
        }
        else if (DrawerTransform.localPosition.z >= DrawerLimitZ + LimitDistance.z)
        {   // pulled out too far so let go and push drawer back in a little bit beyond the z limit
            IsGrabbed = false;
            DrawerTransform.localPosition = new Vector3(DrawerTransform.localPosition.x,
                DrawerTransform.localPosition.y, DrawerLimitZ - .01f);
            ChangeLayerMask(DEFAULT_LAYER);
        }
    }

    /// <summary>
    /// Helper function for changing layers
    /// </summary>
    /// <param name="mask"></param>
    private void ChangeLayerMask(string mask)
    {
        interactionLayers = InteractionLayerMask.GetMask(mask);
    }
}
