using UnityEngine;

public class MesaController : MonoBehaviour
{
    [Header("Assentos")]
    public Transform[] assentos;

    [Header("Estado")]
    public bool ocupada = false;

    private NPCController npcSentado;

    /// <summary>Retorna true se conseguiu reservar a mesa.</summary>
    public bool Reservar(NPCController npc)
    {
        Debug.Log($"[MESA] Tentando reservar {name}. ocupada={ocupada}");
        if (ocupada) return false;
        ocupada = true;
        npcSentado = npc;
        Debug.Log($"[MESA] {name} reservada para {npc.name}");
        return true;
    }

    /// <summary>Libera a mesa (NPC saiu).</summary>
    public void Liberar()
    {
        Debug.Log($"[MESA] {name} liberada (antes: ocupada={ocupada})");
        ocupada = false;
        npcSentado = null;
    }

    /// <summary>Retorna o primeiro assento livre (pra o NPC andar até ele).</summary>
    public Transform GetAssentoLivre()
    {
        if (assentos == null || assentos.Length == 0) return null;
        return assentos[0]; // simplificado: sempre o primeiro
    }

    public NPCController GetNPC()
    {
        return npcSentado;
    }
}