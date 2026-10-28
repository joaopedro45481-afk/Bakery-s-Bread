using UnityEngine;

public class CustomerOrder : MonoBehaviour
{
    public Item pedido;
    public NPCController npc;
    public bool waitingForItem = false;
    public string[] frasesPedido;

    private bool orderCreated = false;

    public void TryGenerateOrder()
{
    if (orderCreated)
        return;

    CounterZone counter = FindObjectOfType<CounterZone>();

    Debug.Log($"[TRY ORDER] counter = {(counter != null ? "achou" : "NULL")}");

    if (counter != null && counter.playerOnCounter)
    {
        Debug.Log("[TRY ORDER] player está no balcão, gerando pedido...");
        GenerateOrder();
        orderCreated = true;
    }
    else
    {
        Debug.Log($"[TRY ORDER] esperando player. playerOnCounter = {(counter != null ? counter.playerOnCounter.ToString() : "N/A")}");
    }
}

    public void GenerateOrder()
    {
        ItemDictionary dictionary = FindObjectOfType<ItemDictionary>();
        int random = Random.Range(0, dictionary.ItemPrefabs.Count);
        pedido = dictionary.ItemPrefabs[random];

        string fala;
        if (frasesPedido != null && frasesPedido.Length > 0)
            fala = frasesPedido[Random.Range(0, frasesPedido.Length)];
        else
            fala = "Olá! Eu gostaria de um {item}.";

        fala = fala.Replace("{item}", pedido.Name);

        FindObjectOfType<DialogueManager>().ShowDialogue(fala, this);

        Debug.Log("Cliente pediu: " + pedido.Name);
    }

    public void ConfirmOrder()
    {
        if (QueueManager.Instance != null)
            QueueManager.Instance.LeaveQueue(npc);

        npc.currentState = NPCController.NPCState.Leaving;
        waitingForItem = false;
    }
}