namespace SlotGame.Core.Transition {
    public interface ITransition {
        void OnFadeIn();
        void OnFadeOut();

        void OnSnapIn();
        void OnSnapOut();
    }
}
