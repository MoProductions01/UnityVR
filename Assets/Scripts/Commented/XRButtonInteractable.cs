using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.UI;

/// <summary>
/// Inherited class so we can customize button interactions
/// </summary>
public class XrButtonInteractable : XRSimpleInteractable
{
    [SerializeField] GameObject KeyIndicatorLight;
    [SerializeField] Image buttonImage; // GameObject used for button graphics

    // Colors for various states of the buttons
    [SerializeField] private Color normalColor;
    [SerializeField] private Color highlightedColor;
    [SerializeField] private Color pressedColor;
    [SerializeField] private Color selectedColor;

    private bool isPressed; // Keep track of if this button is pressed or not

    void Start()
    {
        ResetColor(); // Put button color back to default
    }

    /// <summary>
    /// Overridden function for when we start hovering over a button
    /// with our VR controller 
    /// </summary>
    /// <param name="args"></param>
    protected override void OnHoverEntered(HoverEnterEventArgs args)
    {
        base.OnHoverEntered(args);

        isPressed = false; // We're not pressing 
        buttonImage.color = highlightedColor; // Change color to highlighted
    }

    /// <summary>
    /// Overridden function for when we stop hovering over a button
    /// with our VR controller 
    /// </summary>
    /// <param name="args"></param>
    protected override void OnHoverExited(HoverExitEventArgs args)
    {
        base.OnHoverExited(args);

        if(!isPressed) 
        {   // Only change the collor if we're not pressing it
            buttonImage.color = normalColor; 
        }
    }

    /// <summary>
    /// Overridden function for when we start pressing a button
    /// </summary>
    /// <param name="args"></param>
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {        
        base.OnSelectEntered(args);

        isPressed = true; // We are now pressing
        KeyIndicatorLight.SetActive(true);
        buttonImage.color = pressedColor; // 
    }

    /// <summary>
    /// Overridden function for when we stop pressing a button
    /// </summary>
    /// <param name="args"></param>
    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);

        buttonImage.color = selectedColor; // Let go of button so back to selected color
    }
    
    /// <summary>
    /// Resets the color of the button to the neutral state
    /// </summary>
    public void ResetColor()
    {
        buttonImage.color = normalColor;
    }
}

