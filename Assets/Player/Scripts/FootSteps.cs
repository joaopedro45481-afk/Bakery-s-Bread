using UnityEngine;

public class FootSteps : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip[] footstepSounds;
    public Rigidbody2D rb;

    public float stepInterval = 0.4f;

    private float timer;

    void Update()
    {
        if (Mathf.Abs(rb.linearVelocity.x) > 0.1f)
        {
            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                PlayFootstep();
                timer = stepInterval;
            }
        }

         if (Mathf.Abs(rb.linearVelocity.y) > 0.1f)
        {
            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                PlayFootstep();
                timer = stepInterval;
            }
        }
    }

    void PlayFootstep()
    {
        AudioClip clip = footstepSounds[Random.Range(0, footstepSounds.Length)];
        audioSource.PlayOneShot(clip);
    }
}