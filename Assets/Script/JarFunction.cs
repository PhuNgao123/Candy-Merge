using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class JarFunction : MonoBehaviour
{
    Rigidbody2D rb;

    [Header("Shake Settings")]
    public float shakeDistance = 0.05f;
    public float moveDuration = 0.05f;   
    public float pauseBetween = 0.1f;   
    public int shakesPerUse = 5;

    [Header("Gameplay")]
    public int shakeCount = 0;            

    private bool isShaking = false;
    private Vector2 startPos;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        startPos = rb.position;
        shakeCount = 0;
        FreezeJar(true);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            ShakeJar();
        }
    }
    public void ShakeJar()
    {
        if(isShaking) return;
        if(shakeCount <=0) return;

        shakeCount--;
        StartCoroutine(ShakeRoutine());

    }

    IEnumerator ShakeRoutine()
    {
        isShaking = true;

        startPos = rb.position;
        FreezeJar(false);

        List<Vector2> directions = new List<Vector2>()
        {
            Vector2.left,
            Vector2.right,
            Vector2.up,
            Vector2.down
        };

        
        for (int i = 0; i < directions.Count; i++)
        {
            int rand = Random.Range(i, directions.Count);
            (directions[i], directions[rand]) = (directions[rand], directions[i]);
        }

        int dirIndex = 0;

        for (int i = 0; i < shakesPerUse; i++)
        {
            Vector2 dir = directions[i % directions.Count];

            FreezeJar(false);

            rb.MovePosition(startPos + dir * shakeDistance);

            yield return new WaitForFixedUpdate();

            rb.MovePosition(startPos);
            FreezeJar(true);

            yield return new WaitForSeconds(pauseBetween);
        }

        rb.MovePosition(startPos);
        FreezeJar(true);
        isShaking = false;
    }

    void FreezeJar(bool freeze)
    {
        if (freeze)
        {
            rb.constraints = RigidbodyConstraints2D.FreezePositionX
                           | RigidbodyConstraints2D.FreezePositionY
                           | RigidbodyConstraints2D.FreezeRotation;
        }
        else
        {
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }

}
