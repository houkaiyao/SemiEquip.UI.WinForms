using System;

namespace SemiEquip.UI.WinForms.Controls
{
    public class AlarmEventArgs : EventArgs
    {
        public AlarmEventArgs(AlarmInfo alarm)
        {
            Alarm = alarm;
        }

        public AlarmInfo Alarm { get; private set; }
    }
}
