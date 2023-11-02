using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour {

    private Rigidbody2D rb;
    private Vector2 movementVector;
    private DialogueTrigger triggerDialogue;
    private float speed = 5f;
    private bool isRunning = false;
    private int contInteracoes = 0;

    public TextMeshProUGUI txtTutorialInteractions;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update() {
        /*Mec�nica de corrida*/
        if (Input.GetKey(KeyCode.LeftShift)) {
            if (!isRunning)
                speed *= 2;
            isRunning = true;
        }
        else {
            isRunning = false;
            speed = 5;
        }

        if (triggerDialogue != null) {
            if (Input.GetKey(KeyCode.Z) && !DialogueController.GetInstance().dialogueActive) {
                if (contInteracoes == 0) {
                    contInteracoes++;
                    txtTutorialInteractions.gameObject.SetActive(false);
                }
                triggerDialogue.TriggerDialogue();
            }
        }

    }

    public void SetMovement(InputAction.CallbackContext value) {
        movementVector = value.ReadValue<Vector2>();
    }

    private void FixedUpdate() {
        Vector2 vectorResult = new Vector2(0, 0);
        if (!DialogueController.GetInstance().dialogueActive)    //Se n�o estiver no meio de um di�logo
            vectorResult = new Vector2(movementVector.x, movementVector.y).normalized;

        rb.velocity = (vectorResult * speed);
    }

    //Fun��es para checar se o player se aproximou o suficiente de um NPC para acionar o di�logo:
    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.tag == "NPC") {    //Aqui ser� detectado quando a hitbox de di�logo enconstar no NPC
            if (contInteracoes == 0)
                txtTutorialInteractions.gameObject.SetActive(true);
            triggerDialogue = collision.gameObject.GetComponent<DialogueTrigger>();
            Debug.Log("Entrou hitbox di�logo");
        }

        if (collision.tag == "Item") {    //Aqui ser� detectado quando o player tocar no item, j� que foi configurado para que n�o haja detec��o entre colis�o do hitbox de di�logo com itens
            
            if (collision.gameObject.name == "comidaCorvo")
                DialogueController.GetInstance().dialogueVariablesController.ChangeSpecificVariable("updateComidaCorvo");


            collision.gameObject.SetActive(false);
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.tag == "NPC") {
            if (contInteracoes == 0)
                txtTutorialInteractions.gameObject.SetActive(false);
            triggerDialogue = null;
            Debug.Log("Saiu hitbox di�logo");
        }
    }
}
