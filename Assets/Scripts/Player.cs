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
        /*Mecânica de corrida*/
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
        if (!DialogueController.GetInstance().dialogueActive)    //Se não estiver no meio de um diálogo
            vectorResult = new Vector2(movementVector.x, movementVector.y).normalized;

        rb.velocity = (vectorResult * speed);
    }

    //Funções para checar se o player se aproximou o suficiente de um NPC para acionar o diálogo:
    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.tag == "NPC") {    //Aqui será detectado quando a hitbox de diálogo enconstar no NPC
            if (contInteracoes == 0)
                txtTutorialInteractions.gameObject.SetActive(true);
            triggerDialogue = collision.gameObject.GetComponent<DialogueTrigger>();
            Debug.Log("Entrou hitbox diálogo");
        }

        if (collision.tag == "Item") {    //Aqui será detectado quando o player tocar no item, já que foi configurado para que não haja detecção entre colisão do hitbox de diálogo com itens
            
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
            Debug.Log("Saiu hitbox diálogo");
        }
    }
}
