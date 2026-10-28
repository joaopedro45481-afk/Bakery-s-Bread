using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Fica no objeto visual da massa. Suporta várias FASES em sequência
/// (ex: sovar por clique, depois abrir por arraste), cada uma com sua
/// própria lista de frames e seu próprio tipo de interação.
///
/// - Fase tipo Clique: cada clique avança 1 frame.
/// - Fase tipo Arrasto: cada "PixelsPorFrame" de distância arrastada
///   (pra cima ou pra baixo, valor absoluto) avança 1 frame.
///
/// Ao chegar no último frame da fase atual, passa pra próxima fase
/// (frame 0 dela). Ao terminar o último frame da ÚLTIMA fase, a task
/// avança sozinha pro fogão — sem confirmação extra.
///
/// No Inspector: monta o array "Fases". Ex: Fases[0] = sovar (Clique),
/// Fases[1] = abrir (Arrasto, com PixelsPorFrame tipo 40-60).
///
/// Onde encaixa:
/// - Recebe o CookingTaskController ativo via Iniciar(), chamado pela
///   CookingUI quando o painel abre.
/// - Ao terminar todas as fases, chama controller.AvancarParaSova() e
///   controller.AvancarParaFogao() (a CookingUI escuta OnEstadoMudou
///   e manda a massa pro Stove).
///
/// IMPORTANTE: exige um Collider2D no mesmo objeto do SpriteRenderer
/// (OnMouseDown/OnMouseDrag dependem dele pra saber que o mouse está
/// sobre a massa).
/// </summary>
public class DoughInteraction : MonoBehaviour
{
    public enum TipoInteracao { Clique, Arrasto }

    [System.Serializable]
    public class FaseMassa
    {
        public string nome; // só organização no Inspector
        public TipoInteracao tipo = TipoInteracao.Clique;
        [Tooltip("Só usado se o tipo for Arrasto: pixels arrastados pra avançar 1 frame.")]
        public float pixelsPorFrame = 50f;
        public Sprite[] frames;
    }

    [SerializeField] private SpriteRenderer visual; // ou Image, se for UI
    [SerializeField] private FaseMassa[] fases;

    private int faseAtual = 0;
    private int frameAtual = 0;
    private float distanciaAcumulada = 0f;
    private CookingTaskController controller;

    /// <summary>Chamado pela CookingUI quando a tela abre com uma task nova.</summary>
    public void Iniciar(CookingTaskController taskController, RecipeSO receita)
    {
        controller = taskController;
        faseAtual = 0;
        frameAtual = 0;
        distanciaAcumulada = 0f;
        AtualizarVisual();
    }

    private FaseMassa FaseAtualObj => (fases != null && fases.Length > 0) ? fases[faseAtual] : null;

    private void OnMouseDown()
    {
        FaseMassa fase = FaseAtualObj;
        if (fase == null) return;

        if (fase.tipo == TipoInteracao.Clique)
        {
            AvancarFrame();
        }
        else
        {
            distanciaAcumulada = 0f; // começa a contar o arraste do zero a cada novo clique
        }
    }

    private void OnMouseDrag()
    {
        FaseMassa fase = FaseAtualObj;
        if (fase == null || fase.tipo != TipoInteracao.Arrasto) return;
        if (Mouse.current == null) return;

        float deltaY = Mouse.current.delta.ReadValue().y;
        distanciaAcumulada += Mathf.Abs(deltaY);

        while (distanciaAcumulada >= fase.pixelsPorFrame)
        {
            distanciaAcumulada -= fase.pixelsPorFrame;
            AvancarFrame();

            // se AvancarFrame mudou de fase (ou terminou), para de consumir
            // arraste com as regras da fase antiga
            if (FaseAtualObj != fase) break;
        }
    }

    private void AvancarFrame()
    {
        if (fases == null || fases.Length == 0) return;

        FaseMassa fase = fases[faseAtual];
        bool ultimoFrameDaFase = frameAtual >= fase.frames.Length - 1;

        if (!ultimoFrameDaFase)
        {
            frameAtual++;
            AtualizarVisual();
            return;
        }

        bool ultimaFase = faseAtual >= fases.Length - 1;

        if (!ultimaFase)
        {
            faseAtual++;
            frameAtual = 0;
            distanciaAcumulada = 0f;
            AtualizarVisual();
            return;
        }

        // já estava na última fase, no último frame dela: massa pronta
        controller.AvancarParaSova();
        controller.AvancarParaFogao();
    }

    private void AtualizarVisual()
    {
        if (visual == null || fases == null || fases.Length == 0) return;
        Sprite[] framesDaFase = fases[faseAtual].frames;
        if (framesDaFase == null || framesDaFase.Length == 0) return;
        visual.sprite = framesDaFase[frameAtual];
    }
}