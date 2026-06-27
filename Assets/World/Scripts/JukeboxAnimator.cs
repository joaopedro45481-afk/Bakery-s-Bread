using UnityEngine;

public class JukeboxAnimator : MonoBehaviour
{
    [SerializeField] private Jukebox jukebox;
    [SerializeField] private Animator animator;

    void Update()
    {
        if (jukebox == null || animator == null)
            return;

        bool tocando = jukebox.EstaTocando;

        animator.SetBool("IsPlaying", tocando);

        Debug.Log("isPlaying = " + tocando);
    }
}