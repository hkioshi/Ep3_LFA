// using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;




public class Pilha
{
    Stack<string> stack = new Stack<string>();
    public void Colocar(string s)
    {
        Console.WriteLine("empilhou: " + s);
        stack.Push(s);
    } // Vai colocar na pilha e mensagem
    public void Recolocar(string s)
    {
        stack.Push(s);
    }//Recolococar o que foi tirado, sem mensagens
    public string Tirar(ref string tirado)
    {
        if (stack.Count > 0) tirado = stack.Pop();
        else tirado = null;
        return tirado;
    }//Tirar primeiro da pilha
}
public class Json
{

    string key = "", objeto = "";// objeto e key

    Pilha pilha = new Pilha();// pilha instaciada
    string inteiro = "", character = "", topo = "";// aq ficam armazenadas as parte que falta processar, o caracter sendo processado e, o topo da pilha
    string estado = "q0";// estado
    bool f = true;// flag pra controlar o while
    string tipo= "";

    List<(string,string, string)> lista_de_Objetos = new List<(string, string, string)>();// aq ficam armazenados as tuplas(key,Objeto)
    Regex letra = new Regex(@"[a-zA-Z0-9]", RegexOptions.IgnorePatternWhitespace);
    Regex numero = new Regex(@"\d+(\.\d+)?");
    void Proximo(ref string inteiro, ref string topo)//refs sao ponteiros, coloquei ai pra retornar mais valores
    {
        if (inteiro.Substring(1) != "")
        {
            topo = inteiro[0].ToString(); // Obtém o primeiro caractere
            inteiro = inteiro.Substring(1); // Obtém a string sem o primeiro caractere
        }
        else
        {
            topo = inteiro[0].ToString(); // Obtém o primeiro caractere
        }
    }

    

    void VerificarNumero(string key, ref bool f)
    {
        if (!numero.IsMatch(key))
        {
            Console.WriteLine("Rejeitado - Nao é um numero");
            f = false;
        }
    }

    void Adicionar()
    {
        
        lista_de_Objetos.Add((key, objeto,tipo));
        key = "";
        objeto = "";
        tipo = "";
    }//O Método vai Adicionar uma tupla(key, objeto) na 
    void Aceitar()
    {
        estado = "qf";
        Console.WriteLine("Aceito");
        f = false;
    }

    void Printar_Objetos()
    {
        foreach ((string, string,string) a in lista_de_Objetos)
        {
            
            Console.WriteLine(a);

        }
    }

