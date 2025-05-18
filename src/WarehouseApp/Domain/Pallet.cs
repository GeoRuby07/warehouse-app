using System;
using System.Collections.Generic;
using System.Linq;

namespace WarehouseApp.Domain {
    public class Pallet : StorageItem {
        public List<Box> Boxes { get; init; } = [];

        public Pallet(IEnumerable<Box> boxes)
        {
            Boxes = [.. boxes];
            // валидация: каждая коробка должна помещаться по ширине и глубине
            foreach (var box in Boxes)
            {
                if (box.Width > Width || box.Depth > Depth)
                    throw new ArgumentException("Box dimensions exceed pallet dimensions");
            }
            // вес паллеты = сумма весов коробок + 30 кг
            Weight = Boxes.Sum(b => b.Weight) + 30m;
        }

        public override DateTime ExpirationDate =>
            Boxes.Min(b => b.ExpirationDate);

        public override decimal Volume =>
            Boxes.Sum(b => b.Volume) + (Width * Height * Depth);
    }
}
