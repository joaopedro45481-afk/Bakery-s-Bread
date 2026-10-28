using UnityEngine;

/// <summary>
/// Painel que abre quando o jogador interage com a mesa. É quem CRIA o
/// CookingTaskController (ele nasce aqui, não existe antes disso) e quem
/// finalmente entrega o item pro inventário quando a task termina.
///
/// Ajusta a assinatura de AdicionarItemAoInventario pro que o teu
/// Inventory já expõe (ex: Inventory.Instance.AddItem(itemResultante)).
///
/// Onde encaixa:
/// - InteractableTable chama AbrirTela(receita) quando o jogador interage.
/// - Cria o DoughInteraction ativo passando o controller (Iniciar()).
/// - Assina controller.OnTaskConcluida pra chamar o inventário e fechar a tela.
/// - Assina controller.OnEstadoMudou se quiser trocar textos/telas conforme o progresso.
/// </summary>
public class CookingUI : MonoBehaviour
{
    [SerializeField] private GameObject painelCozinha;
    [SerializeField] private DoughInteraction doughInteraction;
    [SerializeField] private Stove fogaoDaCena; // referência ao Stove que essa mesa usa
    [SerializeField] private InventoryController inventoryController;
    [SerializeField] private ItemDictionary itemDictionary;

    private CookingTaskController controllerAtivo;

    public void AbrirTela(RecipeSO receita)
    {
        painelCozinha.SetActive(true);
        doughInteraction.gameObject.SetActive(true);

        controllerAtivo = new CookingTaskController(receita);
        controllerAtivo.OnTaskConcluida += OnTaskConcluida;
        controllerAtivo.OnEstadoMudou += OnEstadoMudou;

        doughInteraction.Iniciar(controllerAtivo, receita);
    }

    private void OnEstadoMudou(CookingTaskController.EstadoTask novoEstado)
    {
        // aqui é o lugar pra trocar texto/instrução na tela conforme a etapa
        // ex: "Abra a massa", "Sove a massa", "Leve ao fogão", "Cozinhando..."

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

    /// <summary>
    /// Massa terminou de ser moldada: fecha a tela e guarda a task no
    /// fogão, sem tocar em controllerAtivo (a task continua viva —
    /// ainda vamos precisar do evento OnTaskConcluida dela mais tarde,
    /// quando o jogador for lá cozinhar de verdade).
    /// </summary>
    private void EnviarParaFogaoEFecharTela()
    {
        fogaoDaCena.GuardarMassaPendente(controllerAtivo, controllerAtivo.Receita);
        painelCozinha.SetActive(false);
        doughInteraction.gameObject.SetActive(false);
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
    }
}