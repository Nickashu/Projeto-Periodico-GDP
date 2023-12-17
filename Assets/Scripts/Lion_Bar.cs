using UnityEngine;

public class Lion_Bar : MonoBehaviour {

    private float totalWidth, barWidth;
    public GameObject bar;

    void Start() {
        barWidth = bar.GetComponent<RectTransform>().rect.width;
    }

    void Update() {
        
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

    }
}
