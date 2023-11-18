using UnityEngine;

public class Food : MonoBehaviour {
    private void endAnimationFood() {   //Quando a animação da comida saindo da lixeira acabar, a tag "Item" será adicionada ao objeto
        gameObject.tag = "Item";
    }
}
