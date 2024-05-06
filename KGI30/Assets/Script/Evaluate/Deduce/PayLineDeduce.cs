namespace SlotGame.Deduce {
    using UnityEngine;
    using SlotGame.Core;
    using SlotGame.Result;
    using System.Collections;
    using System.Collections.Generic;
    using UniversalModule.Initialize;
    using UniversalModule.DelaySystem;
    using UniversalModule.SpawnSystem;

    public class PayLineDeduce {
        private static WaitForSeconds Wait;
        private static List<PayLine> Lines;
        private static Dictionary<string, Coroutine> coroutines;
        [AutoLoad]
        private static void Initialize() {
            Lines = new List<PayLine>();
            Wait = new WaitForSeconds(2f);
            coroutines = new Dictionary<string, Coroutine>();
            GlobalObserver.RegisterDeduce(PayMode.Line, Deduce);
            GlobalObserver.RegisterAbortDeduce(PayMode.Line, Abort);
        }
        /// <summary>
        /// 开始推演赔付流程
        /// </summary>
        private static void Deduce(ResultNode node) {
            PayLineResult result = node as PayLineResult;
            if (result.DrawLines.Count > 0) {
                var coroutine = DelayCallback.BeginCoroutine(CycleDeduce(result));
                coroutines.Add(node.ID, coroutine);
            }
        }
        /// <summary>
        /// 终止推演
        /// </summary>
        private static void Abort(ResultNode node) {
            if (Lines.Count > 0) {
                PayLineResult result = node as PayLineResult;
                for (int i = 0; i < Lines.Count; i++) {
                    Lines[i].Hide();
                    for (int j = 0; j < result.WinSymbols[i].Length; j++) {
                        result.WinSymbols[i][j].PlayIdleAnim();
                    }
                    Lines[i].Recycle();
                }
                Lines.Clear();
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
        private static IEnumerator CycleDeduce(PayLineResult result) {
            Lines.Clear();
            for (int i = 0; i < result.DrawLines.Count; i++) {
                PayLine line = SpawnFactory.GetObject<PayLine>(SpawnItemNames.PayLine);
                Lines.Add(line);
                line.DrawLine(result.DrawLines[i]);
                line.Show();
                for (int j = 0; j < result.WinSymbols[i].Length; j++) {
                    result.WinSymbols[i][j].PlayAwardAnim();
                }
            }
            yield return Wait;
            if (Lines.Count > 1) {
                for (int i = 0; i < Lines.Count; i++) {
                    Lines[i].Hide();
                    for (int j = 0; j < result.WinSymbols[i].Length; j++) {
                        result.WinSymbols[i][j].PlayIdleAnim();
                    }
                }
                while (true) {
                    for (int i = 0; i < Lines.Count; i++) {
                        Lines[i].Show();
                        for (int j = 0; j < result.WinSymbols[i].Length; j++) {
                            result.WinSymbols[i][j].PlayAwardAnim();
                        }
                        yield return Wait;
                        Lines[i].Hide();
                        for (int j = 0; j < result.WinSymbols[i].Length; j++) {
                            result.WinSymbols[i][j].PlayIdleAnim();
                        }
                    }
                }
            }
        }
    }
}
