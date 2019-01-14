using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonMaterial : MonoBehaviour
{
    private new Renderer renderer;
    private MaterialPropertyBlock propBlock;
    private Coroutine lerpColorTo;

    public AnimationCurve curve;

    private void Awake()
    {
        propBlock = new MaterialPropertyBlock();
        renderer = GetComponent<Renderer>();
    }

    public void LerpValueTo(string name, float speed, AnimationCurve curve)
    {
        if (lerpColorTo != null)
            StopCoroutine(lerpColorTo);
        lerpColorTo = StartCoroutine(_LerpValueTo(name, speed, curve));
    }

    private IEnumerator _LerpValueTo(string name, float speed, AnimationCurve curve)
    {
        float time = 0;

        while (true)
        {
            renderer.GetPropertyBlock(propBlock);

            time += Time.deltaTime * speed;
            if (time >= 1)
                time = 0;

            propBlock.SetFloat(name, curve.Evaluate(time));          
            renderer.SetPropertyBlock(propBlock);
            Debug.Log(renderer.material.GetFloat(name));
            yield return null;
        }
    }

    public void ResetLerpValue(string name)
    {
        renderer.GetPropertyBlock(propBlock);
        propBlock.SetFloat(name, 0);
        renderer.SetPropertyBlock(propBlock);
    }
}
