using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    public GameObject cam;
    public Bullet bullet;

    public float walkSpeed = 4f, speed = 0f, currentTime = 0f, g1ReloadTime = 0.1f, g2ReloadTime = 5f, g1CurrReloadTime = 0f, g2CurrReloadTime = 0f;
    public int damage = 10, health = 0, maxAmmo = 0, currAmmo = 0;
    public bool reloading = false, shooting = false, blasting = false, showingEnemy = false;

    public Vector3 rotation = Vector3.zero, movement = Vector3.zero, offset = Vector3.zero;
    public Rigidbody rb;

    public TextMeshProUGUI eHealth, eIndicator, pHealth, pDamage, Enemies, pReloading, pAmmo, pTime, fps, g1Ready, g2Ready, pGoal;

    private RaycastHit hit;

    public LayerMask wallMask, bulletMask, cardMask, enemyMask;

    void Start()
    {
        // set layermasks
        this.wallMask = LayerMask.GetMask("Wall");
        this.bulletMask = LayerMask.GetMask("Bullet");
        this.cardMask = LayerMask.GetMask("Cards");
        this.enemyMask = LayerMask.GetMask("Enemy");

        // get rigidbody
        this.rb = this.GetComponent<Rigidbody>();

        // get initial rotation
        this.rotation = this.transform.rotation.eulerAngles;

        // 
        this.damage = 10;
        this.health = 100;
        this.maxAmmo = 10;
        this.currAmmo = this.maxAmmo;
        this.walkSpeed = 4f;
        this.speed = 0f;
        this.currentTime = 0f;
        this.g1ReloadTime = 0.1f;
        this.g2ReloadTime = 5f;
        this.g1CurrReloadTime = this.g1ReloadTime;
        this.g2CurrReloadTime = this.g2ReloadTime;

        // 
        this.reloading = false;
        this.shooting = false;
        this.blasting = false;
        this.showingEnemy = false;

        this.eHealth = GameObject.Find("Enemy Health").GetComponent<TextMeshProUGUI>();
        this.pHealth = GameObject.Find("Player Health").GetComponent<TextMeshProUGUI>();
        this.pDamage = GameObject.Find("Player Damage").GetComponent<TextMeshProUGUI>();
        this.pReloading = GameObject.Find("Player Reloading").GetComponent<TextMeshProUGUI>();
        this.pAmmo = GameObject.Find("Player Ammo").GetComponent<TextMeshProUGUI>();
        this.pTime = GameObject.Find("Player Time").GetComponent<TextMeshProUGUI>();
        this.fps = GameObject.Find("FPS").GetComponent<TextMeshProUGUI>();
        this.g1Ready = GameObject.Find("Gun 1 Ready").GetComponent<TextMeshProUGUI>();
        this.g2Ready = GameObject.Find("Gun 2 Ready").GetComponent<TextMeshProUGUI>();
        this.Enemies = GameObject.Find("Enemies Remaining").GetComponent<TextMeshProUGUI>();
        this.pGoal = GameObject.Find("Goal Distance").GetComponent<TextMeshProUGUI>();
        this.eIndicator = GameObject.Find("Enemy Indicator").GetComponent<TextMeshProUGUI>();



        this.eHealth.text = "";
        this.pHealth.text = "";
        this.pDamage.text = "";
        this.pReloading.text = "";
        this.pAmmo.text = "";
        this.pTime.text = "";
        this.fps.text = "";
        this.g1Ready.text = "";
        this.g2Ready.text = "";
        this.Enemies.text = "";
        this.pGoal.text = "";
        this.eIndicator.text = "";


        this.g1Ready.text = "Gun 1: Ready";
        this.g2Ready.text = "Gun 2: Ready";

        this.pReloading.text = "";
        this.pAmmo.text = "Ammo: " + this.currAmmo;

        this.pHealth.text = "Health: " + this.health;
        this.pDamage.text = "Damage: " + this.damage;

        Cursor.lockState = CursorLockMode.Locked;

        InvokeRepeating("UpdateFPS", 1f, 0.5f);
        InvokeRepeating("CheckEnemies", 1f, 5f);
    }

    // Update is called once per frame
    void Update()
    {
        this.currentTime += Time.deltaTime;
        this.pTime.text = Mathf.RoundToInt(this.currentTime) + " : Time";
        this.movement = Vector3.zero;


        if (Input.GetKey(KeyCode.W)) this.movement += Vector3.forward;
        if (Input.GetKey(KeyCode.S)) this.movement += Vector3.back;
        if (Input.GetKey(KeyCode.A)) this.movement += Vector3.left;
        if (Input.GetKey(KeyCode.D)) this.movement += Vector3.right;

        if (Input.GetKey(KeyCode.T) && !this.showingEnemy) StartCoroutine(showClosestEnemy());

        if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();


        if (Input.GetMouseButton(0) && !this.shooting && this.currAmmo > 0 && !this.reloading) StartCoroutine(Shoot());
        if (Input.GetMouseButton(1) && !this.blasting && this.currAmmo > 0 && !this.reloading) StartCoroutine(Blast());
        if ((this.currAmmo <= 0 || Input.GetKeyDown(KeyCode.R)) && !this.reloading) StartCoroutine(Reload());

        if (Physics.SphereCast(this.cam.transform.position, 0.1f, this.cam.transform.rotation * Vector3.forward, out this.hit, float.PositiveInfinity, ~this.bulletMask.value))
        {
            if (this.hit.transform.CompareTag("Enemy"))
            {
                this.eHealth.text = this.hit.transform.name + ": " + this.hit.transform.gameObject.GetComponent<Finder>().GetHealth();
            }
            else if (this.hit.transform.CompareTag("Card"))
            {
                this.eHealth.text = this.hit.transform.name;
            }
            else
            {
                this.eHealth.text = "";
            }
        }

        this.speed = (Input.GetKey(KeyCode.LeftShift)) ? this.walkSpeed * 2 : this.walkSpeed;

        rotation.y += Input.GetAxis("Mouse X");
        rotation.x += -Input.GetAxis("Mouse Y");

        rotation.x = (rotation.x > 45) ? 45 : rotation.x;
        rotation.x = (rotation.x < -45) ? -45 : rotation.x;

    }

    void FixedUpdate()
    {

        Vector3 move = this.transform.rotation * this.movement * this.speed * Time.deltaTime;

        RaycastHit h;

        if (Physics.Raycast(this.rb.position + Vector3.up, new Vector3(move.x, 0f, 0f), out h, Mathf.Abs(move.x) * 8f, ~(this.cardMask.value | this.enemyMask.value)))
        {
            if (Vector3.Magnitude(h.point - (this.rb.position + Vector3.up)) < 0.5f)
            {
                move.x = 0f;
            }
        }
        if (Physics.Raycast(this.rb.position + Vector3.up, new Vector3(0f, 0f, move.z), out h, Mathf.Abs(move.z) * 8f, ~(this.cardMask.value | this.enemyMask.value)))
        {
            if (Vector3.Magnitude(h.point - (this.rb.position + Vector3.up)) < 0.5f)
            {
                move.z = 0f;
            }
        }

        this.rb.position += move;
        this.rb.rotation = Quaternion.Euler(0, this.rotation.y, 0);

        this.cam.transform.position = this.rb.position + this.offset;
        this.cam.transform.rotation = Quaternion.Euler(this.rotation);

        if (Mathf.Abs(Vector3.Distance(this.rb.position, new Vector3(90, 0, 0))) < 2f && CheckEnemies() == 0)
        {
            StartCoroutine(End());
        }

        this.pGoal.text = Mathf.RoundToInt(Vector3.Distance(this.rb.position, new Vector3(90, 0, 0))) + " : Goal Distance";
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Bullet") || other.gameObject.CompareTag("Enemy Bullet"))
        {
            this.health -= 10;
            if (this.health == 0)
            {
                StartCoroutine(Restart());
            }
            this.pHealth.text = "Health: " + this.health;
            Destroy(other.transform.gameObject);
        }
        else if (other.gameObject.CompareTag("Deathplane"))
        {
            this.rb.position = new Vector3(0, 1.5f, 0);
            this.health = 100;
        }
        else if (other.gameObject.CompareTag("Card"))
        {
            if (other.gameObject.name == "Health Card")
            {
                this.health += 40;
                this.pHealth.text = "Health: " + this.health;
            }
            else if (other.gameObject.name == "Damage Card")
            {
                this.damage += 10;
                this.pDamage.text = "Damage: " + this.damage;
            }
            else if (other.gameObject.name == "Ammo Card")
            {
                this.maxAmmo += 5;
            }
            Destroy(other.gameObject);
        }
    }

    private IEnumerator Shoot()
    {
        this.shooting = true;
        InvokeRepeating("Gun1ShootTime", 0f, 0.1f);
        this.currAmmo -= 1;
        this.pAmmo.text = "Ammo: " + this.currAmmo;
        Bullet b = Instantiate(this.bullet, this.cam.transform.position + (this.cam.transform.rotation * Vector3.forward), Quaternion.Euler(90 + this.cam.transform.rotation.eulerAngles.x, 0 + this.cam.transform.rotation.eulerAngles.y, 0 + this.cam.transform.rotation.eulerAngles.z));
        b.Init(this.cam.transform.rotation.eulerAngles, 10f, this.damage);

        yield return new WaitForSeconds(0.1f);
        this.shooting = false;
        CancelInvoke("Gun1ShootTime");
        this.g1Ready.text = "Gun 1: Ready";
        this.g1CurrReloadTime = this.g1ReloadTime;
    }

    private IEnumerator Blast()
    {
        this.blasting = true;
        InvokeRepeating("Gun2ShootTime", 0f, 0.1f);
        int bullets = this.currAmmo;
        this.currAmmo = 0;
        this.pAmmo.text = "Ammo: " + this.currAmmo;

        Vector3 bOffset = Vector3.zero;
        float angle = 360 / bullets;

        for (int i = 0; i < bullets; i++)
        {
            bOffset = new Vector3(Mathf.Cos(angle * i * Mathf.Deg2Rad), Mathf.Sin(angle * i * Mathf.Deg2Rad), 0) * 0.5f;
            Bullet b = Instantiate(this.bullet, this.cam.transform.position + (this.cam.transform.rotation * Vector3.forward) + (this.cam.transform.rotation * bOffset), Quaternion.Euler(90 + this.cam.transform.rotation.eulerAngles.x, 0 + this.cam.transform.rotation.eulerAngles.y, 0 + this.cam.transform.rotation.eulerAngles.z));
            b.Init(this.cam.transform.rotation.eulerAngles, 10f, this.damage);
            yield return new WaitForSeconds(0.01f);
        }

        yield return new WaitForSeconds(5f);
        this.blasting = false;
        CancelInvoke("Gun2ShootTime");
        this.g2Ready.text = "Gun 2: Ready";
        this.g2CurrReloadTime = this.g2ReloadTime;
    }

    private void Gun1ShootTime()
    {
        this.g1Ready.text = "Gun 1: " + Mathf.Round(this.g1CurrReloadTime * 10) / 10f;
        this.g1CurrReloadTime -= 0.1f;
    }

    private void Gun2ShootTime()
    {
        this.g2Ready.text = "Gun 2: " + Mathf.Round(this.g2CurrReloadTime * 10) / 10f;
        this.g2CurrReloadTime -= 0.1f;
    }

    public IEnumerator Reload()
    {
        this.reloading = true;
        this.pReloading.text = "Reloading ...";
        yield return new WaitForSeconds(1f);
        this.pReloading.text = "";
        this.currAmmo = this.maxAmmo;
        this.pAmmo.text = "Ammo: " + this.currAmmo;
        this.reloading = false;
    }

    public IEnumerator End()
    {
        this.pReloading.text = "Finished";
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(5f);
        Application.Quit();
    }

    public IEnumerator Restart()
    {
        this.pReloading.text = "Lose";
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(3f);
        SceneManager.LoadScene("SampleScene");
        Time.timeScale = 1f;
    }

    private void UpdateFPS()
    {
        this.fps.text = Mathf.Round(Time.frameCount / Time.time) + "FPS";
    }

    private int CheckEnemies()
    {
        int numEnemies = 0;
        numEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length;
        foreach (Transform p in GameObject.Find("Enemies Container").transform)
        {
            foreach (Transform c in p)
            {
                if (!c.gameObject.activeSelf)
                {
                    numEnemies++;
                }
            }
        }
        this.Enemies.text = numEnemies + " : Enemies";
        return numEnemies;
    }

    private IEnumerator showClosestEnemy()
    {
        this.showingEnemy = true;
        Vector3 min = Vector3.positiveInfinity;

        foreach (GameObject g in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            if (Mathf.Abs(Vector3.Distance(g.transform.position, this.rb.position)) < Mathf.Abs(Vector3.Distance(min, this.rb.position)))
            {
                min = g.transform.position;
            }
        }

        foreach (Transform p in GameObject.Find("Enemies Container").transform)
        {
            foreach (Transform c in p)
            {
                if (!c.gameObject.activeSelf)
                {
                    if (Mathf.Abs(Vector3.Distance(c.position, this.rb.position)) < Mathf.Abs(Vector3.Distance(min, this.rb.position)))
                    {
                        min = c.position;
                    }
                }
            }
        }


        float dis = Mathf.Abs(Vector3.Distance(this.rb.position, min));

        if (dis < 1000f)
        {
            Vector3 vPoint = this.cam.GetComponent<Camera>().WorldToScreenPoint(min);
            this.eIndicator.text = "Enemy here";
            float y = (dis < 20) ? 100f : 0f;
            this.eIndicator.transform.position = vPoint + new Vector3(0f, y, 0f);
        }

        yield return new WaitForSeconds(1f);

        this.eIndicator.text = "";
        this.showingEnemy = false;
    }
}