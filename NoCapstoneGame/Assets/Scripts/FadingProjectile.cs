using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadingProjectile : Projectile
{
    [Header("Stats")]
    [SerializeField] protected AnimationCurve fadeCurve;

    float startAlpha;

    protected override void Start()
    {
        base.Start();
        startAlpha = projectileRenderer.color.a;
    }


    // Update is called once per frame
    override protected void Update()
    {
        base.Update();
        float alpha = UpdateAlpha();
        if (alpha <= 0.01)
        {
            Destroy(this.gameObject);
        }
    }

    virtual protected float UpdateAlpha()
    {
        float currentTime = Time.time;
        float alpha = fadeCurve.Evaluate(currentTime - startTime) * startAlpha;
        Debug.Log(fadeCurve.Evaluate(currentTime - startTime));
        Debug.Log(startAlpha);
        Debug.Log(alpha);
        projectileRenderer.color = new Color(projectileRenderer.color.r, projectileRenderer.color.g, alpha);
        Debug.Log(projectileRenderer.color.a);
        return projectileRenderer.color.a;
    }
}
