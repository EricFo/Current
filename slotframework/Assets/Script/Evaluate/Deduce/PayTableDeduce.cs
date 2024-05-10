namespace SlotGame.Deduce {
    using UnityEngine;
    using SlotGame.Core;
    using SlotGame.Symbol;
    using SlotGame.Result;
    using System.Collections;
    using System.Collections.Generic;
    using UniversalModule.Initialize;
    using UniversalModule.DelaySystem;

    public class PayTableDeduce{
        private static bool isDeduce;
        private static WaitForSeconds Wait;
        private static Dictionary<string, Coroutine> coroutines;
        [AutoLoad]
        private static void Initialize() {
            Wait = new WaitForSeconds(3f);
            coroutines = new Dictionary<string, Coroutine>();
            GlobalObserver.RegisterDeduce(PayMode.Table, Deduce);
            GlobalObserver.RegisterAbortDeduce(PayMode.Table, Abort);
        }
        /// <summary>
        /// 开始推演赔付流程
        /// </summary>
        private static void Deduce(ResultNode node) {
            PayTableResult result = node as PayTableResult;
            if (result.WinSymbols.Values.Count > 0) {
                var coroutine = DelayCallback.BeginCoroutine(CycleDeduce(result));
                coroutines.Add(node.ID, coroutine);
            }
        }
        /// <summary>
        /// 终止推演
        /// </summary>
        private static void Abort(ResultNode node) {
            if (isDeduce) {
                PayTableResult result = node as PayTableResult;
                foreach (List<CommonSymbol> item in result.WinSymbols.Values) {
                    for (int i = 0; i < item.Count; i++) {
                        item[i].PlayIdleAnim();
                    }
                }
                isDeduce = false;
                Coroutine coroutine = null;
                if (coroutines.TryGetValue(node.ID, out coroutine)) {
                    DelayCallback.AbortCoroutine(coroutine);
                    coroutines.Remove(node.ID);
                }
            }
        }
        /// <summary>
        /// 循环推演
        /// </summary>
        /// <returns></returns>
        private static IEnumerator CycleDeduce(PayTableResult result) {
            isDeduce = true;
            yield return null;
            //默认所有的中奖Symbol先播一遍
            foreach (List<CommonSymbol> item in result.WinSymbols.Values) {
                for (int i = 0; i < item.Count; i++) {
                    item[i].PlayAwardAnim();
                }
            }
            yield return Wait;
            //如果中奖结果数量大于1
            if (result.WinSymbols.Count > 1) {
                //先重置所有Symbol的动画
                foreach (List<CommonSymbol> item in result.WinSymbols.Values) {
                    for (int i = 0; i < item.Count; i++) {
                        item[i].PlayIdleAnim();
                    }
                }
                while (true) {
                    foreach (List<CommonSymbol> item in result.WinSymbols.Values) {
                        for (int i = 0; i < item.Count; i++) {
                            item[i].PlayAwardAnim();
                        }
                        yield return Wait;
                        for (int i = 0; i < item.Count; i++) {
                            item[i].PlayIdleAnim();
                        }
                    }
                }
            }
        }
    }
}
