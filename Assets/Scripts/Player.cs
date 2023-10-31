using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour {

    private Rigidbody2D rb;
    private Vector2 movementVector;
    private float speed = 5f;
    private bool isRunning = false;

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
    }

    public void SetMovement(InputAction.CallbackContext value) {
        movementVector = value.ReadValue<Vector2>();
    }

    private void FixedUpdate() {
        if (!DialogueController.GetInstance().dialogueActive) {    //Se não estiver no meio de um diálogo
            Vector2 vectorResult = new Vector2(movementVector.x, movementVector.y).normalized;
            rb.velocity = (vectorResult * speed);
        }
    }

}
