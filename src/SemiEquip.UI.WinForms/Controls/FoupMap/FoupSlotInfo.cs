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
        }

        public int SlotNumber { get; private set; }

        public FoupSlotState State { get; set; }

        public Color Color { get; set; }
    }
}
