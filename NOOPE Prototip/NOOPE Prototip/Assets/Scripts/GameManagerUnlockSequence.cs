//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManagerUnlockSequence : MonoBehaviour
{
    private GameObject focusedLocation;
    private GameObject focusedOrderLocation;
    public GameObject location1;
    public GameObject location2;
    public GameObject showOrderObject;

    private Quaternion beginningOrderQua = Quaternion.Euler(0f, -90f, 90);
    private Quaternion midOrderQua = Quaternion.Euler(0f, -90f, 180f);
    private Quaternion endOrderQua1 = Quaternion.Euler(270f, -360f, 45.33f);
    private Quaternion pickedEndQua;

    private float lerpCalc;
    private Vector3 arrowHidingPos = new Vector3(0f, -6f, 0f);
    
    public Slider unlockSlider;
    public TMP_Text unlockTxt;
    private CanvasGroup unlockSliderCanvasGroup;

    public Image unlockOrderImage;
    public TMP_Text unlockOrderInfoText;
    private CanvasGroup unlockOrderCanvasGroup;


    public static int locationIndex;
    public static int unlockedPlacesAmount;
    public static int unlockPercentile;
    private int location1UnlockPercentile;
    private int location2UnlockPercentile;

    private bool isLocation1Unlocked;
    private bool isLocation2Unlocked;
    private bool hideArrow;
    public static bool initializeValuesBeforeUpdating;
    public static bool updateUnlockUI;
    public static bool closeUnlockUI;
    public static bool initializeValuesBeforeClosing;
    public static bool isItLocked;
    public static bool showUnlockOrder;
    public static bool closeUnlockOrder;
    public static bool initializeValueForUnlockOrder;

    void Start()
    {
        unlockedPlacesAmount        = PlayerPrefs.GetInt("Unlocked Places Amount");
        location1UnlockPercentile   = PlayerPrefs.GetInt("Location 1 Unlock Percentile");
        location2UnlockPercentile   = PlayerPrefs.GetInt("Location 2 Unlock Percentile");

        isLocation1Unlocked = PlayerPrefs.GetInt("Is Location 1 Unlocked") == 0 ? false : true;  //PlayerPref'te GetBool yok maalsefki
        isLocation2Unlocked = PlayerPrefs.GetInt("Is Location 2 Unlocked") == 0 ? false : true;  //

        unlockSliderCanvasGroup = unlockSlider.GetComponent<CanvasGroup>();
        unlockOrderCanvasGroup = unlockOrderImage.GetComponent<CanvasGroup>();

        unlockTxt.text = "Kilitli";
        unlockOrderInfoText.text = "Önce Þuranýn Kilidini Açmalýsýn!";
    }

    void Update()
    {
        if(initializeValueForUnlockOrder)
            InitializingValuesForUnlockOrder();

        if (showUnlockOrder)
            ShowingUnlockOrder();

        if(closeUnlockOrder)
            ClosingUnlockOrder();

        if (initializeValuesBeforeUpdating)
            InitializingValuesBeforeUpdating();

        if (updateUnlockUI)
            UpdatingUnlockUI();

        if (initializeValuesBeforeClosing)
            InitializingBeforeClose();

        if (closeUnlockUI)
            ClosingUnlockUI();
    }

    private void InitializingValuesForUnlockOrder()
    {
        initializeValueForUnlockOrder = false;

        unlockOrderImage.gameObject.SetActive(true);


        switch (unlockedPlacesAmount)
        {
            case 0: focusedOrderLocation = location1; break;
            case 1: focusedOrderLocation = location2; break;
        }

        switch (locationIndex)
        {
            case 0: focusedLocation = location1; break; //case 0 için Ending Quaternion seçmemize gerek yok

            case 1:
                focusedLocation = location2;
                pickedEndQua = endOrderQua1;
                break;
        }

        showOrderObject.transform.position = focusedLocation.transform.position + new Vector3(0f, 1.5f, 0f);

        //showOrderObject.transform.LookAt(focusedOrderLocation.transform);
        //endOrderQua = showOrderObject.transform.rotation;
        showOrderObject.transform.rotation = beginningOrderQua;
        lerpCalc = 1f;
        hideArrow = true;
    }

    private void ShowingUnlockOrder()
    {
        unlockOrderImage.transform.position = Camera.main.WorldToScreenPoint(focusedLocation.transform.position + new Vector3(0f, .5f, 1f));
        unlockOrderCanvasGroup.alpha = Mathf.Lerp(unlockOrderCanvasGroup.alpha, 1f, 3f * Time.deltaTime);

        float lerpPercentage = LerpCalculation(ref lerpCalc);
        showOrderObject.transform.rotation = SquareQuaternionBezierishPath
                                                                (
                                                                    beginningOrderQua,
                                                                    midOrderQua,
                                                                    pickedEndQua,
                                                                    lerpPercentage
                                                                );
    }

    private float LerpCalculation(ref float lerpCalc)
    {
        lerpCalc -= lerpCalc * Time.deltaTime * .8f;
        return Mathf.Lerp(1f, 0f, lerpCalc);
    }

    private void ClosingUnlockOrder()
    {
        if(hideArrow)
        {
            hideArrow = false;
            showOrderObject.transform.position = arrowHidingPos;
        }

        unlockOrderCanvasGroup.alpha = Mathf.Lerp(unlockOrderCanvasGroup.alpha, 0f, 3f * Time.deltaTime);

        if (unlockOrderCanvasGroup.alpha < .01f)
        {
            unlockOrderImage.gameObject.SetActive(false);
            closeUnlockOrder = false;
        }
    }

    private void InitializingValuesBeforeUpdating()
    {
        initializeValuesBeforeUpdating = false;

        unlockSlider.gameObject.SetActive(true);
        unlockSliderCanvasGroup.alpha = 0f;
        isItLocked = true;

        switch (locationIndex)
        {
            case 0:
                unlockPercentile = location1UnlockPercentile;
                focusedLocation = location1;
                    break;

            case 1:
                unlockPercentile = location2UnlockPercentile;
                focusedLocation = location2;
                    break;
        }
    }

    private void UpdatingUnlockUI()
    {
        if (unlockPercentile >= 20)
        {
            switch (locationIndex)
            {
                case 0:
                    isLocation1Unlocked = true;
                    PlayerPrefs.SetInt("Is Location 1 Unlocked", 1);

                    unlockedPlacesAmount = 1;
                    PlayerPrefs.SetInt("Unlocked Places Amount", 1);

                    break;

                case 1:
                    isLocation2Unlocked = true;
                    PlayerPrefs.SetInt("Is Location 2 Unlocked", 1);

                    unlockedPlacesAmount = 2;
                    PlayerPrefs.SetInt("Unlocked Places Amount", 2);

                    break;
            }

            GameManagerUpgradeUI.initializeLerpValues = true;

            GameManagerUpgradeUI.showUpgradeUI = true;
            GameManagerUpgradeUI.hideUpgradeUI = false;
            GameManagerUpgradeUI.locationIndex = locationIndex;

            InitializingBeforeClose();
            closeUnlockUI = true;
            updateUnlockUI = false;
        }
        float unlockPercentileLerpValue = unlockPercentile / 20f;

        unlockSlider.transform.position = Camera.main.WorldToScreenPoint(focusedLocation.transform.position);
        unlockSlider.value = Mathf.Lerp(unlockSlider.value, unlockPercentileLerpValue, 20f * Time.deltaTime);
        unlockSliderCanvasGroup.alpha = Mathf.Lerp(unlockSliderCanvasGroup.alpha, 1f, 3f * Time.deltaTime);
    }

    private void ClosingUnlockUI()
    {
        unlockSliderCanvasGroup.alpha = Mathf.Lerp(unlockSliderCanvasGroup.alpha, 0f, 3f * Time.deltaTime);

        if (unlockSliderCanvasGroup.alpha < .01f)
        {
            closeUnlockUI = false;
            unlockSlider.gameObject.SetActive(false);
        }
    }

    private void InitializingBeforeClose()
    {
        initializeValuesBeforeClosing = false;
        isItLocked = false;

        switch (locationIndex)
        {
            case 0:
                if (!isLocation1Unlocked)
                {
                    location1UnlockPercentile = unlockPercentile;
                    PlayerPrefs.SetInt("Location 1 Unlock Percentile", location1UnlockPercentile);
                }
                break;

            case 1:
                if (!isLocation2Unlocked)
                {
                    location2UnlockPercentile = unlockPercentile;
                    PlayerPrefs.SetInt("Location 2 Unlock Percentile", location2UnlockPercentile);
                }
                break;
        }

    }

    private Quaternion SquareQuaternionBezierishPath(Quaternion a, Quaternion b, Quaternion c, float t)
    {                                                   
        Quaternion ab = Quaternion.Lerp(a, b, t);       
        Quaternion bc = Quaternion.Lerp(b, c, t);       

        return Quaternion.Lerp(ab, bc, t);
    }

}
