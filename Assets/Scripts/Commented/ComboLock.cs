using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

/// <summary>
/// Class to manage the combination lock for the cabinet
/// </summary>
public class ComboLock : MonoBehaviour
{
    // Actions to invoke on lock/unlock
    public UnityAction UnlockAction;  
    public UnityAction LockAction;  
    private void OnUnlocked() => UnlockAction?.Invoke();  
    private void OnLocked() => LockAction?.Invoke();

    [SerializeField] TMP_Text UserInputText; // Text for the number in the combo
    [SerializeField] XrButtonInteractable[] ComboButtons; // The buttons for the combo
    [SerializeField] TMP_Text InfoText; // Instructions on what to do
    private const string Start_String = "Enter 3 Digit Combo"; // The actual text to put in the InfoText
    private const string Reset_String = "Enter 3 Digits To Reset Combo";
    [SerializeField] Image LockedPanel; // Panel with the text for if the combo is locked or not
    [SerializeField] Color UnlockedColor; // Color for when it's unlocked
    [SerializeField] Color LockedColor; // Color for when it's locked
    [SerializeField] TMP_Text LockedText; // The TMP_Text for the two states below
    private const string Unlocked_String = "Unlocked";
    private const string Locked_String = "Locked";
    [SerializeField] bool IsLocked; // Keep track if we're locked or not
    [SerializeField] bool IsResettable; // Whether or not we should reset
    private bool ShouldResetCombo;
    [SerializeField] int[] ComboValues = new int[3]; // The combo values 
    [SerializeField] int[] InputValues; // Numbers the user has set in the code
    private int MaxButtonPresses; // Max presses needed for combo
    private int NumButtonPresses; // Current number of button presses

    void Start()
    {
        MaxButtonPresses = ComboValues.Length; // Max button presses can change depending on the value set for the array
        ResetUserValues(); // Reset user input

        for (int i = 0; i < ComboButtons.Length; i++)
        {
            ComboButtons[i].selectEntered.AddListener(OnComboButtonPressed); // Add the pressed listener to each combo button
        }
    }

    /// <summary>
    /// Listener function added to all the combo buttons
    /// </summary>
    /// <param name="arg0"></param>
    private void OnComboButtonPressed(SelectEnterEventArgs arg0)
    {   
        if (NumButtonPresses >= MaxButtonPresses)
        {
            // Too many button presses            
        }
        else
        {
            for (int i = 0; i < ComboButtons.Length; i++)
            {   // Look for the correct button
                if (arg0.interactableObject.transform.name == ComboButtons[i].transform.name)
                {   // Found button so update the text and input values
                    UserInputText.text += i.ToString(); // Add the digit based on the combo button index
                    InputValues[NumButtonPresses] = i; // Keep track of button presses
                }
                else
                {   // Not our button so reset
                    ComboButtons[i].ResetColor();
                }
            }
            //Check to see if the combo is correct if we've matched max button presses for code
            NumButtonPresses++;
            if (NumButtonPresses == MaxButtonPresses)
            {
                CheckCombo();
            }
        }
    }

    /// <summary>
    /// Checks whether or not the combo is correct
    /// </summary>
    private void CheckCombo()
    {
        if (ShouldResetCombo == true)
        {   // If we're resetting just lock and bail
            ShouldResetCombo = false;
            LockCombo();
            return;
        }
        
        int matches = 0;
        for (int i = 0; i < MaxButtonPresses; i++)
        {   // Cound the matches for all the buttons
            if (InputValues[i] == ComboValues[i])
            {
                matches++;
            }
        }
        if (matches == MaxButtonPresses)
        {   // If the number of matches is equal to the number of digits in the combo, unlock
            UnlockCombo();
        }
        else
        {   // Didn't unlock so reset things
            ResetUserValues();
        }
    }

    /// <summary>
    /// Handles the functionality for a successful unlock
    /// </summary>
    private void UnlockCombo()
    {
        IsLocked = false; // No longer locked
        OnUnlocked(); // Call the code to do the actual unlocking
        LockedPanel.color = UnlockedColor; // Change color and string 
        LockedText.text = Unlocked_String;
        if (IsResettable == true)   
        {   // If we're resetable handle resetting the combo
            ResetCombo();
        }
    }

    /// <summary>
    /// Reset the lock when re-locked
    /// </summary>
    private void LockCombo()
    {
        IsLocked = true; // we are locked
        OnLocked(); // Handle the actual locking
        // Reset all the UI for the combo
        LockedPanel.color = LockedColor;
        LockedText.text = Locked_String;
        InfoText.text = Start_String;
        for(int i=0; i<MaxButtonPresses; i++)
        {
            ComboValues[i] = InputValues[i];
        }
        ResetUserValues();
    }

    /// <summary>
    /// Resets the combo based on user input
    /// </summary>
    private void ResetCombo()
    {
        InfoText.text = Reset_String;
        ResetUserValues();
        ShouldResetCombo = true;
    }

    /// <summary>
    /// Resets the values the user has inputted
    /// </summary>
    private void ResetUserValues()
    {
        InputValues = new int[MaxButtonPresses];
        UserInputText.text = "";
        NumButtonPresses = 0;
    }
}