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
        pilha.Tirar(ref topo);
        estado = "q7";
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
                    
                    estado = "q8";
                    tipo = "";
                    pilha.Recolocar(topo);
                    pilha.Colocar("k");
                    break;
                case ("\"", "q8", "k"):
                    estado = "q2";
                 

                    pilha.Colocar("o");
                    pilha.Colocar("s");

                    break;
                // fim da string

                // Inicio Bool
                case ("t", "q4", _):
                    pilha.Recolocar(topo);
                    objeto += character;
                    estado = "q10";
                    break;
                case ("r", "q10", _):
                    pilha.Recolocar(topo);
                    objeto += character;
                    estado = "q11";
                    break;
                case ("u", "q11", _):
                    pilha.Recolocar(topo);
                    objeto += character;
                    estado = "q12";
                    break;
                case ("e", "q12", _):
                    tipo = "bool";
                    pilha.Recolocar(topo);
                    objeto += character;
                    Adicionar();
                    estado = "q13";
                    break;
                case ("f", "q4", _):
                    pilha.Recolocar(topo);
                    objeto += character;
                    estado = "q14";
                    break;
                case ("a", "q14", _):
                    pilha.Recolocar(topo);
                    objeto += character;
                    estado = "q15";
                    break;
                case ("l", "q15", _):
                    pilha.Recolocar(topo);
                    objeto += character;
                    estado = "q16";
                    break;
                case ("s", "q16", _):
                    pilha.Recolocar(topo);
                    objeto += character;
                    estado = "q12";
                    break;
                case ("}", "q13", "$"):
                    Aceitar();
                    break;

                case (",","q13","$"):
                    estado = "q8";
                    pilha.Recolocar(topo);
                    pilha.Colocar("k");
                    break;
                // Fim Bool

                //Inicio do Lista
                case ("[", "q4", _):
                    tipo = "lista";
                    
                    pilha.Recolocar(topo);
                    pilha.Colocar("c");
                    
                    estado = "q17";
                    break;
                case ("\"", "q17", _):
                    pilha.Recolocar(topo);
                    pilha.Colocar("s");
                    estado = "q18";
                    break;
                case ("\"", "q18", "s"):
                    list.Add(objeto + "-string");
                    estado = "q28";
                    break;
                case (_, "q18", _):
                    pilha.Recolocar(topo);
                    objeto += character;
                    break;
                case ("t", "q17", _):
                    pilha.Recolocar(topo);

                    objeto += character;
                    estado = "q19";
                    break;
                case ("r", "q19", _):
                    pilha.Recolocar(topo);
                    objeto += character;
                    estado = "q20";
                    break;
                case ("u", "q20", _):
                    pilha.Recolocar(topo);
                    objeto += character;
                    estado = "q21";
                    break;
                case ("e", "q21", _):
                    pilha.Recolocar(topo);
                    objeto += character;
                    list.Add(objeto + "-bool");
                    estado = "q28";
                    break;
                case ("f", "q17", _):
                    pilha.Recolocar(topo);
                    objeto += character;
                    estado = "q22";
                    break;
                case ("a", "q22", _):
                    pilha.Recolocar(topo);
                    objeto += character;
                    estado = "q23";
                    break;
                case ("l", "q23", _):
                    pilha.Recolocar(topo);
                    objeto += character;
                    estado = "q24";
                    break;
                case ("s", "q24", _):
                    pilha.Recolocar(topo);
                    objeto += character;
                    estado = "q21";
                    break;
                case ("[", "q17", _):
                    pilha.Recolocar(topo);
                    pilha.Colocar("c");
                    estado = "q17";
                    break;
                case (_, "q17", _):
                    pilha.Recolocar(topo);
                    objeto += character;
                    VerificarNumero(character, ref f);
                    estado = "q28";

                    break;
                case (",", "q28", "o"):
                    estado = "q8";
                    pilha.Colocar("k");
                    
                    break;
                case (",", "q28", _):
                    pilha.Recolocar(topo);
                    objeto = "";
                    estado = "q17";
                    break;
                case ("]", "q28", "c"):

                    string valoresConcatenados = string.Join(" ", list);
                    objeto = valoresConcatenados;
                    Adicionar();

                    break;
                case ("}", "q28", "$"):
                    Aceitar();
                    break;               
                case (_, "q28", _):
                    pilha.Recolocar(topo);
                    objeto += character;
                    estado = "q123";


                    break;

                case (",", "q123", _):
                    pilha.Recolocar(topo);
                    list.Add(objeto + "-num");
                    objeto = "";
                    estado = "q17";
                    
                    break;
                case ("]", "q123", "c"):
                    pilha.Tirar(ref topo);

                    pilha.Recolocar(topo);
                    VerificarNumero(objeto, ref f);
                    list.Add(objeto + "-num");
                    objeto = "";

                    estado = "q28";
                    break;
                case (_, "q123",_):
                    pilha.Recolocar(topo);
                    objeto += character;

                    break;

                //fim Lista


                //Inicio do objeto
                case ("{", "q4", _):
                    estado = "q1";
                    tipo = "objeto";
                    pilha.Recolocar(topo);
                    pilha.Colocar("z");
                    break;

                case ("{", "q8", "k"):
                    pilha.Recolocar(topo);
                    break;

                case ("}", _, "z"):
                    estado = "q29";
                    break;

                case (",", "q29", "$"):
                    estado = "q8";
                    pilha.Recolocar(topo);
                    pilha.Colocar("k");
                    break;

                case ("}", "q7", "$"):
                    Aceitar();
                    break;

                case (",", "q7", "$"):
                    pilha.Recolocar(topo);
                    estado = "q8";
                    pilha.Colocar("k");
                    break;

                //fim objeto




                //inicio da int
                case (_, "q4", "$"):
                    estado = "q9";
                    pilha.Recolocar(topo);
                    objeto += character;
                    tipo = "int";
                    VerificarNumero(objeto, ref f);
                    break;
                        
                case ("}", "q9", "$"):
                    Adicionar();
                    Aceitar();
                    break;

                case (",", "q9", "$"):
                    estado = "q8";
                    pilha.Recolocar(topo);
                    pilha.Colocar("k");
                    Adicionar();
                    break;
                   
                case (_, "q9", _):
                    pilha.Recolocar(topo);
                    objeto += character;
                    VerificarNumero(objeto, ref f);
                    break;

                //fim da int

                case ("}", "q1", "$"):
                    estado = "q7";
                    Console.WriteLine("Aceito");
                    
                    f = false;
                    break;
                default:
                    Console.WriteLine("Rejeitado");
                    f = false;
                    break;


            }

         }
        Printar_Objetos();
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


