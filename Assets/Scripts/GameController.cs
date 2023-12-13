using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
    public static List<string> tagsInteractable = new List<string>() { "NPC" };    //Esta lista armazena todas as tags de objetos possíveis de se intergair no jogo
    public static List<string> tagsFoods = new List<string>() { "Donut", "Espeto", "Pipoca", "Racao", "Salsichao", "Sorvete" };    //Esta lista armazena todas as tags de comida
    public static Dictionary<int, string> dicIdFood = new Dictionary<int, string> { { 1, "Donut" }, { 2, "Espeto" }, { 3, "Pipoca" }, { 4, "Racao" }, { 5, "Salsichao" }, { 6, "Sorvete" } };
}
