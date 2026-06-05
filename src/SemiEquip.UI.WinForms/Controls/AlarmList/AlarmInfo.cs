using System;

namespace SemiEquip.UI.WinForms.Controls
{
    public class AlarmInfo
    {
        public AlarmInfo()
        {
        }

        public AlarmInfo(string alarmId, string alarmEvent, string alarmDescription, AlarmLevel alarmLevel)
        {
            AlarmId = alarmId;
            AlarmEvent = alarmEvent;
            AlarmDescription = alarmDescription;
            AlarmLevel = alarmLevel;
        }

        public string AlarmId { get; set; }

        public string AlarmEvent { get; set; }

        public string AlarmDescription { get; set; }

        public AlarmLevel AlarmLevel { get; set; }

        public DateTime OccurTime { get; set; }
    }
}
