# Core Bond

This projects deals with 2 coding challenges relating to benchmarking corporate bond yields. 

* Created this project in new .NET Core framework using C# using just new Visual Studio Code editor on a Mac. 
* Created unit tests for this project which tests different functions from the project. 
* Used ServiceStack library for CSV import as it is one of high performance libraries and is free to use. 
* Documented the code to the best of my ability. Though if I had more time, I would like to extract xml comments 
  from my code. More on this late in Tradeoffs section.

I really enjoyed doing this project as I tried to optimize my code at every possible opportunity though I am sure this code can be optimized and cleaned up further. 


## Getting Started

These instructions will get you a copy of the project up and running on your local machine for development and testing purposes. 

Since this project was created .NET Core, you will need to install .NET Core on your machine. 
Please refer Prerequisites section for installing .NET Core framework on Windows or Mac. 

After you have installed .NET Core, please download this repository and you should have a directory structure like this: 
```
/CoreBond
|__global.json
|__readme.md
|__/BondSrc
   |__Source Files
      |__project.json
|__/BondTest
      |__Test Files
      |__project.json
```
It contains .vscode folder as well but I couldnt utilize it properly. More on this Tradeoffs section.

### Prerequisites

You will need active internet connection and .NET Core framework installed on your machine to setup this project. 
For detailed instructions regarding installing .NET Core framework on your machine(Windows, MacOSX or Linux) 
please refer to https://www.microsoft.com/net/core 

I created this project on Mac so I have included the instructions for the same.
To set it up on your Mac, I am reproducing instructions from above link:
	
In order to use .NET Core, you first need to install the latest version of OpenSSL. 
The easiest way to get this is from Homebrew from http://brew.sh/.
Install Homebrew by running the following command here 
```
/usr/bin/ruby -e "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/master/install)"
```

After installing brew, do the following:

```
brew update
brew install openssl
ln -s /usr/local/opt/openssl/lib/libcrypto.1.0.0.dylib /usr/local/lib/
ln -s /usr/local/opt/openssl/lib/libssl.1.0.0.dylib /usr/local/lib/

```
After that The best way to install .NET Core on macOS is to download the official installer from here 
`https://go.microsoft.com/fwlink/?LinkID=831679`
This installer will install the tools and put them on your PATH so you can run dotnet from the Console. 

Now you are set to go and move on to next section to installing and running the project. 


### Installing and Running the Project

`Building and Running the Project`

Download the code from this repository and follow these instructions. 
If you have a csv file for processing input, please include it in the `BondSrc` directory. If you do not provide one,
the project will use the exisiting sample_input.CSV for input.

To build and run the project, please navigate to `BondSrc` and run the following commands on the Terminal:

```
dotnet restore
dotnet build
dotnet run CSVFileName.csv file if you have one. 
```

`dotnet restore` installs all the dependencies.
`dotnet build` creates the assembly (or assemblies).
`dotnet run` runs the executable with any specified arguments.

After you run these commands you should see the output like this 

```
File used for input: sample_input.csv
Bond spread_to_benchmark
C1 G1 1.60%
C2 G2 1.50%
C3 G3 2.00%
C4 G3 2.90%
C5 G4 0.90%
C6 G5 1.80%
C7 G6 2.50%

Bond spread_to_curve
C1 1.43%
C2 1.63%
C3 2.47%
C4 2.27%
C5 1.90%
C6 1.57%
C7 2.83%

```

To run the tests, please navigate to the `BondTest` directory and type the following commands on the Terminal:

```
dotnet restore
dotnet build
dotnet test
```

`dotnet test` runs all the configured tests.

After you run the `dotnet test`, you will see the output like this: 

```
xUnit.net .NET CLI test runner (64-bit .NET Core osx.10.12-x64)
  Discovering: BondTest
  Discovered:  BondTest
  Starting:    BondTest
File used for input: sampleTestPass.csv
The tests are executing properly
  Finished:    BondTest
=== TEST EXECUTION SUMMARY ===
   BondTest  Total: 5, Errors: 0, Failed: 0, Skipped: 0, Time: 0.209s
SUMMARY: Total: 1 targets, Passed: 1, Failed: 0.

```

