# OrderBookProject

This project builds a limit order book from raw market data (`ticks.raw`) and outputs current best bid and ask (`B0`, `A0`) metrics in a CSV format.

## 📦 Project Structure

OrderBookProject/
├── OrderBookProject/              
│   ├── Models/                    # Order model
│   ├── OrderBooks/                # OrderBook logic
│   ├── Utilities/                 # File reading/writing utilities
│       ├── FileUtility.cs
│   └── Program.cs                 
│
├── OrderBookTestProject/          
│   ├── OrderBookTests.cs
│   ├── OrderBookTestsFullMessage.cs
│   └── OrderBookCornerCaseTests.cs
│
├── Resources/                     # Input/output files
│   ├── ticks.raw                  # Binary input
│   ├── ticks_sample.csv           # Sample decoded input
│   ├── ticks_result_sample.csv    # Sample output for comparison
│   └── output.csv                 # Generated outpu
├── OrderBookProject.sln           # Visual Studio solution file

## 🧾 Input & Output Formats

 ➤ Input: `ticks.raw` (binary)

Each record contains:
- `SourceTime` (Int64)
- `Side` (`1` = BID, `2` = ASK)
- `Action` (`A`, `M`, `D`, `Y`, `F`)
- `OrderId` (Int64)
- `Price` (Int32)
- `Qty` (Int32)

> Format is big-endian binary.

 ➤ Output: `output.csv` (semicolon-separated)

## 🛠️ Build & Run
▶ Requirements
.NET 6 SDK

Visual Studio 2022 or newer

▶ Using Visual Studio
Open OrderBookProject.sln

Set OrderBookProject as the startup project

Press F5 to build and run

▶ Using Command Line
bash
Copy
Edit
Navigate to the solution root
cd OrderBookProject

Build the project
dotnet build

Run the program (outputs to Resources/output.csv)
dotnet run --project OrderBookProject
✔️ The output file will be written to: OrderBookProject/Resources/output.csv

# 🧪 Uruchamianie testów jednostkowych

Testy znajdują się w projekcie `OrderBookTestProject` i są oparte na bibliotece **NUnit**.

## ✅ Wymagania

Aby uruchomić testy, upewnij się, że:

1. Masz zainstalowane pakiety NuGet:

   - `NUnit`
   - `NUnit3TestAdapter`
   - `Microsoft.NET.Test.Sdk`
   
2. Projekt testowy (OrderBookTestProject) ma referencję do projektu głównego (OrderBookProject)
