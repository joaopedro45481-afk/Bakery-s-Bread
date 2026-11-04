using UnityEngine;

public class NPCInteractionZone : MonoBehaviour
{
    [HideInInspector]
    public NPCController npc;
    public bool playerPerto = false;

    void Awake()
    {
        // Pega o NPCController no pai
        npc = GetComponentInParent<NPCController>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerPerto = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerPerto = false;
    }
}