`Note`: You must run `dotnet restore` in the `BondSrc` directory before you can run
the tests. `dotnet build` will follow the dependency and build both the library and unit
tests projects, but it will not restore NuGet packages.

## Coding challenges: Approach

I created BondRun class that contains all the functions used in the challenge including the main function.
I created the Bond Class that holds a Bond with following properties.
        string bondID 
        string bondType 
        double bondTerm 
        double bondYield

I used ServiceStack library for high performance CSV import from a file SampleInput.CSV if user didnt specify any file. 
The csv import is considered first class format in this library so it is able to import large CSV files easily.

I created BondPoco class to take the objects deserialized from CSV as CSV data needs to be formatted before being transferred to Bond object. 
I used String.Replace function to take out `years` and `%` from CSV as it seems to be best performing method out of all 
three methods (String.Replace(), StringBuilder.Replace() and regex.replace()) in general usage. Refer here 
(https://blogs.msdn.microsoft.com/debuggingtoolbox/2008/04/02/comparing-regex-replace-string-replace-and-stringbuilder-replace-which-has-better-performance/)

Once I got the objects from CSV into the list of BondPoco object, I created two lists one for government and one for corporate bond.
Then I looped through corporate bond list, and calculating the result for two coding challenges.

`Coding challenge 1`: 

The first coding challenge required me to calculate the yield spread (return) between a corporate bond and its government bond benchmark.
The sample input is:

| bond   | type       | term        | yield |
|--------|------------|-------------|-------|
| C1     | corporate  | 10.3 years  | 5.30% |
| G1     | government | 9.4 years   | 3.70% |
| G2     | government | 12 years    | 4.80% |

Since, a government bond is a good benchmark if it is as close as possible to the corporate bond in terms of years to maturity (term). 
It required me to calculate the closest government bond to the corporate bond in bond terms. 

I created the function CalSpreadBenchmark that first determines the best candidate in terms of Bond terms for each corporate bond from 
the list of government bonds.After it locates the best candidate, the function outputs the ID of this best candidate Bond and calculates 
the yield by subtracting its value from corporate bond's yield.

The function uses the list of government bonds and given corporate bond as input. It loops through the list of govt bonds and 
keeps track of difference of bond terms between the current govt bond and given corp bond. It also keeps track of current return (yield)
and the Bond ID in a StringBuilder. I used StringBuilder for better performance as String are immutable in C#. 
Pleas refer here for more info:
(http://stackoverflow.com/questions/4274193/what-is-the-difference-between-a-mutable-and-immutable-string-in-c)

Since I am constantly updating my string value to keep track of Bond ID and current return, stringbuilder will use existing string pool 
instead of creating new one like normal Strings in C#. For large number of operations, the performance difference can be quite significant. 

After it determines the best candidate, it outputs the result. However, if the corp bond matches any value in terms of govt bond, it will return
that ID and difference.

The function outputs the result in O(n) time where n is number of elements in the list. 

Also I am making the sure number resulted is rounded to 2 digits as required.

The output for my program is: 

```
File used for input: sample_input.csv
Bond spread_to_benchmark
C1 G1 1.60%
C2 G2 1.50%
C3 G3 2.00%
C4 G3 2.90%
C5 G4 0.90%
C6 G5 1.80%
C7 G6 2.50%


```

`Coding challenge 2`

The second coding challenge required me to calculate the spread to the government bond curve for each corporate bond. 
Since the corporate bond term is not exactly the same as its benchmark term, it required me to use linear interpolation to determin the spread to the curve.

The sample input for the challenge is:

| bond   | type       | term        | yield |
|--------|------------|-------------|-------|
| C1     | corporate  | 10.3 years  | 5.30% |
| C2     | corporate  | 15.2 years  | 8.30% |
| G1     | government | 9.4 years   | 3.70% |
| G2     | government | 12 years    | 4.80% |
| G3     | government | 16.3 years  | 5.50% |


In `linear interpolation, the error is proportional to the square of the distance between the data points`, so it is important that for the government 
bonds(one greater than corp and one lesser than corp bond) that are selected for the linear interpolation are nearest to the each corporate bond.

I created the function closestBonds that return BondHolder object. It followed same logic as CalSpreadBenchmark function specified above. 
The time complexity for it is O(n). 

Once I have selected the nearest bonds, I used the formula:
```
Y = ( ( X - X1 )( Y2 - Y1) / ( X2 - X1) ) + Y1 
        Where,
            X1,Y1 = First co-ordinates,
            X2,Y2 = Second co-ordinates,
            X = Target X co-ordinate,
            Y = Interpolated Y co-ordinate. 
```    
Then I created the function CalSpreadCurve that calculates result of spread to curve for a corporate bond by subtracting the 
bond term from the result of interpolation of two nearest government bonds. 

Once it calculates the curve, it returns the value. The time complexity for this function is O(n).

Also I am making the sure number resulted is rounded to 2 digits as required.

My output for this challenge is 

```
Bond spread_to_curve
C1 1.43%
C2 1.63%
C3 2.47%
C4 2.27%
C5 1.90%
C6 1.57%
C7 2.83%
```

## Testing my code

For testing these challenges, I created different tests. 

* Sample Test: 
    Just to make sure that tests are running properly.
* DeserializeCSVTest 
    This test takes CSV file as a string and checks if it able to import the objects into Bond 
     Poco class from the file sucessfully. 
* CalSpreadBenchmarkTest 
    This test takes the expected input for given set of government bonds and a corporate bond 
     and compares it with the result from CalSpreadBenchmark function. 
* ClosestBondsTest 
    This test takes expected result of nearest higher and lesser government bonds in bond terms
    for a corporate bond and compares it with the the result from closestBonds function. 
* CalSpreadCurveTest
     This test takes the expected  spread to the curve result for a corporate bond against the list of government bonds 
     and compares it with the output from CalSpreadCurve which uses Linear Interpolation to calculate the result. 
 * Helper method LoadBondObjects      
      This function loads the data into List of Bond objects to be used by tests mentioned above.

## ToDo/Improvements or Tradeoffs 

  If I had extra time on this project, I would have focused and improved these areas: 

  * I would have made proper front-end interface for my project. It will allow the user to upload CSV file and display the data in nicely formatted way. 
    Also I would use external library like D3 JS (https://d3js.org/) to project the spread to curve for each corporate bond. 
    I would have ported this project Asp.NET CORE Web API. It will not require much code changes.
    The third-party library I have used ServiceStack is also compatible with it. I would have built the front-end interface with new JS framework Ember.JS.
  
  * I would have to like to extract xml comments from my code and present it as a separate  document. I wanted to use 
    DocFx library(https://dotnet.github.io/docfx/) that is used by .NET Core Team for documenting the code. 
  
  * I would have focused on improving the efficiency of algorithms used in the functions and cleaned up the code in my project.  

  * I would have made tests more robust as right now they are hard-coded. I can use some class data and member data in xUnit to improve the code
    readability for my TestCode. 
  
  * I would have automated Task capability provided by VS code for running automated tasks. I modified tasks.json but for some reason, it wasnt
    running for my project. 

  * Now for something Meta,  I would have to liked to submit pull-requests for committing this project.
  
   

## Built With

* .NET Core Framework 
* Visual Studio Code
* MacOSX Sierra

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details

## Acknowledgments

* Awesome people who contributed to .NET Core framework 
* Awesome people who made ServiceStack library. 
* Also https://docs.microsoft.com/en-us/dotnet/articles/core/testing/unit-testing-with-dotnet-test for giving a very good overview of testing 
  in .NET Core project.

## Author

* Sugat Mahanti



 
 
