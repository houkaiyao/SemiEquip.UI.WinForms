using System;
using System.Drawing;
using System.Windows.Forms;

namespace SemiEquip.UI.WinForms.Controls
{
    public sealed class FoupSlotClickEventArgs : EventArgs
    {
        public FoupSlotClickEventArgs(int slotNumber, MouseButtons button, Point location)
        {
            SlotNumber = slotNumber;
            Button = button;
            Location = location;
        }

        public int SlotNumber { get; private set; }

        public MouseButtons Button { get; private set; }

        public Point Location { get; private set; }
    }
}
