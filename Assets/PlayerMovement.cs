using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    public GameObject cam;
    public Bullet bullet;
    public bool focusScene;

    public float walkSpeed = 10f;
    public int damage = 10;
    private float speed;
    public int health;
    public int maxAmmo = 10;
    public int currAmmo;
    public bool reloading;
    public bool shooting;
    public bool onGround;
    public bool jump;

    public Vector3 rotation = Vector3.zero;
    public Vector3 movement = Vector3.zero;
    public Vector3 offset = Vector3.zero;
    public Rigidbody rb;

    public TextMeshProUGUI eHealth, pHealth, pDamage, Enemies, pReloading, pAmmo, pTime;
    public float currentTime;

    private RaycastHit hit;

    LayerMask wallMask;

    void Start()
    {
        this.wallMask = LayerMask.GetMask("Wall");
        this.rotation = this.transform.rotation.eulerAngles;
        this.rb = this.GetComponent<Rigidbody>();
        this.health = 100;
        this.currAmmo = this.maxAmmo;
        this.reloading = false;
        this.shooting = false;
        this.onGround = false;
        this.jump = false;
        this.eHealth = GameObject.Find("Enemy Health").GetComponent<TextMeshProUGUI>();
        this.pHealth = GameObject.Find("Player Health").GetComponent<TextMeshProUGUI>();
        this.pDamage = GameObject.Find("Player Damage").GetComponent<TextMeshProUGUI>();
        this.pReloading = GameObject.Find("Player Reloading").GetComponent<TextMeshProUGUI>();
        this.pAmmo = GameObject.Find("Player Ammo").GetComponent<TextMeshProUGUI>();
        this.pTime = GameObject.Find("Player Time").GetComponent<TextMeshProUGUI>();

        this.pReloading.text = "";
        this.pAmmo.text = "Ammo: " + this.currAmmo;

        Cursor.lockState = CursorLockMode.Locked;
        
        this.pHealth.text = "Health: " + this.health;
        this.pDamage.text = "Damage: " + this.damage;

        this.currentTime = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        this.currentTime += Time.deltaTime;
        this.pTime.text = Mathf.RoundToInt(this.currentTime) + " : Time";
        this.movement = Vector3.zero;
        // this.jump = false;


        // if (focusScene) UnityEditor.SceneView.FocusWindowIfItsOpen(typeof(UnityEditor.SceneView));

        // if (Input.GetKey(KeyCode.W)) this.transform.Translate(Vector3.forward * this.speed * Time.deltaTime);
        // if (Input.GetKey(KeyCode.S)) this.transform.Translate(Vector3.back    * this.speed * Time.deltaTime);
        // if (Input.GetKey(KeyCode.A)) this.transform.Translate(Vector3.left    * this.speed * Time.deltaTime);
        // if (Input.GetKey(KeyCode.D)) this.transform.Translate(Vector3.right   * this.speed * Time.deltaTime);

        if (Input.GetKey(KeyCode.W)) this.movement += Vector3.forward;
        if (Input.GetKey(KeyCode.S)) this.movement += Vector3.back;
        if (Input.GetKey(KeyCode.A)) this.movement += Vector3.left;
        if (Input.GetKey(KeyCode.D)) this.movement += Vector3.right;

        if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();

        // if (Input.GetKey(KeyCode.Space)) this.jump = true;

        // if (Physics.Raycast(this.rb.position, Vector3.down, out this.hit, Mathf.Infinity)) {
        //     float dis = Mathf.Abs(Vector3.Distance(this.rb.position, this.hit.point));
        //     if (dis < 0.6f && !this.onGround && !this.jump) {
        //         this.onGround = true;
        //         this.rb.position = this.hit.point;
        //     }

        //     if (this.jump && this.onGround) {
        //         // this.rb.velocity += Vector3.up * 1f;
        //         this.rb.AddForce(transform.up * 1500f * Time.deltaTime, ForceMode.Impulse);
        //         this.onGround = false;
        //         this.jump = false;
        //     } else {
        //         // this.rb.velocity += Vector3.down;
        //         // this.rb.AddForce(Vector3.down * Time.deltaTime);
        //     }
        // }
        // if (this.rb.velocity.y < 0.1f) { this.rb.velocity = Vector3.zero; this.onGround = true; }


        if (Input.GetMouseButton(0) && !this.shooting && this.currAmmo > 0 && !this.reloading) StartCoroutine(Shoot());
        if ((this.currAmmo <= 0 || Input.GetKeyDown(KeyCode.R)) && !this.reloading) StartCoroutine(Reload());

        // Debug.DrawLine(this.cam.GetComponent<Rigidbody>().position, this.cam.GetComponent<Rigidbody>().position + this.cam.GetComponent<Rigidbody>().rotation * Vector3.forward * 5, Color.blue);

        if (Physics.SphereCast(this.cam.transform.position, 0.1f, this.cam.transform.rotation * Vector3.forward, out this.hit, float.PositiveInfinity, ~(1 << LayerMask.NameToLayer("Bullet")))) {
            if (this.hit.transform.CompareTag("Enemy")) {
                this.eHealth.text = this.hit.transform.name + ": " + this.hit.transform.gameObject.GetComponent<Finder>().GetHealth();
            } else {
                this.eHealth.text = "";
            }
        } else {
            this.eHealth.text = "";
        }

        this.speed = (Input.GetKey(KeyCode.LeftShift))? this.walkSpeed * 2: this.walkSpeed;

        rotation.y += Input.GetAxis("Mouse X");
        rotation.x += -Input.GetAxis("Mouse Y");

        rotation.x = (rotation.x > 45) ? 45 : rotation.x;
        rotation.x = (rotation.x < -45) ? -45 : rotation.x;

        // this.transform.SetPositionAndRotation(this.transform.position, Quaternion.Euler(0, this.rotation.y, 0));
        // this.cam.transform.SetPositionAndRotation(this.transform.position + (Vector3.up * 0.5f), Quaternion.Euler(this.rotation));
    }

    void FixedUpdate() {

        Vector3 move = this.transform.rotation * this.movement * this.speed * Time.deltaTime;

        // Debug.DrawLine(this.rb.position + Vector3.up, this.rb.position + Vector3.up + new Vector3(move.x, 0, 0) * Mathf.Abs(move.x) * 8f);
        // Debug.DrawLine(this.rb.position + Vector3.up, this.rb.position + Vector3.up + new Vector3(0, 0, move.z) * Mathf.Abs(move.z) * 8f);

        RaycastHit h;

        if (Physics.Raycast(this.rb.position + Vector3.up, new Vector3(move.x, 0f, 0f), out h, Mathf.Abs(move.x) * 8f, ~(1 << LayerMask.NameToLayer("Cards")))) {
            if (Vector3.Magnitude(h.point - (this.rb.position + Vector3.up)) < 0.5f) {
                move.x = 0f;
            }
        }
        if (Physics.Raycast(this.rb.position + Vector3.up, new Vector3(0f, 0f, move.z), out h, Mathf.Abs(move.z) * 8f, ~(1 << LayerMask.NameToLayer("Cards")))) {
            if (Vector3.Magnitude(h.point - (this.rb.position + Vector3.up)) < 0.5f) {
                move.z = 0f;
            }
        }

        this.rb.position += move;
        this.rb.rotation = Quaternion.Euler(0, this.rotation.y, 0);

        this.cam.transform.position = this.rb.position + this.offset;
        this.cam.transform.rotation = Quaternion.Euler(this.rotation);

        if (Mathf.Abs(Vector3.Distance(this.rb.position, new Vector3(90, 0, 0))) < 5f) {
            StartCoroutine(End());
        }
    }

    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.CompareTag("Bullet") || other.gameObject.CompareTag("Enemy Bullet")) {
            this.health -= 10;
            if (this.health == 0) {
                // this.rb.position = new Vector3(0, 1.5f, 0);
                // this.health = 100;
                StartCoroutine(Restart());
            }
            this.pHealth.text = "Health: " + this.health;
            Destroy(other.transform.gameObject);
        } else if (other.gameObject.CompareTag("Deathplane")) {
            this.rb.position = new Vector3(0, 1.5f, 0);
            this.health = 100;
        } else if (other.gameObject.CompareTag("Card")) {
            if (other.gameObject.name == "Health Card") {
                this.health += 40;
                this.pHealth.text = "Health: " + this.health;
            } else if (other.gameObject.name == "Damage Card") {
                this.damage += 5;
                this.pDamage.text = "Damage: " + this.damage;
            }
            Destroy(other.gameObject);
        } else if (other.transform.CompareTag("Ground")) {
            this.onGround = true;
        }
    }

    // private void OnCollisionExit(Collision other) {
    //     if (other.transform.CompareTag("Ground")) {
    //         this.onGround = false;
    //     }
    // }

    private IEnumerator Shoot() {
        this.shooting = true;
        yield return new WaitForSeconds(0.1f);
        this.shooting = false;
        this.currAmmo -= 1;
        this.pAmmo.text = "Ammo: " + this.currAmmo;
        Bullet b = Instantiate(this.bullet, this.cam.transform.position + (this.cam.transform.rotation * Vector3.forward), Quaternion.Euler(90 + this.cam.transform.rotation.eulerAngles.x, 0 + this.cam.transform.rotation.eulerAngles.y, 0 + this.cam.transform.rotation.eulerAngles.z));
        b.Init(this.cam.transform.rotation.eulerAngles, 10f, this.damage);
    }

    public IEnumerator Reload() {
        this.reloading = true;
        this.pReloading.text = "Reloading ...";
        yield return new WaitForSeconds(1f);
        this.pReloading.text = "";
        this.currAmmo = this.maxAmmo;
        this.pAmmo.text = "Ammo: " + this.currAmmo;
        this.reloading = false;
    }

    public IEnumerator End() {
        this.pReloading.text = "Finished";
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(5f);
        Application.Quit();
    }

    public IEnumerator Restart() {
        this.pReloading.text = "Lose";
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(3f);
        SceneManager.LoadScene("SampleScene");
        Time.timeScale = 1f;
    }
}