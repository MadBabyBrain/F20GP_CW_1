using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Vector3 direction;
    public int damage;
    private float speed;
    public void Init(Vector3 dir, float speed, int damage) {
        this.direction = dir;
        this.speed = speed;
        this.damage = damage;
    }

    private void Update() {
        this.transform.SetPositionAndRotation(this.transform.position + Quaternion.Euler(this.direction) * Vector3.forward * speed * Time.deltaTime, this.transform.rotation);
    }

    private void OnCollisionEnter(Collision other) {
        if (other.transform.CompareTag("Enemy Bullet") && this.transform.CompareTag("Bullet")) {
            Destroy(this.gameObject);
            Destroy(other.gameObject);
        } else if (other.transform.CompareTag("Wall") || other.transform.CompareTag("Ground") || other.transform.CompareTag("Enemy") || other.transform.CompareTag("Enemy Bullet")) {
            Destroy(this.gameObject);
        }
    }
}