    public void Parse(string json)
    {
        inteiro = json;
        List<string> list = new List<string>(); // lista temporaria para armazenar os json dentro de objetos
        while (f)
        {
            Proximo(ref inteiro, ref character);
            pilha.Tirar(ref topo);
            switch ((character, estado, topo))
            {
                // inicio da key
                case ("{", "q0", null):
                    estado = "q1";
                    pilha.Colocar("$");
                    break;
                case ("}", "q1", "$"):
                    Aceitar();
                    break;
                case ("\"", "q1", _):
                    estado = "q2";
                    pilha.Recolocar(topo);
                    pilha.Colocar("o");
                    pilha.Colocar("s");
                    break;
                case ("\"", "q2", "s"):
                    estado = "q3";
                    break;
                case (_, "q2", _):
                    key += character;
                    pilha.Recolocar(topo);
                    break;
                case (":", "q3", "o"):
                    estado = "q4";
                    break;
                // fim da key


                //string
                case ("\"", "q4", "$"):
                    estado = "q5";
                    tipo = "string";
                    pilha.Recolocar(topo);
                    pilha.Colocar("s");
                    break;
                case ("\"", "q5", "s"):
                    estado = "q6";
                    break;
                case (_, "q5", _):
                    estado = "q5";
                    objeto += character;
                    pilha.Recolocar(topo);
                    break;
                case ("}", "q6", "$"):
                    Aceitar();
                    break;

                
                case (",", "q6", "$"):
                    estado = "q7";
                    tipo = "";
                    pilha.Recolocar(topo);
                    pilha.Colocar("k");
                    break;
                case ("\"", "q7", "k"):
                    estado = "q2";
                 

                    pilha.Colocar("o");
                    pilha.Colocar("s");

                    break;
                // fim da string

                // Inicio Bool
                case ("t", "q4", _):
                    pilha.Recolocar(topo);
                    objeto += character;
                    estado = "q8";
                    break;
                case ("r", "q8", _):
                    pilha.Recolocar(topo);
                    objeto += character;
                    estado = "q9";
                    break;
                case ("u", "q9", _):
                    pilha.Recolocar(topo);
                    objeto += character;
                    estado = "q10";
                    break;
                case ("e", "q10", _):
                    tipo = "bool";
                    pilha.Recolocar(topo);
                    objeto += character;
                    Adicionar();
                    estado = "q11";
                    break;
                case ("f", "q4", _):
                    pilha.Recolocar(topo);
                    objeto += character;
                    estado = "q12";
                    break;
                case ("a", "q12", _):
                    pilha.Recolocar(topo);
                    objeto += character;
                    estado = "q13";
                    break;
                case ("l", "q13", _):
                    pilha.Recolocar(topo);
                    objeto += character;
                    estado = "q14";
                    break;
                case ("s", "q14", _):
                    pilha.Recolocar(topo);
                    objeto += character;
                    estado = "q15";
                    break;
                case ("e", "q15", _):
                    tipo = "bool";
                    pilha.Recolocar(topo);
                    objeto += character;
                    estado = "q11";
                    break;
                case ("}", "q11", "$"):
                    Aceitar();
                    break;

                case (",","q11","$"):
                    estado = "q7";
                    pilha.Recolocar(topo);
                    pilha.Colocar("k");
                    break;
                // Fim Bool

                //inicio Null
                case ("n", "q4", _):
                    tipo = "null";
                    pilha.Recolocar(topo);
                    objeto += character;
                    estado = "q16";
                    break;
                case ("u", "q16", _):
                    pilha.Recolocar(topo);
                    objeto += character;
                    estado = "q17";

                    break;
                case ("l", "q17", _):
                    pilha.Recolocar(topo);
                    objeto += character;
                    estado = "q18";
                    break;
                case ("l", "q18", _):
                    pilha.Recolocar(topo);
                    objeto += character;
                    estado = "q11";
                    break;
                //fim Null

                //Inicio do Lista
                case ("[", "q4", _):
                    tipo = "lista";
                    
                    pilha.Recolocar(topo);
                    pilha.Colocar("c");
                    
                    estado = "q19";
                    break;

                case (",", _, "c"):
                    pilha.Recolocar(topo);
                    estado = "q19";
                    break;

                case ("]", _, "c"):
                    estado = "q20";
                    break;

                case (",", "q20", "$"):
                    estado = "q7";
                    pilha.Recolocar(topo);
                    pilha.Colocar("k");
                    break;
                case ("}", "q20", "$"):
                    Aceitar();
                    break;

                case ("\"", "q19", _):
                    pilha.Recolocar(topo);
                    pilha.Colocar("s");
                    estado = "q5";
                    break;
                case var _ when int.TryParse(character, out _) && estado == "q19":
                    pilha.Recolocar(topo);
                    estado = "q21";
                    break;
                case ("t", "q19", _):
                    pilha.Recolocar(topo);
                    estado = "q8";
                    break;
                case ("f", "q19", _):
                    pilha.Recolocar(topo);
                    estado = "q12";
                    break;
                case ("n", "q19", _):
                    pilha.Recolocar(topo);
                    estado = "q16";
                    break;
                case ("{", "q19", _):
                    pilha.Recolocar(topo);
                    pilha.Colocar("x");
                    estado = "q1";
                    break;
                //fim Lista


                //Inicio do objeto
                case ("{", "q4", _):
                    estado = "q1";
                    tipo = "objeto";
                    pilha.Recolocar(topo);
                    pilha.Colocar("x");
                    break;

                case ("}", _, "x"):
                    estado = "q22";
                    break;


                case (",", "q22", "$"):
                    estado = "q7";
                    pilha.Recolocar(topo);
                    pilha.Colocar("k");
                    break;

                case ("}", "q22", "$"):
                    Aceitar();
                    break;

                //fim objeto


                //inicio da int
                case var _ when int.TryParse(character, out _) && estado == "q4":
                    tipo = "int";
                    pilha.Recolocar(topo);
                    estado = "q21";
                    break;

                case var _ when int.TryParse(character, out _) && estado == "q21":
                    pilha.Recolocar(topo);
                    objeto += character;
                    break;

                case var _ when int.TryParse(character, out _) && estado == "q23":
                    pilha.Recolocar(topo);
                    objeto += character;
                    break;

                case (".", "q21", _):
                    estado = "q23";
                    pilha.Recolocar(topo);
                    break;
                case ("}", "q21", "$"):
                    Aceitar();
                    break;
                case ("}", "q23", "$"):
                    Aceitar();
                    break;

                case (",", "q21", _):
                    estado = "q7";
                    pilha.Recolocar(topo);
                    pilha.Colocar("k");
                    break;
                case (",", "q23", _):
                    estado = "q7";
                    pilha.Recolocar(topo);
                    pilha.Colocar("k");
                    Adicionar();
                    break;

                

                //fim da int

              
         
                default:
                    Console.WriteLine("Rejeitado");
                    f = false;
                    break;


            }

         }
    }
}


class ep3
{
    public static void Main(string[] args)
    {  
        Json conversor = new Json();
        conversor.Parse(@"{""a"":true,""s"":123,""as"":[""ss"",245,true,false]}");
      }
}


