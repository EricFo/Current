using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniversalModule.AudioSystem;
using UniversalModule.SpawnSystem;

public class StarFlySymbol : SpawnItem
{
    public void FlyStar(Vector3 initPos, Vector3 targetPot) {
        StartCoroutine(Fly(initPos, targetPot));
    }

    private IEnumerator Fly(Vector3 initPos, Vector3 targetPot) {

        transform.position = initPos;
        yield return new WaitForSeconds(20f / 30f);
        AudioManager.Playback("FE_Wheelstop03b");
        Vector3 dir = (targetPot - initPos).normalized;
        transform.up = dir;
        transform.DOMove(targetPot, 21f/30f);
        yield return new WaitForSeconds(1f);
        Recycle();
    }
}
