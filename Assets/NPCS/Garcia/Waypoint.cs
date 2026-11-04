using UnityEngine;

public class Waypoint : MonoBehaviour
{
    [Tooltip("Waypoints conectados a este. O NPC pode ir pra qualquer um deles.")]
    public Waypoint[] vizinhos;

    [Tooltip("Marque se este é o waypoint de entrada (spawn dos NPCs)")]
    public bool isEntrada = false;

    [Tooltip("Marque se este é o waypoint do balcão")]
    public bool isBalcao = false;

    [Tooltip("Marque se este é o waypoint de saída")]
    public bool isSaida = false;

    // Desenha linhas no Scene View pra visualizar as conexões
    void OnDrawGizmos()
    {
        if (vizinhos == null) return;

        Gizmos.color = Color.yellow;
        foreach (var v in vizinhos)
        {
            if (v == null) continue;
            Gizmos.DrawLine(transform.position, v.transform.position);
        }

        // Cor do waypoint por tipo
        if (isEntrada) Gizmos.color = Color.green;
        else if (isBalcao) Gizmos.color = Color.blue;
        else if (isSaida) Gizmos.color = Color.red;
        else Gizmos.color = Color.white;

        Gizmos.DrawSphere(transform.position, 0.2f);
    }
}