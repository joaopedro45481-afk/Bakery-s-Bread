using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 2f;
    public float waypointThreshold = 0.05f;

    [Header("Animation")]
    public bool useDirectionalAnimations = true;

    [Header("Grafo de Waypoints")]
    public Waypoint waypointInicial; // se null, usa o isEntrada
    public Waypoint waypointBalcao;  // se null, usa o isBalcao
    public Waypoint waypointSaida;   // se null, usa o isSaida

    private Transform queueTarget;

    private CustomerOrder customerOrder;
    private Animator animator;

    private bool isWalking;

    // Trava de segurança pra não reentrar no EnterQueue
    private bool enteredQueue = false;

    // Navegação pelo grafo
    private List<Waypoint> rotaAtual = new List<Waypoint>();
    private int indiceRota = 0;

    private Vector2 lastDirection = Vector2.down;

    public NPCState currentState = NPCState.WalkingToCounter;

    private static readonly int IsWalkingHash =
        Animator.StringToHash("IsWalking");
    private static readonly int MoveXHash =
        Animator.StringToHash("MoveX");
    private static readonly int MoveYHash =
        Animator.StringToHash("MoveY");

    void Start()
    {
        customerOrder = GetComponent<CustomerOrder>();
        animator = GetComponentInChildren<Animator>();

        // Resolve waypoints se não foram setados no Inspector
        if (waypointInicial == null)
            waypointInicial = NPCPathfinder.EncontrarWaypoint(entrada: true);
        if (waypointBalcao == null)
            waypointBalcao = NPCPathfinder.EncontrarWaypoint(balcao: true);
        if (waypointSaida == null)
            waypointSaida = NPCPathfinder.EncontrarWaypoint(saida: true);

        // Posiciona o NPC no waypoint de entrada
        if (waypointInicial != null)
            transform.position = waypointInicial.transform.position;

        // Calcula rota até o balcão
        Debug.Log($"[NPC] entrada={(waypointInicial != null ? waypointInicial.name : "NULL")} " +
          $"balcao={(waypointBalcao != null ? waypointBalcao.name : "NULL")} " +
          $"saida={(waypointSaida != null ? waypointSaida.name : "NULL")}");
        IniciarRota(waypointInicial, waypointBalcao, NPCState.WalkingToCounter);

        UpdateDirection(Vector2.zero);
        UpdateAnimation(false);
    }

    void Update()
    {
        switch (currentState)
        {
            case NPCState.WalkingToCounter:
                SeguirRota(WalkingToCounter_Chegou);
                break;

            case NPCState.WaitingAtCounter:
                if (queueTarget != null)
                    MoveTowardsTarget(queueTarget.position);
                else
                    UpdateAnimation(false);
                break;

            case NPCState.WaitingForPlayer:
                UpdateAnimation(false);
                if (customerOrder != null)
                    customerOrder.TryGenerateOrder();
                break;

            case NPCState.WaitingForItem:
                UpdateAnimation(false);
                break;

            case NPCState.WalkingToTable:
                if (mesaAtual == null) { UpdateAnimation(false); break; }
                Transform assento = mesaAtual.GetAssentoLivre();
                if (assento == null) { UpdateAnimation(false); break; }
                MoveTowardsTarget(assento.position);
                if (Vector2.Distance(transform.position, assento.position) < waypointThreshold)
                {
                    currentState = NPCState.Eating;
                    UpdateAnimation(false);
                }
                break;

            case NPCState.Eating:
                UpdateAnimation(false);
                tempoComendo += Time.deltaTime;
                if (tempoComendo >= tempoDeComer)
                {
                    tempoComendo = 0f;
                    if (mesaAtual != null)
                    {
                        mesaAtual.Liberar();
                        mesaAtual = null;
                    }
                    IniciarRota(NPCPathfinder.EncontrarMaisProximo(transform.position), waypointSaida, NPCState.Leaving);
                }
                break;

            case NPCState.Leaving:
                SeguirRota(SaindoDaCena);
                break;
        }
    }

    // ============ Navegação por grafo ============

    void IniciarRota(Waypoint origem, Waypoint destino, NPCState estadoFinal)
    {
        rotaAtual = NPCPathfinder.CalcularRota(origem, destino);
        indiceRota = 0;
        currentState = estadoFinal;

        if (rotaAtual == null || rotaAtual.Count == 0)
        {
            // Já está no destino ou não tem rota
            if (estadoFinal == NPCState.WalkingToCounter)
                WalkingToCounter_Chegou();
            else if (estadoFinal == NPCState.Leaving)
                SaindoDaCena();
        }
    }

    void SeguirRota(System.Action aoChegarNoDestino)
    {
        if (rotaAtual == null || indiceRota >= rotaAtual.Count)
        {
            UpdateAnimation(false);
            aoChegarNoDestino?.Invoke();
            return;
        }

        Waypoint alvo = rotaAtual[indiceRota];
        MoveTowardsTarget(alvo.transform.position);

        if (Vector2.Distance(transform.position, alvo.transform.position) < waypointThreshold)
        {
            indiceRota++;
        }
    }

    void WalkingToCounter_Chegou()
    {
        if (enteredQueue) return;
        enteredQueue = true;
        UpdateAnimation(false);

        if (QueueManager.Instance == null)
        {
            Debug.LogError("QueueManager.Instance é NULL!");
            return;
        }

        currentState = NPCState.WaitingAtCounter;
        QueueManager.Instance.EnterQueue(this);
        Debug.Log("Cliente chegou ao balcão!");
    }

    void SaindoDaCena()
    {
        UpdateAnimation(false);

        if (mesaAtual != null)
        {
            mesaAtual.Liberar();
            mesaAtual = null;
        }

        if (NPCSpawner.Instance != null)
            NPCSpawner.Instance.RemoveNPC(gameObject);

        Destroy(gameObject);
    }

    // ============ Movimento ============

    void MoveTowardsTarget(Vector3 targetPosition)
    {
        Vector2 difference = (Vector2)targetPosition - (Vector2)transform.position;

        if (difference.magnitude <= waypointThreshold)
        {
            UpdateAnimation(false);
            return;
        }

        Vector2 direction;
        if (Mathf.Abs(difference.x) > Mathf.Abs(difference.y))
            direction = difference.x > 0f ? Vector2.right : Vector2.left;
        else
            direction = difference.y > 0f ? Vector2.up : Vector2.down;

        UpdateDirection(direction);
        UpdateAnimation(true);

        transform.position = Vector2.MoveTowards(
            transform.position,
            targetPosition,
            speed * Time.deltaTime
        );
    }

    void UpdateDirection(Vector2 direction)
    {
        if (direction == Vector2.zero) return;
        lastDirection = direction;
        if (animator == null || !useDirectionalAnimations) return;
        animator.SetFloat(MoveXHash, lastDirection.x);
        animator.SetFloat(MoveYHash, lastDirection.y);
    }

    void UpdateAnimation(bool walking)
    {
        if (animator == null) return;
        if (isWalking == walking) return;
        isWalking = walking;
        animator.SetBool(IsWalkingHash, walking);
    }

    // ============ API chamada pelo QueueManager ============

    public void SetQueueTarget(Transform target) => queueTarget = target;

    public void SetAsQueueFirst()
    {
        if (currentState == NPCState.WaitingAtCounter || currentState == NPCState.WaitingForPlayer)
            currentState = NPCState.WaitingForPlayer;
    }

    public void SetAsQueueWaiting()
    {
        if (currentState == NPCState.WaitingAtCounter || currentState == NPCState.WaitingForPlayer)
            currentState = NPCState.WaitingAtCounter;
    }

    // ============ Mesa ============

    private MesaController mesaAtual;
    private float tempoComendo = 0f;
    public float tempoDeComer = 10f;

    public void IrParaMesa(MesaController mesa)
    {
        mesaAtual = mesa;
        currentState = NPCState.WalkingToTable;
    }

    public enum NPCState
    {
        WalkingToCounter,
        WaitingForPlayer,
        WaitingAtCounter,
        WaitingForItem,
        WalkingToTable,
        Eating,
        Leaving
    }
}