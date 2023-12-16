INCLUDE Variaveis.ink

{falouLeao1 == true && comidaLeao != 0: -> darComida}
{falouLeao1 == false: -> main | -> jaFalou}

=== main ===
Amigo. Você está vivo!.   #character: gato  #state: gato_surpresa
Grr. Quem está ai? Veio se juntar ao banquete? Isso seria ótimo. Tenho muita, muita fome.  #character: leao  #state: leao_normal
Miau. Amigo? Que bom te ver mais uma vez. Mas acho que é o meu fim. Fuja enquanto há chance. #character: amigo  #state: amigo_normal
Nunca! Eu vim te salvar e não vou embora sem você.   #character: gato  #state: gato_normal
Quanta coragem para um gatinho. Como exatamente você vai fazer isso? Grr. #character: leao  #state: leao_normal
Um acordo. Eu te trago bastante comida e você liberta o meu amigo.   #character: gato  #state: gato_normal
Hum… Comer algo diferente de gato até que não é má ideia. Mas eu tenho muita, muita fome. #character: leao  #state: leao_normal
Os meus termos são esses: Ou você me traz comida boa e rápido ou eu devoro o seu amigo. Não teste a minha paciência. Grrr. Vá logo! #character: leao  #state: leao_normal
~ falouLeao1 = true
-> END

=== jaFalou ===
Os meus termos são esses: Ou você me traz comida boa e rápido ou eu devoro o seu amigo. Não teste a minha paciência. Grrr. Vá logo! #character: leao  #state: leao_normal
-> END


=== darComida ===
  //Doces:
*if (comidaLeao == 1)
  GRRRR! Só isso? Você já viu o meu tamanho? #character: leao  #state: leao_normal  //Penalidade: - x segundos no timer?
  ->DONE
*elseif (comidaLeao == 2)
  GRRRR! Eu sou carnívoro. Eu quero carne! O seu amigo parece apetitoso. #character: leao  #state: leao_normal  //Penalidade: - Penalidade: Gato recebe um slow?
  ->DONE
*elseif (comidaLeao == 3)
  GRRRR!! Você está brincando comigo? Minha paciência tem limites. #character: leao  #state: leao_normal  //Penalidade: - Penalidade: reduzir a barra invés de aumentar?
  ->DONE
  //Carnes:
*elseif (comidaLeao == 4)
  Grr. Carne assada e temperada é ainda melhor que esses gatos crus. Espera… Gatos… temperados? Mmmm. Eu devia ter pedido isso no acordo. #character: leao  #state: leao_normal  //Buff: Gato anda mais rápido?
  ->DONE
*elseif (comidaLeao == 5)
  Grr. Me pergunto que bicho teria sido esse. Eu comeria vários. #character: leao  #state: leao_normal  //Buff: Gato detecta mais carnes?
  ->DONE
*else
  Grr. Isso é muito bom. Mais, por favor. #character: leao  #state: leao_normal  //Buff: gato ganha mais tempo no timer?
  ->DONE
-> END