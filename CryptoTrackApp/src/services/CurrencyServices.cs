using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Intrinsics.X86;
using System.Threading.Tasks;
using CryptoTrackApp.src.db;
using CryptoTrackApp.src.models;
using ScottPlot;
using ScottPlot.AxisPanels;

// using GLib;
using ScottPlot.Plottables;
using ScottPlot.TickGenerators;
using ScottPlot.TickGenerators.TimeUnits;
using SP = ScottPlot;

namespace CryptoTrackApp.src.services
{

    public class CurrencyServices : ICurrencyServices
    {
        private ICryptoApi cryptoApi;
        private List <(DateTime, double)> _historyValues;

        public CurrencyServices()
        {

            this.cryptoApi = new CoinApi();
        }

//----------------------- AUXILIAR METHODS ----------------------------------------------------------

        private IDictionary<string, string> CurrencyToDictionary (Currency pCurrency)
        {
            
            IDictionary<string, string> currencyData = new Dictionary<string, string>();

            foreach (var property in pCurrency.GetType().GetProperties()) {

                if (property.GetValue(pCurrency) == null)
                {
                    currencyData.Add(property.Name, "");
                }
                else
                {
                    currencyData.Add(property.Name, property.GetValue(pCurrency).ToString());
                }
                
            }

            return currencyData;
        }

        private IDictionary<string, string>[] CurrencyToDictionary(Currency[] pCurrencies)
        {

            return pCurrencies.Select(item => {

                return this.CurrencyToDictionary(item);

            }).ToArray();

        }

// ------------------- INTERFACE IMPLEMENTATIONS -------------------------------------------------

        /// <summary>
        /// Returns a currency with the given Id.
        /// </summary>
        /// <param name="pCurrencyId">Id of the currency we want to return</param>
        /// <returns>
        /// IDictionary<string, string> with the currency inforreturn "";mation.
        /// </returns>
        public async Task<IDictionary<string, string>> GetCurrency(string pCurrencyId)
        {

            try {
                Currency currency = await this.cryptoApi.GetCurrency(pCurrencyId);
                return this.CurrencyToDictionary(currency);
            }
            catch (Exception error) {

                Console.WriteLine($"CurrencyService-Error: {error.Message}");
                throw error;
            }

        }

        /// <summary>
        /// Returs a collection with the information of crypto currencies.
        /// </summary>
        /// <param name="pOffset">The number of currencies we have already queried before.</param>
        /// <param name="pLimit">The number of currencies we want to receive.</param>
        /// <returns>
        /// A collection if Dictionary<string, string> with the currency information as an Array.
        /// </returns>
        /// <exception cref="Exception">
        /// If the query fails for some reason.
        /// </exception>
        public async Task<IDictionary<string, string>[]> GetCurrencies(int pOffset = 0, int pLimit = 100)
        {
            try {

                Currency[] currencyData = await this.cryptoApi.GetCurrencies(offset: pOffset, limit: pLimit);
                return this.CurrencyToDictionary(currencyData);
            }
            catch (Exception error) {
                Console.WriteLine($"CurrencyService-Error: {error.Message}");
                throw new Exception(error.Message);
            }
        }

        public async Task<IDictionary<string, string>[]> GetCurrencies(string[] pIds)
        {
            try {

                Currency[] currencyData = await this.cryptoApi.GetCurrencies(pIds);
                return this.CurrencyToDictionary(currencyData);
            }
            catch (Exception error) {
                Console.WriteLine($"CurrencyService-Error: {error.Message}");
                return new Dictionary<string, string>[0];
            }
        }

        public async Task<List<(DateTime, double)>> GetHistory(string pCurrencyId)
        {
            try
            {
                List<(DateTime, double)> historyValues = await this.cryptoApi.GetHistory(pCurrencyId);

                // 182 days aprox. to 6 months:
                // return historyValues.TakeLast(182).ToList();
                return historyValues.ToList();

            }
            catch (Exception error)
            {

                Console.WriteLine($"CurrencyService-Error: {error.Message}");
                throw new Exception(error.Message);
            }

            
        }


        /// <summary>
        /// Generates a image with the plotbox.
        /// </summary>
        /// <param name="pCurrencyId"></param>
        /// <returns>Path to the plotbox generated image</returns>
        public async Task<string> GetBoxPlot (string pCurrencyId) 
        {
            this._historyValues = await GetHistory(pCurrencyId);
            DateTime actualDate = DateTime.UtcNow;

            Dictionary<DateTime, List<double>> valuesPerMonth = new();


            for (var mes = 6; mes > 0; mes--) {
                valuesPerMonth.Add(actualDate.AddMonths(-mes).AddDays(1-actualDate.Day), new List<double>());
                
                valuesPerMonth.Add(actualDate.AddMonths(-mes).AddDays(1-actualDate.Day).AddDays(14), new List<double>()); // TESTING
            }
            this._historyValues = this._historyValues.Where((elem) => {return valuesPerMonth.Keys.First() <= elem.Item1;}).ToList();
            // Gets the values for each month
            foreach (var value in this._historyValues) {
                var keyToCheck = new DateTime(year: value.Item1.Year, month: value.Item1.Month, day: value.Item1.Day).TimeOfDay;

                foreach (DateTime keyDate in valuesPerMonth.Keys) {

                    if (keyDate.Month == value.Item1.Month && keyDate.Year == value.Item1.Year && 
                    keyDate <= value.Item1 && keyDate.AddDays(14) > value.Item1) {
                        valuesPerMonth[keyDate].Add(Math.Round(value.Item2, 2));
                    }
                }
            }

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
                new Day(), 14,
                dt => new DateTime(dt.Year, dt.Month, 1)
            );
            
            /* plot.RenderManager.RenderStarting += (s, e) =>
            {
                Tick[] ticks = plot.Axes.Bottom.TickGenerator.Ticks;
                for (int i = 0; i < ticks.Length; i++)
                {
                    DateTime dt = DateTime.FromOADate(ticks[i].Position);
                    string label = $"{dt:MMM} {dt:yyyy}";
                    ticks[i] = new Tick(ticks[i].Position, label);
                }
            }; */
            


            //plot.SavePng("/home/pehuen/Desktop/demo.png", 800, 600);
            plot.SaveSvg($"/home/pehuen/Desktop/demo_" +
                         $"{pCurrencyId}_{DateTime.Today.Year}{DateTime.Today.Month}.svg",
                800, 600);
            return "";
                        
        }

        private SP.Box AddBox(List<double> pValuesList, DateTime pDate, SP.Plot pPlot)
        {
            double[] valuesArr = pValuesList.ToArray();
            
            double change = (valuesArr[^1] - valuesArr[0]) / valuesArr[0] * 100;
            
            QuickSort(valuesArr, 0, valuesArr.Length - 1);

            // double change = (valuesArr[^1] - valuesArr[0]) / valuesArr[0] * 100;

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
            // box.FillColor = SP.Colors.Red;
            pPlot.Add.Box(box);
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
        }
    }
}