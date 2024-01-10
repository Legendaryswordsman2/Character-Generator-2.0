using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoinDiscordButton : MonoBehaviour
{
    [SerializeField] Transform leftPOS;
    [SerializeField] Transform rightPOS;

    float defaultPOS;
    private void Awake()
    {
        defaultPOS = transform.localPosition.x;
    }
    private void OnEnable()
    {
        //StopAllCoroutines();
        StartCoroutine(FirstMove());
    }

    IEnumerator FirstMove()
    {
        LeanTween.cancel(gameObject);
        transform.localPosition = new Vector3(defaultPOS, transform.localPosition.y, 0);
        LeanTween.moveLocalX(gameObject, leftPOS.localPosition.x, 2.5f);
        yield return new WaitForSeconds(2.5f);

        StartCoroutine(Move());
    }
    IEnumerator Move()
    {
        //float time = Random.Range(10, 13);
        float time = 5;
        LeanTween.moveLocalX(gameObject, rightPOS.localPosition.x, time);

        yield return new WaitForSeconds(time);

        //time = Random.Range(10, 13);
        LeanTween.moveLocalX(gameObject, leftPOS.localPosition.x, time);

        yield return new WaitForSeconds(time);

        StartCoroutine(Move());
    }
}