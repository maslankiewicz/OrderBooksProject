# OrderBookProject

This project builds a limit order book from raw market data (`ticks.raw`) and outputs current best bid and ask (`B0`, `A0`) metrics in a CSV format.

## 📦 Project Structure

OrderBookProject/ ├── OrderBookProject/ # Core application logic │ ├── Models/ # Order data model │ ├── OrderBooks/ # Order book logic │ ├── Utilities/ # File I/O helpers │ └── Program.cs # Entry point │ ├── OrderBookTestProject/ # NUnit test project │ └── OrderBookTests.cs │ ├── Resources/ # Input/output files │ ├── ticks.raw # Raw binary tick input │ ├── ticks_sample.csv # Decoded sample input (CSV) │ ├── ticks_result_sample.csv # Expected result (CSV) │ └── output.csv # Program output │ ├── .github/workflows/ # GitHub Actions CI (optional) │ └── dotnet.yml │ ├── OrderBookProject.sln # Visual Studio solution └── README.md # This file

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
