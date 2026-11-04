using UnityEngine;
using Unity.Cinemachine; // <-- Namespace correto para Unity 6

public class CookingUI : MonoBehaviour
{
    [SerializeField] private GameObject painelCozinha;
    [SerializeField] private DoughInteraction doughInteraction;
    [SerializeField] private Stove fogaoDaCena;
    [SerializeField] private InventoryController inventoryController;
    [SerializeField] private ItemDictionary itemDictionary;

    private CookingTaskController controllerAtivo;

    public void AbrirTela(RecipeSO receita)
    {
        painelCozinha.SetActive(true);
        doughInteraction.gameObject.SetActive(true);

        SoltarCamera(); // <-- Chama a nova lógica

        controllerAtivo = new CookingTaskController(receita);
        controllerAtivo.OnTaskConcluida += OnTaskConcluida;
        controllerAtivo.OnEstadoMudou += OnEstadoMudou;

        doughInteraction.Iniciar(controllerAtivo, receita);
    }

    private void OnEstadoMudou(CookingTaskController.EstadoTask novoEstado)
    {
        if (novoEstado == CookingTaskController.EstadoTask.LevandoAoFogao)
        {
            EnviarParaFogaoEFecharTela();
        }
    }

    private void OnTaskConcluida(RecipeSO receita)
    {
        GameObject prefabDoItem = itemDictionary.GetItemPrefab(receita.itemIDResultante);

        if (prefabDoItem != null)
        {
            bool coube = inventoryController.addItem(prefabDoItem);
            if (!coube) Debug.Log("Inventário cheio, pão não coube.");
        }
        else
        {
            Debug.LogError($"Prefab do item {receita.itemIDResultante} não encontrado no ItemDictionary.");
        }

        FecharTela();
    }

    private void EnviarParaFogaoEFecharTela()
    {
        fogaoDaCena.GuardarMassaPendente(controllerAtivo, controllerAtivo.Receita);
        painelCozinha.SetActive(false);
        doughInteraction.gameObject.SetActive(false);
        RetomarCamera(); // <-- Chama a nova lógica
    }

    private void FecharTela()
    {
        if (controllerAtivo != null)
        {
            controllerAtivo.OnTaskConcluida -= OnTaskConcluida;
            controllerAtivo.OnEstadoMudou -= OnEstadoMudou;
            controllerAtivo = null;
        }
        painelCozinha.SetActive(false);
        doughInteraction.gameObject.SetActive(false);
        RetomarCamera(); // <-- Chama a nova lógica
    }

    // ==========================================
    // Lógica de Câmera e Movimento (Atualizada)
    // ==========================================

    private void SoltarCamera()
    {
        // 1. Solta a câmera do Cinemachine (Nome novo: CinemachineCamera)
        CinemachineCamera vcam = FindObjectOfType<CinemachineCamera>();
        if (vcam != null)
        {
            vcam.Follow = null; 
        }

        // 2. Impede o player de andar
        PlayerMove player = FindObjectOfType<PlayerMove>();
        if (player != null)
        {
            player.podeAndar = false; 
            
            // 3. Desativa o colisor do player para ele não bloquear os cliques na massa
            Collider2D colisorPlayer = player.GetComponent<Collider2D>();
            if (colisorPlayer != null) colisorPlayer.enabled = false;
        }
    }

    private void RetomarCamera()
    {
        // 1. Devolve a câmera pro player
        CinemachineCamera vcam = FindObjectOfType<CinemachineCamera>();
        PlayerMove player = FindObjectOfType<PlayerMove>();

        if (vcam != null && player != null)
        {
            vcam.Follow = player.transform;
        }

        // 2. Libera o movimento e o colisor do player
        if (player != null)
        {
            player.podeAndar = true;
            
            Collider2D colisorPlayer = player.GetComponent<Collider2D>();
            if (colisorPlayer != null) colisorPlayer.enabled = true;
        }
    }
}