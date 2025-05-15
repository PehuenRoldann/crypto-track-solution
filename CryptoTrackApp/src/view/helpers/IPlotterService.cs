using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CryptoTrackApp.src.view.helpers
{
    public interface IPlotterService
    {

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