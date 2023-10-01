using System.Collections;
using UnityEngine;
using Vuforia;

public class SpecialAttacksAREffects : DefaultObserverEventHandler
{
    public GameObject portalPrefab;
    public GameObject punchPrefab; 

    private GameObject instantiatedPortal;
    private GameObject instantiatedPunch;  

    // This gets called when the target is detected
    protected override void OnTrackingFound()
    {
        base.OnTrackingFound();

    }

    // This gets called when the target is lost
    protected override void OnTrackingLost()
    {
        base.OnTrackingLost();
        DestroyPortal(); // This will ensure the portal disappears when target is lost.
        DestroyPunch();  // This will ensure the punch effect disappears when target is lost.
    }

    // Function to handle the Doctor Strange portal button click
    public void OnDoctorStrangePortalButtonClicked()
    {
        // If a portal already exists, destroy it
        DestroyPortal();

        // Create a new portal on the target's position and make it a child of the target
        instantiatedPortal = Instantiate(portalPrefab, this.transform.position, Quaternion.identity, this.transform);

        // Start the coroutine to display the portal for a specified duration
        StartCoroutine(DisplayPortalForDuration(2.0f)); // Display portal for 2 seconds
    }

    // Function to handle the Punch button click
    public void OnPunchButtonClicked()
    {
        // If a punch effect already exists, destroy it
        DestroyPunch();

        // Create a new punch effect on the target's position and make it a child of the target
        instantiatedPunch = Instantiate(punchPrefab, this.transform.position, Quaternion.identity, this.transform);

        // Start the coroutine to display the punch effect for a specified duration
        StartCoroutine(DisplayPunchForDuration(1.5f)); // Display punch effect for 1.5 seconds
    }

    // Coroutine to handle portal duration
    private IEnumerator DisplayPortalForDuration(float duration)
    {
        yield return new WaitForSeconds(duration);
        DestroyPortal();
    }

    // Coroutine to handle punch effect duration
    private IEnumerator DisplayPunchForDuration(float duration)
    {
        yield return new WaitForSeconds(duration);
        DestroyPunch();
    }

    // Helper function to destroy the portal if it exists
    private void DestroyPortal()
    {
        if (instantiatedPortal != null)
        {
            Destroy(instantiatedPortal);
        }
    }

    // Helper function to destroy the punch effect if it exists
    private void DestroyPunch()
    {
        if (instantiatedPunch != null)
        {
            Destroy(instantiatedPunch);
        }
    }
}