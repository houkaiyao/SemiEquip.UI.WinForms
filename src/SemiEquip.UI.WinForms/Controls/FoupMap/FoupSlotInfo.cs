using System.Drawing;

namespace SemiEquip.UI.WinForms.Controls
{
    public sealed class FoupSlotInfo
    {
        public FoupSlotInfo(int slotNumber)
            : this(slotNumber, FoupSlotState.Empty, Color.White)
        {
        }

        public FoupSlotInfo(int slotNumber, FoupSlotState state, Color color)
        {
            SlotNumber = slotNumber;
            State = state;
            Color = color;
            WaferId = string.Empty;
            SlotData = string.Empty;
            IsSelected = false;
        }

        public int SlotNumber { get; private set; }

        public FoupSlotState State { get; internal set; }

        public Color Color { get; internal set; }

        public string WaferId { get; internal set; }

        public string SlotData { get; internal set; }

        public bool IsSelected { get; internal set; }
    }
}
