using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class MenuButtons : MonoBehaviour
{
    [SerializeField] ParticleSystem particle;
    [SerializeField] MenuStart ms;
    StartupPop sp;
    [SerializeField] GameObject[] mask;

    [SerializeField] GameObject guide;
    [SerializeField] Volume vol;

    private void Start()
    {
        sp = GetComponent<StartupPop>();
    }

    public void PlayGame()
    {
        //Unpop All Items and Alpha particles and disable masks
        sp.Activate(false, 0, 2);
        foreach(GameObject o in mask) { o.SetActive(false); }
        StartCoroutine(Wait());
    }

    IEnumerator Wait()
    {
        float timer = 2;
        float multiplier = 1 / timer;

        Color start = particle.trails.colorOverTrail.colorMax;
        Color end = new Color(start.r, start.g, start.b, 0);
        ParticleSystem.TrailModule trl = particle.trails;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            trl.colorOverTrail = new ParticleSystem.MinMaxGradient(Color.Lerp(start, end, 1 - timer * multiplier));
            yield return null;
        }
        //yield return new WaitForSeconds(1);
        SceneManager.LoadScene("Game");
    }

    public void Guide()
    {
        if (vol.profile.TryGet(out ChromaticAberration c)) { c.intensity.value = 0.05f; };
        Instantiate(guide, GameObject.FindGameObjectWithTag("Canvas").transform);
    }

    public void ResetAllData()
    {
        PlayerPrefs.DeleteAll();
    }

    public void Back(GameObject o)
    {
        if (Camera.main.GetComponent<Volume>().profile.TryGet(out ChromaticAberration c)) { c.intensity.value = 1f; };
        Destroy(o);
    }

    public void Uncover(GameObject o)
    {
        TextMeshProUGUI[] t = o.GetComponentsInChildren<TextMeshProUGUI>();

        foreach(TextMeshProUGUI text in t)
        {
            text.enabled = !text.enabled;
        }
    }
}
