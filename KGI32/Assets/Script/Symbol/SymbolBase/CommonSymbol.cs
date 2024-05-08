using UnityEngine;
using UniversalModule.SpawnSystem;

namespace SlotGame.Symbol
{
    public class CommonSymbol : SpawnItem
    {
        [SerializeField] protected Animator MainAnim;
        [SerializeField] protected SpriteRenderer MainTex;

        protected int originSortingOrder = -1;

        #region 属性
        /// <summary>
        /// Symbol在Reel中的位置信息
        /// </summary>
        public int IndexOnReel { get; protected set; }
        protected int DefaultSortingOrder { get; set; }
        #endregion

        #region 动画Hash
        protected int IDLE = Animator.StringToHash("Idle");
        protected int HIDE = Animator.StringToHash("Hide");
        protected int AWARD = Animator.StringToHash("Award");
        #endregion

        #region 动画状态更新及播放动画相关虚函数接口
        /// <summary>
        /// 播放动画
        /// </summary>
        /// <param name="animHashCode">动画哈希值</param>
        protected void PlayAnimation(int animHashCode)
        {
            if (MainAnim != null && MainAnim.isActiveAndEnabled)
            {
                MainAnim.Play(animHashCode, 0, 0);
            }
        }
        /// <summary>
        /// 播放奖励动画
        /// </summary>
        public virtual void PlayAwardAnim()
        {
            PlayAnimation(AWARD);
        }
        /// <summary>
        /// 播放Idle动画
        /// </summary>
        public virtual void PlayIdleAnim()
        {
            PlayAnimation(IDLE);
            //ResetLayer();
        }
        #endregion

        #region 设置相关的虚函数接口
        /// <summary>
        /// 更新Symbol信息
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="localPos"></param>
        public virtual void Install(Transform parent, Vector3 localPos, int reelID, int sortingOrder = 101)
        {
            base.transform.SetParent(parent);
            base.transform.SetAsFirstSibling();  //新出现的Symbol在最上面
            base.transform.localPosition = localPos;
            if (reelID == 7)
            {
                originSortingOrder = sortingOrder + reelID * 20 + 300;
            }
            else
            {
                originSortingOrder = sortingOrder + reelID * 20;
            }
            SetMaskMode(SpriteMaskInteraction.VisibleInsideMask);
            DefaultSortingOrder = originSortingOrder;
            SetSortingOrder(DefaultSortingOrder);
        }
        /// <summary>
        /// 设置遮罩交互模式
        /// </summary>
        /// <param name="interaction"></param>
        public virtual void SetMaskMode(SpriteMaskInteraction interaction)
        {
            MainTex.maskInteraction = interaction;
        }
        public virtual void SetSortingLayer()
        {
            //MainTex.
        }
        /// <summary>
        /// 设置层级
        /// </summary>
        /// <param name="order"></param>
        public virtual void SetSortingOrder(int order)
        {
            MainTex.sortingOrder = order;
        }
        /// <summary>
        /// 更新Symbol的索引值
        /// </summary>
        /// <param name="idx"></param>
        public virtual void UpdateIndexOnReel(int idx)
        {
            IndexOnReel = idx;
            DefaultSortingOrder = originSortingOrder + idx;
            SetSortingOrder(DefaultSortingOrder);
        }
        /// <summary>
        /// 设置symbol是否显示
        /// </summary>
        /// <param name="isDisplay"></param>
        public virtual void SetDisplay(bool isDisplay)
        {
            PlayAnimation(isDisplay ? IDLE : HIDE);
        }
        #endregion

        #region 辅助工具
        protected virtual void ResetLayer()
        {
            SetSortingOrder(DefaultSortingOrder);
            SetMaskMode(SpriteMaskInteraction.VisibleInsideMask);
        }
        #endregion
    }
}
