//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

public class FirstLocationSpendBehavior : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Character")
        {
            CinematicCam.focusPlaceIndex = 0;
            CinematicCam.initializeCalcValues = true;
            CinematicCam.zoomForCollect = false;
            CinematicCam.zoomForThrow = true;

            if (GameManagerUnlockSequence.unlockedPlacesAmount > 0) //E�er kilidi a��lm��sa
            {
                GameManagerUpgradeUI.initializeLerpValues   = true;
                GameManagerUpgradeUI.showUpgradeUI  = true;
                GameManagerUpgradeUI.hideUpgradeUI  = false;
                GameManagerUpgradeUI.locationIndex  = 0;

                GameManagerMoneyObject.startSpending = true;
                GameManagerMoneyObject.moneySpendPlaceIndex = 0;
                GameManagerMoneyObject.stopGenerating = true;
            }
            else if (GameManagerUnlockSequence.unlockedPlacesAmount == 0) //Tam kilidini a�mam�z gereken yerdeysek
            {
                GameManagerUnlockSequence.locationIndex = 0;
                GameManagerUnlockSequence.updateUnlockUI = true;
                GameManagerUnlockSequence.closeUnlockUI = false;
                GameManagerUnlockSequence.initializeValuesBeforeUpdating = true;

                GameManagerMoneyObject.startSpending = true;
                GameManagerMoneyObject.moneySpendPlaceIndex = 0;
                GameManagerMoneyObject.stopGenerating = true;
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
            GameManagerMoneyObject.stopSpending = true;
            GameManagerMoneyObject.stopCollecting = true;
            GameManagerMoneyObject.dontReverse = true;
            GameManagerMoneyObject.moneySpendPlaceIndex = 0; 
            GameManagerMoneyObject.moneyCollectPlaceIndex = 0;

            GameManagerUpgradeUI.showUpgradeIncereaseUI = false;

            CinematicCam.zoomForThrow = false;

            if (GameManagerUnlockSequence.unlockedPlacesAmount > 0) //E�er kilidi a��lm��sa
            {
                GameManagerUpgradeUI.showUpgradeUI = false;
                GameManagerUpgradeUI.hideUpgradeUI = true;
            }
            else if (GameManagerUnlockSequence.unlockedPlacesAmount == 0) //Tam kilidini a�mam�z gereken yerdeysek
            {
                GameManagerUnlockSequence.initializeValuesBeforeClosing = true;
                GameManagerUnlockSequence.closeUnlockUI = true;
                GameManagerUnlockSequence.updateUnlockUI = false;
            }
            else //Kilidini a�madan �nce ba�ka yerin kilidinin a��lmas� gerekiyorsa
            {
                GameManagerUnlockSequence.closeUnlockOrder = true;
                GameManagerUnlockSequence.showUnlockOrder = false;
            }

        }
    }
}
