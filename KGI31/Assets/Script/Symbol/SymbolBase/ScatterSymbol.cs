using UnityEngine;
using DG.Tweening;
using SlotGame.Symbol;
using UniversalModule.DelaySystem;
using UniversalModule.AudioSystem;

public class ScatterSymbol : CommonSymbol {

    public SpriteRenderer MainTexFly;

    public override void Install(Transform parent, Vector3 localPos, int reelID, int sortingOrder = 101) {
        base.Install(parent, localPos, reelID, sortingOrder + 20);
        transform.transform.localScale = Vector3.one;
    }

    protected int FLY = Animator.StringToHash("Fly");
    protected int WIN = Animator.StringToHash("Win");

    public void PlayFlyAnim(Vector3 target) {
        if (MainTex.sortingOrder < 200) {
            MainTex.sortingOrder = MainTex.sortingOrder + 200;
            DelayCallback.Delay(this, 1f, () => {
                if (MainTex.sortingOrder > 200) {
                    MainTex.sortingOrder = MainTex.sortingOrder - 200;
                }
            });
        }
        MainTexFly.enabled = true;
        Vector3 dir = (target - transform.position).normalized;
        float angle = Vector2.SignedAngle(Vector2.up, dir);
        MainTexFly.transform.rotation = Quaternion.Euler(0, 0, angle);
        PlayAnimation(FLY);
        MainTexFly.transform.DOMove(target, 22/30f).OnComplete(() => {
            AudioManager.PlayOneShot("FE_BonusSymbolFly");
            DelayCallback.Delay(this, 0.5f, () => {
                MainTexFly.transform.position = MainTex.transform.position;
                MainTexFly.enabled = false;
            });
        });
    }

    public override void PlayIdleAnim() {
        base.PlayIdleAnim();

        if (MainTex.sortingOrder > 200) {
            MainTex.sortingOrder = MainTex.sortingOrder - 200;
        }
    }

    public void PlayWinAnim() {
        if (MainTex.sortingOrder < 200) {
            MainTex.sortingOrder = MainTex.sortingOrder + 200;
        }
        PlayAnimation(WIN);
    }

    public override void SetSortingOrder(int order) {
        base.SetSortingOrder(order);
        MainTexFly.sortingOrder = order + 900;
    }

    public override void SetMaskMode(SpriteMaskInteraction interaction) {
        base.SetMaskMode(interaction);
    }

}
