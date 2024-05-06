using System;

namespace Script.BonusGame.GamePlay.Wheel
{
    public class StatusChangeEventArgs : EventArgs
    {
        public Wheel.Status From;
        public Wheel.Status To;

       public StatusChangeEventArgs( Wheel.Status from,Wheel.Status to)
       {
           From = from;
           To = to;
       }

    }
}