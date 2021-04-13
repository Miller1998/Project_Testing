using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PlayerMinions;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //note to me : please make the new class or something for tidiness of code 

    #region Private_Variable
    
    //Numeric
    [SerializeField]
    private float timeLeft = 136f;
    //UI
    [SerializeField]
    private Image enemyBarImage;
    [SerializeField]
    private Image playerBarImage;
    //Class
    private Power enemyPower;
    private Power playerPower;
    //gameobject
    [SerializeField]
    private GameObject ballPrefabs;
    //bool
    private bool enemyWin = false;
    private bool playerWin = false;
    [SerializeField]
    bool pmActive = false;

    #endregion

    #region Public_Variable

    [Header("UI Caller")]
    public TextMeshProUGUI timerTextSpace;
    public GameObject enemyWinsUI;
    public GameObject playerWinsUI;
    public GameObject drawUI;
    public GameObject pauseMenuUI;

    [Header("Data and GameObject Caller")]
    public PlayerSpawnerData[] minionDatas;
    public GameObject parentAttacker;
    public GameObject parentDefender;
    public Camera mainCamera;
    
    #endregion


    private void Awake()
    {

        enemyPower = new Power();
        playerPower = new Power();

        minionDatas = Resources.LoadAll("Datas/Minions", typeof(PlayerSpawnerData)).Cast<PlayerSpawnerData>().ToArray();

        Time.timeScale = 1;

    }

    private void Update()
    {

        EnergyBar();
        CountDownTimer();
        PlayerActionTouchEvent();
        //if timer reach 0 battle ends

            if (playerWin == true)
            {
                Debug.Log("Player Wins");
                playerWinsUI.SetActive(true);
                enemyWinsUI.SetActive(false);
                drawUI.SetActive(false);

                Time.timeScale = 0;
            }
            else if (enemyWin == true)
            {
                Debug.Log("Enemy Wins");
                playerWinsUI.SetActive(false);
                enemyWinsUI.SetActive(true);
                drawUI.SetActive(false);

                Time.timeScale = 0;
            }
            else
            {
                
                if (timeLeft <= 0)
                {
                    Debug.Log("Battle End...");
                    Debug.Log("Match Draw");
                    playerWinsUI.SetActive(false);
                    enemyWinsUI.SetActive(false);
                    drawUI.SetActive(true);

                    Time.timeScale = 0;
                }

            }

    }

    public void GoalDetection(bool player, bool enemy)
    {
        playerWin = player;
        enemyWin = enemy;
    }

    public void CountDownTimer()
    {
        timeLeft -= 1 * Time.deltaTime;
        timerTextSpace.text = timeLeft.ToString("0") + "s";
    }

    #region EnergyBarMechanic_and_Spawning
    
    void EnergyBar()//this function just for execute the class
    {

        enemyPower.RegenerationUpdate(); //Debug.Log("Enemy energy amount : " + enemyPower.eAmount);
        playerPower.RegenerationUpdate(); //Debug.Log("Player energy amount : " + playerPower.eAmount);
        enemyBarImage.fillAmount = enemyPower.GetEnergyNormalized();
        playerBarImage.fillAmount = playerPower.GetEnergyNormalized();

    }

    
    public void PlayerActionTouchEvent()
    {

        Vector3 mousePos = Mouse.current.position.ReadValue();
        Ray ray = mainCamera.ScreenPointToRay(mousePos);
        RaycastHit rayCastHit;

        if (Physics.Raycast(ray, out rayCastHit/**, float.MaxValue, layerMask**/))
        {

            string colliderName = rayCastHit.collider.name;
            //Debug.Log("Mouse On the" + rayCastHit.collider.name);

            //need screen touch for execute the SpendEnergy(int amount) order (both player and enemy)
            for (int i = 0; i < minionDatas.Length; i++)
            {

                if (minionDatas[i].playerMinionName.Contains("Attacker") && colliderName.Contains("Attacker") /**&& player phase**/)
                {

                    if (playerPower.eAmount >= minionDatas[i].energyCost && Mouse.current.leftButton.wasPressedThisFrame)
                    {

                        SpawnAtLocation(
                            rayCastHit.point, 
                            minionDatas[i].minionModel, 
                            parentAttacker, 
                            minionDatas[i].playerMinionName, 
                            minionDatas[i].spawnTime, 
                            minionDatas[i].normalSpeed,
                            minionDatas[i].carryingSpeed,
                            minionDatas[i].returnSpeed,
                            minionDatas[i].reactiveTime,
                            minionDatas[i].ballSpeed,
                            minionDatas[i].detectionRange);
                        playerPower.SpendEnergy(minionDatas[i].energyCost);

                    }

                }

                if (minionDatas[i].playerMinionName.Contains("Defender") && colliderName.Contains("Defender"))
                {

                    if (enemyPower.eAmount >= minionDatas[i].energyCost && Mouse.current.leftButton.wasPressedThisFrame)
                    {
                        
                        SpawnAtLocation(
                            rayCastHit.point, 
                            minionDatas[i].minionModel, 
                            parentDefender, 
                            minionDatas[i].playerMinionName, 
                            minionDatas[i].spawnTime,
                            minionDatas[i].normalSpeed,
                            minionDatas[i].carryingSpeed,
                            minionDatas[i].returnSpeed,
                            minionDatas[i].reactiveTime,
                            minionDatas[i].ballSpeed,
                            minionDatas[i].detectionRange);
                        enemyPower.SpendEnergy(minionDatas[i].energyCost);

                    }

                }

            }

        }

    }

    public void SpawnAtLocation(Vector3 spawnPosition, GameObject minionModel, GameObject parent, string name, float spTime, float normalSpeed, float carryingSpeed, float returnSpeed, float reactiveTime, float ballSpeed, float detectionRange)
    {

        GameObject deployedMinion = Instantiate(minionModel, spawnPosition, Quaternion.identity);
        deployedMinion.transform.parent = parent.transform;
        deployedMinion.transform.rotation = parent.transform.rotation;
        deployedMinion.gameObject.GetComponent<MinionBehaviour>().setDefault(name, spTime, normalSpeed, returnSpeed, carryingSpeed, reactiveTime, ballSpeed, detectionRange);

    }

    #endregion


    #region Clearing_Area_Every_Phase_Begin

    void ClearAll()
    {

        foreach (Transform child in parentAttacker.transform)
        {
            Destroy(child);
        }
        foreach (Transform child in parentDefender.transform)
        {
            Destroy(child);
        }

    }

    #endregion

    #region UISystem

    public void ChangeScenes(string scene)
    {
        SceneManager.LoadScene(scene);
    }

    public void PauseSystem()
    {

        pmActive = !pmActive;

        if (pmActive == true)
        {
            Time.timeScale = 0;
            pauseMenuUI.gameObject.SetActive(true);
        }
        else
        {
            Time.timeScale = 1;
            pauseMenuUI.gameObject.SetActive(false);
        }

    }

    #endregion

}