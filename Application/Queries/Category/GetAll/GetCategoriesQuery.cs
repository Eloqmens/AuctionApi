using Core.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries.Category.GetAll
{
    public class GetCategoriesQuery : IRequest<List<Core.Entities.Category>>
    {
    }
}
