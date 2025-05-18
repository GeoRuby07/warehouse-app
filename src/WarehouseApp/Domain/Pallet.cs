using System;
using System.Collections.Generic;
using System.Linq;

namespace WarehouseApp.Domain {
    public class Pallet : StorageItem {
        /// <summary>
        /// Список коробок на паллете.
        /// </summary>
        public List<Box> Boxes { get; init; }

        /// <summary>
        /// Создаёт паллету с указанными размерами и списком коробок.
        /// </summary>
        /// <param name="width">Ширина паллеты.</param>
        /// <param name="height">Высота паллеты.</param>
        /// <param name="depth">Глубина паллеты.</param>
        /// <param name="boxes">Коробки, которые на ней лежат.</param>
        public Pallet(decimal width, decimal height, decimal depth, IEnumerable<Box> boxes)
        {
            Width = width;
            Height = height;
            Depth = depth;

            Boxes = boxes?.ToList()
                ?? throw new ArgumentNullException(nameof(boxes));

            // проверяем, что каждая коробка помещается по габаритам
            foreach (var box in Boxes)
            {
                if (box.Width > Width ||
                    box.Depth > Depth)
                {
                    throw new ArgumentException(
                        $"Box {box.Id} ({box.Width}×{box.Depth}) " +
                        $"does not fit into pallet {Width}×{Depth}");
                }
            }

            // вес паллеты = сумма весов коробок + 30 кг (собственный вес)
            Weight = Boxes.Sum(b => b.Weight) + 30m;
        }

        /// <summary>
        /// Срок годности паллеты — минимальный из сроков годности коробок.
        /// </summary>
        public override DateTime ExpirationDate =>
            Boxes.Min(b => b.ExpirationDate);

        /// <summary>
        /// Объём паллеты — её собственный объём + суммарный объём коробок.
        /// </summary>
        public override decimal Volume =>
            (Width * Height * Depth) + Boxes.Sum(b => b.Volume);
    }
}
