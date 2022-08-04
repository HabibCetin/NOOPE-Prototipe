using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerMoneyObject : MonoBehaviour
{
    #region Values

    public List<GameObject> moneyStackList = new List<GameObject>();

    public GameObject moneyPrefab;
    public GameObject spawnPoint1;
    public GameObject spawnPoint2;
    public GameObject player;
    public GameObject throwPoint1;
    public GameObject throwPoint2;

    public static GameObject randomlyTouchedMoney;

    private int colomnLenght    = 5;    //Spawn için
    private int rowLenght       = 10;   //Stack için
    public int throwCount;             //Throw için

    public static int moneyCollectPlaceIndex = -1; // baþlangýçta hepsinden spawn olmaya baþlamasý için -1 deðeri verdik
    public static int moneySpendPlaceIndex;

    public static bool startCollecting;
    public static bool stopCollecting = true;
    public static bool startSpending;
    public static bool stopSpending;
    public static bool collectTheRandomMoney;
    public static bool dontReverse;
    public static bool stopGenerating;

    /// ///////////////////////////////////////
    private float spawnDistanceZAxis = .6f; //Spawn için
    private float spawnDistanceXAxis = .3f; //
    private float spawnDistanceYAxis = .22f;//
    private float currentZOffset;           //
    private float currentXOffset;           //
    private float currentYOffset;           //
    /// //////////////////////////////////////////////
    private float stackDistanceLocalYAxis = .22f;   //Stack için
    private float stackDistanceLocalZAxis = .3f;    //
    private float currentLocalYOffset;              //
    private float currentLocalZOffset;              //
    /// //////////////////////////////////////////////


    private IEnumerator moneyGenerationEnum1;   // IEnumeratorlere Parametre verebilmek için böyle yapýyoruz
    private IEnumerator moneyGenerationEnum2;   // 
    private IEnumerator moneyStackEnum;         //
    private IEnumerator moneyThrowEnum;         //

    private bool startSpawning;
    private bool currentlyThrowing;
    private bool reversedStack;
    private bool reversedLocation1List;
    private bool reversedLocation2List;

    #endregion

    private void Start()
    {
        StartCoroutine("SpawnRandomMoney");
    }

    void Update()
    {
        if(collectTheRandomMoney)
        {
            collectTheRandomMoney = false;
            StackRandomlyTouchedMoney();
        }

        if (stopCollecting)    //spawn bölgesinden çýkýnca
        {
            if(moneyStackEnum != null)
                StopCoroutine(moneyStackEnum);
            stopCollecting = false;

            startSpawning = true;
            RemoveFromSpawnList(moneyCollectPlaceIndex);
            stopGenerating = false;
        }

        if (startSpawning)
            GenerateNewMoney(moneyCollectPlaceIndex);

        if (startCollecting)    //spawn bölgesine girince
        {
            startCollecting = false;

            stopGenerating = true;
            moneyStackEnum = KeepStackingMoney(.3f, moneyCollectPlaceIndex);
            StartCoroutine(moneyStackEnum);
        }

        if (startSpending)    //Upgrade alanýna girince
        {
            ThrowStarter(.13f, moneySpendPlaceIndex);

            if (moneyStackEnum != null)
                StopCoroutine(moneyStackEnum);
        }

        if (stopSpending)
        {
            stopSpending = false;
            currentlyThrowing = false;

            if (moneyThrowEnum != null)
                StopCoroutine(moneyThrowEnum);

            moneyStackList.RemoveRange(0, throwCount);
            throwCount = 0;
        }

    }

    #region Money Generation At Location
    /// /////// BAK BURAYA TEKRAR
    private void GenerateNewMoney(int locationIndex)
    {
        int unlockedPlacesAmount = GameManagerUnlockSequence.unlockedPlacesAmount;

        switch (locationIndex)
        {
            case -1:                                                                                            // Baþlangýçta hepsinden spawn
                if(unlockedPlacesAmount > 0)                                                                    // olmasý için case -1'e tüm
                {                                                                                               // bölgeleri giriyoruz
                    moneyGenerationEnum1 = GenerateMoneyAtLocation(CalculateSpawnWaitTime(0), spawnPoint1, 0);  //
                    StartCoroutine(moneyGenerationEnum1);                                                       // 
                }                                                                                               //
                                                                                                                //
                if (unlockedPlacesAmount > 1)                                                                   //
                {                                                                                               //
                    moneyGenerationEnum2 = GenerateMoneyAtLocation(CalculateSpawnWaitTime(1), spawnPoint2, 1);  // 
                    StartCoroutine(moneyGenerationEnum2);                                                       // 
                }                                                                                               //
                                                                                                                //
                    break;                                                                                      //

            case 0:
                if (unlockedPlacesAmount > 0)
                {
                    moneyGenerationEnum1 = GenerateMoneyAtLocation(CalculateSpawnWaitTime(0), spawnPoint1, 0);
                    StartCoroutine(moneyGenerationEnum1);
                }
                    break;

            case 1:
                if (unlockedPlacesAmount > 1)
                {
                    moneyGenerationEnum2 = GenerateMoneyAtLocation(CalculateSpawnWaitTime(1), spawnPoint2, 1);
                    StartCoroutine(moneyGenerationEnum2);
                }

                    break;
        }

        startSpawning = false;
    }

    private float CalculateSpawnWaitTime(int locationIndex)
    {
        float waitTime = 2f; //default olarak 2 saniye seçtim, istenirse arttýrýlabilinir
        int levelIndex = 0;

        switch(locationIndex)
        {
            case 0: levelIndex = GameManagerUpgradeUI.location1LevelCount; break;
            case 1: levelIndex = GameManagerUpgradeUI.location2LevelCount; break;
        }

        for (int i = 0; i < levelIndex; i++) //Her seviyede üretme hýzý arttýrýlýyor
            waitTime *= .95f;

        return waitTime;
    }

    private IEnumerator GenerateMoneyAtLocation(float waitTime, GameObject locationObject, int listIndex)
    {
        //if (listIndex <= GameManagerUnlockSequence.unlockedPlacesAmount)
        //    yield break; 

        ReverseListOf(listIndex);
        List<GameObject> moneyList = WhichList(listIndex);

        int loopTime = SpawnCapacity(listIndex) - moneyList.Count;

        for (int i = 0; i < loopTime; i++)
        {
            if (stopGenerating)
                yield break;

            CalculateSpawnOffset(listIndex);
            Vector3 moneySpawnOffset = new Vector3(currentXOffset, currentYOffset, currentZOffset);
            GameObject newMoney = Instantiate(moneyPrefab, locationObject.transform.position + moneySpawnOffset, locationObject.transform.rotation);
            AddIntoListOf(newMoney, listIndex);

            yield return new WaitForSeconds(waitTime);
        }
    }

    private int SpawnCapacity(int locationIndex)
    {
        switch(locationIndex)
        {
            case 0: return GameManagerUpgradeUI.location1LevelCount + 6;
            case 1: return GameManagerUpgradeUI.location2LevelCount + 6;
            default: Debug.Log("Spawn amount için location index bulunamadý"); return 0;
        }
    }

    private void CalculateSpawnOffset(int listIndex)
    {
        List<GameObject> moneyList = WhichList(listIndex);
        int howMuchMoneyWeHave = moneyList.Count;
        currentXOffset = ((howMuchMoneyWeHave % 20) / colomnLenght) * spawnDistanceXAxis;   //X kordinatýnda spawnlama offseti
        currentZOffset = ((howMuchMoneyWeHave % 20) % colomnLenght) * spawnDistanceZAxis;    //Z kordinatýnda spawnlama offseti
        currentYOffset = howMuchMoneyWeHave / 20 * spawnDistanceYAxis;
    }

    #endregion

    #region Money Generation At Random Ground

    private IEnumerator SpawnRandomMoney()
    {
        while(true)
        {
            float randomX = Random.Range(-2.3f, -3.3f);
            float randomZ = Random.Range(-15f, 10f);

            Vector3 randomPoint = new Vector3(randomX, .4f, randomZ);
            Quaternion randomSpawnQua = Quaternion.Euler(moneyPrefab.transform.rotation.eulerAngles.x, Random.Range(0f, 90f), moneyPrefab.transform.rotation.eulerAngles.z);

            GameObject randomMoneyObj = Instantiate(moneyPrefab, randomPoint, randomSpawnQua);

            randomMoneyObj.AddComponent<BoxCollider>();
            randomMoneyObj.AddComponent<Rigidbody>();
            randomMoneyObj.AddComponent<RandomlySpawnedMoneyBehavior>();

            yield return new WaitForSeconds(Random.Range(1f, 5f));
        }
    }

    #endregion

    #region Money Stacking

    private IEnumerator KeepStackingMoney(float waitTime, int listIndex)
    {
        List<GameObject> moneyList = WhichList(listIndex);
        ReverseListOf(listIndex);
        
        foreach (GameObject moneyObj in moneyList)
        {
            CalculateStackOffset(moneyStackList.Count);
            Vector3 moneyStackOffset = new Vector3(0f, currentLocalYOffset, currentLocalZOffset);
            GameObject money = moneyObj;

            IEnumerator stackMoneyMovement = StackTheMoney(money, moneyStackOffset, listIndex);
            StartCoroutine(stackMoneyMovement);

            if (currentlyThrowing)
                yield break;

            yield return new WaitForSeconds(waitTime);
        }

        RemoveFromSpawnList(listIndex);
        stopGenerating = false;
    }

    private void StackRandomlyTouchedMoney()
    {
        CalculateStackOffset(moneyStackList.Count);
        Vector3 moneyStackOffset = new Vector3(0f, currentLocalYOffset, currentLocalZOffset);

        Destroy(randomlyTouchedMoney.GetComponent<BoxCollider>());
        Destroy(randomlyTouchedMoney.GetComponent<Rigidbody>());
        Destroy(randomlyTouchedMoney.GetComponent<RandomlySpawnedMoneyBehavior>());

        IEnumerator stackMoneyMovement = StackTheMoney(randomlyTouchedMoney, moneyStackOffset, -1); //son parametreye -1 girmemiz, parayý rastgele bölgede spawn olmuþ yerden aldýðýmýzý gösteriyor
        StartCoroutine(stackMoneyMovement);
    }

    private List<GameObject> WhichList(int listIndex)
    {
        switch(listIndex)
        {
            case 0: return FirstLocationCollectBehavior.moneySpawnList;
            case 1: return SecondLocationCollectBehavior.moneySpawnList;
            default: return null;
        }
    }

    private void ReverseListOf(int listIndex)
    {
        if(!dontReverse)
            switch (listIndex)
            {
                case 0:
                    FirstLocationCollectBehavior.moneySpawnList.Reverse();
                    reversedLocation1List = !reversedLocation1List;
                        break;
                case 1:
                    SecondLocationCollectBehavior.moneySpawnList.Reverse();
                    reversedLocation2List = !reversedLocation2List;
                    break;

                default: Debug.Log("LÝSTE BULUNAMADI!"); break;
            }
    }

    private void AddIntoListOf(GameObject money ,int listIndex)
    {
        switch(listIndex)
        {
            case 0: FirstLocationCollectBehavior.moneySpawnList.Add(money); break;
            case 1: SecondLocationCollectBehavior.moneySpawnList.Add(money); break;
        }
    }

    private IEnumerator StackTheMoney(GameObject money, Vector3 stackOffset, int listIndex)
    {
        if (reversedStack)
        {
            moneyStackList.Reverse();
            reversedStack = false;
        }

        moneyStackList.Add(money);
        float lerpCalculation = 1f;
        float lerpPercentage = LerpPercentageCalculation(ref lerpCalculation, 1f);
        Vector3 moneyFirstPos = money.transform.position;
        Vector3 offset = Vector3.zero;
        Quaternion beginningMoneyRot = money.transform.rotation;

        while(lerpCalculation > .005f)
        {
            offset = (player.transform.position) - (player.transform.forward * stackOffset.z);
            offset.y = stackOffset.y + 1.5f;
            offset -= player.transform.forward;

            Quaternion desiredRotation = Quaternion.Euler
                                                    (
                                                        player.transform.rotation.eulerAngles.x,
                                                        player.transform.rotation.eulerAngles.y + 90f,
                                                        player.transform.rotation.eulerAngles.z
                                                    );

            money.transform.position = KubikBezierPath
                                    (
                                        moneyFirstPos,
                                        offset + new Vector3(0f, 3f, 0f),
                                        offset - player.transform.right * -1.5f,
                                        offset,
                                        lerpPercentage
                                    );

            money.transform.rotation = Quaternion.Lerp(beginningMoneyRot, desiredRotation, lerpPercentage);

            if (currentlyThrowing)
            {
                RemoveFromSpawnList(listIndex);
                money.transform.SetParent(player.transform);
                yield break;
            }

            lerpPercentage = LerpPercentageCalculation(ref lerpCalculation, 2f);
            yield return new WaitForEndOfFrame();
        }

        offset = (player.transform.position) - (player.transform.forward * stackOffset.z);
        offset.y = stackOffset.y + 1.5f;
        offset -= player.transform.forward;

        money.transform.position = KubikBezierPath
                                    (
                                        moneyFirstPos,
                                        offset + new Vector3(0f, 3f, 0f),
                                        offset - player.transform.right * -1.5f,
                                        offset,
                                        lerpPercentage
                                    );

        money.transform.SetParent(player.transform);
    }

    private void CalculateStackOffset(int stackCount)
    {
        currentLocalYOffset = (stackCount % rowLenght) * stackDistanceLocalYAxis; //Y kordinatýnda local stackleme offseti
        currentLocalZOffset = (stackCount / rowLenght) * stackDistanceLocalZAxis; //Z kordinatýnda local stackleme offseti
    }

    private void RemoveFromSpawnList(int listIndex)
    {
        switch(listIndex)
        {
            case -1: break; //Silmeye gerek yok çünkü -1 ise yerden alýndý

            case 0:
                    foreach (GameObject money in moneyStackList)
                        FirstLocationCollectBehavior.moneySpawnList.Remove(money);
                break;

            case 1:
                    foreach (GameObject money in moneyStackList)
                        SecondLocationCollectBehavior.moneySpawnList.Remove(money);
                break;
        }

    }

    #endregion

    #region Money Throw

    private void ThrowStarter(float waitTime, int throwIndex)
    {
        moneyThrowEnum = SpendingMoney(waitTime, moneySpendPlaceIndex);
        StartCoroutine(moneyThrowEnum);
        startSpending = false;
        currentlyThrowing = true;
    }

    private IEnumerator SpendingMoney(float waitTime, int throwIndex)
    {
        if(!reversedStack)
        {
            moneyStackList.Reverse();
            reversedStack = true;
        }

        throwCount = 0;

        foreach(GameObject money in moneyStackList)
        {
            ThrowTheMoney(money, throwIndex);

            yield return new WaitForSeconds(waitTime);
        }

        currentlyThrowing = false;
    }

    private void ThrowTheMoney(GameObject money, int throwIndex)
    {
        if (!GameManagerUnlockSequence.isItLocked)
            UpdateUIValues(throwIndex);
        else
            GameManagerUnlockSequence.unlockPercentile++;

        throwCount++;
        money.transform.SetParent(null);
        money.AddComponent<BoxCollider>();
        Rigidbody rb = money.AddComponent<Rigidbody>();

        rb.mass = 4f;
        float randomX = Random.Range(-1f, 1f);
        GameObject throwPlace = WhichThrowPlace(throwIndex);

        Vector3 throwPointRandomizer = new Vector3(randomX, 0f, 0f);
        Vector3 throwDirection = (throwPlace.transform.position - money.transform.position + Vector3.up * 10f + throwPointRandomizer).normalized;

        rb.AddForce(throwDirection * 1500f);
        float randomTorqueForce = Random.Range(-40f, 40f);
        rb.AddTorque(throwDirection * 20f);
        Destroy(money, 2.5f);
    }

    private void UpdateUIValues(int throwIndex)
    {
        switch (throwIndex)
        {
            case 0:
                GameManagerUpgradeUI.location1LevelPercentile++;

                if (GameManagerUpgradeUI.location1LevelPercentile >= 10)
                {
                    GameManagerUpgradeUI.location1LevelPercentile -= 10;
                    GameManagerUpgradeUI.location1LevelCount++;
                    GameManagerUpgradeUI.upgradeIncereaseUIValueSetUp = true;
                }

                PlayerPrefs.SetInt("Location 1 Level Percentile", GameManagerUpgradeUI.location1LevelPercentile);
                PlayerPrefs.SetInt("Location 1 Level Count", GameManagerUpgradeUI.location1LevelCount);

                break;

            case 1:
                GameManagerUpgradeUI.location2LevelPercentile++;

                if (GameManagerUpgradeUI.location2LevelPercentile >= 10)
                {
                    GameManagerUpgradeUI.location2LevelPercentile -= 10;
                    GameManagerUpgradeUI.location2LevelCount++;
                    GameManagerUpgradeUI.upgradeIncereaseUIValueSetUp = true;
                }

                PlayerPrefs.SetInt("Location 2 Level Percentile", GameManagerUpgradeUI.location2LevelPercentile);
                PlayerPrefs.SetInt("Location 2 Level Count", GameManagerUpgradeUI.location2LevelCount);

                break;
        }
    }

    private GameObject WhichThrowPlace(int throwIndex)
    {
        switch(throwIndex)
        {
            case 0: return throwPoint1;
            case 1: return throwPoint2;
            default: return null;
        }
    }

    #endregion

    #region Helper Methods

    private float LerpPercentageCalculation(ref float lerpCalc, float lerpDevision)
    {
        lerpCalc -= lerpCalc * lerpDevision * Time.deltaTime;
        return Mathf.Lerp(1f, 0f, lerpCalc);
    }

    private Vector3 KubikBezierPath(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float t)
    {
        Vector3 ab = Vector3.Lerp(a, b, t);
        Vector3 bc = Vector3.Lerp(b, c, t);
        Vector3 cd = Vector3.Lerp(c, d, t);

        Vector3 ab_bc = Vector3.Lerp(ab, bc, t);
        Vector3 bc_cd = Vector3.Lerp(bc, cd, t);

        return Vector3.Lerp(ab_bc, bc_cd, t);
    }

    #endregion
}
