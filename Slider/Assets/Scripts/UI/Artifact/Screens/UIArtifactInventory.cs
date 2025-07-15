using System.Collections;
using System.Collections.Generic;
using Localization;
using UnityEngine;
using TMPro;

public class UIArtifactInventory : MonoBehaviour, IDialogueTableProvider
{
    public List<ArtifactInventoryCollectible> collectibles;

    public ArtifactInventoryCollectible anchorCollectible; // the check is player.pickedupanchor
    public ArtifactInventoryCollectible bootsCollectible; 
    public ArtifactInventoryCollectible bootsUpgradeCollectible; 
    public ArtifactInventoryCollectible scrollCollectible; 
    public ArtifactInventoryCollectible scrollScrapCollectible;

    public TextMeshProUGUI inventoryText;

    enum ArtifactInventoryStrings
    {
        Collection,
        LegendaryCheeseburger,
    }
    
    public Dictionary<string, LocalizationPair> TranslationTable { get; } = 
        IDialogueTableProvider.InitializeTable(
            new Dictionary<ArtifactInventoryStrings, string>
            {
                {
                    ArtifactInventoryStrings.Collection, "Collection" 
                },
                {
                    ArtifactInventoryStrings.LegendaryCheeseburger, "Legendary \"Burger\"" 
                }
            });

    [Header("Special Collectible Counters")] // could be refactored
    public ArtifactInventoryCollectible oilCollectible;
    public ArtifactInventoryCollectible breadgeCollectible;
    public Animator breadgeAnimator;
    public TextMeshProUGUI sunglassesCount;
    public TextMeshProUGUI oilCount;
    public TextMeshProUGUI breadgeCount;

    private void OnEnable() 
    {
        UpdateIcons();
        if (Controls.UsingControllerOrKeyboardOnly())
        {
            if (!TrySelectLeftmostSelectible())
                UpdateText(this.GetLocalizedSingle(ArtifactInventoryStrings.Collection));
        }
        else
        {
            UpdateText(this.GetLocalizedSingle(ArtifactInventoryStrings.Collection));
        }

        UpdateCollectibleCounters(this, null);
        PlayerInventory.OnPlayerGetCollectible += UpdateCollectibleCounters;
    }

    private void OnDisable() 
    {
        PlayerInventory.OnPlayerGetCollectible -= UpdateCollectibleCounters;
    }

    public void UpdateIcons()
    {
        foreach (ArtifactInventoryCollectible c in collectibles)
        {
            c.SetVisible(PlayerInventory.Contains(c.collectibleName));
        }

        for (int i = 1; i <= 4; i++)
        {
            // Collectibles are called "Oil #1", "Oil #2", "Oil #3"
            if (PlayerInventory.Contains($"{oilCollectible.collectibleName} #{i}"))
            {
                oilCollectible.SetVisible(true);
                break;
            }
        }

        anchorCollectible.SetVisible(PlayerInventory.Instance.GetHasCollectedAnchor());

        bootsCollectible?.SetVisible(!PlayerInventory.Contains("Boots Upgrade") && PlayerInventory.Contains("Boots"));
        bootsUpgradeCollectible.SetVisible(PlayerInventory.Contains("Boots Upgrade"));

        scrollCollectible.SetVisible(PlayerInventory.Contains("Scroll of Realigning"));
        scrollScrapCollectible?.SetVisible(!PlayerInventory.Contains("Scroll of Realigning") && PlayerInventory.Contains("Scroll Scrap"));

        if (PlayerInventory.Contains("Legendary Cheese Burger", Area.MagiTech))
        {
            breadgeCollectible.displayName = this.GetLocalizedSingle(breadgeCollectible.displayName);
            breadgeAnimator.SetBool("isOn", true);
        }
    }

    public void UpdateText(string text)
    {
        inventoryText.text = text;
    }


    private void UpdateCollectibleCounters(object sender, PlayerInventory.InventoryEvent e)
    {
        int numBreadge = 0;
        int numSunglasses = 0;
        int numOil = 0;

        for (int i = 1; i <= 9; i++)
        {
            if (PlayerInventory.Contains("Breadge", (Area)i))
                numBreadge += 1;
            if (PlayerInventory.Contains("Sunglasses", (Area)i))
                numSunglasses += 1;
        }

        for (int i = 1; i <= 4; i++)
        {
            if (PlayerInventory.Contains($"Oil #{i}"))
                numOil += 1;
        }
        
        breadgeCount.text = numBreadge.ToString();
        breadgeCount.gameObject.SetActive(numBreadge > 1);
        
        sunglassesCount.text = numSunglasses.ToString();
        sunglassesCount.gameObject.SetActive(numSunglasses > 1);
        
        oilCount.text = numOil.ToString();
        oilCount.gameObject.SetActive(numOil > 1);
    }

    public bool TrySelectLeftmostSelectible()
    {
        ArtifactInventoryCollectible leftmost = null;
        float smallestX = float.MaxValue;
        foreach(ArtifactInventoryCollectible c in collectibles)
        {
            if(c.controllerSelectible.enabled && c.isVisible && c.transform.position.x < smallestX)
            {
                leftmost = c;
                smallestX = c.transform.position.x;
            }
        }
        if(leftmost !=null)
        {
            Select(leftmost);
            return true;
        }
        return false;
    }

    public void TrySelectRightmostSelectible()
    {
        ArtifactInventoryCollectible rightMost = null;
        float largestX = float.MinValue;
        foreach(ArtifactInventoryCollectible c in collectibles)
        {
            if(c.controllerSelectible.enabled && c.isVisible && c.transform.position.x > largestX)
            {
                rightMost = c;
                largestX = c.transform.position.x;
            }
        }
        if(rightMost != null)
            Select(rightMost);
    }

    private void Select(ArtifactInventoryCollectible s)
    {
        s.controllerSelectible.Select();
        s.controllerSelectionImage.enabled = true;
        s.UpdateInventoryName();
    }
}
