﻿using System;
using System.Collections.Generic;
using System.Linq;
using UltraMapper.Internals;

namespace UltraMapper.Conventions
{
    public class DefaultConvention : IMappingConvention
    {
        public IMemberProvider SourceMemberProvider { get; set; }
        public IMemberProvider TargetMemberProvider { get; set; }

        public IMatchingRulesEvaluator MatchingRulesEvaluator { get; set; }
        public MatchingRules MatchingRules { get; set; }

        public DefaultConvention()
        {
            this.SourceMemberProvider = new SourceMemberProvider();
            this.TargetMemberProvider = new TargetMemberProvider();

            this.MatchingRules = new MatchingRules( cfg =>
            {
                cfg.GetOrAdd<ExactNameMatching>( rule => rule.IgnoreCase = true );
            } );
        }

        public IEnumerable<MemberPair> MapByConvention( Type source, Type target )
        {
            if( MatchingRulesEvaluator == null )
                MatchingRulesEvaluator = new DefaultMatchingRuleEvaluator( this.MatchingRules );

            var sourceMembers = this.SourceMemberProvider.GetMembers( source );
            var targetMembers = this.TargetMemberProvider.GetMembers( target ).ToList();

            foreach( var sourceMember in sourceMembers )
            {
                foreach( var targetMember in targetMembers )
                {
                    if( this.MatchingRulesEvaluator.IsMatch( sourceMember, targetMember ) )
                    {
                        yield return new MemberPair( sourceMember, targetMember );
                        break; //sourceMember is now mapped, jump directly to the next sourceMember
                    }
                }
            }
        }
    }
}
