INCLUDE Variaveis.ink

{temComidaCorvo == true && falouCorvo1 == true: -> darComida}
{falouCorvo1 == false: -> main | -> jaFalou}

=== main ===
Olá senhor! Sim, tu que me olhas, felino da noite.   #character: corvo  #state: corvo_normal
Miau. Quem é você? Pode me ajudar?  #character: gato  #state: gato_normal
Apenas um corvo. Nada mais importa. O que te aborrece, caro amigo?   #character: corvo  #state: corvo_normal
Estou a procura de um amigo, você o viu por aí? #character: gato  #state: gato_normal
Um amigo felino, estás a perguntar? Acho que posso ter cruzado meus olhos com os dele por um breve momento antes da Noite Eterna.   #character: corvo  #state: corvo_normal
Noite Eterna? O que você quer dizer com isso? #character: gato  #state: gato_normal
Basta! Tão eterna quanto a Noite Eterna, é a minha fome. Traga-me uma das delícias mundanas desperdiçadas e tornamos a conversa. Tão singulares… mas ainda assim, jazem na gaiola de prata. Agora vá! Vá que eu tenho fome!   #character: corvo  #state: corvo_normal
~ falouCorvo1 = true
//(Faz o puzzle e retorna  com o salsichão. Corvo desce e se comunica mais claramente)
-> END

=== jaFalou ===
Noite Eterna? O que você quer dizer com isso? #character: gato  #state: gato_normal
Basta! Tão eterna quanto a Noite Eterna, é a minha fome. Traga-me uma das delícias mundanas desperdiçadas e tornamos a conversa. Tão singulares… mas ainda assim, jazem na gaiola de prata. Agora vá! Vá que eu tenho fome!   #character: corvo  #state: corvo_normal
-> END

=== darComida ===
Cuá. Agora sim! Devidamente alimentado. Esses espetos são uma maravilha.   #character: corvo  #state: corvo_normal
Então, o que você estava falando sobre a Noite Eterna? #character: gato  #state: gato_normal
Ah! A terrível escuridão que um dia abraça a todos nós. Para sempre. Temo que seu amigo tenha completado a travessia. Mas, se tiver sorte, ele pode ainda vagar por aí. Quem sabe.   #character: corvo  #state: corvo_normal
Você quer dizer… que ele está morto? #character: gato  #state: gato_normal
Depende do que consideras morto. Ah, não desanimes. Pode ser que seu amigo ainda esteja nesse plano. Há uma chance, mas eu não iria atrás dele se fosse você. Aproveite sua liberdade e saia o quanto antes desse terrível lugar.   #character: corvo  #state: corvo_normal
//Nesse momento, o jogador tem a escolha de fugir ou insistir.
Quero encontrar meu amigo. Onde você o viu?   #character: gato  #state: gato_normal
Hmmm ou és ousado ou completamente louco. O caminho que tu procuras é para lá, após a grande gaiola branca e vermelha. Não digas que eu não te avisei. Cuá. Cuá. #character: corvo  #state: corvo_normal
~ terminouDeFalarCorvo = true
-> END