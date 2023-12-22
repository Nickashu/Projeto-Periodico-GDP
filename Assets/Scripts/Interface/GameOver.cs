using UnityEngine;

public class GameOver : MonoBehaviour {

    //M�todos que ser�o ativados depois de algum bot�o da tela de game over ser pressionado
    public void ReturnToMenu() {
        TransitionsController.GetInstance().LoadNextScene();
    }
    public void RestartGame() {
        TransitionsController.GetInstance().LoadMainScene();
    }
    public void QuitGame() {
        Application.Quit();
    }
}
