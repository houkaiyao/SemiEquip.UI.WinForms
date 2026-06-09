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
            ValidateDuplicateSlotNumber(item, -1);
            base.InsertItem(index, item);
            _owner.Invalidate();
        }

        protected override void SetItem(int index, FoupSlotInfo item)
        {
            ValidateItem(item);
            ValidateDuplicateSlotNumber(item, index);
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

        private void ValidateItem(FoupSlotInfo item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            if (item.SlotNumber < 1 || item.SlotNumber > _owner.SlotCount)
            {
                throw new ArgumentOutOfRangeException("item", "SlotNumber 必须在当前 SlotCount 范围内。");
            }
        }

        private void ValidateDuplicateSlotNumber(FoupSlotInfo item, int ignoredIndex)
        {
            for (int index = 0; index < Count; index++)
            {
                if (index != ignoredIndex && this[index].SlotNumber == item.SlotNumber)
                {
                    throw new ArgumentException("不能添加重复 SlotNumber 的槽位。", "item");
                }
            }
        }
    }
}
