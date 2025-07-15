using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArtifactInventoryCollectible : MonoBehaviour
{
    [Tooltip("Name that shows up in the inventory screen")]
    public string displayName;
    [Tooltip("ID of the collectible to be saved")]
    public string collectibleName;
    public Selectable controllerSelectible;
    public Image controllerSelectionImage;

    public bool isSpecialItem = false;
    
    public UIArtifactInventory inventory;

    [HideInInspector] public bool isVisible;

    public void SetVisible(bool value)
    {
        isVisible = value;
        gameObject.SetActive(value);
    }

    public void UpdateInventoryName()
    {
        var locName = isSpecialItem
            ? LocalizationLoader.LoadSpecialItemTranslation(collectibleName)
            : LocalizationLoader.LoadCollectibleTranslation(collectibleName, SGrid.Current.GetArea());
        
        inventory.UpdateText(locName);
    }

    private void OnEnable()
    {
        Controls.OnControlSchemeChanged += ToggleNavigation;
        ToggleNavigation();
    }

    private void OnDisable()
    {
        Controls.OnControlSchemeChanged -= ToggleNavigation;
    }

    private void ToggleNavigation()
    {
        if (Controls.UsingControllerOrKeyboardOnly())
        {
            EnableNavigation();
        } 
        else
        {
            DisableNavigation();
        }
    }

    private void DisableNavigation()
    {
        controllerSelectible.enabled = false;
        controllerSelectionImage.enabled = false;
    }

    private void EnableNavigation()
    {
        controllerSelectible.enabled = true;
    }
}
