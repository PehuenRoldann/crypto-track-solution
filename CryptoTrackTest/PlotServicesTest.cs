using System;
using System.Threading.Tasks;
using Xunit;
using CryptoTrackApp.src.services;


namespace CryptoTrackTest {


public class PlotServicesTest
{
   
    [Fact]
    public async void GenerateFinancialPlot() {

      CurrencyServices service1 = new();
      PloterService service2 = new();
      string path = "";

      try {
        var history = await service1.GetHistory("bitcoin");
        path = await service2.GetFinancialPlot(history);

      } catch (Exception error) {
        Console.WriteLine(error.Message);
      }
      
      Assert.True(path != "");
    }
}

}
