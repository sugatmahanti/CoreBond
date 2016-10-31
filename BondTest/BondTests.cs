using System;
using Xunit;
using System.Collections.Generic;
using Bond;

namespace BondTests
{
    public class BondTests
    {
       
      
        // /// <summary>
        // /// This is a sample test that just makes sure test are running properly. 
        // /// </summary>
        [Fact]
        public void SampleTest() 
        {
            Assert.True(true);
            System.Console.WriteLine("The tests are executing properly");
        }

        /// <summary>
        /// This test takes CSV file as a string. It takes 2 files:
        /// and checks if it able to import the objects into Bond 
        /// Poco class from the file sucessfully
        /// </summary>

        [Theory]
        [InlineData("sampleTestPass.csv")]
        //[InlineData("sampleTestFail.csv")]
        public void DeserializeCSVTest(string CSVFileName){
            int count =1;
            var result =  BondRun.DeserializeCSVPoco(CSVFileName);
            Assert.Equal(result.Count,count);
            foreach (var item in result)
            {
                Assert.Equal(item.bond,"C1");
               // Assert.Equal(item.bond,"C2");
            }
        }

        //ToDo: Implement class data attribute in Xunit tests which will improve quality of this test. 

        /// <summary>
        /// This test takes the expected input for given set of government bonds and a corporate bond 
        /// and compares it with the result from CalSpreadBenchmark function.
        /// </summary>
        [Theory]
        [InlineData("G1" + " " + "1.60%")]
        public void CalSpreadBenchmarkTest(string input){
            Bond.Bond sampleInput1 =  new  Bond.Bond ("C1",Bond.BondConst.corp,10.3,5.30);
           
            List<Bond.Bond > bondList = LoadBondObjects();
           
            string actual= Bond.BondRun.CalSpreadBenchmark(bondList,sampleInput1);
            Assert.True(input.Equals(actual),input); 
        }
         //ToDo: Implement class data attribute in Xunit tests which will improve quality of this test. 
         /// <summary>
         /// This test takes expected result of nearest higher and lesser government bonds in bond terms
         /// for a corporate bond and compares it with the the result from closestBonds function.
         /// </summary>
         [Theory]
         [InlineData("G2G3")]
         public void ClosestBondsTest(string input){
             Bond.Bond  sampleInput1 = new Bond.Bond("C2",Bond.BondConst.corp,15.2,8.30);
             List<Bond.Bond > govBondList = LoadBondObjects();
             Bond.BondHolder closestBonds = Bond.BondRun.closestBonds(govBondList,sampleInput1.bondTerm);
             string actual =   closestBonds.closeLesserBond.bondID.ToString() + closestBonds.closeGreaterBond.bondID.ToString();
             Assert.True(input.Equals(actual));
         }
        
        //ToDo: Implement class data attribute in Xunit tests which will improve quality of this test. 
        /// <summary>
        /// This test takes the expected  spread to the curve result for a corporate bond against the list of government bonds 
        /// and compares it with the output from CalSpreadCurve which uses Linear Interpolation to calculate the result. 
        /// </summary>
        [Theory]
        [InlineData("1.22")]
        public void CalSpreadCurveTest(string input){
           Bond.Bond sampleInput1 =  new  Bond.Bond ("C1",Bond.BondConst.corp,10.3,5.30);
            List<Bond.Bond > bondList = LoadBondObjects();
            double actualNum= Bond.BondRun.CalSpreadCurve(bondList,sampleInput1);
            string actual = Math.Round(actualNum, 2, MidpointRounding.AwayFromZero).ToString("N2");
            Assert.True(input.Equals(actual),input); 
            
        }
        ///ToDo: improve this function to take the input from CSV file instead of manually entering results.
        /// <summary>
        /// This function just loads the List of Bond objects for testing purposes.  
        /// </summary>
  
        public static List<Bond.Bond> LoadBondObjects(){
             Bond.Bond sampleInput1 =  new  Bond.Bond ("C1",Bond.BondConst.corp,10.3,5.30);
           
            Bond.Bond  sampleInput2 =  new Bond.Bond ("G1",Bond.BondConst.gov,9.4,3.70);
            Bond.Bond  sampleInput3 =  new Bond.Bond ("G2",Bond.BondConst.gov,12,4.80);
            Bond.Bond  sampleInput4 =  new Bond.Bond ("G3",Bond.BondConst.gov,16.3,5.50);
            List<Bond.Bond > bondList = new List<Bond.Bond>();
            bondList.Add(sampleInput2);
            bondList.Add(sampleInput3);
            bondList.Add(sampleInput4);
            return bondList;
        }


    }

 

}
