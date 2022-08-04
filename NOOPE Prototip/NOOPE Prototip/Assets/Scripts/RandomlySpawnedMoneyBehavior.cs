using UnityEngine;

public class RandomlySpawnedMoneyBehavior : MonoBehaviour
{

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.name == "Character")
        {
            GameManagerMoneyObject.randomlyTouchedMoney = gameObject;
            GameManagerMoneyObject.collectTheRandomMoney = true;
        }
    }

}
