using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CryptoTrackApp.src.utils;
using Microsoft.EntityFrameworkCore.Migrations;
using Pango;
using ScottPlot;
using ScottPlot.Panels;
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

        private const string PLOT_BG_COLOR_HEX = "#2A2E32";



        public async Task<string> GetCandlesPlot(List<(DateTime, double)> history, int width = 700, int height = 300, string title = "Candle") {

            _logger.Log($"[EXEC - Operation GetFinancialPlot at PloterService - Parameters: [width: {width}; height: {height} ]]");
            var targetDate = DateTime.UtcNow.AddMonths(-6);
            history = history.Where(tupla => tupla.Item1 >= targetDate).ToList();

            DateTime initialDate = history[0].Item1;
            TimeSpan timeSpan = new(days: 7, 0, 0, 0);
            DateTime todayDateFull = DateTime.UtcNow;
            DateTime dateToday = new DateTime(year: todayDateFull.Year, month: todayDateFull.Month, day: todayDateFull.Day + 1);
            

            List<OHLC> prices = new();

            try {


                for (
                    DateTime currentDate = initialDate;
                    currentDate < dateToday;
                    currentDate = currentDate.AddDays(timeSpan.Days)) {
                    
                    double[] values = history
                    .Where(tupla => tupla.Item1 >= currentDate 
                        && tupla.Item1 < currentDate.AddDays(timeSpan.Days))
                    .Select(item => item.Item2)
                    .ToArray();

                    if (values.Length != 0) {
                        double open = values[0];
                        double high = values.Max();
                        double low = values.Min();
                        int closeArrIndex = values.Length - 1;
                        double close = values[closeArrIndex];

                        prices.Add(new OHLC(open, high, low, close, currentDate, timeSpan));

                    }

                    
                }

                Plot plot = new();
                // plot.Add.Palette = new SP.Palettes.Penumbra();

                 
                plot.Add.Candlestick(prices);
                plot.Axes.DateTimeTicksBottom();

                // Estilizar plot
                plot.Font.Set(ScottPlot.Fonts.Serif);
                plot.FigureBackground.Color = SP.Color.FromHex(PLOT_BG_COLOR_HEX);
                plot.Axes.Color(SP.Color.FromHex("#d7d7d7"));
                plot.Title(title, 16);
                plot.YLabel("Value in USD");

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
