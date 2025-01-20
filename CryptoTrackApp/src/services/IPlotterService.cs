using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CryptoTrackApp.src.services
{
    public interface IPlotterService
    {
        /* /// <summary>
        /// Generates a image with the plotbox.
        /// </summary>
        /// <param name="pCurrencyId"></param>
        /// <returns>Path to the plotbox generated image</returns>
        public Task<string> GetBoxPlot (Dictionary<DateTime, List<double>> valuesPerMonth, int width = 700, int heigh = 300); */

        /// <summary>
        /// Generates a jpeg with a candles plot for the given history.
        /// </summary>
        /// <param name="history">History of values</param>
        /// <param name="width">Width in px</param>
        /// <param name="height">Heigh in px</param>
        /// <returns>
        ///     - Path to the image 
        ///     - Empty string if there was an error.
        /// </returns>
        public Task<string> GetCandlesPlot(List<(DateTime, double)> history, int width = 700, int height = 300, string title = "Candle Plot!");

    }
}