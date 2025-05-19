namespace WarehouseApp.Domain
{
    public abstract class StorageItem
    {
        public Guid Id { get; init; }

        private decimal _width;
        public decimal Width
        {
            get => _width;
            init
            {
                if (value <= 0)
                    throw new ArgumentException("Width must be greater than zero", nameof(Width));
                _width = value;
            }
        }

        private decimal _height;
        public decimal Height
        {
            get => _height;
            init
            {
                if (value <= 0)
                    throw new ArgumentException("Height must be greater than zero", nameof(Height));
                _height = value;
            }
        }

        private decimal _depth;
        public decimal Depth
        {
            get => _depth;
            init
            {
                if (value <= 0)
                    throw new ArgumentException("Depth must be greater than zero", nameof(Depth));
                _depth = value;
            }
        }

        private decimal _weight;
        public decimal Weight
        {
            get => _weight;
            init
            {
                if (value < 0)
                    throw new ArgumentException("Weight cannot be negative", nameof(Weight));
                _weight = value;
            }
        }

        /// <summary>
        /// Объём: Width x Height x Depth
        /// </summary>
        public virtual decimal Volume => Width * Height * Depth;

        /// <summary>
        /// Срок годности.
        /// </summary>
        public abstract DateTime ExpirationDate { get; }
    }
}
