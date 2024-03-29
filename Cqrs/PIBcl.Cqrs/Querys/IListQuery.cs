﻿using MediatR;
using System.Collections;

namespace PIBcl.Cqrs.Querys
{
   public interface IListQuery<out T> : IRequest<T> where T: IEnumerable
   {
      int PageNumber { get; set; }
      int PageSize { get; set; }
   }

   public interface IListQuery : IListQuery<IEnumerable>
   {
   }
}
