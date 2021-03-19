﻿using System;
using System.Linq.Expressions;
using UltraMapper.Internals;

namespace UltraMapper.MappingExpressionBuilders
{
    public sealed class BuiltInTypeMapper : PrimitiveMapperBase
    {
        public BuiltInTypeMapper( Configuration configuration )
            : base( configuration ) { }

        public override bool CanHandle( Type source, Type target )
        {
            bool areTypesBuiltIn = source.IsBuiltIn( false )
                && target.IsBuiltIn( false );

            return (areTypesBuiltIn ) && (source == target ||
                source.IsImplicitlyConvertibleTo( target ) ||
                source.IsExplicitlyConvertibleTo( target ));
        }

        protected override Expression GetValueExpression( MapperContext context )
        {
            if( context.SourceInstance.Type == context.TargetInstance.Type )
                return context.SourceInstance;

            return Expression.Convert( context.SourceInstance,
                context.TargetInstance.Type );
        }
    }
}
