using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveDistance = 1;
    public float moveTime = 0.4f;
    public float colliderDistCheck = 1.1f;

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
}
