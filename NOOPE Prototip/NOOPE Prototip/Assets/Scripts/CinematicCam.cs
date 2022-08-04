//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

public class CinematicCam : MonoBehaviour
{
    private GameObject choosenFocusPlace;
    public GameObject player;
    public GameObject location1;
    public GameObject location2;

    private Vector3 freeLookOffset  = new Vector3(0f, 4f, 6f);
    private Vector3 collectOffset   = new Vector3(7f, 4f, 5f);
    private Vector3 throwOffset     = new Vector3(-4f, 6f, 4.5f);
    private Vector3 beginningCamPos;

    private Quaternion beginningQua;
    private Quaternion freeLookQua  = Quaternion.Euler(35.146f, 180.103f, 0.029f);
    private Quaternion collectQua   = Quaternion.Euler(35.146f, 240.103f, 0.029f);
    private Quaternion throwQua     = Quaternion.Euler(50.146f, 135.115f, 0.038f);

    private float lerpT = 1f;
    private float lerpCalc;
    private float lerpPercentage;

    public static bool zoomForCollect;
    public static bool zoomForThrow;
    public static bool initializeCalcValues;
    public static int focusPlaceIndex;

    private void LateUpdate()
    {
        if (initializeCalcValues)
            InitializeBeforeCinematicLook();

        if (zoomForCollect)
            CinematicLook(ref lerpCalc, collectOffset, collectQua, 5f);

        else if (zoomForThrow)
            CinematicLook(ref lerpCalc, throwOffset, throwQua, -3f);

        else FreeLook();
    }

    private void CinematicLook(ref float lerpCalculation, Vector3 offset, Quaternion desiredQua, float xValueForBeginning)
    {
        lerpPercentage = LerpPercentageCalculation(ref lerpCalculation);

        transform.position = KubikBezierPath
                                    (
                                        beginningCamPos,
                                        choosenFocusPlace.transform.position + new Vector3(xValueForBeginning, 0f, 8f),
                                        choosenFocusPlace.transform.position + offset + new Vector3(0f, 4f, 0f),
                                        choosenFocusPlace.transform.position + offset,
                                        lerpPercentage
                                    );

        transform.rotation = Quaternion.Lerp(beginningQua, desiredQua, lerpPercentage);
    }

    private void InitializeBeforeCinematicLook()
    {
        lerpCalc = 1f;
        choosenFocusPlace = WhichFocusPlace(focusPlaceIndex);
        beginningCamPos = transform.position;
        beginningQua = transform.rotation;
        initializeCalcValues = false;
    }

    private GameObject WhichFocusPlace(int placeIndex)
    {
        switch(placeIndex)
        {
            case 0: return location1;
            case 1: return location2;
            default: Debug.Log("Odaklanacak Yer Bulunamadý"); return null;
        }
    }

    private float LerpPercentageCalculation(ref float lerpCalc)
    {
        lerpCalc -= lerpCalc * Time.deltaTime * .36f; //evet lerpCalc'tan lerpCalc'ý da hesaba katarak çýkarmamýz lazým
        return Mathf.Lerp(1f, 0f, lerpCalc);
    }


    private void FreeLook()
    {
        Vector3 desiredPos = player.transform.position + freeLookOffset;
        transform.position = Vector3.Lerp(transform.position, desiredPos, lerpT * Time.deltaTime);
        //transform.LookAt(player.transform);
        transform.rotation = Quaternion.Lerp(transform.rotation, freeLookQua, lerpT * Time.deltaTime);
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
}
