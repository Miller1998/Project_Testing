using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MinionBehaviour : MonoBehaviour
{
    #region Private_Variable
    //numeric and string 
    [SerializeField]
    private string minionName;
    private float spawnTime;
    private float tStart = 0f;
    private float returnSpeed;
    [SerializeField]
    private float normalSpeed;
    private float carryingSpeed;
    private float reactiveTime;
    private float ballSpeed;
    private float detectionRange;
    private int point = 0;   
    //GameObject Caller
    private GameManager gM;
    private GameObject parentAtk;
    private GameObject parentDef;
    [SerializeField]
    private Transform[] childrenAtk;
    [SerializeField]
    private Transform[] childrenDef;

    //Physics
    private Vector3 originalPos;
    private Rigidbody thisMinionRB;

    //bool
    private bool hasBall = false;
    private bool hasCaptured = false;

    //navmesh agent
    //private NavMeshAgent nav;
    #endregion

    #region Public_Variable

    public GameObject ball;

    #endregion

    public void setDefault(string mN, float sT, float nS, float rS, float cS, float rT, float bS, float dR)
    {

        minionName = mN;
        spawnTime = sT;
        reactiveTime = rT;
        normalSpeed = nS;
        carryingSpeed = cS;
        ballSpeed = bS;
        detectionRange = dR;
        returnSpeed = rS;

    }

    void Awake()
    {
        
        this.gameObject.GetComponent<MeshRenderer>().materials[0].color = Color.white;
        originalPos = new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y, this.gameObject.transform.position.z);
        ball = GameObject.FindGameObjectWithTag("ball");
        gM = GameObject.FindGameObjectWithTag("GameManager").gameObject.GetComponent<GameManager>();
        thisMinionRB = this.gameObject.GetComponent<Rigidbody>();
        //nav = this.GetComponent<NavMeshAgent>();

        parentAtk = GameObject.FindGameObjectWithTag("AttackerArea");
        parentDef = GameObject.FindGameObjectWithTag("DefenderArea");

    }

    void Start()
    {

        

    }

    void Update()
    {

        childrenAtk = new Transform[parentAtk.transform.childCount];
        for (int i = 0; i < parentAtk.transform.childCount; i++)
        {
            childrenAtk[i] = parentAtk.transform.GetChild(i);
        }

        childrenDef = new Transform[parentDef.transform.childCount];
        for (int i = 0; i < parentDef.transform.childCount; i++)
        {
            childrenDef[i] = parentDef.transform.GetChild(i);
        }

        MinionsDeployment();

    }

    void FixedUpdate()
    {
        
    }

    #region Player/Minion_ACTION

    void MinionsDeployment()
    {

        if (this.tag.Contains("Player"))
        {

            //Debug.Log(tStart);
            this.gameObject.GetComponent<MeshRenderer>().materials[0].color = Color.Lerp(Color.grey, Color.blue, tStart);
            if (tStart < 1)
            {
                tStart += Time.deltaTime / spawnTime;
            }

            if (tStart >= 1)
            {
                AttackerMovement();
            }

        }
        if (this.tag.Contains("Enemy"))
        {
            //Debug.Log("Enemy has been deployed");
            this.gameObject.GetComponent<MeshRenderer>().materials[0].color = Color.Lerp(Color.grey, Color.red, tStart);
            if (tStart < 1)
            {
                tStart += Time.deltaTime / spawnTime;
            }

            if (tStart >= 1)
            {
                DefenderMovement();
            }
            
            //detect player with the ball by the detection Range

        }

    }

    #endregion

    #region Collider_And_Trigger

    void OnCollisionEnter(Collision col)
    {
        //player colision detection
        if (this.tag.Contains("Player"))
        {

            if (col.collider.tag == "Enemy" && this.hasBall == true)
            {
                hasCaptured = true;
                hasBall = false;
            }

            if (col.collider.tag == "ball")
            {
                ball.gameObject.transform.parent = this.gameObject.transform;
                hasBall = true;
            }

        }

        //enemy colision detection
        if (this.tag.Contains("Enemy"))
        {
            if (col.collider.tag == "Player")
            {
                //If caught by the target, become Inactivated for a period of time (4) (At Collider)
                hasCaptured = true;
            }
        }

        //both of player and enemy
        if (col.collider.tag == "EnemyWall" || col.collider.tag == "PlayerWall")
        {
            Destroy(this.gameObject);
        }

    }

    void OnTriggerEnter(Collider other)
    {



    }

    #endregion

    #region MinionMovement

    void AttackerMovement()
    {
        
        //nav.SetDestination(ball.transform.position);
             

        for (int i = 0; i < childrenAtk.Length; i++)
        {

            if (childrenAtk[i].gameObject.GetComponent<MinionBehaviour>().hasBall == true && hasCaptured == false)
            {

                point = i;
                childrenAtk[point].GetComponent<NavMeshAgent>().speed = carryingSpeed;

                childrenAtk[point].GetComponent<MeshRenderer>().material.SetColor("_EMISSION", new Color(129f, 255f, 0f));
                childrenAtk[point].GetComponent<MeshRenderer>().material.EnableKeyword("_EMISSION");
                //go to enemy gate
                Transform gate = GameObject.FindGameObjectWithTag("EnemyGate").transform;
                childrenAtk[point].GetComponent<NavMeshAgent>().SetDestination(gate.position);

            }
            else if (childrenAtk[point].gameObject.GetComponent<MinionBehaviour>().hasBall == true 
                && childrenAtk[i].gameObject.GetComponent<MinionBehaviour>().hasBall == false
                && childrenAtk[i].gameObject.GetComponent<MinionBehaviour>().hasCaptured == false)
            {
                //when this object has a ball, others must stop following the ball
                //after that execute this : 
                childrenAtk[i].transform.position += Vector3.forward * Time.deltaTime * normalSpeed;
                childrenAtk[i].GetComponent<MeshRenderer>().material.DisableKeyword("_EMISSION");
            }
            else if (childrenAtk[i].gameObject.GetComponent<MinionBehaviour>().hasBall == false
                && childrenAtk[i].gameObject.GetComponent<MinionBehaviour>().hasCaptured == false)
            {

                childrenAtk[i].GetComponent<NavMeshAgent>().speed = normalSpeed;
                childrenAtk[i].GetComponent<NavMeshAgent>().SetDestination(ball.transform.position);
                childrenAtk[i].GetComponent<MeshRenderer>().material.DisableKeyword("_EMISSION");

            }
            //If caught the chaser, become Inactivated for a period of time (4) (At Collider)
            else if (childrenAtk[point].gameObject.GetComponent<MinionBehaviour>().hasCaptured == true)
            {
                          
                //Greyscale until reactivated
                childrenAtk[point].GetComponent<MeshRenderer>().materials[0].color = Color.grey;
                childrenAtk[i].GetComponent<MeshRenderer>().material.DisableKeyword("_EMISSION");

                //If caught, pass the Ball to nearest active Attacker at a speed (7) and become Inactivated for a period of time (4)
                ball.transform.parent = null;

                //Moving back to its origin position at faster speed (8)
                childrenAtk[point].GetComponent<NavMeshAgent>().speed = returnSpeed;
                childrenAtk[point].GetComponent<NavMeshAgent>().SetDestination(originalPos);

            }

        }

    }

    void DefenderMovement()
    {

        for (int i = 0; i < childrenAtk.Length; i++)
        {
            
            for (int j = 0; j < childrenDef.Length; j++)
            {

                if (Vector3.Distance(childrenDef[j].position, childrenAtk[point].position) <= detectionRange
                    && childrenAtk[point].gameObject.GetComponent<MinionBehaviour>().hasBall == true)
                {

                    // Chasing when the attacker with Ball reach the Detection circle:
                    //Lock target to that attacker, and chasing it at a speed (5)
                    childrenDef[j].GetComponent<NavMeshAgent>().speed = normalSpeed;
                    childrenDef[j].GetComponent<NavMeshAgent>().SetDestination(childrenAtk[point].position);
                    
                }
                //If caught the target, become Inactivated for a period of time (4) (At Collider)
                else if (childrenAtk[point].gameObject.GetComponent<MinionBehaviour>().hasCaptured == true && 
                    childrenAtk[point].gameObject.GetComponent<MinionBehaviour>().hasBall == false 
                    && childrenDef[j].gameObject.GetComponent<MinionBehaviour>().hasCaptured == true)
                {
                    int capsByThis = j;
                    //Greyscale until reactivated
                    childrenDef[capsByThis].gameObject.GetComponent<MeshRenderer>().materials[0].color = Color.gray;

                    //Moving back to its origin position at faster speed (8)
                    childrenDef[capsByThis].GetComponent<NavMeshAgent>().speed = returnSpeed;
                    childrenDef[capsByThis].GetComponent<NavMeshAgent>().SetDestination(originalPos);
                }
                else
                {
                    // Standby after activated
                    //show the area detection
                }

            }

        }

    }

    

    #endregion

}
