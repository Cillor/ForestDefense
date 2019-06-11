using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AlienController : MonoBehaviour
{
    public AudioSource laserShootSource;
    public float fastMoveSpeed = 2.5f;
    public float normalMoveSpeed = 1.2f;
    public float attackDst;

    [Space]
    public Transform gunBarrel;
    public int damage = 10;
    public float weaponRange = 50f;

    private WaitForSeconds castTime = new WaitForSeconds(.5f);
    private WaitForSeconds shotDuration = new WaitForSeconds(.07f);
    private LineRenderer laserLine;
    private bool allowedToShoot = true;

    [Space]
    public float viewAngle, viewDistance;
    public float searchAreaRadius, earRangeRadius;
    public LayerMask playerMask, obstacleMask;
    public bool playerInFoV, playerInsideSearchArea, playerInsideEarRange, hasInterest = true;
    bool huntPlayer = false;

    private bool active = false;

    #region SpawnFX
    [Space]
    public GameObject respawnTeleportFX;
    public Light teleportLight;
    public GameObject characterGeometry;

    private float t = 0.0f;
    private float fadeStart = 2;
    private float fadeEnd = 0;
    private float fadeTime = 1;
    private float pauseTime = 0;

    IEnumerator SpawnCharacter()
    {
        respawnTeleportFX.SetActive(true);

        yield return new WaitForSeconds(1.75f);

        characterGeometry.SetActive(true);
        StartCoroutine("FadeLight");
    }

    IEnumerator FadeLight()
    {
        while (t < fadeTime)
        {

            if (pauseTime == 0)
            {
                t += Time.deltaTime;
            }

            teleportLight.intensity = Mathf.Lerp(fadeStart, fadeEnd, t / fadeTime);
            yield return 0;

        }
        t = 0;

        active = true;
    }
    #endregion

    private GameObject player;
    private NavMeshAgent agent;

    private bool patrolHelper = true;

    void Start()
    {
        active = false;
        characterGeometry.SetActive(false);
        respawnTeleportFX.SetActive(false);
        StartCoroutine("SpawnCharacter");
        player = GameObject.FindGameObjectWithTag("Player");
        laserLine = GetComponent<LineRenderer>();
        agent = GetComponent<NavMeshAgent>();
        agent.speed = normalMoveSpeed;
        patrolHelper = false;
        Patrol();
    }

    void WhereIsPlayer()
    {
        float dstToPlayer;
        playerInFoV = PlayerInFov(out dstToPlayer);

        if (dstToPlayer < searchAreaRadius && dstToPlayer > viewDistance && hasInterest)
        {
            playerInsideSearchArea = true;
        }
        else
        {
            playerInsideSearchArea = false;
        }

        if (dstToPlayer < earRangeRadius)
        {
            playerInsideEarRange = true;
            huntPlayer = true;
        }
        else
        {
            playerInsideEarRange = false;
        }
    }

    bool PlayerInFov(out float dstToPlayer)
    {
        dstToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (dstToPlayer < viewDistance)
        {
            Vector3 dirToPlayer = (player.transform.position - transform.position).normalized;
            float angleBtwDroneAndPlayer = Vector3.Angle(transform.forward, dirToPlayer);
            if (angleBtwDroneAndPlayer < viewAngle / 2f)
            {
                if (!Physics.Linecast(transform.position, player.transform.position, obstacleMask))
                {
                    hasInterest = true;
                    return true;
                }
            }
        }
        return false;
    }

    void Update()
    {
        if (active)
        {
            WhereIsPlayer();
            if (huntPlayer)
            {
                Hunt();
            }
            else
            {
                if (patrolHelper)
                {
                    patrolHelper = false;
                    Patrol();
                }

                if (playerInFoV)
                {
                    huntPlayer = true;
                    StopAllCoroutines();
                }
            }

            if (agent.remainingDistance < .1f)
            {
                patrolHelper = true;
            }
        }
    }

    void Patrol()
    {
        NavMeshPath path = new NavMeshPath();
        Vector3 patrolPoint;
        do
        {
            float gridXhalf = Grid.gridSize.x / 2f;
            float gridYhalf = Grid.gridSize.y / 2f;
            float _xPos = Random.Range(gridXhalf * -1f, gridXhalf);
            float _yPos = Random.Range(gridYhalf * -1f, gridYhalf);
            patrolPoint = new Vector3(_xPos, transform.position.y, _yPos);
            agent.CalculatePath(patrolPoint, path);
        } while (path.status == NavMeshPathStatus.PathPartial || path.status == NavMeshPathStatus.PathInvalid);

        agent.SetDestination(patrolPoint);
    }

    void Hunt()
    {
        patrolHelper = true;
        float dstToTarget;
        PlayerInFov(out dstToTarget);

        Vector3 target = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);

        if (dstToTarget > attackDst)
        {
            if (playerInFoV || playerInsideEarRange)
            {
                agent.SetDestination(target);
                gameObject.transform.LookAt(player.transform);

                laserLine.enabled = false;
                allowedToShoot = true;
                StopCoroutine("Shot");
            }
            if (playerInsideSearchArea)
            {
                agent.SetDestination(Vector3.forward * searchAreaRadius * 2);

                if (!agent.hasPath)
                {
                    if (patrolHelper)
                    {
                        patrolHelper = false;
                        Patrol();
                    }
                    hasInterest = false;
                }
                transform.LookAt(Vector3.forward * 10);

                laserLine.enabled = false;
                allowedToShoot = true;
                StopCoroutine("Shot");
            }
        }
        else if (dstToTarget < attackDst)
        {
            agent.ResetPath();

            if (allowedToShoot)
            {
                allowedToShoot = false;
                StartCoroutine("Shot");
            }
        }
        else if (dstToTarget < attackDst - 3)
        {
            agent.ResetPath();

            transform.position = Vector3.MoveTowards(transform.position, target, -agent.speed * Time.deltaTime);
            gameObject.transform.LookAt(player.transform);
        }

        if (!playerInsideSearchArea && !playerInFoV && !playerInsideEarRange)
        {
            agent.speed = normalMoveSpeed;
            huntPlayer = false;
        }

        if (playerInFoV || (playerInFoV && playerInsideEarRange))
            agent.speed = fastMoveSpeed;
        else
            agent.speed = normalMoveSpeed;
    }

    private IEnumerator Shot()
    {
        RaycastHit hit;
        gameObject.transform.LookAt(player.transform);

        //castAnimationStart
        yield return castTime;
        //castAnimationOff

        laserShootSource.Play();
        laserLine.SetPosition(0, gunBarrel.position);

        if (Physics.Raycast(gunBarrel.position, gunBarrel.forward, out hit, weaponRange, playerMask))
        {
            laserLine.SetPosition(1, hit.point);

            PlayerStatus pStat = hit.collider.gameObject.GetComponentInParent<PlayerStatus>();
            if (pStat != null)
                pStat.PoisonChange(0.03f);
        }
        else
        {
            laserLine.SetPosition(1, gunBarrel.position + (gunBarrel.forward * weaponRange));
        }

        laserLine.enabled = true;

        yield return shotDuration;

        laserLine.enabled = false;
        allowedToShoot = true;
    }

    private void LateUpdate()
    {
        transform.position = new Vector3(transform.position.x, 1.6f, transform.position.z);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!huntPlayer)
            Patrol();
    }
}
