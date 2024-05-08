using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniversalModule.AudioSystem;

public class FeaturePicked : Popup
{
    public GameObject[] transitionAnim;
    //[SerializeField] private GameObject Bonus = null;
    //[SerializeField] private BoxCollider2D SelectBox = null;
    //[SerializeField] protected Animator FeatureIntroAnim;
    // Start is called before the first frame update
    protected override void OnClick()
    {
        StartCoroutine(_OnClick());
    }

    IEnumerator _OnClick()
    {
        AudioManager.Playback("FE_BonusTurn");
        foreach (var trans in transitionAnim)
        {
            trans.SetActive(false);
            trans.SetActive(true);
        }
        yield return new WaitForSeconds(25 / 30f);
        base.OnClick();
    }
}
