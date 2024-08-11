using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.Wallet.AddWallet
{
    public class AddCardCommand : IRequest<Unit>
    {
        public string UserId { get; set; }
        public string CardNumber { get; set; }
    }
}
