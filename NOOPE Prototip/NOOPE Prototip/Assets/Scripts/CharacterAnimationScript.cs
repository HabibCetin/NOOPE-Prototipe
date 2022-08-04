//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimationScript : MonoBehaviour
{
    public Animator animator;

    void Update()
    {
        animator.speed = PlayerController.inputStrenght;
    }
}
