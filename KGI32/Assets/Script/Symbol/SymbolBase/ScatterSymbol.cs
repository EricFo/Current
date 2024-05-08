using UnityEngine;
using SlotGame.Symbol;
using UniversalModule.SpawnSystem;
using System.Collections;
using UniversalModule.DelaySystem;
using UnityEngine.Rendering;
using UniversalModule.AudioSystem;

public class ScatterSymbol : CommonSymbol
{
    public SortingGroup BGFire;
    public StencilInteraction BGParticle;
    public SortingGroup fire2;
    public ParticleSystemRenderer[] catterFire;
    public ParticleSystemRenderer fire2Particle;

    #region 动画Hash
    protected int HIT = Animator.StringToHash("Hit");
    protected int UPGRADE = Animator.StringToHash("Upgrade");
    protected int WIN = Animator.StringToHash("Win");
    protected int STOP = Animator.StringToHash("Stop");
    protected int UPGRADE1 = Animator.StringToHash("Upgrade1");
    protected int UPGRADE2 = Animator.StringToHash("Upgrade2");
    #endregion

    public override void Install(Transform parent, Vector3 localPos, int reelID, int sortingOrder = 101)
    {
        base.Install(parent, localPos, reelID, sortingOrder + 10);
        BGParticle.gameObject.SetActive(true);
        BGFire.transform.localScale = Vector3.one;
    }

    public void UpSortingOrder(bool isUp=true)
    {
        SetSortingOrder(DefaultSortingOrder + (isUp ? 400 : 0));
    }

    public override void SetSortingOrder(int order)
    {
        base.SetSortingOrder(order);
        BGFire.sortingOrder = order - 1;
        fire2.sortingOrder = order - 2;
    }

    public override void SetMaskMode(SpriteMaskInteraction interaction)
    {
        base.SetMaskMode(interaction);
        BGParticle.Interaction = interaction;
        foreach(var item in catterFire)
        {
            item.maskInteraction = interaction;
        }
        fire2Particle.maskInteraction = interaction;
    }

    public override void SetDisplay(bool isDisplay)
    {
        base.SetDisplay(isDisplay);
        BGParticle.gameObject.SetActive(isDisplay);
        fire2.gameObject.SetActive(isDisplay);
    }

    public void OnStop()
    {
        fire2.gameObject.SetActive(false);
        BGFire.transform.localScale = Vector3.one;
    }

    public void PlayStopAnim()
    {
        PlayAnimation(STOP);
    }

    public void PlayHitAnim()
    {
        StartCoroutine(_PlayHitAnim());
    }
    IEnumerator _PlayHitAnim()
    {
        SetSortingOrder(DefaultSortingOrder + 400);
        PlayAnimation(HIT);
        yield return new WaitForSeconds(24/30f);
        //SetSortingOrder(DefaultSortingOrder);
    }

    public void PlayUpgradeAnim(bool isBig)
    {
        SetSortingOrder(DefaultSortingOrder + 400);
        if (isBig)
        {
            AudioManager.Playback("FE_SpecialActive");
            PlayAnimation(UPGRADE2);
        }
        else
        {
            AudioManager.Playback("FE_SpecialActive02");
            PlayAnimation(UPGRADE1);
        }
        //DelayCallback.Delay(this, 38 / 30f, () =>
        //{
        //    SetSortingOrder(DefaultSortingOrder);
        //});
    }

    public void PlayBonusUpgradeAnim()
    {
        AudioManager.Playback("FE_SpecialActive Bonus（bonus）");
        SetSortingOrder(DefaultSortingOrder + 400);
        PlayAnimation(UPGRADE2);
        //DelayCallback.Delay(this, 38 / 30f, () =>
        //{
        //    SetSortingOrder(DefaultSortingOrder);
        //});
    }

    public void PlayWinAnim()
    {
        PlayAnimation(WIN);
    }



}
