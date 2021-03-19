﻿using System;
using System.Linq.Expressions;

namespace UltraMapper.MappingExpressionBuilders
{
    public abstract class PrimitiveMapperBase : IMappingExpressionBuilder
    {
        protected readonly Configuration MapperConfiguration;

        public PrimitiveMapperBase( Configuration configuration )
        {
            this.MapperConfiguration = configuration;
        }

        public LambdaExpression GetMappingExpression( Type sourceType, Type targetType, IMappingOptions options )
        {
            var context = this.GetContext( sourceType, targetType, options );
            var getValueExpression = this.GetValueExpression( context );

            var delegateType = typeof( Func<,> )
                .MakeGenericType( sourceType, targetType );

            return Expression.Lambda( delegateType,
                getValueExpression, context.SourceInstance );
        }

        protected virtual MapperContext GetContext( Type sourceType, Type targetType, IMappingOptions options )
        {
            return new MapperContext( sourceType, targetType, options );
        }

        public abstract bool CanHandle( Type sourceType, Type targetType );

        protected abstract Expression GetValueExpression( MapperContext context );
    }
}
