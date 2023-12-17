using System.Collections;
using TMPro;
using UnityEngine;

public class Lion_Bar : MonoBehaviour {

    private float totalWidth, barWidth, durationAnimationBar=1f;
    public GameObject bar, pointsFood;

    void Start() {
        barWidth = bar.GetComponent<RectTransform>().rect.width;
        totalWidth = gameObject.GetComponent<RectTransform>().rect.width;
    }

    public void changeBar(int points) {
        float newBarWidth = barWidth + points;
        GameController.changingLionBar = false;
        Debug.Log(newBarWidth);
        if(newBarWidth > totalWidth) {
            Debug.Log("Barra chegou no limite superior");
            newBarWidth = totalWidth;
        }
        else if(newBarWidth < 0) {
            Debug.Log("Barra chegou no limite inferior");
            newBarWidth = 0;
        }

        StartCoroutine(animateBar(barWidth, newBarWidth));
        if (points > 0) {
            pointsFood.GetComponent<TextMeshProUGUI>().color = Color.green;
            pointsFood.GetComponent<TextMeshProUGUI>().text = "+ ";
        }
        else if (points < 0) {
            pointsFood.GetComponent<TextMeshProUGUI>().color = Color.red;
            pointsFood.GetComponent<TextMeshProUGUI>().text = "- ";
        }
        else {
            pointsFood.GetComponent<TextMeshProUGUI>().color = Color.white;
            pointsFood.GetComponent<TextMeshProUGUI>().text = "+ ";
        }
        pointsFood.GetComponent<TextMeshProUGUI>().text += Mathf.Abs(points).ToString();
        pointsFood.SetActive(true);
    }

    private IEnumerator animateBar(float initialWidth, float finalWidth) {
        float timePassed = 0f;

        while (timePassed < durationAnimationBar) {
            float currentWidth = Mathf.Lerp(initialWidth, finalWidth, timePassed / durationAnimationBar);
            bar.GetComponent<RectTransform>().sizeDelta = new Vector2(currentWidth, bar.GetComponent<RectTransform>().sizeDelta.y);
            timePassed += Time.deltaTime;
            yield return null;
        }

        StopAllCoroutines();
        bar.GetComponent<RectTransform>().sizeDelta = new Vector2(finalWidth, bar.GetComponent<RectTransform>().sizeDelta.y);
        barWidth = finalWidth;
    }
}
