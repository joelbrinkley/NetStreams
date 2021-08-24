using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace NetStreams.Internal
{
    internal class Throttler
    {
        private Stopwatch _sw;

        private TimeSpan _timeSpan;

        private bool _requestSent = false;

        private void Toggle()
        {
            _requestSent = !_requestSent;
        }

        public Throttler(TimeSpan timeSpan)
        {
            _sw = new Stopwatch();
            _sw.Start();
            _timeSpan = timeSpan;
        }

        internal void Throttle(Action action)
        {
            if(_sw.Elapsed > _timeSpan && !_requestSent)
            {
                action();
                Toggle();
                return;
            }

            if(_sw.Elapsed > _timeSpan && _requestSent)
            {
                Toggle();
                _sw.Restart();
            }
        }
    }
}
