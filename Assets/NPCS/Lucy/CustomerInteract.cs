using UnityEngine;

public class CustomerInteract : MonoBehaviour, IInteractable
{
    private CustomerOrder customerOrder;

    void Start()
    {
        customerOrder = GetComponent<CustomerOrder>();
    }


    public void Interact()
{
    if(customerOrder == null)
        return;


    if(customerOrder.waitingForItem)
    {
        InventoryController inventory = FindObjectOfType<InventoryController>();

        bool entregue = inventory.RemoveItem(
            customerOrder.pedido.ID
        );


       if(entregue)
{
    Debug.Log("Pedido entregue!");

    customerOrder.waitingForItem = false;


    PlayerMove player = FindObjectOfType<PlayerMove>();

    player.moedas += customerOrder.pedido.Price;


    Debug.Log(
        "Recebeu " + customerOrder.pedido.Price + " moedas!"
    );


    QueueManager.Instance.LeaveQueue(customerOrder.npc);

    customerOrder.npc.currentState = NPCController.NPCState.Leaving;
}
        else
        {
            Debug.Log("Você não tem esse item!");
        }
    }
}
}