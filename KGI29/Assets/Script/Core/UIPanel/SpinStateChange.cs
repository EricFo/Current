using UnityEngine;
using SlotGame.Core;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SpinStateChange : MonoBehaviour, IPointerClickHandler {
    [SerializeField] private Sprite SpinSprite;
    [SerializeField] private Sprite AutoSpinSprite;

    private Button SpinButton;

    private void Awake() {
        SpinButton = GetComponent<Button>();
        GlobalObserver.OnAutoPlayChangeEvent += UpdateSpinSate;
    }

    public void OnPointerClick(PointerEventData eventData) {
        if (eventData.button == PointerEventData.InputButton.Right) {
            if (SpinButton != null && this.gameObject.activeInHierarchy && SpinButton.interactable) {
                if (GlobalObserver.AutoSpinEnabled == false) {
                    GlobalObserver.SetAutoPlay(true);
                } else {
                    GlobalObserver.SetAutoPlay(false);
                }
            }
        }
    }

    public void UpdateSpinSate(bool autoPlayEnabled) {
        SpinButton.image.sprite = autoPlayEnabled ? AutoSpinSprite : SpinSprite;
        SpinButton.image.SetNativeSize();
    }
}
