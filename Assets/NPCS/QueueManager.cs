using System.Collections.Generic;
using UnityEngine;

public class QueueManager : MonoBehaviour
{
    public static QueueManager Instance;

    public Transform[] queuePoints;

    private List<NPCController> queue = new();

    void Awake()
    {
        Instance = this;
    }

    public int EnterQueue(NPCController npc)
    {
        queue.Add(npc);
        UpdateQueuePositions();
        return queue.Count - 1;
    }

    public void LeaveQueue(NPCController npc)
    {
        queue.Remove(npc);
        UpdateQueuePositions();
    }

    public bool IsFirstInQueue(NPCController npc)
    {
        return queue.Count > 0 && queue[0] == npc;
    }

    void UpdateQueuePositions()
    {
        for (int i = 0; i < queue.Count; i++)
        {
            NPCController npc = queue[i];

            // Define o alvo físico (slot)
            if (i < queuePoints.Length && queuePoints[i] != null)
                npc.SetQueueTarget(queuePoints[i]);

            // Só o primeiro vira "WaitingForPlayer".
            // Os outros ficam em "WaitingAtCounter" se movendo pro slot.
            if (i == 0)
                npc.SetAsQueueFirst();
            else
                npc.SetAsQueueWaiting();
        }
    }
}