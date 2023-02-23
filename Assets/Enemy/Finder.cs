using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Finder : MonoBehaviour
{
    public GameObject target;
    public Vector3 tPos;
    // public Animator anim;
    // public Animation idle, walk, shoot;
    public Animation anim;
    public Bullet bullet;
    public int health = 0;
    public float followDistance = 0f;
    public NavMeshAgent agent;
    // public NavMeshObstacle obstacle;
    public Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        this.agent = GetComponent<NavMeshAgent>();
        // this.obstacle = GetComponent<NavMeshObstacle>();
        this.target = GameObject.FindWithTag("Player");
        this.rb = this.GetComponent<Rigidbody>();

        // this.obstacle.enabled = false;
        tPos = target.transform.position;
        agent.destination = target.transform.position;

        this.anim = this.GetComponent<Animation>();

        // this.idle  = this.anim.GetClip("Idle");
        // this.walk  = this.anim.GetClip("Walking");
        // this.shoot = this.anim.GetClip("Shooting");
        
        this.health += 100;
    }

    private void Update() {
        if (Mathf.Abs(Vector3.Distance(this.tPos, target.GetComponent<Rigidbody>().position)) > 0.1f) {
            // this.obstacle.enabled = false;
            // this.agent.enabled = true;
            
            agent.destination = target.transform.position;
        }

        this.tPos = target.GetComponent<Rigidbody>().position;

        if (RemainingDistance(agent.path.corners) > followDistance) {
            agent.isStopped = true;
            if (!this.anim.IsPlaying("Idle"))
                this.anim.Play("Idle");
            // this.agent.enabled = false;
            // this.obstacle.enabled = true;
            // return;
        } else {
            // this.obstacle.enabled = false;
            // this.agent.enabled = true;

            agent.destination = target.transform.position;
            agent.isStopped = false;

            if (agent.velocity.magnitude < 0.1f) {
                
                this.rb.transform.rotation = Quaternion.Lerp(this.rb.rotation, Quaternion.LookRotation(this.target.GetComponent<Rigidbody>().transform.position - this.rb.transform.position), 0.7f);
                Vector3 rot = this.rb.transform.rotation.eulerAngles;
                rot.x = 0;
                rot.z = 0;
                this.rb.transform.rotation = Quaternion.Euler(rot);
                RaycastHit hit;
                // if (Physics.Raycast(this.rb.position + Vector3.up, this.rb.transform.rotation * Vector3.forward, out hit, 10f, ~(1 << LayerMask.NameToLayer("Bullet")))) {
                if (Physics.SphereCast(this.rb.position + Vector3.up, 0.3f, this.rb.rotation * Vector3.forward, out hit, 10f, ~(1 << LayerMask.NameToLayer("Bullet")))) {
                    Debug.DrawLine(this.rb.position + Vector3.up, hit.point, Color.green);
                    if (hit.transform.CompareTag("Player")) {
                        // this.agent.enabled = false;
                        // this.obstacle.enabled = true;
                        if (!this.anim.IsPlaying("Shooting"))
                            this.anim.Play("Shooting");
                    } else {
                        if (!this.anim.IsPlaying("Idle"))
                            this.anim.Play("Idle");
                    }
                }
            } else {
                if (!this.anim.IsPlaying("Walking"))
                    this.anim.Play("Walking");
            }
            
        }
    }

    public float RemainingDistance(Vector3[] points) {
        if (points.Length < 2) return 0;
        float distance = 0;
        for (int i = 0; i < points.Length - 1; i++)
            distance += Vector3.Distance(points[i], points[i + 1]);
        return distance;
    }

    public void Hit(int damage) {
        this.health -= damage;

        if (this.health <= 0) {
            Destroy(this.transform.gameObject);
        }
    }

    public void Shoot() {
        Bullet b = Instantiate(this.bullet, this.rb.worldCenterOfMass + (this.rb.rotation * Vector3.forward), Quaternion.Euler(90 + this.rb.rotation.eulerAngles.x, 0 + this.rb.rotation.eulerAngles.y, 0 + this.rb.rotation.eulerAngles.z));
        b.gameObject.tag = "Enemy Bullet";
        // b.gameObject.layer = LayerMask.NameToLayer("Bullet");
        b.Init(this.rb.rotation.eulerAngles, 1f, 10);
    }

    private void OnCollisionEnter(Collision other) {
        if (other.transform.CompareTag("Bullet")) {
            Hit(other.transform.GetComponent<Bullet>().damage);
            Destroy(other.transform.gameObject);
        }
    }

    public int GetHealth() {
        return this.health;
    }


}
