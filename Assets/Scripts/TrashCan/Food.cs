using UnityEngine;

public class Food : MonoBehaviour {
    private void endAnimationFood() {   //Quando a anima��o da comida saindo da lixeira acabar, a tag e layer "Item" ser�o adicionadas ao objeto
        gameObject.tag = "Item";
        gameObject.layer = LayerMask.NameToLayer("Item");
    }
}
