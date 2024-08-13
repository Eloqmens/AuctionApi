using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.Lot.Delete
{
    public class DeleteLotCommandValidator : AbstractValidator<DeleteLotCommand>
    {
        public DeleteLotCommandValidator()
        {
            RuleFor(deleteNoteCommand => deleteNoteCommand.Id).NotEmpty();
            RuleFor(deleteNoteCommand => deleteNoteCommand.UserId).NotEmpty();
        }
    }
}
