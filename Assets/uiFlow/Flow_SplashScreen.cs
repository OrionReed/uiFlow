using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class Flow_SplashScreen : MonoBehaviour
{
    [SerializeField]
    float Delay;
    [SerializeField]
    bool SkipWithInput;
    [SerializeField]
    bool SkipInEditor;
    [HideInInspector]
    public List<Flow_SplashScreenInfo> Screens = new List<Flow_SplashScreenInfo>();

    RawImage splashImage;
    GameObject splashScreen;
    GameObject canvas;
    Coroutine co;

    void Start()
    {
        if (!SkipInEditor)
        {
            CreateSplashScreen();
            co = StartCoroutine(RunSplashScreen(splashImage));
        }
    }

    void Update()
    {
        if (Input.anyKeyDown && SkipWithInput)
        {
            StopCoroutine(co);
            StartCoroutine(ExitSplashScreen(splashImage, Color.black, 0, 1));
        }
    }

    void CreateSplashScreen()
    {
        canvas = new GameObject("_SplashCanvas");
        canvas.transform.SetParent(this.transform);
        canvas.AddComponent<Canvas>();
        canvas.AddComponent<CanvasScaler>();
        canvas.AddComponent<GraphicRaycaster>();
        canvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;

        splashScreen = new GameObject("_SplashScreen", typeof(RawImage));
        splashScreen.transform.SetParent(canvas.transform, false);

        splashImage = splashScreen.GetComponent<RawImage>();
    }

    IEnumerator RunSplashScreen(RawImage r)
    {

        for (int i = 0; i < Screens.Count; i++)
        {
            r.texture = Screens[i].Image;
            r.color = new Color(r.color.r, r.color.g, r.color.b, 0);
            r.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width, Screen.height);

            yield return new WaitForSeconds(Delay);

            for (float alpha = 0; alpha < 1; alpha += Time.deltaTime / Screens[i].In)
            {
                r.color = new Color(r.color.r, r.color.g, r.color.b, alpha);
                yield return null;
            }

            yield return new WaitForSeconds(Screens[i].Sustain);

            for (float alpha = 1; alpha > 0; alpha -= Time.deltaTime / Screens[i].Out)
            {
                r.color = new Color(r.color.r, r.color.g, r.color.b, alpha);
                yield return null;
            }
        }
        StartCoroutine(ExitSplashScreen(r, Color.black, 1, 1));
    }

    IEnumerator ExitSplashScreen(RawImage r, Color color, float Sustain, float Out)
    {
        yield return new WaitForSeconds(Sustain);
        r.texture = null;
        r.color = color;

        for (float alpha = 1; alpha > 0; alpha -= Time.deltaTime / Out)
        {
            r.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }
        DeactivateSplashScreen();
    }

    void DeactivateSplashScreen()
    {
        canvas.gameObject.SetActive(false);
        this.enabled = false;
    }
}