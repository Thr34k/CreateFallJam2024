using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehavior : MonoBehaviour
{
    [Header("Behavior variables")]
    public float projectileSpeed = 2.5f;
    public int projectileDamage = 1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(transform.right * projectileSpeed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("GhoulUnit")) 
        { 
            Debug.Log($"{gameObject.name}: Hit a ghoul");
            collision.gameObject.GetComponent<Ghoul>().TakeDamage(projectileDamage);
            Destroy(gameObject);
        }
    }
}
