using System;
using System.Collections.Generic;
using System.IO;

public class parser
{


   public static int MAXFOMALS = 20;

    public class PList//a linked list about preser tree
    {
        public string Token = null;
        public int CloudSkip = 0;
        public PList next = null;
    }





     public struct FormalInfo
    {
       public int Num;
       public List<string> Types;
       public List<string> Modes;
    }

    static public void Parser()
    {
        string FileASM = "";
        string FileSymbol = "";
        string FileN = LexicalAnalyzer.GetFileName();
        for (int i = FileN.Length-1; i > 0; i--)
        {
           if(FileN[i]=='.')
            {
                FileASM = FileN.Substring(0, i) + ".asm";
                FileN = FileN.Substring(0,i)+".TAC";
                FileSymbol = FileN.Substring(0, i) + ".txt";
            }
            
        }
        string A = "";
        string B = "";
        string D = "";
        string Output = Path.Combine(Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.Parent.FullName, FileN);
        using (StreamWriter bd = File.CreateText(Output)) { }
        string Output2 = Path.Combine(Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.Parent.FullName, FileASM);
        using (StreamWriter bd = File.CreateText(Output2)) { }
        string Outputs = Path.Combine(Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.Parent.FullName, FileSymbol);
        using (StreamWriter bd = File.CreateText(Output2)) { }
        ToA("       .MODEL SMALL");
        ToA("       .586");
        ToA("       .STACK 100H");
        ToA("       .DATA");
        string Start = "";

        bool Success = true;
        PList Top = new PList();
        SymbolTable PTable = new SymbolTable();
        Top.next = null;
        int Depth = 0;
        int Offset=0;

        int NewBP = 0;
        #region Helping
        int Snum = 0;
        string C = "";
        void ToA(string In)
        {
            A += In+"\n";
        }
        void ToB(string In)
        {
            B += In + "\n";
        }
        void ToC(string In)
        {
            C += In + "\n"; 
        }
        void ToD(string In)
        {
            D += In + "\n";
        }
        void MergeAB()
        {
            using (StreamWriter bd = File.AppendText(Output2))
            {
                bd.WriteLine(A+B);
            }
        }
        void ToO(string In)
        {
            using (StreamWriter bd = File.AppendText(Output))
            {
                bd.WriteLine(In);
            }
        }

        SymbolTable.Entry Lookup(string lex, int D)
        {
            return PTable.Lookup(lex,D);
        }

        void DeleteDepth()
        {
            ToD(PTable.Writelable(Depth));
            PTable.deleteDepth(Depth);
            Depth--;
        }

        void Insert(string lex, string token, int depth)
        {
            SymbolTable.Entry Goal = new SymbolTable.Entry();
            Goal = Lookup(LexicalAnalyzer.GetThisTL(),depth);

            if (Goal != null)
            {
                Success = false;
                Console.WriteLine("ERROR- Same name found at same depth: {0} at {1}", Goal.Lexeme, depth);
            }
            else
            {
                PTable.Insert(lex, token, depth);
                LexicalAnalyzer.RLOCC++;
            }
            LexicalAnalyzer.LOCC--;
        }
        void Add(string T)//add new to tree
        {
           // Console.WriteLine("Added: {0}", T);
            PList N = new PList();
            N.Token = T;
            N.next = Top;
            N.CloudSkip = 0;
            Top = N;

        }
        void Addors(string T, int a)//add new to tree, set number of skip by the way
        {
            PList N = new PList();
            N.Token = T;
            N.next = Top;

            N.CloudSkip = a;
            Top = N;
        } 
        
        
        void Pop()

        {
           // Console.WriteLine("Poped: {0}", Top.Token);
           
            Top = Top.next;
        }
        void showlist(PList Ptr)
        {
            if (Ptr != null)
            {
                Console.Write("{0} ", Ptr.Token);
                showlist(Ptr.next);
            }
        }
        #endregion
        #region Grammar

        void SeqOfStatments()
        {
            Add("StatTail");
            Add("semit");
            Add("IO_Stat AssignStat");
            Pop();
            bool skip = false;
            if (!IO_Stat())
                if (!AssignStat())
                    skip = true;
            if (!skip)
            {
                
                if (TopMatch(LexicalAnalyzer.GetThisTN()))
                {
                    LexicalAnalyzer.GetNextT();
                    Pop();
                    SeqOfStatments();
                }
            }
            else
            {
                Pop();Pop();
            }
        }

        bool StatTail()
        {
            Pop();
            Add("StatTail");
            Add("semit");
          
            bool skip = false;
            if (!AssignStat())
                if (!IO_Stat())
                    skip = true;
            if (!skip)
            {
              
                if (TopMatch(LexicalAnalyzer.GetThisTN()))
                {
                    LexicalAnalyzer.GetNextT();
                    StatTail();
                }
                return true;
            }
            else {
                Pop();Pop(); return false;}
        }
        void Statement()
        {
            Add("IO_Stat AssignStat");
            if (TopMatch(LexicalAnalyzer.GetThisTN()))
            {
                switch (LexicalAnalyzer.GetThisTN())
                {

                    case "IO_Stat":
                        Pop(); IO_Stat();
                        break;
                    case "AssignStat": Pop(); AssignStat(); break;

                }
            }
        }
        int temNum = 0;
        int FacNum = 0;
        int NewSP = 0;
        string LastNewT;
        string Tem()
        {
        
            string ret="";
            
            temNum++;
            if(Depth==1)
            {
                ret = "_t" + temNum.ToString();
                ToA(ret + " DW ?");
            }
            else
            {
                NewSP = NewSP - 2;

                ret = "_bp" + (NewSP ).ToString();
                
            }
           
            
            return ret;
        }
        string GetName(SymbolTable.Entry This)
        {
            string FrontHalf="";
            if (This == null)
            {
                This = Lookup(LexicalAnalyzer.GetThisTL(), 1);
                if (This == null)
                {
                    Success = false;
                    Console.WriteLine("ERROR-Variable not found:{0}", LexicalAnalyzer.GetThisTL());
                    return null;
                }
            }
            if (This.depth != 1)
            {
                if (This.RecType.Offset > 0)
                    FrontHalf = "_bp" + "+" + This.RecType.Offset.ToString();
                else
                    FrontHalf = "_bp" + This.RecType.Offset.ToString();
            }
            else
            {
                
                    FrontHalf = This.Lexeme;
                
            }

            if (This.RecType.TypeName == "CONSTANT")
            {
                if (This.RecType.IntergerV != null)
                    FrontHalf = This.RecType.IntergerV.ToString();
                else if (This.RecType.RealV != null)
                    FrontHalf = This.RecType.RealV.ToString();
            }
                    return FrontHalf;
        }

        string FrontHalf = "";
        string BackHalf = "";
        bool AssignStat()//the return is to determine if skiped, false is skiped.
        { 
            Add("Expr");
            Addors("Assignt",2);
            Addors("idt",3);
            if (TopMatch(LexicalAnalyzer.GetThisTN()))//idt
            {
                FrontHalf = "";
                BackHalf = "";
                SymbolTable.Entry This= Lookup( LexicalAnalyzer.GetThisTL(),Depth);

                string ThisName = GetName(This);
                

                LexicalAnalyzer.GetNextT();
                    if (TopMatch(LexicalAnalyzer.GetThisTN()))//Assignt?
                    {
                    FacNum = 0;

                    LexicalAnalyzer.GetNextT();
                        Pop();  Expr();
                    FrontHalf = FrontHalf + "\n"+ThisName+" = "+LastNewT;
                    string ToOs = FrontHalf + BackHalf;
                    ToO(ToOs);

                    }else
                {
                    
                    ProcCall();
                    ToO("call "+This.Lexeme);
                }
                    return true;
               
            }
            else return false;
        }
        void ProcCall()
        {
            Add("rbrackt");
            Add("Params");
            Add("lbrackt");
            if (TopMatch(LexicalAnalyzer.GetThisTN()))
            {
                LexicalAnalyzer.GetNextT();
                Pop();Params();
                if(TopMatch(LexicalAnalyzer.GetThisTN()))
                {
                    LexicalAnalyzer.GetNextT();
                }
            }
        }
        void Params()
        {
            Add("ParamsTail");
            Addors("idt numt", 2);
            if (TopMatch(LexicalAnalyzer.GetThisTN()))
            {
                string tem = "\nPush " + LexicalAnalyzer.GetThisTL(); ToO(tem);

                LexicalAnalyzer.GetNextT();
                Pop(); ParamsTail();
            }
        }
        void ParamsTail()
        {
            Add("ParamsTail");
            Add("idt numt");
            Addors("commat", 3); 
            if (TopMatch(LexicalAnalyzer.GetThisTN()))
            {
                LexicalAnalyzer.GetNextT();
                if (TopMatch(LexicalAnalyzer.GetThisTN()))
                 {
                    string tem = "\nPush " + LexicalAnalyzer.GetThisTL(); ToO(tem);
                    LexicalAnalyzer.GetNextT();
                    Pop(); ParamsTail();
                }
             }
        }
        bool IO_Stat()
        {
            if (In_Stat())
            {
                return true;
            }
            else if(Out_Stat())
            {
                return true;
            }else
            return false;
        }
        string instructionis = "";

        bool In_Stat()
        {
            Add("rbrackt");
            Add("Id_List");
            Add("lbrackt");
            Addors("get",4);
            if (TopMatch(LexicalAnalyzer.GetThisTL().ToLower()))
            {
                
                    instructionis = "rd";
                LexicalAnalyzer.GetNextT();
                if(TopMatch(LexicalAnalyzer.GetThisTN()))
                {
                    LexicalAnalyzer.GetNextT();
                    Pop(); Id_List();
                    TopMatch(LexicalAnalyzer.GetThisTN());
                    LexicalAnalyzer.GetNextT(); 
                }
                return true;
            }else
            return false;
        }
        void Id_List()
        {
            Add("Id_List_Tail");
            Add("idt");
            if (TopMatch(LexicalAnalyzer.GetThisTN()))
            {
                ToO(instructionis + "i " + GetName(Lookup(LexicalAnalyzer.GetThisTL(), Depth)));
                LexicalAnalyzer.GetNextT();

                Pop(); Id_List_Tail();
            }
        }
        void Id_List_Tail()
        {
            Add("Id_List_Tail");
            Add("idt");
            Addors("commat",3);
            if (TopMatch(LexicalAnalyzer.GetThisTN()))
            {
                LexicalAnalyzer.GetNextT();
                if (TopMatch(LexicalAnalyzer.GetThisTN()))
                {
                    ToO(instructionis + "i " + GetName(Lookup(LexicalAnalyzer.GetThisTL(), Depth)));
                    LexicalAnalyzer.GetNextT();
                    Pop(); Id_List_Tail();
                }
               
            }
        }
        
        bool wrln = false ;
        bool Out_Stat()
        {
            Add("rbrackt");
            Add("Write_List");
            Add("lbrackt");
            Addors("put putln", 4);
            if (TopMatch(LexicalAnalyzer.GetThisTL().ToLower()))
            {
                wrln = false;
                if ("putln" == LexicalAnalyzer.GetThisTL().ToLower())
                    wrln = true;
                    instructionis ="wr";
                LexicalAnalyzer.GetNextT();
                if (TopMatch(LexicalAnalyzer.GetThisTN()))
                {
                    LexicalAnalyzer.GetNextT();
                    Pop(); Write_List();
                    TopMatch(LexicalAnalyzer.GetThisTN());
                    LexicalAnalyzer.GetNextT();
                }
                if(wrln)
                    ToO( "wrln");
                return true;
            }
            else
                return false;
        }
       void Write_List()
        {
            Add("Write_List_Tail");
            Add("idt numt Literalst");
            if (TopMatch(LexicalAnalyzer.GetThisTN()))
            {
                if ("literalst" == LexicalAnalyzer.GetThisTN().ToLower())
                { ToO(instructionis + "s _S"+Snum.ToString());
                    ToA("_S"+Snum.ToString() + " DB "+LexicalAnalyzer.GetThisTL()+", \"$\"");

                    Snum++;
                }
                if ("idt" == LexicalAnalyzer.GetThisTN().ToLower())
                    ToO(instructionis+"i "+GetName(Lookup(LexicalAnalyzer.GetThisTL(),Depth)));
                if ("numt" == LexicalAnalyzer.GetThisTN().ToLower())
                    ToO(instructionis + "i " + LexicalAnalyzer.GetThisTL().ToString());
                LexicalAnalyzer.GetNextT();
                Pop(); Write_List_Tail();
            }
            }
        void Write_List_Tail()
        {
            Add("Write_List");
            Addors("commat", 2);
            if (TopMatch(LexicalAnalyzer.GetThisTN()))
            {
                LexicalAnalyzer.GetNextT();
               
                    Pop(); Write_List();
                

            }
        }
        string Expr()
        {
            Add("Relation");
            Pop();  return Relation();
            
        }
        string Relation()
        {
            Add("SimpleExpr");
            Pop(); return SimpleExpr();
        }
        string SimpleExpr()
        {
            Add("MoreTerm");
            Add("Term");
            Pop(); Pop();string T= Term();
           string M=  MoreTerm();
            LastNewT = Tem();
            FrontHalf = FrontHalf + "\n" + LastNewT + " = " + T  + M;
            return LastNewT;
        }
        string MoreTerm()
        {
            Add("MoreTerm");
            Add("Term");
            Addors("Addopt Minopt ort", 3);
            if (TopMatch(LexicalAnalyzer.GetThisTN()))
            {
                string ThisF;
                string op = LexicalAnalyzer.GetThisTL();
                LexicalAnalyzer.GetNextT();
                Pop(); Pop(); ThisF = Term();

                MoreTerm();
                return " " + op + " " + ThisF;

            }
            else return null;

        }
        string Term()
        {
            Add("MoreFactor");
            Add("Factor");
            Pop(); Pop(); string r= Factor();
            string more= MoreFactor();
            if (FacNum > 2 && more!=null)
            {
                string LastNewT= Tem();
                FrontHalf=FrontHalf+"\n"+ LastNewT + " = " + r + more; 
                FacNum--;
                return LastNewT;
            }else
            return r+more;
        }
        string MoreFactor()
        {
            Add("MoreFactor");
            Add("Factor");
            Addors("* / mod rem and", 3);
            if (TopMatch(LexicalAnalyzer.GetThisTN()))

            {
                string op = LexicalAnalyzer.GetThisTL();
                String ThisF;
                LexicalAnalyzer.GetNextT();
                Pop(); Pop(); ThisF = Factor();
                MoreFactor();
               return " " + op + " " + ThisF;

            }
            else return null;
        }
        string Factor()
        {
            Add("idt numt lbrackt nott signopt Addopt Minopt");
            if (TopMatch(LexicalAnalyzer.GetThisTN()))
            {
                switch(LexicalAnalyzer.GetThisTN())
                {
                    case "Addopt":  break;
                    case "Minopt":  break;
                    case "idt": string Re= GetName(Lookup(LexicalAnalyzer.GetThisTL(), Depth)); LexicalAnalyzer.GetNextT();FacNum++; return Re;  break;
                    case "numt": string Re2 = LexicalAnalyzer.GetThisTL(); LexicalAnalyzer.GetNextT(); FacNum++; return Re2; break;
                    case "lbrackt": return  Exprewithin();
                        break;
                    case "nott":  nottFactor(); break;
                    case "signopt":  signoptFactor(); break;
                }
            }
            return null;
        }
        string Exprewithin()
        {
            Add("rbrackt");
            Add("Expr");
            Add("lbrackt"); 
            if (TopMatch(LexicalAnalyzer.GetThisTN()))
            {
                LexicalAnalyzer.GetNextT();
                Pop(); string re= Expr();
                if (TopMatch(LexicalAnalyzer.GetThisTN()))
                {
                    LexicalAnalyzer.GetNextT();
                    return re;
                }

            }
            return null;
        }
        void nottFactor()
        {
            Add("Factor");
            Add("nott");
            if (TopMatch(LexicalAnalyzer.GetThisTN()))
            {
                LexicalAnalyzer.GetNextT();
                Pop(); Factor();
            }
        }
        void signoptFactor()
        {
            Add("Factor");
            Add("signopt");
            Pop(); Pop(); signopt();
             Factor();
        }
        void addopt()
        {
            Add("+ - or");
            if (TopMatch(LexicalAnalyzer.GetThisTN()))
            {
                LexicalAnalyzer.GetNextT();
            }
        }
        void mulopt()
        {
            Add("* / mod rem and");
            if (TopMatch(LexicalAnalyzer.GetThisTN()))
            {
                LexicalAnalyzer.GetNextT();
            }
        }
        void signopt()
        {
            Add("Addopt Minopt");
            if (TopMatch(LexicalAnalyzer.GetThisTN()))
            {
                LexicalAnalyzer.GetNextT();
            }
        }
        void Prog()
        {
            Add("semit");
            Add("idt");
            Add("endt");
            Add("SeqOfStatments");
            Add("begint");
            Add("Procedures");
            Add("DeclarativePart");
            Add("ist");
            Add("Args");
            Add("idt");
            Add("proceduret");
            if (TopMatch(LexicalAnalyzer.GetThisTN()))//Matching proceduret
            {
                LexicalAnalyzer.GetNextT();
                if (TopMatch(LexicalAnalyzer.GetThisTN()))//Matching idt
                {
                    //insert This Token
                    string Nameofproc = LexicalAnalyzer.GetThisTL();
                    Insert(Nameofproc, LexicalAnalyzer.GetThisTN(), Depth);
                    if (Depth == 0)
                        Start = "START "+Nameofproc;
                    LexicalAnalyzer.GetNextT();
                    //increase the depth
                    Depth++;
                    
                    Pop();                   FormalInfo Formal= Args();//Expand Args and matching in function.
                    if (TopMatch(LexicalAnalyzer.GetThisTN()))//Matching ist
                    {
                        LexicalAnalyzer.GetNextT();
                        Pop(); int SizeofLocal =DeclarativePart(0);//Mathcing Decl..

                        PTable.ATypeisProcedure(Lookup(Nameofproc,Depth-1), SizeofLocal, Formal);
                        Pop();      Procedures();//Mathcing Proc..
                        if (TopMatch(LexicalAnalyzer.GetThisTN()))//Mathcing  Begin
                        {
                            string procN = "Proc " +Nameofproc;
                            
                            ToO(procN);
                            LexicalAnalyzer.GetNextT();
                            int oldTN = temNum;
                            Pop();  SeqOfStatments();//Mathcing SeqOfStatments
                            if (TopMatch(LexicalAnalyzer.GetThisTN()))//Mathcing End
                            {
                                LexicalAnalyzer.GetNextT();
                                if (TopMatch(LexicalAnalyzer.GetThisTN()))//Mathcing idt
                                {
                                    if(Nameofproc!=LexicalAnalyzer.GetThisTL())
                                    {
                                        
                                            Success = false;
                                            Console.WriteLine("ERROR- Ending {0} don't mathing start proce idt's lex", LexicalAnalyzer.GetThisTL());
                                        
                                    }
                                    procN = "endp " + Nameofproc;
                                    ToO(procN);
                                    LexicalAnalyzer.GetNextT();
                                    TopMatch(LexicalAnalyzer.GetThisTN());//Mathcing semit
                                    PTable.ATypeisProcedure(Lookup(Nameofproc, Depth - 1), (temNum - oldTN) * 2 + SizeofLocal, Formal);
                                    LexicalAnalyzer.GetNextT();
                                    DeleteDepth();

                                    
                                }
                            }
                        }
                    }

                }
            }

            //Three Address code writing for deepth 0
        }
        List<string> Lex=new List<string>(); ;
        int DeclarativePart(int SizeofLocal)
        {
            Add("DeclarativePart");
            Add("semit");
            Add("TypeMark");
            Add("colont");
            Add("IdentifierList");
            Addors("commat", 2);
            Addors("idt", 7);
            if (TopMatch(LexicalAnalyzer.GetThisTN()))//idt
            {
                 Offset = 0;
                
                int num = 1;
                 Lex = new List<string>();
                Lex.Add(LexicalAnalyzer.GetThisTL());
                Insert(Lex[0], LexicalAnalyzer.GetThisTN(), Depth);
                if(Depth==1)
                ToA(LexicalAnalyzer.GetThisTL() + " DW ?");
                LexicalAnalyzer.GetNextT();
                if (TopMatch(LexicalAnalyzer.GetThisTN()))//commant
                {
                    LexicalAnalyzer.GetNextT();
                    Pop();  num+=IdentifierList();//Identi..
                }
                if (TopMatch(LexicalAnalyzer.GetThisTN()))//colont
                {
                    LexicalAnalyzer.GetNextT();
                    Pop();  string type=TypeMark();//Type..
                    if (Depth > 1)
                    { NewBP = Offset; }
                    switch (type.ToLower())
                    {
                        case "chart": for(int i=num-1; i>=0;i--)
                            {
                                PTable.ATypeisVariable(Lookup(Lex[i],Depth),"chart",1,Offset-NewBP); 
                                Offset += 1;
                            }
                                ; break;
                        case "floatt":
                            for (int i = num - 1; i >= 0; i--)
                            {
                                PTable.ATypeisVariable(Lookup(Lex[i], Depth), "floatt", 4, Offset-NewBP);
                                Offset += 4;
                            }; break;
                        case "integert":
                            for (int i =0; i < Lex.Count ; i++)
                            {
                                Offset += 2;
                                PTable.ATypeisVariable(Lookup(Lex[i], Depth), "integert", 2, NewBP-Offset);
                                NewSP =NewBP-Offset;
                               

                            }; break;
                        

                    }
                    SizeofLocal += Offset;
                    if (TopMatch(LexicalAnalyzer.GetThisTN()))//semit
                    {
                        LexicalAnalyzer.GetNextT();
                        Pop(); SizeofLocal= DeclarativePart(SizeofLocal);//Decl..
                    }
                }
            }
            return SizeofLocal;
        }
        int IdentifierList()
        {
            Add("IdentifierList");
            Addors("commat", 2);
            Add("idt");
            if (TopMatch(LexicalAnalyzer.GetThisTN()))
            {
                Lex.Add(LexicalAnalyzer.GetThisTL());
                Insert(LexicalAnalyzer.GetThisTL(), LexicalAnalyzer.GetThisTN(), Depth);
                if(Depth==1)
                ToA(LexicalAnalyzer.GetThisTL() + " DW ?");
                LexicalAnalyzer.GetNextT();
                if (TopMatch(LexicalAnalyzer.GetThisTN()))
                {
                    LexicalAnalyzer.GetNextT();
                    Pop(); return 1+IdentifierList();
                }
                return 1;
            }return 0;
        }
        string TypeMark()
        {
            Add("Value");
            Addors("Assignt", 2);                        // This part cloud be wrong while code is something like: " b:INTEGER:=5;"

            Add("chart floatt integert constantt");
            string re = LexicalAnalyzer.GetThisTN();
            if (TopMatch(LexicalAnalyzer.GetThisTN()))
            {
                LexicalAnalyzer.GetNextT();
                if (TopMatch(LexicalAnalyzer.GetThisTN()))
                {
                    LexicalAnalyzer.GetNextT();
                    Pop(); string V=Value();
                    if(re.ToLower()=="constantt")
                    PTable.ATypeisConstant(Lookup(Lex[0], Depth), V);
                }
            }
            return re;
        }
        string Value()
        {
           
            Add("numt");
           TopMatch(LexicalAnalyzer.GetThisTN()) ;
            string re = LexicalAnalyzer.GetThisTL();
            LexicalAnalyzer.GetNextT();
            return re;
        }
        void Procedures()
        {
            Add("Procedures");
            Add("semit");
            Add("idt");
            Add("endt");
            Add("SeqOfStatments");
            Add("begint");
            Add("Procedures");
            Add("DeclarativePart");
            Add("ist");
            Add("Args");
            Add("idt");
            Addors("proceduret",12);
            if (TopMatch(LexicalAnalyzer.GetThisTN()))//Matching proceduret
            {
                LexicalAnalyzer.GetNextT();
                if (TopMatch(LexicalAnalyzer.GetThisTN()))//Matching idt
                {
                    //insert This Token
                    string Nameofproc = LexicalAnalyzer.GetThisTL();
                    Insert(Nameofproc, LexicalAnalyzer.GetThisTN(), Depth);

                    LexicalAnalyzer.GetNextT();
                    //increase the depth
                    Depth++;
                    if (Depth > 1)
                    { NewBP = Offset;  }
                    Pop(); FormalInfo Formal = Args();//Expand Args and matching in function.
                    if (TopMatch(LexicalAnalyzer.GetThisTN()))//Matching ist
                    {
                        LexicalAnalyzer.GetNextT();
                        
                        Pop(); int SizeofLocal = DeclarativePart(0);//Mathcing Decl..
                        
                        PTable.ATypeisProcedure(Lookup(Nameofproc, Depth-1), SizeofLocal, Formal);
                        Pop(); Procedures();//Mathcing Proc..
                        if (TopMatch(LexicalAnalyzer.GetThisTN()))//Mathcing  Begin
                        {
                            C = "";
                            
                            string procN = "Proc " + Nameofproc;
                            ToO(procN);
                            LexicalAnalyzer.GetNextT();
                            int oldTN = temNum;
                            Pop(); SeqOfStatments();//Mathcing SeqOfStatments
                            if (TopMatch(LexicalAnalyzer.GetThisTN()))//Mathcing End
                            {
                                LexicalAnalyzer.GetNextT();
                                if (TopMatch(LexicalAnalyzer.GetThisTN()))//Mathcing idt
                                {
                                    if (Nameofproc != LexicalAnalyzer.GetThisTL())
                                    {

                                        Success = false;
                                        Console.WriteLine("ERROR- Ending {0} don't mathing start proce idt's lex", LexicalAnalyzer.GetThisTL());

                                    }
                                    LexicalAnalyzer.GetNextT();
                                    TopMatch(LexicalAnalyzer.GetThisTN());//Mathcing semit
                                    PTable.ATypeisProcedure(Lookup(Nameofproc, Depth - 1), (temNum-oldTN)*2+SizeofLocal, Formal);
                                   
                                    procN = "endp " + Nameofproc;
                                    ToO(procN);
                                    LexicalAnalyzer.GetNextT();
                                    Pop(); Procedures();
                                    DeleteDepth();


                                }
                            }
                        }
                    }

                }
            }
        }
        FormalInfo Args()
        {
            FormalInfo Formal=new FormalInfo();
            Add("rbrackt");
            Add("ArgList");
            Addors("lbrackt", 3);
            if (TopMatch(LexicalAnalyzer.GetThisTN()))
            {
                LexicalAnalyzer.GetNextT();
                Pop();
                Formal.Num = 0;
                Formal.Modes = new List<string>();
                Formal.Types = new List<string>();
                 Formal = ArgList(Formal);
                TopMatch(LexicalAnalyzer.GetThisTN());
                LexicalAnalyzer.GetNextT();

            }
            return Formal;
            
        }
        FormalInfo ArgList(FormalInfo Formal)
        {
            
            Add("MoreArgs");
            Add("TypeMark");
            Add("colont");
            Add("IdentifierList");
            Add("Mode");
            Pop();  string New= Mode();
            Lex = new List<string>();
            if(New!=null)
            {
                Formal.Modes.Add(New);
            }
            int num = 0;
            Pop(); num+= IdentifierList();
            Formal.Num += num;
            New = null;
            if(TopMatch(LexicalAnalyzer.GetThisTN()))
            {
                LexicalAnalyzer.GetNextT();
                Pop(); New=TypeMark();
                List<string> Tem = Lex;
               
                        if (New!=null)
                {
                    Formal.Types.Add(New);
                }
                Pop(); Formal=MoreArgs(Formal);
                switch (New.ToLower())
                {
                    case "chart":
                        for (int i = num - 1; i >= 0; i--)
                        {
                            PTable.ATypeisVariable(Lookup(Tem[i], Depth), "chart", 1, Offset - NewBP + 4);
                            Offset += 1;
                        }
                                ; break;
                    case "floatt":
                        for (int i = num - 1; i >= 0; i--)
                        {
                            PTable.ATypeisVariable(Lookup(Tem[i], Depth), "floatt", 4, Offset - NewBP + 4);
                            Offset += 4;
                        }; break;
                    case "integert":
                        for (int i = num - 1; i >= 0; i--)
                        {

                            PTable.ATypeisVariable(Lookup(Tem[i], Depth), "integert", 2, Offset - NewBP + 4);
                            Offset += 2;


                        }; break;
                }
            }
            return Formal;
        }
        FormalInfo MoreArgs(FormalInfo Formal)
        {

            Add("ArgList");
            Addors("semit", 2);
           if( TopMatch(LexicalAnalyzer.GetThisTN()))
            {
                LexicalAnalyzer.GetNextT();
                Pop();Formal = ArgList(Formal);
            }
            return Formal;
        }
        string Mode()
        {
            List<string> lists = new List<string>();
            Addors("int outt inoutt", 1);
            if (TopMatch(LexicalAnalyzer.GetThisTN()))
            {
                string re= LexicalAnalyzer.GetThisTN(); 
                LexicalAnalyzer.GetNextT();
                return re;
            }
            else return null;

        }
        
        #endregion

        bool TopMatch(string Token)
        {

            if (Token != null)
            {
                if (Top.Token == null)
                {
                    Console.WriteLine("Error- The prog should ended, but there is more token:{0}", Token);
                    Success = false;
                    return false;
                }
                else
                {
                    string[] PTokens = Top.Token.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string PToken in PTokens)// for the or cases in Top.Token
                    {
                        if (PToken.ToLower() == Token.ToLower())
                        {
                           // Console.WriteLine("{0}  meet {1}", Token, Top.Token);showlist(Top);Console.WriteLine();for (int i = 0; i < 50; i++)Console.Write("*"); Console.WriteLine();
                            Pop();
                            return true;

                        }

                    }
                    if (Top.CloudSkip == 0)
                    {
                        Console.WriteLine("Error-{0} didn't meet {1}", Token, Top.Token);showlist(Top);Console.WriteLine();for (int i = 0; i < 50; i++)Console.Write("*"); Console.WriteLine();
                        Success = false;
                        return false;
                    }
                    else
                    {

                        int skiptimes = Top.CloudSkip;
                        for (int i = 0; i < skiptimes; i++)
                        {
                            Pop();
                        }
                        return false ;
                    }

                }
            }
            else
            {
                if (Top.Token != null)
                {
                    Console.WriteLine("Error- Expecting the following:");
                    showlist(Top);Console.WriteLine();
                    Success = false;
                    return false;
                }
                else
                {
                  
                    return true;
                }
            }

        }



