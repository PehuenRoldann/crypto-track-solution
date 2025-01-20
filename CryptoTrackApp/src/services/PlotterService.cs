using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CryptoTrackApp.src.utils;
using Microsoft.EntityFrameworkCore.Migrations;
using ScottPlot;
using ScottPlot.TickGenerators;
using ScottPlot.TickGenerators.TimeUnits;
using SP = ScottPlot;

namespace CryptoTrackApp.src.services
{
    public class PlotterService : IPlotterService
    {

        public string _basePath = AppDomain.CurrentDomain.BaseDirectory;

        private Logger _logger = new Logger();

        private string FONT_NAME = "Arial";



        public async Task<string> GetCandlesPlot(List<(DateTime, double)> history, int width = 700, int height = 300) {

            try {

                /* DateTime[] dateKeysArr = valuesPerMonth.Keys.ToArray(); */
                _logger.Log($"[EXEC - Operation GetFinancialPlot at PloterService - Parameters: [ history: {history}; width: {width}; height: {height} ]]");
                var targetDate = DateTime.UtcNow.AddMonths(-6);
                history = history.Where(tupla => tupla.Item1 >= targetDate).ToList();

                DateTime initialDate = history[0].Item1;
                TimeSpan timeSpan = new(days: 7, 0, 0, 0);
                

                List<OHLC> prices = new();

                for (
                    DateTime currentDate = initialDate;
                    currentDate < DateTime.Now.Date;
                    currentDate = currentDate.AddDays(timeSpan.Days)) {
                    
                    double[] values = history
                    .Where(tupla => tupla.Item1 >= currentDate 
                        && tupla.Item1 < currentDate.AddDays(timeSpan.Days))
                    .Select(item => item.Item2)
                    .ToArray();

                    double open = values[0];
                    double high = values.Max();
                    double low = values.Min();
                    double close = values[values.Length - 1];

                    prices.Add(new OHLC(open, high, low, close, currentDate, timeSpan));

                }

                Plot plot = new();

                plot.Add.Candlestick(prices);
                plot.Axes.DateTimeTicksBottom();
                plot.Font.Automatic();

                // Definir el path donde guardar el plot
                string plotDirectory = System.IO.Path.Combine(this._basePath, "plots");

                // Verificar si el directorio existe y crearlo si no es así
                if (!System.IO.Directory.Exists(plotDirectory))
                {
                    System.IO.Directory.CreateDirectory(plotDirectory);
                }

                string pathToPlot = System.IO.Path.Combine(plotDirectory, "financialplot.png");

                // plot.SavePng(pathToPlot, width, height);
                plot.SavePng(pathToPlot, width, height);

                _logger.Log($"[SUCCESS - Operation GetCandlePlot at PloterSerivce - Generated path: {pathToPlot}]");


                //return plot;
                return pathToPlot;
            }
            catch (Exception error) {

                _logger.Log($"[ERROR - Operation GetCandlePlot at PloterSerivce - Message: {error.Message}]");

                return "";
            }
            

        }
             
    }
}


/// <summary>
        /// Generates a image with the plotbox.
        /// </summary>
        /// <param name="pCurrencyId"></param>
        /// <returns>Path to the plotbox generated image</returns>
        /* public async Task<string> GetBoxPlot (Dictionary<DateTime, List<double>> valuesPerMonth, int width = 700, int height = 300) 
        {


            var datesArr = valuesPerMonth.Keys.ToArray();

            SP.Plot plot = new ();
            double? minValue = null;
            double? maxValue = null;
            

            foreach(var keyDate in valuesPerMonth.Keys) {

                var box = AddBox(valuesPerMonth[keyDate], keyDate, plot);

                if (minValue == null || minValue > box.BoxMin) {
                    minValue = box.BoxMin;
                }

                if (maxValue == null || maxValue < box.BoxMax) {
                    maxValue = box.BoxMax;
                }

                

                //boxes.Add(box);
                // plot.Add.Box(box);
                // box.FillStyle.Color = Color.FromHex("#C68FE6");
                
            }
            
            // plot.Add.Boxes(boxes);
            plot.Axes.SetLimits(
                datesArr[0].ToOADate(),
                datesArr[^1].ToOADate(),
                minValue,
                maxValue
            );

            var dtAx = plot.Axes.DateTimeTicksBottom();

            dtAx.TickGenerator = new DateTimeFixedInterval (
                new Month(), 1,
                new Month(), 1,
                dt => new DateTime(dt.Year, dt.Month, 1)
            );
            
            plot.RenderManager.RenderStarting += (s, e) =>
            {
                Tick[] ticks = plot.Axes.Bottom.TickGenerator.Ticks;
                for (int i = 0; i < ticks.Length; i++)
                {
                    DateTime dt = DateTime.FromOADate(ticks[i].Position);
                    string label = $"{dt:MMM} {dt:yyyy}";
                    
                    ticks[i] = new Tick(ticks[i].Position, label);
                }
            };
            

            // Definir el path donde guardar el plot
            string plotDirectory = System.IO.Path.Combine(this._basePath, "plots");

            // Verificar si el directorio existe y crearlo si no es así
            if (!System.IO.Directory.Exists(plotDirectory))
            {
                System.IO.Directory.CreateDirectory(plotDirectory);
            }

            string pathToPlot = System.IO.Path
            .Combine(plotDirectory, "boxplot.png");
            plot.SavePng(pathToPlot, width, height);

            

            //return plot;
            return pathToPlot;
                        
        }

        private SP.Box AddBox(List<double> pValuesList, DateTime pDate, SP.Plot pPlot)
        {
            double[] valuesArr = pValuesList.ToArray();
            
            double change = (valuesArr[^1] - valuesArr[0]) / valuesArr[0] * 100;
            
            QuickSort(valuesArr, 0, valuesArr.Length - 1);

            SP.Color color = new();

            if (change >= 0) { color = SP.Color.FromHex("#38E54D"); }
            else { color = SP.Color.FromHex("#F94C10"); }

            SP.Box box = new()
            {
                Position = pDate.ToOADate(),
                WhiskerMax = valuesArr[^1],
                BoxMax = CalcQuertile(3, valuesArr),
                BoxMiddle = CalcQuertile(2, valuesArr),
                BoxMin = CalcQuertile(1, valuesArr),
                WhiskerMin = valuesArr[0],
                Width = 5,
                FillColor = color
            };

            pPlot.Add.Box(box);
            // If changed before added, will have a random color
            box.FillStyle.Color = color; 
            return box;
        }

        private double CalcQuertile(int quartile, double[] values) {
            
            double n = values.Length;

            double q = quartile*(n+1)/4;

            return values[(int)Math.Round(q)];
        }


        private void QuickSort(double[] arr, int low, int high) {

            if ( low < high ) {

                int pi = Partition(arr, low, high);

                QuickSort(arr, low, pi-1);
                QuickSort(arr, pi+1, high);
            }

        }

        private int Partition (double[] arr, int low, int high) {

            double pivot = arr[high]; // Pivot value

            int pivotActualIndex = low; //Index to put the pivot, also cant of elements before the pivot

            for (int j = low; j <= high - 1; j++) {

                if (arr[j] < pivot) {

                    pivotActualIndex++;
                    SwapValues(arr, pivotActualIndex-1, j);
                }
            }

            SwapValues(arr, pivotActualIndex, high);
            return pivotActualIndex;
        }

        private void SwapValues (double[] arr, int low, int high) {

            double aux = arr[high];
            arr[high] = arr[low];
            arr[low] = aux;
        } */