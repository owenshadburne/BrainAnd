using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class MenuStart : MonoBehaviour
{
    [SerializeField] Light _light;
    [SerializeField] Volume vol;
    [SerializeField] GameObject[] toPop;
    [SerializeField] GameObject[] mask;

    [SerializeField] float delay = 7.72f;

    void Start()
    {
        Dissapear(true);
        StartCoroutine(Pop(false));
    }

    void Dissapear(bool alpha)
    {
        foreach(GameObject s in mask)
        {
            s.SetActive(!alpha);
        }

        foreach(GameObject o in toPop)
        {
            foreach(Image i in o.GetComponentsInChildren<Image>())
            {
                i.enabled = !alpha;
            }
            foreach (TextMeshProUGUI i in o.GetComponentsInChildren<TextMeshProUGUI>())
            {
                i.enabled = !alpha;
            }
        }
    }

    public IEnumerator Pop(bool dissapear)
    {
        yield return new WaitForSeconds(delay);

        float og = 0;
        if(vol.profile.TryGet(out Bloom bloom))
        {
            og = bloom.intensity.value;
            bloom.intensity.value = 10000;
        }
        
        if (vol.profile.TryGet(out ChromaticAberration c)) { c.intensity.value = dissapear ? 0.05f : 1; };

        Dissapear(dissapear);

        float timer = .5f;
        float multiplier = 1 / timer;
        while(timer > 0)
        {
            timer -= Time.deltaTime;
            bloom.intensity.value = Mathf.Lerp(og, 10000, timer * multiplier);
            yield return null;
        }
    }
}
