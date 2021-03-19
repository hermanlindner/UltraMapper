﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using UltraMapper.Internals;
using UltraMapper.Internals.ExtensionMethods;
using UltraMapper.MappingExpressionBuilders.MapperContexts;

namespace UltraMapper.MappingExpressionBuilders
{
    /*Stack<T> and other LIFO collections require the list to be read in reverse  
    * to preserve order and have a specular clone */
    public class StackMapper : CollectionMappingViaTempCollection
    {
        //protected override bool IsCopySourceToTempCollection => true;
        //protected override bool IsCopyTargetToTempCollection => true;

        public StackMapper( Configuration configuration )
            : base( configuration ) { }

        public override bool CanHandle( Type source, Type target )
        {
            return base.CanHandle( source, target ) &&
                target.IsCollectionOfType( typeof( Stack<> ) );
        }

        protected override MethodInfo GetTargetCollectionInsertionMethod( CollectionMapperContext context )
        {
            //It is forbidden to use nameof with unbound generic types. We use 'int' just to get around that.
            var methodName = nameof( Stack<int>.Push );
            var methodParams = new[] { context.TargetCollectionElementType };

            return context.TargetInstance.Type.GetMethod( methodName, methodParams );
        }

        protected override Type GetTemporaryCollectionType( CollectionMapperContext context )
        {
            //by copying data in a temp stack and then in the target collection 
            //the correct order of the items is preserved
            return typeof( Stack<> ).MakeGenericType( context.SourceCollectionElementType );
        }

        protected override Expression GetNewInstanceFromSourceCollection( MemberMappingContext context, CollectionMapperContext collectionContext )
        {
            var targetConstructor = context.TargetMember.Type.GetConstructor(
               new[] { typeof( IEnumerable<> ).MakeGenericType( collectionContext.TargetCollectionElementType ) } );

            return Expression.New( targetConstructor, 
                Expression.New( targetConstructor, context.SourceMember ) );
        }

        protected override MethodInfo GetUpdateCollectionMethod( CollectionMapperContext context )
        {
            return typeof( LinqExtensions ).GetMethod
            (
                nameof( LinqExtensions.UpdateStack ),
                BindingFlags.Static | BindingFlags.Public
            )
            .MakeGenericMethod
            (
                context.SourceCollectionElementType,
                context.TargetCollectionElementType
            );
        }
    }
}