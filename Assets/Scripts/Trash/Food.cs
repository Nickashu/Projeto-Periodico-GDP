using UnityEngine;

public class Food : MonoBehaviour {
    private void endAnimationFood() {   //Quando a anima��o da comida saindo da lixeira acabar, a tag "Item" ser� adicionada ao objeto
        gameObject.tag = "Item";
    }
}
