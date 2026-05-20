using System;

namespace AcGridGeneratorUi
{
    public class CarGridDisplayItem
    {
        private readonly CarAllocation _allocation;

        public CarGridDisplayItem(CarAllocation allocation, string carName)
        {
            _allocation = allocation ?? throw new ArgumentNullException(nameof(allocation));
            CarName = carName;
        }

        public string CarId => _allocation.CarId;
        public string CarName { get; }

        public int Count
        {
            get => _allocation.Count;
            set => _allocation.Count = Math.Max(1, value);
        }

        public int Ballast
        {
            get => _allocation.Ballast;
            set => _allocation.Ballast = Math.Max(0, value);
        }

        public int Restrictor
        {
            get => _allocation.Restrictor;
            set => _allocation.Restrictor = Math.Clamp(value, 0, 100);
        }

        public CarAllocation GetUnderlyingAllocation()
        {
            return _allocation;
        }
    }
}
