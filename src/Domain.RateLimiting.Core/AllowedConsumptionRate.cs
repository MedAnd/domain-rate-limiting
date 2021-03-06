using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain.RateLimiting.Core
{
    /// <summary>
    /// Rate Limit Policy Attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class AllowedConsumptionRate : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Attribute" /> class.
        /// </summary>
        /// <param name="limit">The limit.</param>
        /// <param name="unit">The unit.</param>
        public AllowedConsumptionRate(int limit, RateLimitUnit unit)
        {
            if (limit <= 0)
                throw new ArgumentOutOfRangeException($"{nameof(limit)} has to be greater than 0");

            Limit = limit;
            Unit = unit;
        }
        
        public AllowedConsumptionRate(int limit, LimitPeriod period):this(limit, RateLimitUnit.PerCustomPeriod)
        {
            Period = period;
        }

        public AllowedConsumptionRate(int limit, RateLimitUnit unit, int maxBurst) : this(limit, unit)
        {
            if (maxBurst <= 0)
                throw new ArgumentOutOfRangeException($"{nameof(maxBurst)} has to be greater than 0");

            MaxBurst = maxBurst;
        }

        public AllowedConsumptionRate(int limit, LimitPeriod period, int maxBurst) : this(limit, RateLimitUnit.PerCustomPeriod, maxBurst)
        {
            Period = period;
        }

        /// <summary>
        /// Gets the limit.
        /// </summary>
        /// <value>The limit.</value>
        public int Limit { get; }

        /// <summary>
        /// Gets the unit.
        /// </summary>
        /// <value>The unit.</value>
        public RateLimitUnit Unit { get; }
        
        public LimitPeriod Period { get; }
        public int MaxBurst { get; }

        public override string ToString()
        {
            var durationInTicks = Unit != RateLimitUnit.PerCustomPeriod ? ((long)Unit) : Period.Duration.Ticks;
            return $"{Limit} tokens per { durationInTicks / TimeSpan.TicksPerSecond  } seconds";
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;

            var compareObj = (AllowedConsumptionRate)obj;

            return compareObj.ToString() == ToString();
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
    }
}