using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniversalModule.SpawnSystem;

public class DropStar : SpawnItem
{
    [SerializeField] protected Animator MainAnim;
    [SerializeField] public SpriteRenderer MainTex;

    

    #region 动画Hash
    protected int IDLE = Animator.StringToHash("Idle");
    protected int DROPGRANDSTAR = Animator.StringToHash("DropGrandStar");
    protected int DROPMAJORSTAR = Animator.StringToHash("DropMajorStar");
    protected int DROPMINISTAR = Animator.StringToHash("DropMiniStar");
    #endregion

    protected void PlayAnimation(int animHashCode)
    {
        if (MainAnim != null && MainAnim.isActiveAndEnabled)
        {
            MainAnim.Play(animHashCode, 0, 0);
        }
    }
    public void PlayDropStarAnim(int jpType)
    {
        StartCoroutine(PlayDropStarAnimIen(jpType));
    }
    public IEnumerator PlayDropStarAnimIen(int jpType)
    {
        switch (jpType)
        {
            case 0:
                PlayAnimation(DROPGRANDSTAR);
                break;
            case 1:
                PlayAnimation(DROPMAJORSTAR);
                break;
            case 2:
                PlayAnimation(DROPMINISTAR);
                break;
        }
        yield return new WaitForSeconds(30 / 30f);
        Recycle();
    }


}
