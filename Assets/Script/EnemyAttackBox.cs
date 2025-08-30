using UnityEngine;

public class EnemyAttackBox : MonoBehaviour
{
    public float damage = 5f;
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
        if (other.CompareTag("Player"))
        {
            PlayerScript player = other.GetComponent<PlayerScript>();
            player.GetHit(damage);
        }
    }
}
