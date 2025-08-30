using UnityEngine;

public class BackroundMap : MonoBehaviour
{
    public GameObject mainCamera;
    private float mapWidth;
    public int mapNum;

    private float totalWidth;

    void Start()
    {
        mapWidth = GetComponent<SpriteRenderer>().bounds.size.x;
        totalWidth = mapWidth * mapNum;
    }

    void Update()
    {
        Vector3 tempPosition = transform.position;
        if(mainCamera.transform.position.x > transform.position.x + totalWidth/2)
        {
            tempPosition.x = tempPosition.x + totalWidth;
            transform.position = tempPosition;
        }
        else if(mainCamera.transform.position.x < transform.position.x - totalWidth / 2)
        {
            tempPosition.x = tempPosition.x - totalWidth;
            transform.position = tempPosition;
        }
    }
}
