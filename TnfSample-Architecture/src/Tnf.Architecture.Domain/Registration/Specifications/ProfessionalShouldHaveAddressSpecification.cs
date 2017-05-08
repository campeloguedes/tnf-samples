﻿using System;
using System.Linq.Expressions;
using Tnf.Specifications;

namespace Tnf.Architecture.Domain.Registration.Specifications
{
    internal class ProfessionalShouldHaveAddressSpecification : Specification<Professional>
    {
        public override Expression<Func<Professional, bool>> ToExpression()
        {
            return (p) => p.Address != null && !string.IsNullOrWhiteSpace(p.Address.Street);
        }
    }
}
