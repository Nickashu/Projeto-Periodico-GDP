VAR falouCorvo1 = false
VAR terminouDeFalarCorvo1 = false
VAR comidaCorvo=0

VAR falouLeao1 = false
VAR comidaLeao = 0
VAR numComidasLeao = 0
VAR leaoSatisfeito = false


=== function updateComida(intComida) ===
~comidaLeao = intComida
~comidaCorvo = intComida


=== function matouFomeLeao() ===
~leaoSatisfeito = true


=== function resetVariables() ===
~falouCorvo1 = false
~terminouDeFalarCorvo1 = false
~comidaCorvo=0
~falouLeao1 = false
~comidaLeao = 0
~numComidasLeao = 0
~leaoSatisfeito = false