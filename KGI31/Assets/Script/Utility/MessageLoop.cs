using DG.Tweening;
using UnityEngine;

public class MessageLoop : MonoBehaviour {
    public float interval;
    public float fadeTimer;
    public int orderInLayer;
    public Sprite[] messages;

    private int count;
    private int ID;
    private SpriteRenderer[] sprites;
    private void Awake() {
        ID = 1;
        count = 1;
        GameObject temp = new GameObject("Message-1");
        temp.transform.SetParent(this.transform);
        temp.transform.ResetLocalProperty();
        temp.AddComponent<SpriteRenderer>();
        temp = new GameObject("Message-2");
        temp.transform.SetParent(this.transform);
        temp.transform.ResetLocalProperty();
        temp.AddComponent<SpriteRenderer>();
        sprites = GetComponentsInChildren<SpriteRenderer>();
        sprites[0].sprite = messages[0];
        sprites[1].sprite = messages[1];
        sprites[0].DOFade(1, 0).SetAutoKill();
        sprites[1].DOFade(0, 0).SetAutoKill();
        sprites[0].sortingOrder = orderInLayer;
        sprites[1].sortingOrder = orderInLayer + 1;
        InvokeRepeating("Loop", interval, interval);
    }

    private void Loop() {
        count++;
        count %= messages.Length;
        int currMessage = count;
        int id = ID;
        ID = ++ID % 2;
        sprites[0].DOFade(id == 0 ? 1 : 0, fadeTimer).OnComplete(() => {
            if (id == 1)
                sprites[0].sprite = messages[currMessage];
        }).SetAutoKill();
        sprites[1].DOFade(id == 0 ? 0 : 1, fadeTimer).OnComplete(() => {
            if (id == 0)
                sprites[1].sprite = messages[currMessage];
        }).SetAutoKill();
    }
}
