using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class LexicalAnalyzer
{
     public struct OneToken
    {
        public string name;
        public string Lexme;
        public int Ivalue;
        public float Rvalue;
        public int LOC;
    }

  
    static public List<OneToken> Ts =new List<OneToken>();
    static public void Add(string Name, string lexme, int I)
    {
        OneToken New = new OneToken();
        New.name = Name;
        New.Lexme = lexme;
        New.Ivalue = I;
        New.LOC = LexicalAnalyzer.GetLOC();
        Ts.Add(New);
    }
    static public void Add(string Name, string lexme, float R)
    {
        OneToken New = new OneToken();
        New.name = Name;
        New.Lexme = lexme;
        New.Rvalue = R;
        Ts.Add(New);

    }
    static public int LOCC = 0;
    static public int RLOCC = 0;
    private static readonly object p = 100;
    public static int[] LOCArr=new int[100];
    static public void LOCCounting(int LOC)
    {
        LOCC += 1;
        LOCArr[LOCC] = LOC;
    }
    static public int GetLOCat(int Ind)
    {
        return LOCArr[Ind];
    }
    static public void Add(string Name, string lexme)
    {
        OneToken New = new OneToken();
        New.name = Name;
        New.Lexme = lexme;
        New.LOC = LexicalAnalyzer.GetLOC();
        Ts.Add(New);
    }
    static public string GetNextT()
    {
        if ((Ts != null) && (!Ts.Any()))
            return null;
        else {
            
            Ts.RemoveAt(0);
            if ((Ts != null) && (!Ts.Any()))
                return null;
            else
                return Ts[0].name;
            
                }
    }
    static public string GetThisTN()
    {
        if ((Ts != null) && (!Ts.Any()))
            return null;
        else
        {

            return Ts[0].name;

        }
    }
    static public string GetThisTL()
    {
        if ((Ts != null) && (!Ts.Any()))
            return null;
        else
        {

            return Ts[0].Lexme;

        }
    }

    static public string FileName;
    static public string GetFileName()
    {
        return FileName;
    }
    static public int LineOfCode = 0;
    static public int GetLOC()
    {
        return LineOfCode;
    }
    static public void LexicalA(string File1)
	{
        string Output = Path.Combine(Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.Parent.FullName, "LAOutput.txt");
        using (StreamWriter db = File.CreateText(Output))
        {
            if (!File.Exists(File1))
            {
                Console.WriteLine("Can't find the file:{0} ", File1);
                Console.WriteLine("Enter the name of the file");
                File1 = Console.ReadLine();
                File1 = Path.Combine(Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.Parent.FullName, File1);
                if (!File.Exists(File1))
                {
                    Console.WriteLine("Can't find the file:{0} ", File1);
                    return;
                }
            }
            FileName = File1;
            string[] Sentences = File.ReadAllLines(File1);

           
        Console.WriteLine("Token       Lexeme          Attrebute");
            
        //remove comments follows, NoComm is the output of the sentence without Comment
        foreach (string Sentence in Sentences)
        {   LexicalAnalyzer.LineOfCode  += 1;
            TokenIden(Sentence);
        }
        
    }
        int TokenIden(string word)
        {

            string[] Reserved;
            Reserved = new string[20]
            {
            "BEGIN",
            "MODULE",
            "CONSTANT",
            "PROCEDURE",
            "IS",
            "IF",
            "THEN",
            "ELSE",
            "ELSIF",
            "WHILE",
            "LOOP",
            "FLOAT",
            "INTEGER",
            "CHAR",
            "GET",
            "PUT",
            "END",
            "IN",
            "OUT",
            "INOUT",
            };
            if (word.Length >= 1)
                if (word[0] == '\n' || word[0] == '\t' || word[0] == ' ' || String.IsNullOrEmpty(word))
                {

                    TokenIden(word.Substring(1));
                    return 0;
                }
                else if (char.IsLetter(word[0]))
                {

                    for (int i = 0; i < word.Length; i++)
                    {
                        if (!char.IsLetterOrDigit(word[i]))
                        {
                            if (word[i] != '_')
                            {
                                TokenIden(word.Substring(0, i));
                                TokenIden(word.Substring(i));
                                return 0;
                            }
                        }
                    }
                    for (int i = 0; i < Reserved.Length; i++)
                    {
                        if (Reserved[i] == word.ToUpper())
                        {
                            string TokenName = word + "T";
                            Console.WriteLine("{0,-12:D}'{1}' ", TokenName, word);
                            //db.WriteLine("{0,-12:D}'{1}' ", TokenName, word);
                            Add(TokenName,word);
                            return 0;
                        }
                    }
                    if (word == "or")
                    {
                        Console.WriteLine("ort          '{0}' ", word);
                        // db.WriteLine("Addopt          '{0}' ", word);
                        Add("ort", word);
                        return 0;
                    }
                    if (word == "rem" || word == "mod" || word == "and")
                    {
                        Console.WriteLine("mulopt          '{0}' ", word);
                        // db.WriteLine("mulopt          '{0}' ", word);
                        Add("mulopt",word);
                        return 0;
                    }
                    if (word.Length > 17)

                        Console.WriteLine("ERROR-", word, " is too long(>17)");
                    else
                    {
                        Console.WriteLine("idt          '{0}' ", word);
                        //Console.WriteLine(LexicalAnalyzer.GetLOC());
                        LOCCounting(LexicalAnalyzer.GetLOC());//Record current Line of Code and counting +1
                        // db.WriteLine("idt          '{0}' ", word);
                        Add("idt",word);
                    }


                    return 0;
                }
                else if (char.IsDigit(word[0]))
                {
                    int Float = 0;
                    for (int i = 1; i < word.Length; i++)
                    {
                        if (!char.IsDigit(word[i]))
                        {
                            if (word[i] == '.')
                            {
                                Float++;

                            }
                            else
                            {
                                TokenIden(word.Substring(0, i));
                                TokenIden(word.Substring(i));
                                return 0;
                            }
                        }
                    }
                    if (Float == 1)
                    {
                        if (word[word.Length - 1] != '.')
                        {
                            float val = float.Parse(word);
                            Console.Write("numt".PadRight(12));
                            Console.Write(word.PadRight(14), word);
                            Console.WriteLine("{0}".PadLeft(4), val);
                            //db.Write("numt".PadRight(12));
                            // db.Write(word.PadRight(14), word);
                            //  db.WriteLine("{0}".PadLeft(4), val);
                           Add("numt",word,val);
                            return 0;
                        }
                        else
                        {
                            Console.WriteLine("ERROR- {0} is not illgal as float, Ending with '.'", word);
                            return 0;
                        }
                    }
                    else if (Float == 0)
                    {
                        int Val = Int32.Parse(word);
                        Console.Write("numt".PadRight(12));
                        Console.Write(word.PadRight(14), word);
                        Console.WriteLine("{0}".PadLeft(4), Val);
                        // db.Write("numt".PadRight(12));
                        //   db.Write(word.PadRight(14), word);
                        //  db.WriteLine("{0}".PadLeft(4), Val);
                        Add("numt", word, Val);
                        return 0;
                    }
                    else
                    {
                        Console.WriteLine("ERROR- {0} is not illgal as float, too many '.'", word);
                        return 0;
                    }
                }
                else if (word[0] == '.')
                {
                    if (word.Length > 1)
                        if (char.IsDigit(word[1]))
                            Console.WriteLine("ERROR- {0} start with '.', it is not a number", word);
                    return 0;
                }
                else
                {

                    if (word[0] == '/' || word[0] == '<' || word[0] == '>')
                    {
                        if (word.Length > 1)
                        {
                            if (word[1] == '=')
                            {
                                if (word.Length == 2)
                                {
                                    Console.WriteLine("Relopt            '{0}' ", word);

                                    // db.WriteLine("Relopt            '{0}' ", word);
                                    Add("Relopt",word);
                                    return 0;
                                }
                                else
                                {
                                    TokenIden(word.Substring(0, 1));
                                    TokenIden(word.Substring(1));
                                    return 0;
                                }
                            }
                            else
                            {
                                TokenIden(word.Substring(0, 1));
                                TokenIden(word.Substring(1));
                            }
                        }
                        else if (word.Length == 1 && word[0] != '/')
                        {
                            Console.WriteLine("Relopt           '{0}' ", word);
                            //db.WriteLine("Relopt           '{0}' ", word);
                            Add("Relopt",word);
                        }
                        else if (word.Length == 1 && word[0] == '/')
                        {
                            Console.WriteLine("/          '{0}' ", word);
                            //db.WriteLine("mulopt          '{0}' ", word);
                            Add("/",word);
                        }

                    }
                    else if (word[0] == ':')
                    {
                        if (word.Length > 1)
                            Console.WriteLine("                          {0}, {1}", word, word[1]);
                        if (word.Length > 1)
                        {
                            if (word[1] == '=')
                            {
                                if (word.Length >= 2)
                                {
                                    Console.WriteLine("Assignt       '{0}' ", word.Substring(0, 2));
                                    // db.WriteLine("Assignt       '{0}' ", word.Substring(0, 2));
                                    Add("Assignt",word.Substring(0,2));
                                    if (word.Length > 2)
                                    {

                                        TokenIden(word.Substring(2));
                                    }
                                }
                                else
                                {
                                    TokenIden(word.Substring(0, 1));
                                    TokenIden(word.Substring(1));
                                }
                            }
                            else
                            {
                                TokenIden(word.Substring(0, 1));
                                TokenIden(word.Substring(1));
                            }
                        }
                        else if (word.Length == 1 && word[0] == ':')
                        {
                            Console.WriteLine("colont         '{0}' ", word);
                            // db.WriteLine("colont         '{0}' ", word);
                            Add("colont",word);
                        }
                    }
                    else
                    {

                        for (int i = 0; i < word.Length; i++)
                        {
                            if (word[i] == '"')
                            {

                                for (int j = i + 1; j < word.Length; j++)
                                {
                                    if (word[j] == '"')
                                    {
                                        TokenIden(word.Substring(0, i));


                                        Console.WriteLine("Literalst          '{0}' ", word.Substring(i, j - i + 1));
                                        // db.WriteLine("Literalst          '{0}' ", word.Substring(i, j - i + 1));
                                        Add("Literalst",word.Substring(i,j-i+1));
                                        if (word.Length > j + 1)
                                            TokenIden(word.Substring(j + 1));
                                        return 0;
                                    }
                                }
                                Console.WriteLine("ERROR-For '{0}', didn't find the second quott", word);
                                return 0;
                            }
                            if (word[i] == '-')
                            {
                                if (word.Length > i + 1)
                                    if (word[i + 1] == '-')
                                    {
                                        TokenIden(word.Substring(0, i));
                                        return 0;
                                    }
                            }
                            if (char.IsLetterOrDigit(word[i]))
                            {
                                TokenIden(word.Substring(0, i));
                                TokenIden(word.Substring(i));
                                return 0;
                            }
                        }
                        if (word.Length > 1)
                        {
                            TokenIden(word.Substring(0, 1));

                            TokenIden(word.Substring(1));
                            return 0;
                        }
                        if (word == "+")
                        {
                            Console.WriteLine("Addopt          '{0}' ", word);
                            //db.WriteLine("Addopt          '{0}' ", word);
                            Add("Addopt",word);
                            return 0;
                        }
                        if ( word == "-")
                        {
                            Console.WriteLine("Minopt          '{0}' ", word);
                            //db.WriteLine("Addopt          '{0}' ", word);
                            Add("Minopt", word);
                            return 0;
                        }
                        if (word == "*")
                        {
                            Console.WriteLine("*          '{0}' ", word);
                            //db.WriteLine("mulopt          '{0}' ", word);
                            Add("*",word);
                            return 0;
                        }
                        if (word == "(")
                        {

                            Console.WriteLine("lbrackt          '{0}' ", word);
                            //db.WriteLine("lbrackt          '{0}' ", word);
                            Add("lbrackt",word);
                            return 0;
                        }
                        if (word == ")")
                        {

                            Console.WriteLine("rbrackt          '{0}' ", word);
                            //db.WriteLine("rbrackt          '{0}' ", word);
                            Add("rbrackt",word);
                            return 0;
                        }
                        if (word == ",")
                        {
                            Console.WriteLine("commat          '{0}' ", word);
                            //db.WriteLine("commat          '{0}' ", word);
                            Add("commat",word);
                            return 0;
                        }
                        if (word == ";")
                        {
                            Console.WriteLine("semit          '{0}' ", word);
                            //db.WriteLine("semit          '{0}' ", word);
                            Add("semit",word);
                            return 0;
                        }
                        if (word == ".")
                        {
                            Console.WriteLine("periodt          '{0}' ", word);
                            //db.WriteLine("periodt          '{0}' ", word);
                            Add("periodt",word);
                            return 0;
                        }
                        if (word == "=")
                        {
                            Console.WriteLine("Relopt          '{0}' ", word);
                            //db.WriteLine("Relopt          '{0}' ", word);
                            Add("Relopt",word);
                            return 0;
                        }
                        if (!String.IsNullOrEmpty(word) && word != " " && word != "\t" && word != "\n")
                        {

                            Console.WriteLine("ERROR-'{0}' is unknown token for now, it is not space or empty ", word);
                        }
                    }
                }
            return 0;
        }
    }
}


