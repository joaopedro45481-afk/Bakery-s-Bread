using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class Jukebox : MonoBehaviour
{
    [Header("Interação")]
    [SerializeField] private bool IsPlayer;

    [Header("Áudio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip coinSound;
    [SerializeField] private AudioClip[] musicas;

    private int musicaAtual = 0;
    private bool tocando = false;
    private bool esperandoCoin = false;

    void Start()
    {
        if (musicas.Length > 0)
        {
            audioSource.clip = musicas[musicaAtual];
        }
    }

    void Update()
    {
        if (!IsPlayer || Keyboard.current == null)
            return;

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (!tocando && !esperandoCoin)
            {
                StartCoroutine(TocarComMoeda());
            }
            else if (tocando)
            {
                audioSource.Pause();
                tocando = false;
            }
        }

        if (Keyboard.current.nKey.wasPressedThisFrame)
            ProximaMusica();

        if (Keyboard.current.bKey.wasPressedThisFrame)
            MusicaAnterior();
    }

    IEnumerator TocarComMoeda()
    {
        esperandoCoin = true;

        // toca som da moeda
        audioSource.clip = coinSound;
        audioSource.Play();

        // espera o som acabar
        yield return new WaitForSeconds(coinSound.length);

        // troca para música
        audioSource.clip = musicas[musicaAtual];
        audioSource.Play();

        tocando = true;
        esperandoCoin = false;

        Debug.Log("Tocando música: " + musicas[musicaAtual].name);
    }

    void ProximaMusica()
    {
        if (musicas.Length == 0) return;

        musicaAtual++;
        if (musicaAtual >= musicas.Length)
            musicaAtual = 0;

        TrocarMusica();
    }

    void MusicaAnterior()
    {
        if (musicas.Length == 0) return;

        musicaAtual--;
        if (musicaAtual < 0)
            musicaAtual = musicas.Length - 1;

        TrocarMusica();
    }

    void TrocarMusica()
    {
        bool estavaTocando = tocando;

        audioSource.clip = musicas[musicaAtual];

        if (estavaTocando)
        {
            audioSource.Play();
        }

        Debug.Log("Mudou para: " + musicas[musicaAtual].name);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            IsPlayer = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            IsPlayer = false;
        }
    }

        public bool EstaTocando
    {
        get { return tocando; }
    }
}