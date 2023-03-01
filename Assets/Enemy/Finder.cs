using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Finder : MonoBehaviour
{
    public GameObject target;
    public Vector3 tPos;
    public Animation anim;
    public Bullet bullet;
    public int health = 0;
    public float followDistance = 0f;
    public NavMeshAgent agent;
    public Rigidbody rb;

    public LayerMask EnemyMask;

    public Vector3 destination;


    [Range(0, 360)]
    public int points = 0;
    [Range(0, 360)]
    public float viewingAngle = 90;
    public float viewRange = 10f;
    public Vector3 currentlyFacing;
    public List<RaycastHit> hits;

    public bool viewingPlayer;

    void Start()
    {
        this.agent = GetComponent<NavMeshAgent>();
        this.target = GameObject.FindWithTag("Player");
        this.rb = this.GetComponent<Rigidbody>();
        this.EnemyMask = LayerMask.GetMask("Enemy");

        tPos = target.transform.position;
        agent.destination = target.transform.position;

        this.anim = this.GetComponent<Animation>();

        this.health += 100;
        this.viewingPlayer = false;
        this.agent.isStopped = false;
    }

    private void Update()
    {

        if (Mathf.Abs(Vector3.Distance(this.tPos, target.GetComponent<Rigidbody>().position)) > 0.1f)
        {

            agent.destination = target.transform.position;
        }

        this.tPos = target.GetComponent<Rigidbody>().position;

        if (RemainingDistance(agent.path.corners) > followDistance)
        {
            agent.isStopped = true;
            if (!this.anim.IsPlaying("Idle"))
                this.anim.Play("Idle");
        }
        else
        {
            agent.isStopped = false;

            if (!this.anim.IsPlaying("Walking") && !viewingPlayer)
                this.anim.Play("Walking");


            this.destination = CheckInFront();

            if (this.destination == Vector3.zero)
            {
                float angle = Random.Range(Mathf.PI / 4, Mathf.PI * 3 / 4);

                this.destination = this.rb.position + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
            }

            agent.destination = this.destination;

            CheckforPlayer();
        }
    }

    public Vector3 CheckInFront()
    {
        this.currentlyFacing = this.transform.rotation.eulerAngles;
        for (int i = 0, i2 = 0; i < (this.points * 2) + 1; i++)
        {

            float t = Mathf.Pow(-1, i) * (this.viewingAngle / ((this.points * 2) + 1)) * i2 + this.currentlyFacing.y;
            Vector3 end = new Vector3(Mathf.Sin(Mathf.Deg2Rad * t), 0, Mathf.Cos(Mathf.Deg2Rad * t));

            if (i % 2 == 0) i2++;

            this.hits = new List<RaycastHit>(Physics.RaycastAll(this.transform.position + Vector3.up, end, this.viewRange, this.EnemyMask.value | 1 << LayerMask.NameToLayer("Wall") | 1 << LayerMask.NameToLayer("Cards")));

            if (this.hits.Count == 0) return this.rb.position + new Vector3(end.x, 0, end.z);
        }
        return Vector3.zero;
    }

    public void CheckforPlayer()
    {
        for (int i = 0, i2 = 0; i < 36; i++)
        {
            float t = Mathf.Pow(-1, i) * (360 / 36) * i2 + this.currentlyFacing.y;
            Vector3 end = new Vector3(Mathf.Sin(Mathf.Deg2Rad * t), 0, Mathf.Cos(Mathf.Deg2Rad * t));

            if (i % 2 == 0) i2++;

            this.hits = new List<RaycastHit>(Physics.RaycastAll(this.transform.position + Vector3.up, end, this.viewRange * 3f, 1 << LayerMask.NameToLayer("Player")));

            if (this.hits.Count > 0)
            {
                this.agent.destination = this.rb.position;
                this.rb.transform.rotation = Quaternion.Lerp(this.rb.rotation, Quaternion.LookRotation(this.target.GetComponent<Rigidbody>().transform.position - this.rb.transform.position), 0.7f);
                Vector3 rot = this.rb.transform.rotation.eulerAngles;
                rot.x = 0;
                rot.z = 0;
                this.rb.transform.rotation = Quaternion.Euler(rot);

                RaycastHit hit;
                Physics.Raycast(this.transform.position + Vector3.up, end, out hit, this.viewRange * 3f, ~(1 << LayerMask.NameToLayer("Bullet")));

                if (hit.transform.CompareTag("Player"))
                {
                    if (!this.anim.IsPlaying("Shooting"))
                        this.anim.Play("Shooting");
                }
                else
                {
                    if (!this.anim.IsPlaying("Idle"))
                        this.anim.Play("Idle");
                }
                this.viewingPlayer = true;
                return;
            }
        }
        this.viewingPlayer = false;
    }

    public float RemainingDistance(Vector3[] points)
    {
        if (points.Length < 2) return 0;
        float distance = 0;
        for (int i = 0; i < points.Length - 1; i++)
            distance += Vector3.Distance(points[i], points[i + 1]);
        return distance;
    }

    public void Hit(int damage)
    {
        this.health -= damage;

        if (this.health <= 0)
        {
            Destroy(this.transform.gameObject);
        }
    }

    public void Shoot()
    {
        Bullet b = Instantiate(this.bullet, this.rb.worldCenterOfMass + (this.rb.rotation * Vector3.forward), Quaternion.Euler(90 + this.rb.rotation.eulerAngles.x, 0 + this.rb.rotation.eulerAngles.y, 0 + this.rb.rotation.eulerAngles.z));
        b.gameObject.tag = "Enemy Bullet";
        b.Init(this.rb.rotation.eulerAngles, 1f, 10);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.CompareTag("Bullet"))
        {
            Hit(other.transform.GetComponent<Bullet>().damage);
            Destroy(other.transform.gameObject);
        }
    }

    public int GetHealth()
    {
        return this.health;
    }
}
