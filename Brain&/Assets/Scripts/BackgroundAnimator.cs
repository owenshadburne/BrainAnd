using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundAnimator : MonoBehaviour
{
    [SerializeField] Transform _light, plane;
    AnimationCurve curve;
    float debugSpeedup = 1f;

    void Start()
    {
        curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        StartCoroutine(Rotate(0));
        StartCoroutine(RotLight());
    }

    void Update()
    {
        OffsetPosition();
        RotLight();
    }

    void OffsetPosition()
    {
        Vector3 vec = plane.position;
        float planeRot = plane.rotation.eulerAngles.z;
        planeRot += planeRot > 180 ? -360 : 0;
        plane.position = new Vector3(vec.x, Mathf.Abs(planeRot / 180), vec.z);
    }

    IEnumerator Rotate(float lastAngle)
    {
        if(lastAngle > 50) { lastAngle = 0; }
        float possibleAngle = 20;
        float addedAngle = Random.Range(-possibleAngle - (possibleAngle < 0 ? lastAngle : 0),
                                        possibleAngle - (possibleAngle > 0 ? lastAngle : 0));

        Quaternion startingRot = plane.rotation;
        Quaternion finalRot = Quaternion.Euler(startingRot.eulerAngles.x, startingRot.eulerAngles.y, plane.rotation.eulerAngles.z + addedAngle);
        //print(startingRot.eulerAngles + " " + finalRot.eulerAngles);

        float quicken = Mathf.Abs(addedAngle / 20);
        float timeOfRot = Random.Range(4f * quicken * debugSpeedup, 6f * quicken * debugSpeedup);
        float timer = timeOfRot, multiplier = 1 / timeOfRot;

        while(timer > 0)
        {
            timer -= Time.deltaTime;
            plane.rotation = Quaternion.Slerp(startingRot, finalRot, curve.Evaluate(1 - (timer * multiplier)));
            yield return null;
        }

        StartCoroutine(Rotate(addedAngle));
    }
    IEnumerator RotLight()
    {
        Quaternion startingRot = _light.rotation;
        Quaternion finalRot = Quaternion.Euler(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));

        //print(startingRot.eulerAngles + " " + finalRot.eulerAngles);
        //print(Quaternion.Dot(startingRot, finalRot));
        //float quicken = Mathf.Abs(Quaternion.Dot(startingRot, finalRot) * 2);
        float timeOfRot = Random.Range(4f * debugSpeedup, 6f  * debugSpeedup);
        float timer = timeOfRot, multiplier = 1 / timeOfRot;

        while (timer > 0)
        {
            timer -= Time.deltaTime;
            _light.rotation = Quaternion.Slerp(startingRot, finalRot, curve.Evaluate(1 - (timer * multiplier)));
            yield return null;
        }

        StartCoroutine(RotLight());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