        Prog();
        TopMatch(LexicalAnalyzer.GetThisTN());
        DeleteDepth();
        ToO(Start);

        ToA("       .CODE");
        ToA("       INCLUDE IO.ASM");
        ToA("start PROC");
        ToA("       mov ax, @data");
        ToA("       mov ds,ax");
        ToA("       call "+Start.Substring(6));
        ToA("       mov ah, 4ch");
        ToA("       mov al,0");
        ToA("       int 21h");
        ToA("start ENDP");
        string[] Sentences = File.ReadAllLines(Output);
        foreach(string Sentence in Sentences)
        {
            string[] word = Sentence.Split(' ');
            ToB("");
            if (word[0].ToLower() == "proc")
            {

                ToB(word[1]+" PROC");
                ToB("       PUSH BP");
                ToB("       MOV BP, SP");
                SymbolTable.Entry Proc = null; int i = 0;
                while (Proc == null && i < 9)
                {
                    Proc = Lookup(word[1], i);
                    
                    i++;
                }
                if (Proc != null)
                {
                    if (Proc.RecType.TypeName == "PROCEDURE")
                        ToB("       SUB SP, " + (Proc.RecType.SizeOfLocal).ToString());
                }
                else
                {
                    Console.WriteLine("This proc ["+word[1]+"] is not found");
                }
                
            } else if (word[0].ToLower() == "endp")
            {
                SymbolTable.Entry Proc = null; int i = 0;
                while (Proc == null && i < 9)
                {
                    Proc = Lookup(word[1], i);
                    i++;
                }
                if (Proc != null)
                    if (Proc.RecType.TypeName == "PROCEDURE")
                        ToB("       ADD SP, " + (Proc.RecType.SizeOfLocal).ToString());
                ToB("       POP BP");
                ToB("       RET 0");
            }
            else if (word[0].ToLower() == "wrs")
            {
                ToB("       mov dx, offset" + word[1]);
                ToB("       call writestr");
            }
            else if (word[0].ToLower() == "wri")
            {
                ToB("       mov dx,[" + word[1] + "]");
                ToB("       call writeint");
            }
            else if (word[0].ToLower() == "wrln")
            {
                ToB("       call writeln");
            }
            else if (word[0].ToLower() == "rdi")
            {
                ToB("       call readint");
                ToB("       mov [" + word[1] + "], bx");
            }
            else if (word[0].ToLower() == "call")
            {
                ToB("       call " + word[1]);
            }
            else if (word[0].ToLower() == "push")
            {
                ToB("       push " + word[1]);
            } else if (word.Length>1)
            { if(word[1] == "=")
             {
                    if(word.Length>4)
                    {
                       
                        if(word[3]=="*")
                        {
                            ToB("       mov ax, ["+word[2]+"]");
                            ToB("       mov bx, ["+word[4] + "]");
                            ToB("       imul bx");
                            ToB("       mov [" + word[0] + "], ax");
                        }
                        else if (word[3] == "/")
                        {
                            ToB("       mov ax, [" + word[2]+"]");
                            ToB("       mov bx, [" + word[4] + "]");
                            ToB("       idv bx");
                            ToB("       mov [" + word[0] + "], ax");
                        }
                        else if (word[3] == "+")
                        {
                            ToB("       mov ax, [" + word[2] + "]");
                            ToB("       add ax, [" + word[4] + "]");
                            ToB("       mov ["+word[0]+"], ax");
                        }
                        else if (word[3] == "-")
                        {
                            ToB("       mov ax, [" + word[2] + "]");
                            ToB("       sub ax, [" + word[4] + "]");
                            ToB("       mov [" + word[0] + "], ax");
                        }
                        else if (word[3] == "mod")
                        {
                            ToB("       mov ax, [" + word[2] + "]");
                            ToB("       mov bx, [" + word[4] + "]");
                            ToB("       mod bx");
                            ToB("       mov [" + word[0] + "], ax");
                        }
                        else if (word[3] == "rem")
                        {
                            ToB("       mov ax, [" + word[2] + "]");
                            ToB("       mov bx, [" + word[4] + "]");
                            ToB("       rem bx");
                            ToB("       mov [" + word[0] + "], ax");
                        }
                        else if (word[3] == "or")
                        {
                            ToB("       mov ax, [" + word[2] + "]");
                            ToB("       or ax, [" + word[4] + "]");
                            ToB("       mov [" + word[0] + "], ax");
                        }
                        else if (word[3] == "and")
                        {
                            ToB("       mov ax, [" + word[2] + "]");
                            ToB("       mov bx, [" + word[4] + "]");
                            ToB("       and bx");
                            ToB("       mov [" + word[0] + "], ax");
                        }
                    }
                    else
                    {
                        ToB("       mov ax, "+word[2]);
                        ToB("       mov["+word[0]+"],ax");
                    }
            } }
        }
        

        ToB("main PROC");
        ToB("       call "+Start.Substring(6));
        ToB("main endp");
        ToB("end start");
        MergeAB();
        using (StreamWriter bd = File.CreateText(Outputs))
        {
            bd.WriteLine(D);
        }
        if (Success)
            Console.WriteLine("Done- Seccess");
        else
            Console.WriteLine("Done- Error");

    }
}

