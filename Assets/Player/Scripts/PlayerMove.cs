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

    // --- NOVA VARIÁVEL AQUI ---
    [HideInInspector] public bool podeAndar = true; 

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
        // Se não pode andar, zera a velocidade e para a animação
        if (!podeAndar)
        {
            rb.linearVelocity = Vector2.zero;
            animator.SetBool("IsWalking", false);
            return;
        }

        rb.linearVelocity = moveInput * moveSpeed;
    }

    void LateUpdate()
    {
        if (!podeAndar) return; // Não atualiza profundidade se estiver travado
        AtualizarProfundidade();
    }

    public void Move(InputAction.CallbackContext context)
    {
        if (!podeAndar) return; // Ignora comandos de teclado se estiver travado

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

        if (maisProximoTrigger == null) return;

        SpriteRenderer movelRenderer = maisProximoTrigger.GetComponent<SpriteRenderer>();
        if (movelRenderer == null) return;

        float centroMovel = maisProximoFisico.bounds.center.y;
        bool estaAtras = spriteRenderer.sortingOrder < movelRenderer.sortingOrder;

        if (estaAtras)
        {
            if (playerPos.y < centroMovel - margemHisterese)
            {
                spriteRenderer.sortingOrder = movelRenderer.sortingOrder + 1;
            }
        }
        else
        {
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