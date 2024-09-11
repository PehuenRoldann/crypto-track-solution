using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CryptoTrackApp.src.services
{
    public interface IPloterService
    {
        /// <summary>
        /// Generates a image with the plotbox.
        /// </summary>
        /// <param name="pCurrencyId"></param>
        /// <returns>Path to the plotbox generated image</returns>
        public Task<string> GetBoxPlot (Dictionary<DateTime, List<double>> valuesPerMonth, int width = 700, int heigh = 300);

        public Task<string> GetFinancialPlot(List<(DateTime, double)> history, int width = 700, int height = 300);

    }
}