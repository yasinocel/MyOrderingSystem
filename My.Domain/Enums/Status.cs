using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace My.Domain.Enums
{
    public enum Status
    {
        OrderSubmitted,
        OrderValidated,
        OrderOutOfStock,
        PaymentProcessed,
        PaymentFailed,
        OrderDispatched
    }
}
