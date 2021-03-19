﻿using System;
using System.Linq.Expressions;

namespace UltraMapper.MappingExpressionBuilders
{
    public class DictionaryMapperContext : CollectionMapperContext
    {
        public ParameterExpression SourceCollectionElementKey { get; private set; }
        public ParameterExpression SourceCollectionElementValue { get; private set; }

        public ParameterExpression TargetCollectionElementKey { get; private set; }
        public ParameterExpression TargetCollectionElementValue { get; private set; }

        public DictionaryMapperContext( Type source, Type target, IMappingOptions options )
            : base( source, target, options )
        {
            var sourceCollectionElementKeyType = SourceCollectionElementType.GetGenericArguments()[ 0 ];
            var sourceCollectionElementValueType = SourceCollectionElementType.GetGenericArguments()[ 1 ];

            var targetCollectionElementKeyType = TargetCollectionElementType.GetGenericArguments()[ 0 ];
            var targetCollectionElementValueType = TargetCollectionElementType.GetGenericArguments()[ 1 ];

            SourceCollectionElementKey = Expression.Variable( sourceCollectionElementKeyType, "sourceKey" );
            SourceCollectionElementValue = Expression.Variable( sourceCollectionElementValueType, "sourceValue" );

            TargetCollectionElementKey = Expression.Variable( targetCollectionElementKeyType, "targetKey" );
            TargetCollectionElementValue = Expression.Variable( targetCollectionElementValueType, "targetValue" );
        }
    }
}
