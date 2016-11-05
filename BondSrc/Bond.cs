using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ServiceStack;
namespace Bond
{
  /// <ClassName> Bond Const </ClassName> 
  /// <ClassSummary>
  /// This class is used for constant variable used throughout this application. 
  /// </ClassSummary>
  public static class BondConst{
      public const string gov = "government";
      public const string corp = "corporate";
     
  }
  /// <ClassName> BondPoco </ClassName>
  /// <ClassSummary>
  /// This class is used for holding objects deserialize from CSV file using ServiceStack library. 
  /// </ClassSummary>
  public class BondPoco {

      public string bond {get;set;}
      public string type {get;set;}
      public string term {get;set;}
      public string bondYield {get;set;}

  }
    /// <ClassName> Bond</ClassName>
    /// <ClassSummary>
    /// This is the main class for the application which holds the properties for a Bond and 
    /// also initializes a bond from BondPoco object
    /// </ClassSummary>
   public class Bond
    {

        public string bondID{get;set;}
        public string bondType{get;set;}
        public double bondTerm{get;set;}
        public double bondYield {get;set;}

        public Bond(string _bondID,string _type,double _term, double _yield){
            this.bondID = _bondID;
            this.bondType=_type;
            this.bondTerm=_term;
            this.bondYield=_yield;
        }
        public Bond(BondPoco pocoItem){
            this.bondID = pocoItem.bond;
            this.bondType=pocoItem.type;
            this.bondTerm=double.Parse(pocoItem.term.Replace("years", ""));
            this.bondYield=double.Parse(pocoItem.bondYield.Replace("%",""));
        }
    }
    /// <ClassName> BondHolder</ClassName>
    /// <ClassSummary>
    /// This class contains objects to hold nearest bonds for a corporate bond in bond terms. It contains two bonds: 
    /// one nearest bond whose term is lesser than corporate bond.
    /// one nearest bond whose term is greater than coporate bond.
    /// </ClassSummary>
    public class BondHolder {
      public Bond closeLesserBond;
      public Bond closeGreaterBond;

      public BondHolder(Bond _closeLesserBond, Bond _closeGreaterBond){
          this.closeLesserBond = _closeLesserBond;
          this.closeGreaterBond = _closeGreaterBond;
      }

  }
    /// <ClassName> CompareBondType /ClassName>
    /// <summary>
    /// This class implements IComparer for Bond class elements for comparing BondTerms for Bond Objects
    /// </summary>
    public class CompareBondType : IComparer<Bond>
    {
        public int Compare(Bond bondA, Bond bondB)
        {
            if (bondA == bondB) return 0;
            if (bondA == null) return -1;
            if (bondB == null) return -1;

            return bondA.bondTerm.CompareTo(bondB.bondTerm);
        }
    }

    /// <ClassName> BondRun</ClassName>
    ///  <ClassSummary>
    /// This class contains all the functions that takes required for outputting the results. 
    /// It takes the CSV file in the main function and executes two functions CalSpreadCurve and CalSpreadBenchmark to calculate 
    /// </ClassSummary>
    public static class BondRun {
   
         /// <FunctionName> closestBonds </FunctionName>
         /// <FunctionSummary>
         /// This function outputs Bond Holder objects class which contains     
         /// one nearest bond whose term is lesser than corporate bond.
         /// one nearest bond whose term is greater than corporate bond.
         /// It is necessary for reducing the error for linear interpolation as error is 
         /// proportional to the square of the distance between the data points.
         /// </FunctionSummary>
         /// <param name="govBondList">List of Bonds whose type is government</param>
         /// <param name="corpBondYears">A double value representing bond years for a corporate Bond</param>
         /// <returns>BondHolder object that contains two specified objects</returns>
         public static BondHolder ClosestBonds(List<Bond> govBondList, Bond corpBond) { 
          
            Bond nearLesserBond =null;
            Bond nearGreaterBond =null;


            govBondList.Sort((x, y) => x.bondTerm.CompareTo(y.bondTerm));        //In-place Sorting 
            CompareBondType compareBondType = new CompareBondType();
            int index = govBondList.BinarySearch(corpBond, compareBondType);

            int lesserBond = (~index) - 1;
            nearLesserBond = govBondList[lesserBond];

            int greaterBond = +~index;
            nearGreaterBond = govBondList[greaterBond];
          
            BondHolder resultBondList= new BondHolder(nearLesserBond,nearGreaterBond);
            return resultBondList;

        } 
        /// <FunctionName> calInterpol </FunctionName>
        /// <FunctionSummary>
        /// This function takes in two points in terms of nearest government bonds and calculates the interpolation in terms 
        /// corporate bond term years by using Linear interpolation formula:
        ///  Y = ( ( X - X1 )( Y2 - Y1) / ( X2 - X1) ) + Y1
        ///  Where,
        ///  X1,Y1 = First co-ordinates,
        ///  X2,Y2 = Second co-ordinates,
        ///  X = Target X co-ordinate,
        ///  Y = Interpolated Y co-ordinate.
        /// </FunctionSummary>
        /// <param name="corpBondYears">X coordinate of Corporate Bond representing Bond Terms</param>
        /// <param name="govBond1X">X coordinate of 1st government bond representing Bond Terms</param>
        /// <param name="govBond1Y">Y coordinate of 1st Government Bond representing Bond yields</param>
        /// <param name="govBond2X">X coordinate of 2nd Government Bond representing Bond Terms</param>
        /// <param name="govBond2Y">Y coordinate of 2nd Government Bond representing Bond yields</param>
        /// <returns>It returns the calculated value of interpolation</returns>
        public static  double CalInterpol(double corpBondYears, double govBond1X, double govBond1Y, double govBond2X, double govBond2Y)
        {

            if((govBond2X-govBond1X) ==0){
                return (govBond1Y+govBond2Y)/2;
            }
          return ((( corpBondYears-govBond1X)*(govBond2Y-govBond1Y))/(govBond2X-govBond1X)) + govBond1Y; 
           
        }

