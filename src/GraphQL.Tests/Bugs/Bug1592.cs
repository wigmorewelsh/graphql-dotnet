using System;
using GraphQL.StarWars.IoC;
using GraphQL.Types;
using Xunit;

namespace GraphQL.Tests.Bugs
{
    public class Bug1592
    {
        [Fact]
        public void duplicated_type_names_should_throw_error()
        {
            var services = new SimpleContainer();
            //Create a derived type FooType2 for FooType in type resolving
             services.Register<FooType>(() => new FooType2());
            var schema = new Bug1592Schema(new SimpleContainerAdapter(services));

            schema.Initialize();
        }
    }

    public class Foo
    {
        public string Name { get; set; }
    }

    public class FooType : ObjectGraphType<Foo>
    {
        public FooType()
        {
            Name = "Foo";
            Field(x => x.Name);
            //This nested field cause 'A loop has been detected while registering schema types.' exception in GraphTypesLookup.AddTypeWithLoopCheck
            //Field<FooType>("parent", resolve: (ctx) => new Foo());
        }
    }

    public class FooType2 : FooType
    {
    }

    public class RootQuery : ObjectGraphType<object>
    {
        public RootQuery()
        {
            Field<FooType>(
                "foo",
                resolve: context => new FooType() { }
            );
        }
    }

    public class Bug1592Schema : Schema
    {
        public Bug1592Schema(IServiceProvider provider)
            : base(provider)
        {
            Query = new RootQuery();
        }
    }
}
