using UnityEngine;

public class BGTransitionAfterCaught : MonoBehaviour {
    public GameObject player;

    private void repositionPlayer() {
        player.gameObject.GetComponent<Player>().returnToInitialPosition();   //Fazendo o player retornar para o in�cio do mapa durante a anima��o da tela de transi��o
        GameController.gamePaused = false;
    }
}
