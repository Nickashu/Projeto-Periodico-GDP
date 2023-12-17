using Ink.Runtime;
using System.Collections.Generic;
using UnityEngine;

public class DialogueVariablesController {

    public Dictionary<string, Ink.Runtime.Object> variablesValues { get; private set; }    //Este dicionário conterá, aqui no código, os valores de todas as variáveis presentes no arquivo de variáveis do ink
    public Story dialogueOfVariables { get; private set; }

    public DialogueVariablesController(TextAsset variablesJSON) {    //Aqui no construtor é onde vamos inicializar o dicionário para termos todas as variáveis criadas no ink
        dialogueOfVariables = new Story(variablesJSON.text);
        variablesValues = new Dictionary<string, Ink.Runtime.Object>();

        foreach(string varName in dialogueOfVariables.variablesState) {
            Ink.Runtime.Object varValue = dialogueOfVariables.variablesState.GetVariableWithName(varName);   //Aqui estou pegando o valor da variável no arquivo ink
            variablesValues.Add(varName, varValue);
        }
    }

    public void StartListening(Story dialogue) {   //Esta função vai ser responsável por checar em todo momento durante o diálogo as mudanças de variáveis
        LoadDictionaryToInk(dialogue);
        dialogue.variablesState.variableChangedEvent += ChangeVariables;   //A função ChangeVariables será chamada a cada vez que for detectada uma mudança de variável no diálogo
    }

    public void StopListening(Story dialogue) {   //Esta função vai ser responsável por parar a checagem de mudanças de variáveis (será chamada quando o diálogo chegar ao fim)
        dialogue.variablesState.variableChangedEvent -= ChangeVariables;
    }


    private void ChangeVariables(string variableName, Ink.Runtime.Object variableValue) {   //Por ser uma sobrecarga de outro método do Ink, este método precisa ter exatamente este esqueleto. Ele será responsável por atualizar os valores das variávies no dicionário
        if(variablesValues.ContainsKey(variableName))
            variablesValues[variableName] = variableValue;
    }

    private void LoadDictionaryToInk(Story dialogue) {   //Esta função será responsável por jogar as variáveis devidamente atualizadas no arquivo de variáveis do ink (é chamada quando o diálogo se inicia)
        foreach(KeyValuePair<string, Ink.Runtime.Object> variable in variablesValues) {
            dialogue.variablesState.SetGlobal(variable.Key, variable.Value);
        }
    }


    public void ChangeSpecificVariable(string nameInkFunction, object argument=null) {    //Este método será chamado se eu quiser alterar uma variável específica após uma certa ação durante o jogo
        StartListening(dialogueOfVariables);
        //dialogueOfVariables.ChoosePathString(nameInkFunction);
        //while (dialogueOfVariables.canContinue)
        //    dialogueOfVariables.Continue();

        //Sei que argument só será bool ou int:
        bool boolValue = false;
        int intValue = 0;
        if(argument is bool)
            boolValue = (bool)argument;
        else if(argument is int)
            intValue = (int)argument;
        if(argument != null)
            dialogueOfVariables.EvaluateFunction(nameInkFunction, argument);
        else
            dialogueOfVariables.EvaluateFunction(nameInkFunction);
        StopListening(dialogueOfVariables);
    }

    public void CheckVariableValues() {    //Este método será usado para debug
        foreach (KeyValuePair<string, Ink.Runtime.Object> variable in variablesValues) {
            Debug.Log("Variável: " + variable.Key + "   Valor: " + variable.Value);
        }
    }
}
