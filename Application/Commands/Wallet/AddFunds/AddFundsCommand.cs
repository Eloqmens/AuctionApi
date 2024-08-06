using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.Wallet.AddFunds
{
    public class AddFundsCommand : IRequest<Unit>
    {
        public string UserId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethodId { get; set; }
    }
}
