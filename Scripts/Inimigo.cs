using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Inimigo : MonoBehaviour
{
    public Transform pointA;            
    public Transform pointB;             
    public float speed = 2f;             
    public float knockbackForce = 1f;    
    public float knockbackRange = 1f;    

    private float fixedY;               
    private Vector2 posA;               
    private Vector2 posB;              
    private Vector2 targetPos;         

    private int hp = 3;                 

    void Start()
    {
       
        fixedY = transform.position.y;

        
        posA = pointA.position;
        posB = pointB.position;
        posA.y = fixedY;
        posB.y = fixedY;
        targetPos = posB; 
    }

    void Update()
    {
        
        float newX = Mathf.MoveTowards(transform.position.x, targetPos.x, speed * Time.deltaTime);
        transform.position = new Vector3(newX, fixedY, transform.position.z);

       
        if (Mathf.Abs(transform.position.x - targetPos.x) < 0.1f)
        {
            targetPos = (targetPos == posB) ? posA : posB;
            Flip();
        }

       
        if (Input.GetMouseButtonDown(0))
        {
            
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0f;

            
            Collider2D col = GetComponent<Collider2D>();
            bool clicked = col != null && col.OverlapPoint(mousePos);
            float distance = Vector2.Distance(mousePos, transform.position);

            if (clicked || distance < knockbackRange)
            {
                
                TakeDamage(1);

                
                if (mousePos.x > transform.position.x)
                {
                    Knockback(new Vector2(-1, 0));
                }
                else
                {
                    Knockback(new Vector2(1, 0));
                }
            }
        }
    }

    
    void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

   
    public void TakeDamage(int damage = 1)
    {
        hp -= damage;
        if (hp <= 0)
        {
            Die();
        }
    }

    
    void Die()
    {
        Destroy(gameObject);
    }

    
    void Knockback(Vector2 direction)
    {
        transform.position += new Vector3(direction.x * knockbackForce, 0, 0);
    }
}