using UnityEngine;
using UnityEngine.Advertisements;

public class InterstitialAdsScript : MonoBehaviour { 

    string gameId = "1234567";
    bool testMode = true;

    void Start () {
        // Initialize the Ads service:
        Advertisement.Initialize(gameId, testMode);
    }

    public void ShowInterstitialAd()
    {
        // Check if the ad is ready before showing
        if (Advertisement.isInitialized)
        {
            Advertisement.Show("Interstitial_Ad");
        }
        else
        {
            Debug.Log("Interstitial ad not ready at the moment! Please try again later!");
        }
    }
}