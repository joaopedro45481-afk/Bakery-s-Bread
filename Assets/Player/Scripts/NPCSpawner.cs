using UnityEngine;
using System.Collections.Generic;

public class NPCSpawner : MonoBehaviour
{
    public static NPCSpawner Instance;

    public List<GameObject> npcPrefabs = new List<GameObject>();

    [Header("Tempo de Spawn")]
    public float tempoMinimo = 8f;
    public float tempoMaximo = 20f;

    private float tempoAtual;

    private List<GameObject> npcsAtivos = new List<GameObject>();

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        AgendarProximoSpawn();
    }

    void Update()
    {
        tempoAtual -= Time.deltaTime;

        if (tempoAtual <= 0f)
        {
            SpawnNPC();
            AgendarProximoSpawn();
        }
    }

    void AgendarProximoSpawn()
    {
        tempoAtual = Random.Range(tempoMinimo, tempoMaximo);
    }

    public void SpawnNPC()
    {
        GameObject prefab = EscolherNPC();

        if (prefab == null)
        {
            Debug.Log("Todos os NPCs disponíveis já estão na loja.");
            return;
        }

        GameObject npc = Instantiate(prefab, transform.position, Quaternion.identity);

        NPCController controller = npc.GetComponent<NPCController>();

        if (controller == null)
        {
            Debug.LogError(prefab.name + " não possui NPCController!");
            Destroy(npc);
            return;
        }

        npcsAtivos.Add(npc);

        Debug.Log("NPC spawnado: " + prefab.GetComponent<NPCData>().npcID);
    }

    GameObject EscolherNPC()
    {
        List<GameObject> disponiveis = new List<GameObject>();

        foreach (GameObject prefab in npcPrefabs)
        {
            NPCData prefabData = prefab.GetComponent<NPCData>();

            if (prefabData == null)
            {
                Debug.LogWarning(prefab.name + " não possui NPCData!");
                continue;
            }

            bool existe = false;

            foreach (GameObject npc in npcsAtivos)
            {
                if (npc == null)
                    continue;

                NPCData npcData = npc.GetComponent<NPCData>();

                if (npcData != null && npcData.npcID == prefabData.npcID)
                {
                    existe = true;
                    break;
                }
            }

            if (!existe)
            {
                disponiveis.Add(prefab);
            }
        }

        if (disponiveis.Count == 0)
            return null;

        return disponiveis[Random.Range(0, disponiveis.Count)];
    }

    public void RemoveNPC(GameObject npc)
    {
        npcsAtivos.Remove(npc);
    }
}