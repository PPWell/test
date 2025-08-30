using UnityEngine;

public class PlayerAttackBox : MonoBehaviour
{
    public float damage = 10f;
    public float destroyTime = 0.5f;


    void Start()
    {
        Destroy(gameObject, destroyTime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Enemy"))
        {
            EnemyBase enemy = other.GetComponent<EnemyBase>();
            enemy.GetHit(damage);
        }   
    }
}
