using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using SlotGame.Core;

namespace SlotGame.UI {
    [Serializable]
    public class StatisticsPanel {
        [Serializable]
        public class Statistics {
            public string Details;
            public Text Tag;
            public Text Content;
        }
        [SerializeField]
        private Statistics[] statisticInfos;

        private Dictionary<string, Text> records;

        private int spinsPlayed;
        private int winningSpins;
        private int winGreaterThanBet;
        private int bonusWon;
        private string sequenceInfo;

        public int SpinsPlayed { 
            get { return spinsPlayed; }
            private set { 
                spinsPlayed = value;
                UpdateStatisticPanel("Spins Played", spinsPlayed.ToString());
            }
        }
        public int WinningSpins {
            get { return winningSpins; }
            private set {
                winningSpins = value;
                UpdateStatisticPanel("Winning Spins", winningSpins.ToString());
            }
        }
        public int WinGreaterThanBet {
            get { return winGreaterThanBet; }
            private set {
                winGreaterThanBet = value;
                UpdateStatisticPanel("Win > Bet", winGreaterThanBet.ToString());
            }
        }
        public int BonusWon
        {
            get { return bonusWon; }
            private set
            {
                bonusWon = value;
                UpdateStatisticPanel("Bonus Won", bonusWon.ToString());
            }
        }
        public string SequenceInfo
        {
            get { return sequenceInfo; }
            private set
            {
                sequenceInfo = value;
                UpdateStatisticPanel("Sequence Info", sequenceInfo);
            }
        }

        public void Initialize() {
            //记录内容文本框，显示UI标签
            records = statisticInfos.ToDictionary(key => key.Details, value => value.Content);
            foreach (var item in statisticInfos) {
                item.Tag.text = item.Details + ":";
            }
            //设置各个面板的初始值，然后添加监听事件，用于刷新统计面板
            SpinsPlayed = 0;
            WinningSpins = 0;
            WinGreaterThanBet = 0;
            BonusWon = 0;
            SequenceInfo = "0--0--0";
            GlobalObserver.OnFinalizedEvent += OnFinalizedCallback;
            GlobalObserver.OnSpinStartedEvent += OnSpinStartedCallback;
            GlobalObserver.OnToBonusEvent += OnToBonusCallback;
            GlobalObserver.OnNewSequenceEvent += OnNewSequenceCallback;
        }

        public void UpdateStatisticPanel(string key, string value) {
            if (records.TryGetValue(key, out Text content)) {
                content.text = value;
            }
        }


        public void OnSpinStartedCallback() {
            SpinsPlayed++;
        }
        public void OnFinalizedCallback() {
            //一轮游戏结束后，赢钱了要统计赢钱次数
            if (GlobalObserver.TotalWin > 0) {
                WinningSpins++;
                if (GlobalObserver.TotalWin > GlobalObserver.BetValue) {
                    WinGreaterThanBet++;
                }
            }
        }
        public void OnToBonusCallback()
        {
            BonusWon++;
        }
        public void OnNewSequenceCallback(string info)
        {
            SequenceInfo = info;
        }
    }
}
