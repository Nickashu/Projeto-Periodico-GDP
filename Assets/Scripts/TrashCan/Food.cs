using UnityEngine;

public class Food : MonoBehaviour {
    private void endAnimationFood() {   //Quando a animação da comida saindo da lixeira acabar, a tag e layer "Item" serão adicionadas ao objeto
        gameObject.tag = "Item";
        gameObject.layer = LayerMask.NameToLayer("Item");
    }
}
