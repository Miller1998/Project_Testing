using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerMinions
{
    [CreateAssetMenu(fileName = "playerMinions", menuName = "PlayerMinion/AddNewMinion")]
    public class PlayerSpawnerData : ScriptableObject
    {

        #region Player_Information

        [Header("Player Minion Info")]
        public string playerMinionName;
        [TextArea(2, 10)]
        public string minionDesc;

        #endregion

        #region Player_Abilities

        [Header("Minion's Abilities")]
        public float energyCost;
        public float spawnTime;
        public float reactiveTime;
        public float normalSpeed;
        public float carryingSpeed;
        public float ballSpeed;

        public float returnSpeed;
        public float detectionRange;

        #endregion

        #region GameObject_Caller

        [Header("GameObject Caller")]
        public GameObject minionModel;
        //public AudioClip jetSFX;
        //public AudioClip destroyedSFX;

        #endregion

    }
}


