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
    }
    public void Recolocar(string s)
    {
        stack.Push(s);
    }
    public string Tirar(ref string tirado)
    {
        if (stack.Count > 0) tirado = stack.Pop();
        else tirado = null;
        return tirado;
    }
}
public class Json
{

    string key = "", objeto = "";

    Pilha pilha = new Pilha();
    string inteiro = "", character = "", topo = "";
    string estado = "q0";
    bool f = true;

    List<(string,string)> lista_de_Objetos = new List<(string, string)>();
    Regex letra = new Regex(@"[a-zA-Z0-9]", RegexOptions.IgnorePatternWhitespace);
    Regex numero = new Regex(@"[0-9]");
    void Proximo(ref string inteiro, ref string topo)
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

    void VerificarString(string key, ref bool f)
    {
        if(!letra.IsMatch(key))
        {
            Console.WriteLine("Rejeitado");
            f = false; 
        }
    }

    void VerificarNumero(string key, ref bool f)
    {
        if (!numero.IsMatch(key))
        {
            Console.WriteLine("Rejeitado");
            f = false;
        }
    }

    void Adicionar()
    {
        lista_de_Objetos.Add((key, objeto));
        key = "";
        objeto = "";
    }
    void Aceitar()
    {
        pilha.Tirar(ref topo);
        estado = "q7";
        Console.WriteLine("Aceito");
        Adicionar();
        f = false;
    }

    public void Parse(string json)
    {
        inteiro = json;
        
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
                    VerificarString(key, ref f);
                    estado = "q3";
                    break;
                case (_, "q2", _):
                    key += character;
                    pilha.Recolocar(topo);
                    break;
                case (":", "q3", _):
                    estado = "q4";
                    pilha.Recolocar(topo);
                    break;
                // fim da key


                //string
                case ("\"", "q4", _):
                    estado = "q5";
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
                case ("}", "q6", "o"):
                    
                    Aceitar();
                    break;
                case (",", "q6", "o"):
                    estado = "q8";
                    pilha.Colocar("k");
                    break;
                case ("\"", "q8", "k"):
                    estado = "q2";
                    Adicionar();

                    pilha.Colocar("o");
                    pilha.Colocar("s");

                    break;
                // fim da string

                // Inicio Bool
                case ("t", "q4", _):
                    pilha.Recolocar(topo);
                    estado = "q10";
                    break;
                case ("r", "q10", _):
                    pilha.Recolocar(topo);
                    estado = "q11";
                    break;
                case ("u", "q11", _):
                    pilha.Recolocar(topo);
                    estado = "q12";
                    break;
                case ("e", "q12", _):
                    pilha.Recolocar(topo);
                    estado = "q13";
                    break;
                case ("f", "q4", _):
                    pilha.Recolocar(topo);
                    estado = "q14";
                    break;
                case ("a", "q14", _):
                    pilha.Recolocar(topo);
                    estado = "q15";
                    break;
                case ("l", "q15", _):
                    pilha.Recolocar(topo);
                    estado = "q16";
                    break;
                case ("s", "q16", _):
                    pilha.Recolocar(topo);
                    estado = "q12";
                    break;
                case ("}", "q13", "o"):
                    Aceitar();
                    break;
                // Fim Bool

                //Inicio do Lista
                case ("[", "q4", _):
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
                    VerificarString(key, ref f);
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
                    objeto += character;
                    estado = "q17";
                    break;
                case ("]", "q28", "c"):
                    
                    break;
                case ("}", "q28", "o"):
                    Aceitar();
                    break;               
                case (_, "q28", _):
                    pilha.Recolocar(topo);
                    objeto += character;
                    VerificarNumero(character, ref f);
                    


                    break;

                //fim Lista


                //Inicio do objeto
                case ("{", "q4", _):
                    estado = "q1";
                    pilha.Recolocar(topo);
                    pilha.Colocar("z");
                    break;

                case ("{", "q8", "k"):
                    pilha.Recolocar(topo);
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
                case (_, "q4", _):
                    estado = "q9";
                    pilha.Recolocar(topo);
                    objeto += character;
                    VerificarNumero(objeto, ref f);
                    break;

                case ("}", "q9", "o"):
                    
                    Aceitar();
                    break;

                case (",", "q9", "o"):
                    estado = "q8";
                    pilha.Colocar("k");
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
    }
}


class ep3
{
    public static void Main(string[] args)
    {  
        Json conversor = new Json();
        conversor.Parse(@"{""asaa"":[false,true,1212,[""ola""]],""asa"":{""asa"":false},""np"":21123}");
    }
}


