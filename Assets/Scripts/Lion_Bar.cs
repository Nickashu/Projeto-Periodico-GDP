using System.Collections;
using UnityEngine;

public class Lion_Bar : MonoBehaviour {

    private float totalWidth, barWidth;
    public GameObject bar;

    void Start() {
        barWidth = bar.GetComponent<RectTransform>().rect.width;
    }

    public void changeBar(int points) {
        float newBarWidth = barWidth + points;
        if(newBarWidth > totalWidth) {
            Debug.Log("Barra chegou no limite superior");
            newBarWidth = totalWidth;
        }
        else if(newBarWidth < totalWidth) {
            Debug.Log("Barra chegou no limite inferior");
            newBarWidth = 0;
        }

        StartCoroutine(animateBar(barWidth, newBarWidth));
    }

    private IEnumerator animateBar(float initialWidth, float finalWidth) {
        float timePassed = 0f;

        while (timePassed < 1) {
            // Interpola linearmente entre a largura inicial e a largura final
            float currentWidth = Mathf.Lerp(initialWidth, finalWidth, timePassed / 1);

            // Atualiza a largura do objeto
            bar.GetComponent<RectTransform>().sizeDelta = new Vector2(currentWidth, bar.GetComponent<RectTransform>().sizeDelta.y);

            // Incrementa o tempo decorrido
            timePassed += Time.deltaTime;

            yield return null; // Aguarda o próximo quadro
        }

        // Garante que a largura seja definida precisamente para o valor final
        bar.GetComponent<RectTransform>().sizeDelta = new Vector2(finalWidth, bar.GetComponent<RectTransform>().sizeDelta.y);
        barWidth = finalWidth;
    }
}
