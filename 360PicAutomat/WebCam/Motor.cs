
using System;
using Windows.Devices.Gpio;
using System.Diagnostics;
using Windows.Foundation;
using Windows.System.Threading;
using System.Threading.Tasks;

namespace PicAutomat
{
    public class Motor
    {
        private GpioPin _motorPin;
        private int _pinNumber = 5;
        private ThreadPoolTimer _timer;
        private int _pulseWidth;
        private int _steps;
        private Stopwatch _stopwatch;
        /// <summary>
        /// High Low Signal
        /// </summary>
        /// <param name="IN_Steps"></param>
        /// <param name="IN_PulseWidth">in ms</param>
        /// <returns></returns>
        public Motor()
        {
            _InitPin();
        }
        async public Task Run(int IN_Steps, int IN_PulseWidth)
        {
            _pulseWidth = IN_PulseWidth;
            _steps = IN_Steps;
            _stopwatch = Stopwatch.StartNew();
            _timer = ThreadPoolTimer.CreatePeriodicTimer(this._Tick, TimeSpan.FromSeconds(2));
            await ThreadPool.RunAsync(this._MotorThread, WorkItemPriority.High);
        }

        private void _InitPin()
        {
            GpioController tmpController = GpioController.GetDefault();
            _motorPin = tmpController.OpenPin(_pinNumber);
            _motorPin.SetDriveMode(GpioPinDriveMode.Output);
        }

        private void _MotorThread(IAsyncAction IN_Action)
        {
            for (int i = 0; i < _steps; i++)
            {
                _motorPin.Write(GpioPinValue.High);
                _Wait(_pulseWidth);

                _motorPin.Write(GpioPinValue.Low);
                _Wait(_pulseWidth);
            }
        }
        private void _Tick(ThreadPoolTimer ÍN_Timer)
        {
        }
        private void _Wait(double IN_Milliseconds)
        {
            long tmpInitialTick = _stopwatch.ElapsedTicks;
            long tmpInitialElapsed = _stopwatch.ElapsedMilliseconds;
            double tmpDesiredTicks = IN_Milliseconds / 1000.0 * Stopwatch.Frequency;
            double tmpFinalTick = tmpInitialTick + tmpDesiredTicks;
            while (_stopwatch.ElapsedTicks < tmpFinalTick)
            {

            }
        }
    }
}