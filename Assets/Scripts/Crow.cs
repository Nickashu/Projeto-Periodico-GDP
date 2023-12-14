using System.Collections;
using UnityEngine;

public class Crow : MonoBehaviour {

    private Animator anim;
    private SpriteRenderer spriteRenderer;
    public GameObject player;
    private bool isJumping = false;
    private float offSetPlayer = 1.5f;
    public int crowVersion;    //Esta variável vai representar qual é o ponto do jogo em que este corvo foi carregado


    void Start() {
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update() {
        if (!isJumping) {
            isJumping = true;
            StartCoroutine(jump());
        }
        //Fazendo o corvo ficar virado para onde o jogador estiver:
        if (transform.position.x > player.transform.position.x + offSetPlayer)
            spriteRenderer.flipX = true;
        else if(transform.position.x < player.transform.position.x - offSetPlayer)
            spriteRenderer.flipX = false;


        //Verificando se o player já deu a comida para o corvo:
        if (((Ink.Runtime.BoolValue)DialogueController.GetInstance().GetVariableState("terminouDeFalarCorvo")).value == true && crowVersion == 1)
            anim.SetBool("fly", true);
    }

    private IEnumerator jump() {
        anim.SetBool("jump", true);
        yield return new WaitForSeconds(3.0f);   //Esperando 5 segundos
        isJumping = false;
    }
    
    private void endJumpAnimation() {
        anim.SetBool("jump", false);
    }

    private void endFlyAnimation() {
        Destroy(transform.gameObject);
    }
}
