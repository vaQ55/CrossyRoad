using UnityEngine;

public class Confider : MonoBehaviour
{
    public Transform Base;
    public Transform Player;
    private CameraFollow cameraFollow;

    float maxOffsetY = 0;

    void Start()
    {
        cameraFollow = GetComponent<CameraFollow>();

        // Khoảng cách ban đầu theo Y
        maxOffsetY = Mathf.Abs(Player.transform.position.y - Base.position.y);
    }

    void Update()
    {
        float currentOffsetY = Mathf.Abs(Player.transform.position.y - Base.position.y);

        // Lệch X quá 6 → tắt camera
        if (Mathf.Abs(Player.transform.position.x - Base.position.x) > 6)
        {
            cameraFollow.enabled = false;
           
        }        
        if (Mathf.Abs(Player.transform.position.x - Base.position.x) < 6)
        {
            cameraFollow.enabled = true;
            Debug.LogWarning("Bat laij");            
        }

        if (currentOffsetY > maxOffsetY)
        {
            maxOffsetY = currentOffsetY;
            cameraFollow.enabled = true;
        }
    }
}
