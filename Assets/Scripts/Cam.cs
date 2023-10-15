using UnityEngine;

public class Cam : MonoBehaviour {
    public Transform player = null;
    private Vector3 follow;
    private float posInitX, posInitY;
    private float leftDistance = 0;     /*Esta vari�vel define o qu�o a esquerda o personagem estar� na c�mera*/

    private void Start() {
        posInitX = player.transform.position.x;
        posInitY = player.transform.position.y;
    }

    void FixedUpdate() {
        if (player != null) {
            /*Fazendo a movimenta��o da c�mera*/
            if (player.position.x != posInitX || player.position.y != posInitY) {    /*Verificando se o personagem se moveu*/
                follow = new Vector3(player.position.x + leftDistance, player.position.y, transform.position.z);    /*Esta vari�vel guardar� a posi��o do personagem apenas no eixo x*/
                //transform.position = follow;
                transform.position = Vector3.Lerp(transform.position, follow, 5 * Time.deltaTime);     /*Aqui � definido o que a camer� ir� seguir e com qual suavidade*/
            }
        }
    }
}