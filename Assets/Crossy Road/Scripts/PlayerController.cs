using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveDistance = 1;
    public float moveTime = 0.4f;
    public float colliderDistCheck = 1.1f;
    private int safeZoneCount = 0;

    public bool isIdle = true;
    public bool isDead = false;
    public bool isMoving = false;
    public bool isJumping = false;
    public bool jumpStart = false;
    public ParticleSystem particle = null;
    public GameObject chick = null;

    private Renderer renderer = null;
    private bool isVisible = false;

    private Coroutine moveCoroutine = null;  

    void Start()
    {
        renderer = chick.GetComponent<Renderer>();
    }

    void Update()
    {
        if (!Manager.instance.CanPlay()) return;
        if (isDead) return;

        CanIdle();
        CanMove();
        IsVisible();
    }

    void CanIdle()
    {
        if (isIdle)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow) ||
                Input.GetKeyDown(KeyCode.DownArrow) ||
                Input.GetKeyDown(KeyCode.LeftArrow) ||
                Input.GetKeyDown(KeyCode.RightArrow))
            {
                CheckIfCanMove();
            }
        }
    }

    void CheckIfCanMove()
    {
        Physics.Raycast(this.transform.position, -chick.transform.up, out RaycastHit hit, colliderDistCheck);
        Debug.DrawRay(this.transform.position, -chick.transform.up * colliderDistCheck, Color.red, 2);

        if (hit.collider == null || hit.collider.tag != "collider")
        {
            SetMove();
        }
    }

    void SetMove()
    {
        isIdle = false;
        isMoving = true;
        jumpStart = true;
    }

    void CanMove()
    {
        if (isMoving)
        {
            if (Input.GetKeyUp(KeyCode.UpArrow))
            {
                Moving(new Vector3(transform.position.x, transform.position.y, transform.position.z + moveDistance));
                SetMoveForwardState();
            }
            else if (Input.GetKeyUp(KeyCode.DownArrow))
            {
                Moving(new Vector3(transform.position.x, transform.position.y, transform.position.z - moveDistance));
            }
            else if (Input.GetKeyUp(KeyCode.LeftArrow))
            {
                Moving(new Vector3(transform.position.x - moveDistance, transform.position.y, transform.position.z));
            }
            else if (Input.GetKeyUp(KeyCode.RightArrow))
            {
                Moving(new Vector3(transform.position.x + moveDistance, transform.position.y, transform.position.z));
            }
        }
    }

    void Moving(Vector3 pos)
    {
        isIdle = false;
        isMoving = false;
        isJumping = true;
        jumpStart = false;


        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);

        moveCoroutine = StartCoroutine(MoveRoutine(pos));
    }

    IEnumerator MoveRoutine(Vector3 target)
    {
        Vector3 start = transform.position;
        float t = 0;

        while (t < moveTime)
        {
            t += Time.deltaTime;
            float lerp = t / moveTime;
            transform.position = Vector3.Lerp(start, target, lerp);
            yield return null;
        }

        transform.position = target;

        MoveComplete();
    }

    void MoveComplete()
    {
        isIdle = true;
        isMoving = false;
        isJumping = false;
        jumpStart = false;

        moveCoroutine = null;
    }

    void SetMoveForwardState()
    {
        Manager.instance.UpdateDistanceCount();
    }

    void IsVisible()
    {
        if (renderer.isVisible) isVisible = true;

        if (!renderer.isVisible && isVisible == true)
        {
            Debug.Log("Player offscreen");
            GetHit();
        }
    }

    public void GetHit()
    {

        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
            moveCoroutine = null;
        }

        isDead = true;

        Manager.instance.GameOver();

        var em = particle.emission;
        em.enabled = true;
    }
     void OnTriggerEnter(Collider other)
    {
        // Nếu chạm vào vùng AN TOÀN (bè, cầu, gỗ...)
        if (other.tag == "Safe") // ← Đổi "Safe" thành tag bạn muốn
        {
            safeZoneCount++;
            Debug.Log("Enter safe zone. Count: " + safeZoneCount);
        }

        // Nếu chạm vào SÔNG
        if (other.tag == "Water") // ← Đổi "Water" thành tag sông của bạn
        {
            // Chỉ chết nếu KHÔNG đứng trên vùng an toàn
            if (safeZoneCount == 0)
            {
                Debug.Log("Touched water without safe zone → DIE");
                GetHit();
            }
            else
            {
                Debug.Log("Touched water but standing on safe zone → SAFE");
            }
        }
    }



void OnTriggerExit(Collider other)
{
    if (other.tag == "Safe")
    {
        safeZoneCount--;
        
        // ❗ CHECK NGAY SAU KHI RỜI BÈ
        StartCoroutine(CheckWaterAfterLeavingSafe());
    }
}

IEnumerator CheckWaterAfterLeavingSafe()
{
    yield return new WaitForFixedUpdate(); // Đợi 1 physics frame
    
    // Nếu không còn đứng trên Safe và đang trong Water → CHẾT
    if (safeZoneCount == 0)
    {
        // Check xem có đang trigger với Water không
        Collider[] hits = Physics.OverlapSphere(transform.position, 0.5f);
        foreach (var hit in hits)
        {
            if (hit.tag == "Water")
            {
                Debug.Log("Left safe zone and still in water → DIE");
                GetHit();
                break;
            }
        }
    }
}
}
