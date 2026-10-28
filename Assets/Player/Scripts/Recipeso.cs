using UnityEngine;

/// <summary>
/// Dados de uma receita. Cria um asset por receita (botão direito > Create >
/// Padaria > Receita) em vez de duplicar lógica em código para cada pão.
///
/// Onde encaixa:
/// - CookingUI: tem um campo (ou lista) de RecipeSO pra saber o que o
///   jogador pode escolher na mesa.
/// - CookingTaskController: guarda a receita escolhida e, ao concluir,
///   é o ItemResultante que vai pro Inventory.AddItem().
/// </summary>
[CreateAssetMenu(fileName = "NovaReceita", menuName = "Padaria/Receita")]
public class RecipeSO : ScriptableObject
{
    [Header("Identificação")]
    public string nomeReceita;
    public Sprite icone;

    [Header("Cozimento")]
    [Tooltip("Quantos cliques o jogador precisa dar pra sovar a massa.")]
    public int cliquesParaSovar = 5;

    [Tooltip("Segundos que o pão fica no fogão até ficar pronto.")]
    public float tempoDeForno = 5f;

    [Header("Resultado")]
    [Tooltip("O ID do item (mesmo ID usado no seu ItemDictionary/Item.ID) que entra no inventário quando a task termina.")]
    public int itemIDResultante;
}