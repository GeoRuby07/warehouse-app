using System;

namespace WarehouseApp.Domain
{
    public class Box : StorageItem
    {
        /// <summary>
        /// Дата изготовления (nullable).
        /// </summary>
        public DateTime? ManufactureDate { get; init; }

        /// <summary>
        /// Введённый срок годности (nullable).
        /// Если не задан, рассчитывается как ManufactureDate + 100 дней.
        /// </summary>
        public DateTime? ExpirationDateInput { get; init; }

        public override DateTime ExpirationDate
        {
            get
            {
                if (ExpirationDateInput.HasValue)
                    return ExpirationDateInput.Value;
                if (ManufactureDate.HasValue)
                    return ManufactureDate.Value.AddDays(100);
                throw new InvalidOperationException("Box must have either ManufactureDate or ExpirationDateInput");
            }
        }
    }
}
