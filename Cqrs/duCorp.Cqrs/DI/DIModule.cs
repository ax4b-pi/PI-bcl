using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using FluentValidation;
using MediatR;
using Autofac;
using Module = Autofac.Module;
using DuCorp.Cqrs.Validators;
using DuCorp.Cqrs.Commands;

namespace DuCorp.Cqrs.DI
{
    public class DIModule : Module
    {
        private readonly Assembly _applicationAssembly;

        private DIModule(Assembly applicationAssembly)
        {
            _applicationAssembly = applicationAssembly;
        }

        public static DIModule Create(Assembly applicationAssembly)
            => new DIModule(applicationAssembly);

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof(IMediator).GetTypeInfo().Assembly)
                .AsImplementedInterfaces();

            builder.RegisterAssemblyTypes(_applicationAssembly)
                .AsClosedTypesOf(typeof(IRequestHandler<,>));

            var handlers = _applicationAssembly.GetTypes()
                .Where(t => t.IsClosedTypeOf(typeof(IRequestHandler<,>)))
                .ToList();

            handlers.ForEach(t =>
            {
                var localHandlers = t.GetInterfaces()
                 .Where(@interface => @interface.IsClosedTypeOf(typeof(IRequestHandler<,>)));

                foreach (var localHandler in localHandlers)
                {
                    var implementation = typeof(IdentifiedCommandHandler<,>)
                     .MakeGenericType(localHandler.GenericTypeArguments);

                    var arg0 = typeof(IdentifiedCommand<,>)
                     .MakeGenericType(localHandler.GenericTypeArguments);
                    var arg1 = localHandler.GenericTypeArguments[1];

                    var service = typeof(IRequestHandler<,>)
                     .MakeGenericType(arg0, arg1);

                    builder.RegisterType(implementation).As(service);
                }
            });

            var sharedLogicMediatr = typeof(IdentifiedCommand<,>).GetTypeInfo().Assembly;
            builder.RegisterAssemblyTypes(sharedLogicMediatr)
                .AsClosedTypesOf(typeof(IRequestHandler<,>));

            builder
                .RegisterAssemblyTypes(sharedLogicMediatr)
                .Where(t => t.IsClosedTypeOf(typeof(IValidator<>)))
                .AsImplementedInterfaces();

            builder
                .RegisterAssemblyTypes(_applicationAssembly)
                .AsClosedTypesOf(typeof(IValidator<>));

            builder.RegisterAssemblyTypes(_applicationAssembly)
                .AsClosedTypesOf(typeof(INotificationHandler<>));

            builder.Register<ServiceFactory>(context =>
            {
                var componentContext = context.Resolve<IComponentContext>();
                return t =>
             {
                   return componentContext.TryResolve(t, out var o) ? o : null;
               };
            });

            builder.RegisterGeneric(typeof(ValidatorsBehavior<,>)).As(typeof(IPipelineBehavior<,>));
        }
    }
}
