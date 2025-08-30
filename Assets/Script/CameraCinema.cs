using UnityEngine;

public class CameraCinema : MonoBehaviour
{

    public Transform backGround;
    private Vector3 lastPos;
    public Vector2 offectSpeed;
    void Start()
    {
        lastPos = transform.position;
    }

    void Update()
    {
        Vector3 amountToMove = transform.position - lastPos;

        Vector3 offect = new Vector3(amountToMove.x * offectSpeed.x, amountToMove.y * offectSpeed.x, 0);

        backGround.position  = backGround.position + offect;

        lastPos = transform.position;
    }
}
