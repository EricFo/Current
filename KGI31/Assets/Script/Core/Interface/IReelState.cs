using System;
using SlotGame.Symbol;
using SlotGame.Reel.Args;

namespace SlotGame.Core.Reel {
    public interface IReelState {
        bool IsPause { get; }
        bool IsShutDown { get; }

        bool IsInfinity { get; set; }
        ReelSettingArgs Setting { get; }

        event Action<ReelBase> OnStepListener;
        event Action<ReelBase> OnStopListener;

        void Initialize(ReelSettingArgs args);
        void Spin(ReelSpinArgs args);
        bool ShutDown();
        CommonSymbol[] GetAllSymbols();
        CommonSymbol[] GetVisibleSymbols();
        void ReplaceAllSymbol(string symbolName);
        void DisplayTheInvisibleSymbol(bool isDisplay);
        CommonSymbol ReplaceSymbol(string symbolName, int id);
        CommonSymbol ReplaceSymbol(string symbolName, CommonSymbol oldSymbol);
    }
}
