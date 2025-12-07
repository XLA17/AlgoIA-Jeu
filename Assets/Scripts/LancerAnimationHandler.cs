using UnityEngine;

public class LancerAnimationHandler : MonoBehaviour
{
    public bool attackAnimationIsPlaying = false;

    public void AttackAnimationStarted()
    {
        attackAnimationIsPlaying = true;
        Debug.Log("animation started");
    }

    public void AttackAnimationFinished()
    {
        attackAnimationIsPlaying = false;
        Debug.Log("animation finished");
    }
}
