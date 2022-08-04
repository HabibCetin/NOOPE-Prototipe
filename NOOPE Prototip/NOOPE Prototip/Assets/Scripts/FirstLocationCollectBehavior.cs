//using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstLocationCollectBehavior : MonoBehaviour
{
    public static List<GameObject> moneySpawnList = new List<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "Character")
        {
            CinematicCam.focusPlaceIndex        = 0;
            CinematicCam.initializeCalcValues   = true;
            CinematicCam.zoomForCollect         = true;
            CinematicCam.zoomForThrow           = false;

            GameManagerMoneyObject.dontReverse = false;

            if (GameManagerUnlockSequence.unlockedPlacesAmount > 0) //E�er kilidi a��lm��sa
            {
                GameManagerMoneyObject.startCollecting = true;
                GameManagerMoneyObject.moneyCollectPlaceIndex = 0;
            }
            else if (GameManagerUnlockSequence.unlockedPlacesAmount == 0) //Tam kilidini a�mam�z gereken yerdeysek
            {
                GameManagerUnlockSequence.initializeValuesBeforeUpdating = true;
                GameManagerUnlockSequence.updateUnlockUI = true;
                GameManagerUnlockSequence.closeUnlockUI = false;
                GameManagerUnlockSequence.locationIndex = 0;
            }
            else //Kilidini a�madan �nce ba�ka yerin kilidinin a��lmas� gerekiyorsa
            {
                GameManagerUnlockSequence.initializeValueForUnlockOrder = true;
                GameManagerUnlockSequence.showUnlockOrder = true;
                GameManagerUnlockSequence.closeUnlockOrder = false;
                GameManagerUnlockSequence.locationIndex = 0;
            }

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "Character")
        {
            CinematicCam.zoomForCollect = false;

            if (GameManagerUnlockSequence.unlockedPlacesAmount > 0) //E�er kilidi a��lm��sa
            {
                GameManagerMoneyObject.stopCollecting = true;
                GameManagerMoneyObject.moneyCollectPlaceIndex = 0;
            }
            else if (GameManagerUnlockSequence.unlockedPlacesAmount == 0) //Tam kilidini a�mam�z gereken yerdeysek
            {
                GameManagerUnlockSequence.initializeValuesBeforeClosing = true;
                GameManagerUnlockSequence.closeUnlockUI     = true;
                GameManagerUnlockSequence.updateUnlockUI    = false;
            }
            else //Kilidini a�madan �nce ba�ka yerin kilidinin a��lmas� gerekiyorsa
            {
                GameManagerUnlockSequence.closeUnlockOrder  = true;
                GameManagerUnlockSequence.showUnlockOrder   = false;
            }

        }

    }
}
