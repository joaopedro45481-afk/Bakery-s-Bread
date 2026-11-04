using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDelivery : MonoBehaviour
{
    [Header("Referências")]
    public InventoryController inventory;

    private NPCController npcProximo;

    void Update()
    {
        AtualizarNPCProximo();

        if (npcProximo == null)
            return;

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            TentarEntregar();
        }
    }

    void AtualizarNPCProximo()
    {
        npcProximo = null;
        NPCInteractionZone[] zonas = FindObjectsOfType<NPCInteractionZone>();

        foreach (var zona in zonas)
        {
            if (zona == null) continue;
            if (!zona.playerPerto) continue;
            if (zona.npc == null) continue;

            Debug.Log($"[DELIVERY] Zona perto de {zona.npc.name} estado={zona.npc.currentState}");

            if (zona.npc.currentState != NPCController.NPCState.WaitingForItem) continue;

            npcProximo = zona.npc;
            break;
        }
    }

    void TentarEntregar()
    {
        if (npcProximo == null) return;

        CustomerOrder pedido = npcProximo.GetComponent<CustomerOrder>();
        if (pedido == null || pedido.pedido == null) return;

        int itemID = pedido.pedido.ID;

        if (!inventory.HasItem(itemID))
        {
            Debug.Log($"Você não tem o item pedido: {pedido.pedido.Name}");
            return;
        }

        inventory.RemoveItem(itemID);

        // ✅ Pagamento 1: moedas do player
        PlayerMove player = FindObjectOfType<PlayerMove>();
        if (player != null)
        {
            player.moedas += pedido.pedido.Price;
            Debug.Log($"Recebeu {pedido.pedido.Price} moedas! Total: {player.moedas}");
        }

        // ✅ Pagamento 2: lucro do dia
        if (DayCycleController.Instance != null)
        {
            DayCycleController.Instance.lucroHoje += pedido.pedido.Price;
            Debug.Log($"Lucro do dia: {DayCycleController.Instance.lucroHoje}");
        }

        // ✅ Manda o NPC pro fluxo (mesa ou Leaving)
        pedido.ReceiveItem();

        Debug.Log($"Entregou: {pedido.pedido.Name} ✅");
    }
}