using System;

namespace WarehouseApp.Domain {
    public abstract class StorageItem {
        public Guid Id { get; init; }
        public decimal Width { get; init; }
        public decimal Height { get; init; }
        public decimal Depth { get; init; }
        public decimal Weight { get; protected set; }

        /// <summary>
        /// Объём: Width × Height × Depth
        /// </summary>
        public virtual decimal Volume => Width * Height * Depth;

        /// <summary>
        /// Срок годности.
        /// </summary>
        public abstract DateTime ExpirationDate { get; }
    }
}