        /// <FunctionName> CalSpreadCurve </FunctionName>
        /// <FunctionSummary>
        /// This function cacluates result of spread to curve for a corporate bond by subtracting the 
        /// bond term from the result of interpolation of two nearest government bonds. 
        /// </FunctionSummary>
        /// <param name="govBondList">List of Bonds whose type is government</param>
        /// <param name="corpBond">A bond object whos type is corpBond</param>
        /// <returns></returns>
        public static double CalSpreadCurve(List<Bond> govBondList, Bond corpBond){ 
            
        BondHolder closestBondsList = ClosestBonds(govBondList,corpBond);  
       

        return corpBond.bondYield - 
            CalInterpol(corpBond.bondTerm,
            closestBondsList.closeLesserBond.bondTerm,
            closestBondsList.closeLesserBond.bondYield,
            closestBondsList.closeGreaterBond.bondTerm,
            closestBondsList.closeGreaterBond.bondYield
        );
       

      }
      
         /// <FunctionName> CalSpreadBenchmark </FunctionName>
         /// <FunctionSummary>
         /// This function first determines the best candidate in terms of Bond terms for 
         /// each corporate bond from the list of government bonds. After it locates the best candidate, 
         /// the function outputs the ID of this best candidate Bond and 
         /// calculates the yield by subtracting its value from corporate bond's yield
         /// </FunctionSummary>
         /// <Paramters>2 Parameters</Paramters>
         /// <param name="govBondList"> List of Government Bonds for which to benchmark</param>
         /// <param name="corpBond">Corporate Bond for which benchmark against </param>
         /// <returns>It prints the Bond ID and yield calculation. </returns>
         public static string CalSpreadBenchmark(List<Bond> govBondList, Bond corpBond){

            Bond resultBond = null;

            BondHolder closestBondsList = ClosestBonds(govBondList, corpBond);
            double lesserBondTerm = Math.Abs(closestBondsList.closeLesserBond.bondTerm - corpBond.bondTerm);
            double greaterBondTerm = Math.Abs(closestBondsList.closeGreaterBond.bondTerm - corpBond.bondTerm);

            if (lesserBondTerm < greaterBondTerm)
                resultBond = closestBondsList.closeLesserBond;
            else
                resultBond = closestBondsList.closeGreaterBond;

            string  resultValue = resultBond.bondID + " " + Math.Abs(corpBond.bondYield - resultBond.bondYield).ToString("N2") + "%";
           
            return resultValue; 

        }
        /// <FunctionName> DeserializeCSVPoco </FunctionName>
        /// <FunctionSummary>
        /// This function takes a csv file name as a string and deserializes the data into List of objects that matches
        /// coloumns in the csv.
        /// </FunctionSummary>
        /// <param name="csvFileName">CSV file name string</param>
        /// <returns>List of Bond Poco Objects</returns>
        public static List<BondPoco> DeserializeCSVPoco(string csvFileName){
            List<BondPoco> bondList = new List<BondPoco>();
            System.Console.WriteLine("File used for input " + csvFileName);
            bondList = File.ReadAllText(csvFileName).FromCsv<List<BondPoco>>();
            return bondList;
        }


        /// <FunctionName> Main </FunctionName>
        /// <FunctionSummary>
        /// This functon takes user-specified csv file or default for input.
        /// It uses ServiceStack library to deserialize CSV to the list of Bond Poco class objects for easy processing of input values
        /// It calls two functions CalSpreadBenchmark and CalSpreadCurve to output required results
        /// </FunctionSummary>
        /// <param name="args"> It takes CSV file as its first argument for processing</param>
        public static void Main(string[] args){
        
        
         string csvFileName =string.Empty;
         
         if(args.Length<1){
             csvFileName = "sample_input.csv";
         }
         else {
             csvFileName=args[0];   
         }
         
         //Deserializing CSV in Bond pocoObject
        List<BondPoco> bondList = DeserializeCSVPoco(csvFileName);
        //Two Lists for keeping government and corporate bond respectively. 
        List<Bond> govBondList = new List<Bond>();                                      
        List<Bond> corpBondList = new List<Bond>();
        Bond newItem =null;
        foreach(BondPoco item in bondList){
             newItem = new Bond(item);
            if(newItem.bondType==BondConst.corp){
                corpBondList.Add(newItem);
            }
            else if(newItem.bondType==BondConst.gov) {
                govBondList.Add(newItem);
            }
        }
      
       // Calculating CalSpreadBenchmark for coding Challenge #1. Please refer to the function definition for more details

       System.Console.WriteLine("Bond" + " " + "spread_to_benchmark");
        foreach(Bond item in corpBondList){
            System.Console.WriteLine(item.bondID + " " + CalSpreadBenchmark(govBondList,item));
        }

        System.Console.WriteLine();
        //Calculating CalSpreadCurve for coding Challenge #2. Please refer to the function definition for more details
        // Math.Round rounds a double-precision floating-point value to a specified number of fractional digits.
        // MidpointRounding: Specifies how mathematical rounding methods should process a number that is midway between two numbers.

        System.Console.WriteLine("Bond" + " " + "spread_to_curve");
          foreach(Bond item in corpBondList){
             System.Console.WriteLine(item.bondID + " " + 
             Math.Round(CalSpreadCurve(govBondList,item), 2, MidpointRounding.AwayFromZero).ToString("N2")+"%");
             
        }
        

        }

    }
}
