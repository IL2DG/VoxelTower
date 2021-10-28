using UnityEngine;
using UnityEngine.Advertisements;
using System.Collections;

public class Ads : MonoBehaviour
{


    string gameId = "4109925";
    bool testMode = false;

    private static int countLoses;

    private void Start()
    {
        Advertisement.Initialize(gameId, testMode);
        countLoses++;
    }

    public void ShowAd()
    {
        if(Advertisement.IsReady() && countLoses % 3 == 0)
        {
            Advertisement.Show("video");
        }
        else
        {
            Debug.Log("Interstitial ad not ready at the moment! Please try again later!");
        }
    }
}
