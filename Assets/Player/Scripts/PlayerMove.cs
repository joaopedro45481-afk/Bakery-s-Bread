using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float margemHisterese = 0.05f;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Collider2D playerCollider;
    public int moedas = 0;

    private List<Collider2D> moveisProximos = new List<Collider2D>();

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerCollider = GetComponent<Collider2D>();
    }

    void Update()
    {
        rb.linearVelocity = moveInput * moveSpeed;
    }

    void LateUpdate()
    {
        AtualizarProfundidade();
    }

    public void Move(InputAction.CallbackContext context)
    {
        animator.SetBool("IsWalking", true);
        if (context.canceled)
        {
            animator.SetBool("IsWalking", false);
            animator.SetFloat("LasInputX", moveInput.x);
            animator.SetFloat("LasInputY", moveInput.y);
        }
        moveInput = context.ReadValue<Vector2>();
        animator.SetFloat("InputX", moveInput.x);
        animator.SetFloat("InputY", moveInput.y);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Movel") && !moveisProximos.Contains(other))
        {
            moveisProximos.Add(other);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (moveisProximos.Contains(other))
        {
            moveisProximos.Remove(other);
        }
    }

    void AtualizarProfundidade()
    {
        Collider2D maisProximoTrigger = null;
        Collider2D maisProximoFisico = null;
        float menorDistancia = float.MaxValue;
        Vector2 playerPos = playerCollider.bounds.center;
        float margemX = 0.3f;

        for (int i = moveisProximos.Count - 1; i >= 0; i--)
        {
            Collider2D movelTrigger = moveisProximos[i];

            if (movelTrigger == null)
            {
                moveisProximos.RemoveAt(i);
                continue;
            }

            Collider2D movelFisico = GetColliderFisico(movelTrigger.gameObject);
            if (movelFisico == null) continue;

            // Ignora móveis que o player não está horizontalmente alinhado
            if (playerPos.x < movelFisico.bounds.min.x - margemX ||
                playerPos.x > movelFisico.bounds.max.x + margemX)
            {
                continue;
            }

            float distancia = Mathf.Abs(playerPos.y - movelFisico.bounds.center.y);
            if (distancia < menorDistancia)
            {
                menorDistancia = distancia;
                maisProximoTrigger = movelTrigger;
                maisProximoFisico = movelFisico;
            }
        }

        if (maisProximoTrigger == null) return; // mantém o último valor, como combinamos

        SpriteRenderer movelRenderer = maisProximoTrigger.GetComponent<SpriteRenderer>();
        if (movelRenderer == null) return;

        float centroMovel = maisProximoFisico.bounds.center.y;
        bool estaAtras = spriteRenderer.sortingOrder < movelRenderer.sortingOrder;

        if (estaAtras)
        {
            // Só passa pra FRENTE se ultrapassar a margem de baixo
            if (playerPos.y < centroMovel - margemHisterese)
            {
                spriteRenderer.sortingOrder = movelRenderer.sortingOrder + 1;
            }
        }
        else
        {
            // Só passa pra TRÁS se ultrapassar a margem de cima
            if (playerPos.y > centroMovel + margemHisterese)
            {
                spriteRenderer.sortingOrder = movelRenderer.sortingOrder - 1;
            }
        }
    }

    Collider2D GetColliderFisico(GameObject obj)
    {
        foreach (Collider2D col in obj.GetComponents<Collider2D>())
        {
            if (!col.isTrigger) return col;
        }
        return null;
    }
}