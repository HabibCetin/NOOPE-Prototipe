//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManagerUpgradeUI : MonoBehaviour
{
    public Image upgradeIncereaseImage;
    public TMP_Text upgradetheSpeedTxt;
    public TMP_Text upgradeTheCapacityTxt;
    private CanvasGroup upgradeIncreaseCanvasGroup;

    public Slider upgradeSlider;
    public TMP_Text upgradeLevelTxt;
    private CanvasGroup sliderCanvasGroup;

    public GameObject location1;
    public GameObject location2;
    private GameObject focusedLocation;


    public static int location1LevelPercentile;
    public static int location2LevelPercentile;
    public static int location1LevelCount;
    public static int location2LevelCount;
    public static int locationIndex;

    public static bool showUpgradeUI;
    public static bool hideUpgradeUI;
    public static bool initializeLerpValues;
    public static bool upgradeIncereaseUIValueSetUp;
    public static bool showUpgradeIncereaseUI;
    private bool closeUpgradeIncereaseUI;

    //private float lerpCalc;

    private void Awake()
    {
        location1LevelPercentile    = PlayerPrefs.GetInt("Location 1 Level Percentile");    
        location2LevelPercentile    = PlayerPrefs.GetInt("Location 2 Level Percentile");
        location1LevelCount         = PlayerPrefs.GetInt("Location 1 Level Count");
        location2LevelCount         = PlayerPrefs.GetInt("Location 2 Level Count");

        sliderCanvasGroup = upgradeSlider.GetComponent<CanvasGroup>();
        upgradeIncreaseCanvasGroup = upgradeIncereaseImage.GetComponent<CanvasGroup>();

        upgradetheSpeedTxt.text = "Hýz arttýrýldý!";
        upgradeTheCapacityTxt.text = "Kapasite arttýrýldý";
    }

    void Update()
    {
        if (initializeLerpValues)
            InitializingValues();

        if (showUpgradeUI)
            ShowingUpgradeUI(locationIndex);

        if (hideUpgradeUI)
            HidingUpgradeUI();
    }

    private void InitializingValues()
    {
        initializeLerpValues = false;

        sliderCanvasGroup.alpha = 0f;
        upgradeSlider.gameObject.SetActive(true);

        switch (locationIndex)
        {
            case 0:
                upgradeLevelTxt.text = "Seviye: " + location1LevelCount;
                focusedLocation = location1;
                break;

            case 1:
                upgradeLevelTxt.text = "Seviye: " + location2LevelCount;
                focusedLocation = location2;
                break;
        }
    }

    private void ShowingUpgradeUI(int locationIndex)
    {
        sliderCanvasGroup.alpha = Mathf.Lerp(sliderCanvasGroup.alpha, 1f, .8f * Time.deltaTime);
        upgradeSlider.transform.position = Camera.main.WorldToScreenPoint(focusedLocation.transform.position);

        ChangeUpgradeIncreaseUI();

        float currentPercentile = 0f;

        switch(locationIndex)
        {
            case 0:
                currentPercentile = location1LevelPercentile / 10f;
                break;

            case 1:
                currentPercentile = location2LevelPercentile / 10f;
                break;
        }
                upgradeSlider.value = Mathf.Lerp(upgradeSlider.value, currentPercentile, 15f * Time.deltaTime);

        switch (locationIndex)
        {
            case 0:
                upgradeLevelTxt.text = "Seviye: " + location1LevelCount;
                focusedLocation = location1;
                break;

            case 1:
                upgradeLevelTxt.text = "Seviye: " + location2LevelCount;
                focusedLocation = location2;
                break;
        }

    }

    private void ChangeUpgradeIncreaseUI()
    {
        if (upgradeIncereaseUIValueSetUp)
        {
            upgradeIncereaseUIValueSetUp = false;
            upgradeIncereaseImage.gameObject.SetActive(true);
            upgradeIncreaseCanvasGroup.alpha = 0f;
            upgradeIncereaseImage.transform.position = upgradeSlider.transform.position;
            closeUpgradeIncereaseUI = false;
            showUpgradeIncereaseUI = true;
        }

        if (showUpgradeIncereaseUI)
        {
            UpgradeIncreaseUIChanger(1f, new Vector3(0f, -100f, 0f));

            if (upgradeIncreaseCanvasGroup.alpha > .8f)
            {
                showUpgradeIncereaseUI = false;
                closeUpgradeIncereaseUI = true;
            }
        }

        if (closeUpgradeIncereaseUI)
        {
            UpgradeIncreaseUIChanger(0f, new Vector3(0f, -200f, 0f));

            if (upgradeIncreaseCanvasGroup.alpha < .1f)
            {
                closeUpgradeIncereaseUI = false;
                upgradeIncereaseImage.gameObject.SetActive(false);
            }
        }
    }

    private void UpgradeIncreaseUIChanger(float targetAlpha, Vector3 targetPosOffset)
    {
        upgradeIncereaseImage.transform.position = Vector3.Lerp
                                                           (
                                                               upgradeIncereaseImage.transform.position,
                                                               upgradeSlider.transform.position + targetPosOffset,
                                                               1f * Time.deltaTime
                                                           );

        upgradeIncreaseCanvasGroup.alpha = Mathf.Lerp(upgradeIncreaseCanvasGroup.alpha, targetAlpha, 2f * Time.deltaTime);
    }

    private void HidingUpgradeUI()
    {
        sliderCanvasGroup.alpha = Mathf.Lerp(sliderCanvasGroup.alpha, 0f, 3f * Time.deltaTime);
        if (sliderCanvasGroup.alpha < .01f)
        {
            upgradeSlider.gameObject.SetActive(false);
            upgradeIncereaseImage.gameObject.SetActive(false);
            hideUpgradeUI = false;
        }
    }
}
