﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TypeMapper.CollectionMappingStrategies;
using TypeMapper.Internals;
using TypeMapper.Mappers;
using TypeMapper.MappingConventions;

namespace TypeMapper.Configuration
{
    public class PropertyConfiguration : IEnumerable<PropertyMapping>
    {
        //A source property can be mapped to multiple target properties
        private Dictionary<PropertyInfoPair, PropertyMapping> _propertyMappings;
        private IEnumerable<IObjectMapper> _objectMappers;

        private LambdaExpression _expression;
        public LambdaExpression MappingExpression
        {
            get
            {
                if( _expression != null )
                    return _expression;

                var sourceType = _propertyMappings.Values.First().SourceProperty.PropertyInfo.DeclaringType;
                var targetType = _propertyMappings.Values.First().TargetProperty.PropertyInfo.DeclaringType;

                var sourceInstance = Expression.Parameter( sourceType, "sourceInstance" );
                var targetInstance = Expression.Parameter( targetType, "targetInstance" );

                var expressions = _propertyMappings.Values.Select( mapping =>
                    Expression.Invoke( mapping.Expression, sourceInstance, targetInstance ) );

                var bodyExp = Expression.Block( expressions.ToArray() );

                var delegateType = typeof( Action<,> )
                    .MakeGenericType( sourceType, targetType );

                return _expression = Expression.Lambda( delegateType,
                    bodyExp, sourceInstance, targetInstance );
            }
        }


        /// <summary>
        /// This constructor is only used by derived classes to allow
        /// casts from PropertyConfiguration to the derived class itself
        /// </summary>
        /// <param name="configuration">An already existing mapping configuration</param>
        protected PropertyConfiguration( PropertyConfiguration configuration, IEnumerable<IObjectMapper> objectMappers )
        {
            _propertyMappings = configuration._propertyMappings;
            _objectMappers = objectMappers;
        }

        public PropertyConfiguration( Type source, Type target, IEnumerable<IObjectMapper> objectMappers )
            : this( source, target, new DefaultMappingConvention(), objectMappers ) { }

        public PropertyConfiguration( Type source, Type target,
            IMappingConvention mappingConvention, IEnumerable<IObjectMapper> objectMappers )
        {
            _objectMappers = objectMappers;
            _propertyMappings = new Dictionary<PropertyInfoPair, PropertyMapping>();

            var bindingAttributes = BindingFlags.Instance | BindingFlags.Public;

            var sourceProperties = source.GetProperties( bindingAttributes )
                .Where( p => p.CanRead && p.GetIndexParameters().Length == 0 ); //no indexed properties

            var targetProperties = target.GetProperties( bindingAttributes )
                .Where( p => p.CanWrite && p.GetIndexParameters().Length == 0 ); //no indexed properties

            foreach( var sourceProperty in sourceProperties )
            {
                foreach( var targetProperty in targetProperties )
                {
                    if( targetProperty.SetMethod != null )
                    {
                        if( mappingConvention.IsMatch( sourceProperty, targetProperty ) )
                        {
                            this.Map( sourceProperty, targetProperty );
                            break; //sourceProperty is now mapped, jump directly to the next sourceProperty
                        }
                    }
                }
            }
        }

