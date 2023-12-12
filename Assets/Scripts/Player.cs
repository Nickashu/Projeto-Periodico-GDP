using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour {

    private Rigidbody2D rb;
    private Animator anim;
    private Vector2 movementVector;
    private DialogueTrigger triggerDialogue;
    private Trash scriptTrash;

    private float speed = 3.5f;
    private bool isRunning = false;
    private int contInteracoes = 0, limitInteractionsTutorial=5;

    public TextMeshProUGUI txtTutorialInteractions;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
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
            speed = 3.5f;
        }

        if (!DialogueController.GetInstance().dialogueActive) {    //Se o gato não estiver no meio de um diálogo
            if (Input.GetKeyDown(KeyCode.Z)) {
                if (triggerDialogue != null) {
                    if (contInteracoes <= limitInteractionsTutorial) {
                        contInteracoes++;
                        txtTutorialInteractions.gameObject.SetActive(false);
                    }
                    triggerDialogue.TriggerDialogue();
                }
            }

            if (Input.GetKey(KeyCode.Z)) {
                if (scriptTrash != null)
                    scriptTrash.startSearchingFood();
            }

            if (Input.GetKeyUp(KeyCode.Z)) {
                if (scriptTrash != null)
                    scriptTrash.stopSearchingFood();
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

        //Animações do gato andando e correndo:
        if (movementVector.x != 0 || movementVector.y != 0)
            anim.SetFloat("Speed", 5);
        else
            anim.SetFloat("Speed", 0);
        anim.SetBool("isRunning", isRunning);
        anim.SetFloat("Horizontal", movementVector.x);
        anim.SetFloat("Vertical", movementVector.y);
        Debug.Log("X: " + movementVector.x + "  Y: " + movementVector.y);
    }

    //Funções para checar se o player se aproximou o suficiente de um NPC para acionar o diálogo:
    private void OnTriggerEnter2D(Collider2D collision) {
        if (GameController.tagsInteractable.Contains(collision.tag)) {
            Debug.Log("Entrou na hitbox de interação");
            if (contInteracoes <= limitInteractionsTutorial)
                txtTutorialInteractions.gameObject.SetActive(true);
        }

        if(collision.tag == "NPC")    //Aqui será detectado quando a hitbox de interação do player enconstar no NPC
            triggerDialogue = collision.gameObject.GetComponent<DialogueTrigger>();

        if (collision.tag == "TrashClosed")    //Aqui será detectado quando a hitbox de interação do player encostar em uma lixeira
            scriptTrash = collision.gameObject.GetComponent<Trash>();

        if (collision.tag == "Item") {    //Aqui será detectado quando o player tocar no item, já que foi configurado para que não haja detecção entre colisão do hitbox de diálogo com itens 
            if (collision.gameObject.name == "comidaCorvo")
                DialogueController.GetInstance().dialogueVariablesController.ChangeSpecificVariable("updateComidaCorvo");

            Destroy(collision.gameObject);
            Debug.Log("Tocou comida!!");
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (GameController.tagsInteractable.Contains(collision.tag)) {
            if (contInteracoes <= limitInteractionsTutorial)
                txtTutorialInteractions.gameObject.SetActive(false);
            Debug.Log("Saiu da hitbox de interação");
        }
        if (collision.tag == "NPC")
            triggerDialogue = null;
        if (collision.tag == "TrashClosed") {
            scriptTrash.stopSearchingFood();
            scriptTrash = null;
        }
    }
}
