using UnityEngine;

public class NPCController : MonoBehaviour
{
    [Header("Waypoints")]
    public Transform[] entranceWaypoints;
    public Transform[] exitWaypoints;

    [Header("Movement")]
    public float speed = 2f;
    public float waypointThreshold = 0.05f;

    [Header("Animation")]
    public bool useDirectionalAnimations = true;

    private int currentWaypoint = 0;
    private int exitIndex = 0;

    private Transform queueTarget;

    private CustomerOrder customerOrder;
    private Animator animator;

    private bool isWalking;

    // Trava de segurança pra não reentrar no EnterQueue
    private bool enteredQueue = false;

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

        UpdateDirection(Vector2.zero);
        UpdateAnimation(false);
    }

    void Update()
    {
        switch (currentState)
        {
            case NPCState.WalkingToCounter:
                WalkToCounter();
                break;

            case NPCState.WaitingAtCounter:
                // Anda até o slot da fila. Se não tem alvo, fica parado.
                if (queueTarget != null)
                    MoveTowardsTarget(queueTarget.position);
                else
                    UpdateAnimation(false);
                break;

            case NPCState.WaitingForPlayer:
                // Primeiro da fila: parado, esperando atendimento.
                UpdateAnimation(false);

                if (customerOrder != null)
                    customerOrder.TryGenerateOrder();
                break;

            case NPCState.WaitingForItem:
                UpdateAnimation(false);
                break;

            case NPCState.WalkingToTable:
                UpdateAnimation(false);
                break;

            case NPCState.Eating:
                UpdateAnimation(false);
                break;

            case NPCState.Leaving:
                LeaveShop();
                break;
        }
    }

    void WalkToCounter()
    {
        // Trava de segurança: se já entrou na fila, não faz nada
        if (enteredQueue)
            return;

        if (currentWaypoint >= entranceWaypoints.Length)
        {
            enteredQueue = true;
            UpdateAnimation(false);

            if (QueueManager.Instance == null)
            {
                Debug.LogError("QueueManager.Instance é NULL!");
                return;
            }

            // Muda o estado ANTES do EnterQueue, pra o QueueManager
            // conseguir atuar via SetAsQueueFirst / SetAsQueueWaiting
            currentState = NPCState.WaitingAtCounter;

            QueueManager.Instance.EnterQueue(this);

            Debug.Log("Cliente chegou ao balcão!");
            return;
        }

        Transform target = entranceWaypoints[currentWaypoint];

        if (target == null)
        {
            UpdateAnimation(false);
            currentWaypoint++;
            return;
        }

        MoveTowardsTarget(target.position);

        if (Vector2.Distance(transform.position, target.position)
            < waypointThreshold)
        {
            currentWaypoint++;
        }
    }

    void LeaveShop()
    {
        if (exitIndex >= exitWaypoints.Length)
        {
            UpdateAnimation(false);

            if (NPCSpawner.Instance != null)
                NPCSpawner.Instance.RemoveNPC(gameObject);

            Destroy(gameObject);
            return;
        }

        Transform target = exitWaypoints[exitIndex];

        if (target == null)
        {
            UpdateAnimation(false);
            exitIndex++;
            return;
        }

        MoveTowardsTarget(target.position);

        if (Vector2.Distance(transform.position, target.position)
            < waypointThreshold)
        {
            exitIndex++;
        }
    }

    void MoveTowardsTarget(Vector3 targetPosition)
    {
        Vector2 difference = (Vector2)targetPosition
                           - (Vector2)transform.position;

        if (difference.magnitude <= waypointThreshold)
        {
            UpdateAnimation(false);
            return;
        }

        Vector2 direction;

        if (Mathf.Abs(difference.x) > Mathf.Abs(difference.y))
        {
            direction = difference.x > 0f ? Vector2.right : Vector2.left;
        }
        else
        {
            direction = difference.y > 0f ? Vector2.up : Vector2.down;
        }

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

    public void SetQueueTarget(Transform target)
    {
        queueTarget = target;
    }

    /// <summary>Sou o primeiro da fila → espero o player.</summary>
    public void SetAsQueueFirst()
    {
        if (currentState == NPCState.WaitingAtCounter
            || currentState == NPCState.WaitingForPlayer)
        {
            currentState = NPCState.WaitingForPlayer;
        }
    }

    /// <summary>Não sou o primeiro → ando até meu slot.</summary>
    public void SetAsQueueWaiting()
    {
        if (currentState == NPCState.WaitingAtCounter
            || currentState == NPCState.WaitingForPlayer)
        {
            currentState = NPCState.WaitingAtCounter;
        }
    }

    // =======================================================

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