        protected PropertyMapping Map( PropertyInfo sourcePropertyInfo, PropertyInfo targetPropertyInfo )
        {
            var typePairKey = new PropertyInfoPair( sourcePropertyInfo, targetPropertyInfo );

            PropertyMapping propertyMapping;
            if( !_propertyMappings.TryGetValue( typePairKey, out propertyMapping ) )
            {
                var sourceProperty = new SourceProperty( sourcePropertyInfo )
                {
                    IsBuiltInType = sourcePropertyInfo.PropertyType.IsBuiltInType( true ),
                    IsEnumerable = sourcePropertyInfo.PropertyType.IsEnumerable(),
                    ValueGetterExpr = sourcePropertyInfo.GetGetterExpression()
                };

                propertyMapping = new PropertyMapping( sourceProperty );
                _propertyMappings.Add( typePairKey, propertyMapping );
            }

            propertyMapping.TargetProperty = new TargetProperty( targetPropertyInfo )
            {
                IsBuiltInType = targetPropertyInfo.PropertyType.IsBuiltInType( true ),
                NullableUnderlyingType = Nullable.GetUnderlyingType( targetPropertyInfo.PropertyType ),
                ValueSetterExpr = targetPropertyInfo.GetSetterExpression()
            };

            propertyMapping.Mapper = _objectMappers.FirstOrDefault(
                mapper => mapper.CanHandle( propertyMapping ) );

            if( propertyMapping == null )
                throw new Exception( $"No object mapper can handle {propertyMapping}" );

            return propertyMapping;
        }

        internal PropertyMapping this[ PropertyInfo sourceProperty, PropertyInfo targetProperty ]
        {
            get
            {
                var typePairKey = new PropertyInfoPair( sourceProperty, targetProperty );
                return _propertyMappings[ typePairKey ];
            }
        }

        internal PropertyMapping this[ PropertyInfoPair typePairKey ]
        {
            get { return _propertyMappings[ typePairKey ]; }
        }

        public IEnumerator<PropertyMapping> GetEnumerator()
        {
            return _propertyMappings.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }

    public class PropertyConfiguration<TSource, TTarget> : PropertyConfiguration
    {
        /// <summary>
        /// This constructor is only used internally to allow
        /// casts from PropertyConfiguration to PropertyConfiguration<>
        /// </summary>
        /// <param name="map">An already existing mapping configuration</param>
        internal PropertyConfiguration( PropertyConfiguration map, IEnumerable<IObjectMapper> objectMappers )
            : base( map, objectMappers ) { }

        public PropertyConfiguration( IMappingConvention mappingConvention, IEnumerable<IObjectMapper> objectMappers )
            : base( typeof( TSource ), typeof( TTarget ), mappingConvention, objectMappers ) { }

        public PropertyConfiguration<TSource, TTarget> MapProperty<TSourceProperty, TTargetProperty>(
            Expression<Func<TSource, TSourceProperty>> sourcePropertySelector,
            Expression<Func<TTarget, TTargetProperty>> targetPropertySelector,
            Expression<Func<TSourceProperty, TTargetProperty>> converter = null )
        {
            var sourcePropertyInfo = sourcePropertySelector.ExtractPropertyInfo();
            var targetPropertyInfo = targetPropertySelector.ExtractPropertyInfo();

            var propertyMapping = base.Map( sourcePropertyInfo, targetPropertyInfo );

            propertyMapping.ValueConverterExp = converter;
            propertyMapping.ValueConverter = (converter == null) ? null :
                converter.EncapsulateInGenericFunc<TSourceProperty>().Compile();

            return this;
        }

        public PropertyConfiguration<TSource, TTarget> MapProperty<TSourceProperty, TTargetProperty>(
           Expression<Func<TSource, TSourceProperty>> sourcePropertySelector,
           Expression<Func<TTarget, TTargetProperty>> targetPropertySelector,
           ICollectionMappingStrategy collectionStrategy,
           Expression<Func<TSourceProperty, TTargetProperty>> converter = null )
           where TTargetProperty : IEnumerable
        {
            var sourcePropertyInfo = sourcePropertySelector.ExtractPropertyInfo();
            var targetPropertyInfo = targetPropertySelector.ExtractPropertyInfo();

            var propertyMapping = base.Map( sourcePropertyInfo, targetPropertyInfo );

            propertyMapping.ValueConverterExp = converter;
            propertyMapping.ValueConverter = (converter == null) ? null :
                converter.EncapsulateInGenericFunc<TSourceProperty>().Compile();

            propertyMapping.TargetProperty.CollectionStrategy = collectionStrategy;

            return this;
        }
    }
}
