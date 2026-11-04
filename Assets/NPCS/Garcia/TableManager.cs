using System.Collections.Generic;
using UnityEngine;

public class TableManager : MonoBehaviour
{
    public static TableManager Instance;

    public MesaController[] mesas;

    void Awake()
    {
        Instance = this;
    }

    /// <summary>Tenta achar uma mesa livre e reservar pro NPC.</summary>
    public MesaController EncontrarMesaLivre(NPCController npc)
    {
        foreach (var mesa in mesas)
        {
            if (mesa == null) continue;

            Debug.Log($"[MESA] {mesa.name} ocupada={mesa.ocupada}");  // ← log aqui

            if (mesa.Reservar(npc))
                return mesa;
        }

        Debug.Log("[MESA] Nenhuma mesa livre!");  // ← e aqui
        return null;
    }
}