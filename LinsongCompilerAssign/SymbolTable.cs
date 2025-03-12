using System;
using System.Collections.Generic;

public class SymbolTable
{
    
	public enum Tokens
    {
		    BEGINT,
            MODULET,
            CONSTANTT,
            PROCEDURET,
            IST,
            IFT,
            THENT,
            ELSET,
            ELSIFT,
            WHILET,
            LOOPT,
            FLOATT,
            INTEGERT,
            CHART,
            GETT,
            PUTT,
            ENDT,
            INT,
            OUTT,
            INOUTT
    }
    const int PRIME = 211;
    public class Entry
    {
        public string Lexeme;
        public string Token;
        public int depth;
        public int LOC;
        public string mode;
        public dynamic RecType;
    }
    public List<Entry>[] Table;
    public SymbolTable()
    {
        Table = new List<Entry>[PRIME];
        for (int i = 0; i < Table.Length; i++)
        {
            Table[i] = new List<Entry>();
        }
    }

    /*  public class NodeList //the list of entry in each index of Table
      {
      NodeList root=null;
      public class Entry
      {
          NodeList next=null;
          public Tokens Token;
          public string Lexeme;
          public int depth;
      }
      }*/
    #region Rectype
    public enum DataType {
        FLOATT,
        INTEGERT,
        CHART,
        NULL,
    }
    //return input token's datatype if it is float, integer, chart
   
    public struct VARIABLE
    {
        public string TypeName;
        public DataType VType;
        public int Offset;
        public int Size;
    }
    
    public struct CONSTANT
    {
        public string TypeName;
        public DataType CType;
        public int Offset;
        public int Size;
        public int IntergerV;
        public double RealV;
    }
    public struct PROCEDURE
    {
        public string TypeName;
        public int SizeOfLocal;
        public parser.FormalInfo Info;
        //DataType RetrunType;
        

    }
    public void ATypeisProcedure(Entry A, int sizeofLocals, parser.FormalInfo In)
    {
       
                    A.RecType = new PROCEDURE();
                    A.RecType.SizeOfLocal = sizeofLocals;
                    A.RecType.Info = In;
        A.RecType.TypeName = "PROCEDURE";
                    
                
    }
    public void ATypeisConstant(Entry A, string value)
    {
        Console.WriteLine(value);
                    A.RecType = new CONSTANT();
        A.RecType.TypeName = "CONSTANT";
        if (!value.Contains('.'))
        { A.RecType.IntergerV = Convert.ToInt32(value);
         
        }
        else
        { A.RecType.RealV = Convert.ToDouble(value);
         
        }
                
    }
    public void ATypeisVariable(Entry A, string datatype, int size, int offset)
    {
       

        A.RecType = new VARIABLE();
                    DataType DataType=DataType.NULL;
                    switch(datatype.ToLower())
                    {
                        case "chart": DataType = DataType.CHART; break;
                        case "floatt": DataType = DataType.FLOATT; break;
                        case "integert": DataType = DataType.INTEGERT; break;
                        default: break;
                    }
                    A.RecType.VType = DataType;
                    A.RecType.Offset = offset;
                    A.RecType.Size = size;
        A.RecType.TypeName = "VARIABLE";
    }
    #endregion



    #region Operations
    int hashpjw(string s)
    {
        uint h = 0, g;

        foreach (var p in s)
        {
            h = (h << 4) + (byte)p;
            if ((g = h & 0xf0000000) != 0)
            {
                h = h ^ (g >> 24);
                h = h ^ g;
            }
        }

        return (int)(h % (uint)PRIME);
    }
    public void Insert(string lex, string token, int depth)
	{
        var add = new Entry { Lexeme = lex, Token = token, depth = depth, LOC= LexicalAnalyzer.GetLOCat(LexicalAnalyzer.RLOCC) };
        int i = hashpjw(lex);
        
        Table[i].Add(add);
	}
    public void deleteDepth(int depth)
    {
        for (int i = 0; i < Table.Length; i++)
            for (int j = 0; j < Table[i].Count; j++)
                if (Table[i][j].depth == depth)
                {if(Table[i][j].RecType.TypeName != "PROCEDURE")
                    Table[i].Remove(Table[i][j]);
                }
    }
    public Entry Lookup(string Goal, int Depth)
    {
        foreach (var List in Table)
            foreach (var Enty in List)
                if (Enty.Lexeme == Goal && Enty.depth==Depth)
                    return Enty;
       
        return null;
    }
    public string Writelable(int Goal)
    {
        //Console.WriteLine("   Table:");
        string output = "";
        foreach (var List in Table)
            foreach (var Enty in List)
                if (Enty.depth == Goal)
                {
                    Console.WriteLine("Lexeme:{0}   Depth:{1}   Type:{2} ", Enty.Lexeme, Enty.depth, PrintType(Enty));
                    output += "Lexeme:"+ Enty.Lexeme + " Line of Code:"+Enty.LOC + "  Depth:"+ Enty.depth +"  Type: "  +PrintType(Enty) +" Value:"+ "\n";
                }
        return output;
    }
    public string PrintType(Entry enty)
    {
        
        string Rec = Convert.ToString(enty.RecType);
        if (Rec != null)
            return Rec;
        else
            return "NULL";
    }
    #endregion
}
