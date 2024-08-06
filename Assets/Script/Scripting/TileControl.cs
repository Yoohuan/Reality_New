using System.Collections;
using UnityEngine;

public class TileControl : MonoBehaviour
{
    public float changeTime = 1f;
    public float disappearTime = 2f; 
    public float appearTime = 0.5f;
    public int test = 0;
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D target;
    private bool haveBegin = false;

    private Color initialColor;
    private Color fadeTargetColor; 

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        target = GetComponent<BoxCollider2D>();

        initialColor = spriteRenderer.color;


        fadeTargetColor = new Color(initialColor.r, initialColor.g, initialColor.b, 0f);


    }

    private void Update()
    {
        if (!haveBegin && LevelManager.Instance.IsPlaying)
        {
            haveBegin = true;
            StopAllCoroutines();
            StartCoroutine(FadeIn(fadeTargetColor));
        }
        else if(haveBegin && !LevelManager.Instance.IsPlaying)
        {
            haveBegin = false;
            StopAllCoroutines();
        }
        Check();
    }

    private IEnumerator FadeIn(Color targetColor)
    {
        float elapsedTime = 0f;
        while (elapsedTime < changeTime)
        {
            float t = elapsedTime / changeTime/100  ;
            Color newColor = Color.Lerp(spriteRenderer.color, targetColor, t);

            spriteRenderer.color = newColor;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        spriteRenderer.color = targetColor;


        if (targetColor == fadeTargetColor)
        {
            targetColor = new Color(initialColor.r, initialColor.g, initialColor.b, 1f);
            target.enabled = false;
            yield return new WaitForSeconds(disappearTime);
        }

        else
        {
            targetColor = fadeTargetColor;
            target.enabled = true;
            yield return new WaitForSeconds(appearTime);
        }

        StartCoroutine(FadeIn(targetColor));
    }

    private void Check()
    {
        Collider2D collision = Physics2D.OverlapBox(transform.position, target.size, 0f, LayerMask.GetMask("Player"));
        if (collision != null)
        {
            LevelManager.Instance.End(false);
        }
    }
}
