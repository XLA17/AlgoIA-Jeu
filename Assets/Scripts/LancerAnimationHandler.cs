using UnityEngine;

public class LancerAnimationHandler : MonoBehaviour
{
    public bool attackAnimationIsPlaying = false;

    public void AttackAnimationFinished()
    {
        attackAnimationIsPlaying = false;
        Debug.Log("animation finished");
    }
}
