using Ink.Runtime;
using System.Collections.Generic;
using UnityEngine;

public class DialogueVariablesController {

    public Dictionary<string, Ink.Runtime.Object> variablesValues { get; private set; }    //Este dicion�rio conter�, aqui no c�digo, os valores de todas as vari�veis presentes no arquivo de vari�veis do ink

    public DialogueVariablesController(TextAsset variablesJSON) {    //Aqui no construtor � onde vamos inicializar o dicion�rio para termos todas as vari�veis criadas no ink
        Story dialogueOfVariables = new Story(variablesJSON.text);
        variablesValues = new Dictionary<string, Ink.Runtime.Object>();

        foreach(string varName in dialogueOfVariables.variablesState) {
            Ink.Runtime.Object varValue = dialogueOfVariables.variablesState.GetVariableWithName(varName);   //Aqui estou pegando o valor da vari�vel no arquivo ink
            variablesValues.Add(varName, varValue);
        }
    }

    public void StartListening(Story dialogue) {   //Esta fun��o vai ser respons�vel por checar em todo momento durante o di�logo as mudan�as de vari�veis
        LoadDictionaryToInk(dialogue);
        dialogue.variablesState.variableChangedEvent += ChangeVariables;   //A fun��o ChangeVariables ser� chamada a cada vez que for detectada uma mudan�a de vari�vel no di�logo
    }

    public void StopListening(Story dialogue) {   //Esta fun��o vai ser respons�vel por parar a checagem de mudan�as de vari�veis (ser� chamada quando o di�logo chegar ao fim)
        dialogue.variablesState.variableChangedEvent -= ChangeVariables;
    }


    private void ChangeVariables(string variableName, Ink.Runtime.Object variableValue) {   //Por ser uma sobrecarga de outro m�todo do Ink, este m�todo precisa ter exatamente este esqueleto. Ele ser� respons�vel por atualizar os valores das vari�vies no dicion�rio
        if(variablesValues.ContainsKey(variableName))
            variablesValues[variableName] = variableValue;
    }

    private void LoadDictionaryToInk(Story dialogue) {   //Esta fun��o ser� respons�vel por jogar as vari�veis devidamente atualizadas no arquivo de vari�veis do ink (� chamada quando o di�logo se inicia)
        foreach(KeyValuePair<string, Ink.Runtime.Object> variable in variablesValues) {
            dialogue.variablesState.SetGlobal(variable.Key, variable.Value);
        }
    }
}
