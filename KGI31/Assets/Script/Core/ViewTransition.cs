using System;
using UnityEngine;
using DG.Tweening;
using SlotGame.Core.Singleton;

public class ViewTransition : GlobalSingleton<ViewTransition> {
    [Serializable]
    public class Transition {
        public float FadeTime = 0.3f;
        public GameObject[] GameObjects;
        public CanvasGroup[] CanvasGroups;
        public SpriteRenderer[] SpriteRenderers;

        /// <summary>
        /// 淡入转换
        /// </summary>
        public void FadeIn() {
            Array.ForEach(GameObjects, item => { item.SetActive(true); });
            Array.ForEach(CanvasGroups, item => { item.DOFade(1f, FadeTime); });
            Array.ForEach(SpriteRenderers, item => { item.DOFade(1f, FadeTime); });
        }
        /// <summary>
        /// 淡出转换
        /// </summary>
        public void FadeOut() {
            Array.ForEach(GameObjects, item => { item.SetActive(false); });
            Array.ForEach(CanvasGroups, item => { item.DOFade(0f, FadeTime); });
            Array.ForEach(SpriteRenderers, item => { item.DOFade(0f, FadeTime); });
        }

        /// <summary>
        /// 直接进入
        /// </summary>
        public void SnapIn() {
            Array.ForEach(GameObjects, item => { item.SetActive(true); });
            foreach (CanvasGroup item in CanvasGroups) {
                item.DOKill();
                item.alpha = 1f;
            }
            foreach (SpriteRenderer item in SpriteRenderers) {
                item.DOKill();
                item.color = Color.white;
            }
        }
        /// <summary>
        /// 直接退出
        /// </summary>
        public void SnapOut() {
            Array.ForEach(GameObjects, item => { item.SetActive(false); });
            foreach (CanvasGroup item in CanvasGroups) {
                item.DOKill();
                item.alpha = 0f;
            }
            foreach (SpriteRenderer item in SpriteRenderers) {
                item.DOKill();
                item.color = new Color(1, 1, 1, 0);
            }
        }
    }

    [SerializeField] private Transition BaseGame = null;
    [SerializeField] private Transition FreeGame = null;
    [SerializeField] private Transition BonusGame = null;
    [SerializeField] private Transition Jackpot = null;

    /// <summary>
    /// 转变窗口视图，用于不同玩法之间的转换
    /// </summary>
    /// <param name="from">需要隐藏的玩法</param>
    /// <param name="to">需要显示的玩法</param>
    /// <param name="isSnap">默认为false,若为true,则没有过渡效果直接变化</param>
    public static void TransitionTo(GameState from, GameState to, bool isSnap = false) {
        Instance.CheckIsInitialized();
        Transition toTransition = GetTransition(to);
        Transition fromTransition = GetTransition(from);
        if (isSnap == false) {
            fromTransition.FadeOut();
            toTransition.FadeIn();
        } else {
            fromTransition.SnapOut();
            toTransition.SnapIn();
        }
    }

    /// <summary>
    /// 根据状态获取对应的转换类型
    /// </summary>
    /// <param name="state"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private static Transition GetTransition(GameState state) {
        switch (state) {
            case GameState.Base:
                return Instance.BaseGame;
            case GameState.Free:
                return Instance.FreeGame;
            case GameState.Bonus:
                return Instance.BonusGame;
            case GameState.JackPot:
                return Instance.Jackpot;
            default:
                throw new Exception(string.Format("未找到{0}状态的Transition模块", state));
        }
    }
}
