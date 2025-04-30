# OrderBookProject

This project builds a limit order book from raw market data (`ticks.raw`) and outputs current best bid and ask (`B0`, `A0`) metrics in a CSV format — suitable for high-frequency trading (HFT) analysis.
## 📁 Project Structure


## 🛠️ Build & Run

### ▶ Requirements

- [.NET 6 SDK](https://dotnet.microsoft.com/en-us/download)
- Visual Studio 2022 or newer

### ▶ Command-line usage

```bash
cd OrderBookProject
dotnet build
dotnet run --project OrderBookProject
➡ Output will be saved to: Resources/output.csv
