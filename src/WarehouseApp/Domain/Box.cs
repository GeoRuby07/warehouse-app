// src/WarehouseApp/Domain/Box.cs
namespace WarehouseApp.Domain
{
    public class Box : StorageItem
    {
        private DateTime? _manufactureDate;
        /// <summary>
        /// Дата изготовления (nullable).
        /// </summary>
        public DateTime? ManufactureDate
        {
            get => _manufactureDate;
            init
            {
                if (value.HasValue && ExpirationDateInput.HasValue && value > ExpirationDateInput.Value)
                    throw new ArgumentException(
                        "ManufactureDate cannot be after ExpirationDateInput",
                        nameof(ManufactureDate));
                _manufactureDate = value;
            }
        }

        private DateTime? _expirationDateInput;
        /// <summary>
        /// Введённый срок годности (nullable).
        /// Если не задан, рассчитывается как ManufactureDate + 100 дней.
        /// </summary>
        public DateTime? ExpirationDateInput
        {
            get => _expirationDateInput;
            init
            {
                if (value.HasValue && ManufactureDate.HasValue && value < ManufactureDate.Value)
                    throw new ArgumentException(
                        "ExpirationDateInput cannot be earlier than ManufactureDate",
                        nameof(ExpirationDateInput));
                _expirationDateInput = value;
            }
        }

        /// <summary>
        /// Внешний ключ на паллету.
        /// </summary>
        public Guid? PalletId { get; set; }

        public override DateTime ExpirationDate
        {
            get
            {
                if (ExpirationDateInput.HasValue)
                    return ExpirationDateInput.Value;
                if (ManufactureDate.HasValue)
                    return ManufactureDate.Value.AddDays(100);
                throw new InvalidOperationException(
                    "Box must have either ManufactureDate or ExpirationDateInput");
            }
        }
    }
}
