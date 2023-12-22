using UnityEngine;

public class GameOver : MonoBehaviour {

    //Métodos que serão ativados depois de algum botão da tela de game over ser pressionado
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
