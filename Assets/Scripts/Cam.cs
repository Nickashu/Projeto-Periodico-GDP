using UnityEngine;

public class Cam : MonoBehaviour {
    public Transform player = null;
    private Vector3 follow;
    private float posInitX, posInitY;
    private float leftDistance = 0;     /*Esta variável define o quão a esquerda o personagem estará na câmera*/

    private void Start() {
        posInitX = player.transform.position.x;
        posInitY = player.transform.position.y;
    }

    void FixedUpdate() {
        if (player != null) {
            /*Fazendo a movimentação da câmera*/
            if (player.position.x != posInitX || player.position.y != posInitY) {    /*Verificando se o personagem se moveu*/
                follow = new Vector3(player.position.x + leftDistance, player.position.y, transform.position.z);    /*Esta variável guardará a posição do personagem apenas no eixo x*/
                //transform.position = follow;
                transform.position = Vector3.Lerp(transform.position, follow, 5 * Time.deltaTime);     /*Aqui é definido o que a camerá irá seguir e com qual suavidade*/
            }
        }
    }
}