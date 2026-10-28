using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering.Universal; // necessário para Light2D

public class DayCycleController : MonoBehaviour
{
    public static DayCycleController Instance;

    [Header("Referências - Relógio")]
    public TextMeshProUGUI clockText;

    [Header("Referências - Painel de fim de dia")]
    public GameObject dailyCiclePanel;
    public TextMeshProUGUI lucroText;
    public TextMeshProUGUI diaText;
    public Button closeButton;

    [Header("Configuração do tempo")]
    public float horaInicial = 6f;
    public float horaFinal = 24f;
    public float duracaoDiaEmSegundos = 600f;

    [Header("Estado atual")]
    public float horaAtual;
    public int diaAtual = 1;

    [HideInInspector]
    public float lucroHoje = 0f;

    // ================= ILUMINAÇÃO =================
    [Header("Referências - Iluminação")]
    public Light2D luzSol; // Directional Light

    [Header("Cores da luz por horário")]
    public Color corLuzInicial = new Color(1f, 0.96f, 0.78f); // amarelo claro (6h)
    public Color corLuzMeioDia = Color.white;                  // branco (12h)
    public Color corLuzTarde = new Color(1f, 0.65f, 0.30f);    // laranja (15h)
    public Color corLuzNoite = new Color(0.60f, 0.30f, 0.80f); // roxo (24h)

    [Header("Intensidade da luz por horário")]
    public float intensidadeInicial = 1.2f;
    public float intensidadeMeioDia = 1.5f;
    public float intensidadeTarde = 1.1f;
    public float intensidadeNoite = 0.3f;

    [Header("Marcos do dia (horas)")]
    public float horaMeioDia = 12f;
    public float horaTarde = 15f;
    // ==============================================

    private float horasPorSegundo;
    private bool diaEncerrado = false;

    private readonly string[] diasDaSemana =
    {
        "Segunda", "Terça", "Quarta", "Quinta", "Sexta", "Sábado", "Domingo"
    };

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        horaAtual = horaInicial;

        float totalHoras = horaFinal - horaInicial;
        horasPorSegundo = totalHoras / duracaoDiaEmSegundos;

        dailyCiclePanel.SetActive(false);
        closeButton.onClick.AddListener(FecharPainelEIniciarNovoDia);

        AtualizarTextoRelogio();
        AtualizarIluminacao();
    }

    void Update()
    {
        if (diaEncerrado)
            return;

        horaAtual += horasPorSegundo * Time.deltaTime;

        if (horaAtual >= horaFinal)
        {
            horaAtual = horaFinal;
            AtualizarTextoRelogio();
            AtualizarIluminacao();
            EncerrarDia();
            return;
        }

        AtualizarTextoRelogio();
        AtualizarIluminacao();
    }

    void AtualizarTextoRelogio()
    {
        int horas = Mathf.FloorToInt(horaAtual);
        int minutos = Mathf.FloorToInt((horaAtual - horas) * 60f);

        clockText.text = $"{horas:00}:{minutos:00}";
    }

    // ================= ILUMINAÇÃO =================
    void AtualizarIluminacao()
    {
        if (luzSol == null) return;

        // Mapeia a hora (6h → 24h) para um valor de 0 a 1
        float t = Mathf.InverseLerp(horaInicial, horaFinal, horaAtual);

        Color corFinal;
        float intensidadeFinal;

        // Marcos reais do dia convertidos para 0..1
        float tMeioDia = Mathf.InverseLerp(horaInicial, horaFinal, horaMeioDia);
        float tTarde   = Mathf.InverseLerp(horaInicial, horaFinal, horaTarde);

        if (t <= tMeioDia)
        {
            // 6h → 12h : amarelo claro → branco
            float local = Mathf.InverseLerp(0f, tMeioDia, t);
            corFinal = Color.Lerp(corLuzInicial, corLuzMeioDia, local);
            intensidadeFinal = Mathf.Lerp(intensidadeInicial, intensidadeMeioDia, local);
        }
        else if (t <= tTarde)
        {
            // 12h → 15h : branco → laranja
            float local = Mathf.InverseLerp(tMeioDia, tTarde, t);
            corFinal = Color.Lerp(corLuzMeioDia, corLuzTarde, local);
            intensidadeFinal = Mathf.Lerp(intensidadeMeioDia, intensidadeTarde, local);
        }
        else
        {
            // 15h → 24h : laranja → roxo
            float local = Mathf.InverseLerp(tTarde, 1f, t);
            corFinal = Color.Lerp(corLuzTarde, corLuzNoite, local);
            intensidadeFinal = Mathf.Lerp(intensidadeTarde, intensidadeNoite, local);
        }

        luzSol.color = corFinal;
        luzSol.intensity = intensidadeFinal;

        // Ambient acompanha o tom da luz (deixando levemente mais escuro)
        RenderSettings.ambientLight = Color.Lerp(corFinal, Color.black, 0.3f);
    }
    // ==============================================

    void EncerrarDia()
    {
        diaEncerrado = true;

        string nomeDia = diasDaSemana[(diaAtual - 1) % diasDaSemana.Length];

        lucroText.text = $"Lucro: {lucroHoje}";
        diaText.text = $"Dia {diaAtual} - {nomeDia}";

        dailyCiclePanel.SetActive(true);

        Time.timeScale = 0f;
    }

    public void FecharPainelEIniciarNovoDia()
    {
        dailyCiclePanel.SetActive(false);

        Time.timeScale = 1f;

        horaAtual = horaInicial;
        diaAtual++;
        lucroHoje = 0f;
        diaEncerrado = false;

        AtualizarTextoRelogio();
        AtualizarIluminacao();
    }
}