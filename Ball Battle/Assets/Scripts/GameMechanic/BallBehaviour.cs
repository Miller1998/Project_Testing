using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallBehaviour : MonoBehaviour
{

    public GameManager gM;

    void OnTriggerEnter(Collider other)
    {

        if (other.tag == "PlayerGate")
        {
            Debug.Log("EnemyGoal");
            gM.GoalDetection(false, true);
        }
        else if (other.tag == "EnemyGate")
        {
            Debug.Log("PlayerGoal");
            gM.GoalDetection(true, false);
        }

    }

}
