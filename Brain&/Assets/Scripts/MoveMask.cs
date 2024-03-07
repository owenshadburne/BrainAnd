using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveMask : MonoBehaviour
{
    [SerializeField] Transform spriteMask;
    Transform sm2, sm3;
    RectTransform rect;
    [SerializeField] 

    private void Start()
    {
        rect = GetComponent<RectTransform>();
        spriteMask.rotation = Quaternion.Euler(0, 0, 90);

        sm2 = Instantiate(spriteMask, spriteMask.position, Quaternion.identity, spriteMask.parent);
        sm3 = Instantiate(spriteMask, spriteMask.position, Quaternion.identity, spriteMask.parent);
    }

    void Update()
    {

        RectTransformUtility.ScreenPointToWorldPointInRectangle(rect, rect.position, Camera.main, out Vector3 pos);
        spriteMask.position = pos;
        sm2.position = new Vector3(pos.x - rect.sizeDelta.x / Screen.width, pos.y, pos.z);
        sm3.position = new Vector3(pos.x + rect.sizeDelta.x / Screen.width, pos.y, pos.z);

        float distance = Vector3.Distance(Camera.main.transform.position, spriteMask.transform.position);
        float width = rect.sizeDelta.y * Screen.width / Screen.height;
        float height = rect.sizeDelta.x * Mathf.Tan(0.5f * Camera.main.fieldOfView * Mathf.Deg2Rad) * distance;
        Vector3 scale = new Vector3(width / 55f, height / 320, 1.0f);
        spriteMask.transform.localScale = scale;
    }
}
