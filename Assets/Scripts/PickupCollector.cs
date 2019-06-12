using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PickupCollector : MonoBehaviour
{
    [SerializeField] GameObject hud;
    [SerializeField] Text NormalScoreText;
    [SerializeField] Text BonusScoreText;
    //   [SerializeField] GameObject canvas;
    [SerializeField] GameObject iconIndicator;
    [SerializeField] float indicatorOffsetX;
    [SerializeField] float indicatorOffsetY;
    [SerializeField] float fadeInSpeed;
    [SerializeField] float fadeOutSpeed;
    [SerializeField] int extraPointsPerDistanceUnit;

    int _bonusScore;
    int _normalScore;

    private void OnEnable()
    {
        PathDrawer.OnPathStarted += CalculatePickupDistances;
    }

    private void OnDisable()
    {
        PathDrawer.OnPathStarted -= CalculatePickupDistances;

    }

    private void CalculatePickupDistances()
    {
        var pickups = FindObjectsOfType<Pickup>();

        foreach (var pickup in pickups)
        {
            pickup.Distance = Vector3.Distance(pickup.transform.position, transform.position);
        }



    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Pickup>())
        {
            //print("got " + (int)other.GetComponent<Pickup>().Distance * extraPointsPerDistanceUnit + " in bonus");

            _bonusScore = (int)other.GetComponent<Pickup>().Distance * extraPointsPerDistanceUnit;
            _normalScore = other.GetComponent<Pickup>().Score;
            ScoreSystem.currentScore += _normalScore  + _bonusScore;
            Destroy(other.gameObject);
            //SpawnIcon();
            ShowHUD();
        }
    }

    void ShowHUD()
    {
        hud.GetComponent<Image>().enabled = true;
        foreach (Transform child in hud.transform)
        {
            child.gameObject.GetComponent<Text>().enabled = true;
        }

        NormalScoreText.text = _normalScore.ToString();
        BonusScoreText.text =   _bonusScore.ToString();


        StartCoroutine("IndicatorPopup");

    }

    void SpawnIcon()
    {
        GameObject icon = Instantiate(iconIndicator);
        icon.transform.position = new Vector3(this.transform.position.x + indicatorOffsetX, this.transform.position.y + indicatorOffsetY, this.transform.position.z);

        icon.transform.SetParent(this.transform);

        StartCoroutine("FadeIn", icon);
        //StartCoroutine("FadeOut", icon);

        //StartCoroutine("IndicatorPopup", icon);
    }


    void HideHUD()
    {
        hud.GetComponent<Image>().enabled = false;
        foreach (Transform child in hud.transform)
        {
            child.gameObject.GetComponent<Text>().enabled = false;
        }
    }

    IEnumerator IndicatorPopup()
    {
        yield return new WaitForSeconds(2.0f);
        HideHUD();
        //Destroy(icon, 2.0f);
    }

    IEnumerator FadeIn(GameObject icon)
    {
        float alpha = icon.GetComponent<SpriteRenderer>().color.a;

        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / fadeInSpeed)
        {
            Color newColor = new Color(1, 1, 1, Mathf.Lerp(alpha, 1.0f, t));
            icon.GetComponent<SpriteRenderer>().color = newColor;

            yield return null;  
        }

        Destroy(icon, 2.0f);




    }
    IEnumerator FadeOut(GameObject icon)
    {
        float alpha = icon.GetComponent<SpriteRenderer>().color.a;
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / fadeInSpeed)
        {
            Color newColor = new Color(1, 1, 1, Mathf.Lerp(alpha, 0.0f, -t));
            icon.GetComponent<SpriteRenderer>().color = newColor;

            yield return null;
        }
        


    }

}


