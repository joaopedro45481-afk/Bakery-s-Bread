using System.Collections.Generic;
using UnityEngine;

public static class NPCPathfinder
{
    /// <summary>
    /// Calcula uma rota aleatória de 'origem' até 'destino' usando BFS.
    /// Retorna a lista de waypoints (sem incluir a origem).
    /// </summary>
    public static List<Waypoint> CalcularRota(Waypoint origem, Waypoint destino)
    {
        if (origem == null || destino == null)
            return null;

        if (origem == destino)
            return new List<Waypoint>();

        // BFS: guarda o "pai" de cada waypoint visitado
        Queue<Waypoint> fila = new Queue<Waypoint>();
        Dictionary<Waypoint, Waypoint> pais = new Dictionary<Waypoint, Waypoint>();

        fila.Enqueue(origem);
        pais[origem] = null;

        bool achou = false;

        while (fila.Count > 0)
        {
            Waypoint atual = fila.Dequeue();

            if (atual == destino)
            {
                achou = true;
                break;
            }

            if (atual.vizinhos == null) continue;

            // Embaralha os vizinhos pra gerar rotas diferentes a cada NPC
            List<Waypoint> vizinhosEmbaralhados = new List<Waypoint>(atual.vizinhos);
            for (int i = 0; i < vizinhosEmbaralhados.Count; i++)
            {
                int j = Random.Range(i, vizinhosEmbaralhados.Count);
                (vizinhosEmbaralhados[i], vizinhosEmbaralhados[j]) =
                    (vizinhosEmbaralhados[j], vizinhosEmbaralhados[i]);
            }

            foreach (var vizinho in vizinhosEmbaralhados)
            {
                if (vizinho == null) continue;
                if (pais.ContainsKey(vizinho)) continue;

                pais[vizinho] = atual;
                fila.Enqueue(vizinho);
            }
        }

        if (!achou)
        {
            Debug.LogWarning($"NPCPathfinder: sem caminho de {origem.name} até {destino.name}");
            return null;
        }

        // Reconstrói a rota de trás pra frente
        List<Waypoint> rota = new List<Waypoint>();
        Waypoint passo = destino;

        while (passo != null && passo != origem)
        {
            rota.Add(passo);
            passo = pais[passo];
        }

        rota.Reverse();
        return rota;
    }

    /// <summary>Encontra o primeiro waypoint com a flag especificada.</summary>
    public static Waypoint EncontrarWaypoint(bool entrada = false, bool balcao = false, bool saida = false)
    {
        Waypoint[] todos = Object.FindObjectsOfType<Waypoint>();

        foreach (var wp in todos)
        {
            if (entrada && wp.isEntrada) return wp;
            if (balcao && wp.isBalcao) return wp;
            if (saida && wp.isSaida) return wp;
        }

        return null;
    }

    /// <summary>Encontra o waypoint mais próximo de uma posição.</summary>
    public static Waypoint EncontrarMaisProximo(Vector3 posicao)
    {
        Waypoint[] todos = Object.FindObjectsOfType<Waypoint>();
        Waypoint maisProximo = null;
        float menorDist = float.MaxValue;

        foreach (var wp in todos)
        {
            float dist = Vector2.Distance(posicao, wp.transform.position);
            if (dist < menorDist)
            {
                menorDist = dist;
                maisProximo = wp;
            }
        }

        return maisProximo;
    }
}