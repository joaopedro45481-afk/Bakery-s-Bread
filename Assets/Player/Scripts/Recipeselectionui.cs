using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Tela que abre ANTES do CookingUI, mostrando um botão por receita
/// disponível naquela mesa. Ao escolher uma, abre o CookingUI com ela.
///
/// Só precisa existir UMA instância dessa tela na cena (compartilhada
/// entre todas as mesas), igual ao CookingUI — cada InteractableTable só
/// passa sua própria lista de receitas na hora de abrir.
///
/// Onde encaixa:
/// - InteractableTable chama AbrirSelecao(receitasDestaMesa) ao apertar E.
/// - Ao clicar num botão, chama cookingUI.AbrirTela(receitaEscolhida).
///
/// Setup no Inspector:
/// - Painel Selecao: o GameObject do painel (deixa desativado por padrão).
/// - Container Botoes: um objeto vazio dentro do painel com um Layout
///   Group (Vertical ou Grid), onde os botões vão nascer.
/// - Botao Receita Prefab: um prefab com componente Button + um
///   TextMeshProUGUI filho pra mostrar o nome da receita.
/// - Cooking Ui: a mesma instância do painel de cozinha já existente.
/// </summary>
public class RecipeSelectionUI : MonoBehaviour
{
    [SerializeField] private GameObject painelSelecao;
    [SerializeField] private Transform containerBotoes;
    [SerializeField] private GameObject botaoReceitaPrefab;
    [SerializeField] private CookingUI cookingUI;

    private readonly List<GameObject> botoesInstanciados = new List<GameObject>();

    public void AbrirSelecao(RecipeSO[] receitas)
    {
        LimparBotoes();

        foreach (RecipeSO receita in receitas)
        {
            GameObject botaoObj = Instantiate(botaoReceitaPrefab, containerBotoes);

            TextMeshProUGUI texto = botaoObj.GetComponentInChildren<TextMeshProUGUI>();
            if (texto != null) texto.text = receita.nomeReceita;

            Button botao = botaoObj.GetComponent<Button>();
            RecipeSO receitaCapturada = receita; // evita bug de closure reutilizando variável do loop
            botao.onClick.AddListener(() => EscolherReceita(receitaCapturada));

            botoesInstanciados.Add(botaoObj);
        }

        painelSelecao.SetActive(true);
    }

    private void EscolherReceita(RecipeSO receita)
    {
        painelSelecao.SetActive(false);
        cookingUI.AbrirTela(receita);
    }

    public void FecharSelecao()
    {
        painelSelecao.SetActive(false);
        LimparBotoes();
    }

    private void LimparBotoes()
    {
        foreach (GameObject botao in botoesInstanciados)
        {
            Destroy(botao);
        }
        botoesInstanciados.Clear();
    }
}