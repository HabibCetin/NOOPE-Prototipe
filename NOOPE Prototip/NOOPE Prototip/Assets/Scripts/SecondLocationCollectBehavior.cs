//using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondLocationCollectBehavior : MonoBehaviour
{
    public static List<GameObject> moneySpawnList = new List<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Character")
        {
            CinematicCam.focusPlaceIndex        = 1;
            CinematicCam.initializeCalcValues   = true;
            CinematicCam.zoomForCollect         = true;
            CinematicCam.zoomForThrow           = false;

            GameManagerMoneyObject.dontReverse = false;

            if (GameManagerUnlockSequence.unlockedPlacesAmount > 1) //Eðer kilidi açýlmýþsa
            {
                GameManagerMoneyObject.startCollecting = true;
                GameManagerMoneyObject.moneyCollectPlaceIndex = 1;
            }
            else if (GameManagerUnlockSequence.unlockedPlacesAmount == 1) //Tam kilidini açmamýz gereken yerdeysek
            {
                GameManagerUnlockSequence.initializeValuesBeforeUpdating = true;
                GameManagerUnlockSequence.updateUnlockUI = true;
                GameManagerUnlockSequence.closeUnlockUI = false;
                GameManagerUnlockSequence.locationIndex = 1;
            }
            else //Kilidini açmadan önce baþka yerin kilidinin açýlmasý gerekiyorsa
            {
                GameManagerUnlockSequence.initializeValueForUnlockOrder = true;
                GameManagerUnlockSequence.showUnlockOrder = true;
                GameManagerUnlockSequence.closeUnlockOrder = false;
                GameManagerUnlockSequence.locationIndex = 1;
            }

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "Character")
        {
            CinematicCam.zoomForCollect         = false;

            if (GameManagerUnlockSequence.unlockedPlacesAmount > 1) //Eðer kilidi açýlmýþsa
            {
                GameManagerMoneyObject.stopCollecting = true;
                GameManagerMoneyObject.moneyCollectPlaceIndex = 1;
            }
            else if (GameManagerUnlockSequence.unlockedPlacesAmount == 1) //Tam kilidini açmamýz gereken yerdeysek
            {
                GameManagerUnlockSequence.initializeValuesBeforeClosing = true;
                GameManagerUnlockSequence.closeUnlockUI = true;
                GameManagerUnlockSequence.updateUnlockUI = false;
            }
            else //Kilidini açmadan önce baþka yerin kilidinin açýlmasý gerekiyorsa
            {
                GameManagerUnlockSequence.closeUnlockOrder = true;
                GameManagerUnlockSequence.showUnlockOrder = false;
            }

        }

    }
}
