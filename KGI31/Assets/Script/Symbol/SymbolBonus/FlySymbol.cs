using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniversalModule.AudioSystem;
using UniversalModule.SpawnSystem;

public class FlySymbol : SpawnItem
{
    [SerializeField] SpriteRenderer mainTex;
    public  Animator WildFly;


    int FLY = Animator.StringToHash("Fly");
    int FLYSPIN = Animator.StringToHash("FlySpin");

    public void PlayFlyAnim(Vector3 startPos, Vector3 targetPos, int order)
    {
        mainTex.sortingOrder = 1000 + order;
        StartCoroutine(Fly(startPos, targetPos));
    }

    private IEnumerator Fly(Vector3 startPos, Vector3 targetPos)
    {
        //WildFly.Play(FLY);
        transform.position = startPos;
        /*Vector3 dir = (targetPos - startPos).normalized;
        transform.up = dir;*/
        transform.DOMove(targetPos, 14f / 30f);
        yield return new WaitForSeconds(38 / 30f);
        Recycle();
    }
    public void PlaySpinFlyAnim(/*Vector3 startPos, Vector3 targetPos,int order*/)
    {
        mainTex.sortingOrder = 3000;
        StartCoroutine(FlySpin(/*startPos, targetPos*/));
    }
    private IEnumerator FlySpin(/*Vector3 startPos, Vector3 targetPos*/)
    {
        WildFly.Play(FLYSPIN);
        //transform.position = startPos;
        //Vector3 dir = (targetPos - startPos).normalized;
        //transform.up = dir;
        //transform.DOMove(targetPos, 14f / 30f);
        yield return new WaitForSeconds(38 / 30f);
        Recycle();
    }

}
