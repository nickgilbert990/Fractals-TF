using System;
using cAlgo.API;
using cAlgo.API.Internals;
using cAlgo.API.Indicators;
using cAlgo.Indicators;

namespace cAlgo
{
    [Indicator(IsOverlay = true, TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class FractalsTF : Indicator, Robot
    {

        [Parameter()]
        public TimeFrame FractalTimeFrame { get; set; }

        [Output("Top Fractal", Color = Colors.Blue, PlotType = PlotType.Points, Thickness = 5)]
        public IndicatorDataSeries TopFractal { get; set; }

        [Output("Bottom Fractal", Color = Colors.Blue, PlotType = PlotType.Points, Thickness = 5)]
        public IndicatorDataSeries BottomFractal { get; set; }

        protected MarketSeries _source;
        private int _lookback = 3;


        protected override void Initialize()
        {

            // Initialize and create nested indicators
            _source = this.MarketData.GetSeries(FractalTimeFrame);
        }

        public override void Calculate(int index)
        {
            DrawUpFractal(index, _lookback, TopFractal);
            DrawDownFractal(index, _lookback, BottomFractal);
        }

        protected void DrawUpFractal(int index, int period, IndicatorDataSeries series)
        {
            var sourceIdx = _source.OpenTime.GetIndexByTime(this.MarketSeries.OpenTime[index]);

            int highIdx = -1;
            double highPoint = double.MinValue;
            int cnt = 0;
            while (cnt < period)
            {
                int idx = sourceIdx - cnt;
                if (_source.High[idx] > highPoint)
                {
                    highPoint = _source.High[idx];
                    highIdx = idx;
                }
                cnt++;
            }
            if (_source.High[highIdx - 1] < highPoint && _source.High[highIdx + 1] < highPoint)
            {
                // We're scaling between timeframes, so we have to find the exact index
                // of the price on the chart timeframe.
                int outPutIdx = this.MarketSeries.OpenTime.GetIndexByTime(_source.OpenTime[highIdx]);
                double currerntHigh = this.MarketSeries.High[outPutIdx];
                while (currerntHigh != highPoint && outPutIdx < this.MarketSeries.High.Count)
                {
                    currerntHigh = this.MarketSeries.High[++outPutIdx];
                    series[outPutIdx] = double.NaN;
                }
                series[outPutIdx] = highPoint;
            }
        }

        protected void DrawDownFractal(int index, int period, IndicatorDataSeries series)
        {
            var sourceIdx = _source.OpenTime.GetIndexByTime(this.MarketSeries.OpenTime[index]);

            int lowIdx = -1;
            double lowPoint = double.MaxValue;
            int cnt = 0;
            while (cnt < period)
            {
                int idx = sourceIdx - cnt;
                series[idx] = double.NaN;
                if (_source.Low[idx] < lowPoint)
                {
                    lowPoint = _source.Low[idx];
                    lowIdx = idx;
                }
                cnt++;
            }
            if (_source.Low[lowIdx - 1] > lowPoint && _source.Low[lowIdx + 1] > lowPoint)
            {
                // We're scaling between timeframes, so we have to find the exact index
                // of the price on the chart timeframe.
                int outPutIdx = this.MarketSeries.OpenTime.GetIndexByTime(_source.OpenTime[lowIdx]);
                double currerntLow = this.MarketSeries.Low[outPutIdx];
                while (currerntLow != lowPoint && outPutIdx < this.MarketSeries.Low.Count)
                {
                    currerntLow = this.MarketSeries.Low[++outPutIdx];
                    series[outPutIdx] = double.NaN;
                }
                series[outPutIdx] = lowPoint;
            }
        }
    }
}
