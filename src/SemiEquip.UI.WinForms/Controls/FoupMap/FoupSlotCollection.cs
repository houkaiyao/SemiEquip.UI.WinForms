using System;
using System.Collections.ObjectModel;

namespace SemiEquip.UI.WinForms.Controls
{
    public sealed class FoupSlotCollection : Collection<FoupSlotInfo>
    {
        private readonly FoupMapControl _owner;

        internal FoupSlotCollection(FoupMapControl owner)
        {
            _owner = owner ?? throw new ArgumentNullException("owner");
        }

        protected override void InsertItem(int index, FoupSlotInfo item)
        {
            ValidateItem(item);
            base.InsertItem(index, item);
            _owner.Invalidate();
        }

        protected override void SetItem(int index, FoupSlotInfo item)
        {
            ValidateItem(item);
            base.SetItem(index, item);
            _owner.Invalidate();
        }

        protected override void RemoveItem(int index)
        {
            base.RemoveItem(index);
            _owner.Invalidate();
        }

        protected override void ClearItems()
        {
            base.ClearItems();
            _owner.Invalidate();
        }

        private static void ValidateItem(FoupSlotInfo item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            if (item.SlotNumber < 1 || item.SlotNumber > FoupMapControl.MaxSlotCount)
            {
                throw new ArgumentOutOfRangeException("item", "SlotNumber 必须在 1 到 25 之间。");
            }
        }
    }
}
