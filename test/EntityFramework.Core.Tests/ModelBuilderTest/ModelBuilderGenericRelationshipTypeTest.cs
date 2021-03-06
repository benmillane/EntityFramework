// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Data.Entity.Metadata.Builders;

// ReSharper disable once CheckNamespace
namespace Microsoft.Data.Entity.Tests
{
    public class ModelBuilderGenericRelationshipTypeTest : ModelBuilderGenericTest
    {
        public class GenericOneToOneType : OneToOneTestBase
        {
            protected override TestModelBuilder CreateTestModelBuilder(ModelBuilder modelBuilder)
            {
                return new GenericTypeTestModelBuilder(modelBuilder);
            }
        }

        private class GenericTypeTestModelBuilder : TestModelBuilder
        {
            public GenericTypeTestModelBuilder(ModelBuilder modelBuilder)
                : base(modelBuilder)
            {
            }

            public override TestEntityTypeBuilder<TEntity> Entity<TEntity>()
                => new GenericTypeTestEntityTypeBuilder<TEntity>(ModelBuilder.Entity<TEntity>());

            public override TestModelBuilder Entity<TEntity>(Action<TestEntityTypeBuilder<TEntity>> buildAction)
                => new GenericTypeTestModelBuilder(ModelBuilder.Entity<TEntity>(entityTypeBuilder =>
                    buildAction(new GenericTypeTestEntityTypeBuilder<TEntity>(entityTypeBuilder))));

            public override TestModelBuilder Ignore<TEntity>()
                => new GenericTypeTestModelBuilder(ModelBuilder.Ignore<TEntity>());
        }

        private class GenericTypeTestEntityTypeBuilder<TEntity> : GenericTestEntityTypeBuilder<TEntity>
            where TEntity : class
        {
            public GenericTypeTestEntityTypeBuilder(EntityTypeBuilder<TEntity> entityTypeBuilder)
                : base(entityTypeBuilder)
            {
            }

            protected override TestEntityTypeBuilder<TEntity> Wrap(EntityTypeBuilder<TEntity> entityTypeBuilder)
                => new GenericTypeTestEntityTypeBuilder<TEntity>(entityTypeBuilder);

            public override TestReferenceNavigationBuilder<TEntity, TRelatedEntity> Reference<TRelatedEntity>(Expression<Func<TEntity, TRelatedEntity>> reference = null)
                => new GenericTypeTestReferenceNavigationBuilder<TEntity, TRelatedEntity>(EntityTypeBuilder.Reference(reference));
        }

        private class GenericTypeTestReferenceNavigationBuilder<TEntity, TRelatedEntity> : GenericTestReferenceNavigationBuilder<TEntity, TRelatedEntity>
            where TEntity : class
            where TRelatedEntity : class
        {
            public GenericTypeTestReferenceNavigationBuilder(ReferenceNavigationBuilder<TEntity, TRelatedEntity> referenceNavigationBuilder)
                : base(referenceNavigationBuilder)
            {
            }

            public override TestReferenceReferenceBuilder<TEntity, TRelatedEntity> InverseReference(Expression<Func<TRelatedEntity, TEntity>> reference = null)
                => new GenericTypeTestReferenceReferenceBuilder<TEntity, TRelatedEntity>(ReferenceNavigationBuilder.InverseReference(reference?.GetPropertyAccess().Name));
        }
        
        private class GenericTypeTestReferenceReferenceBuilder<TEntity, TRelatedEntity> : GenericTestReferenceReferenceBuilder<TEntity, TRelatedEntity>
            where TEntity : class
            where TRelatedEntity : class
        {
            public GenericTypeTestReferenceReferenceBuilder(ReferenceReferenceBuilder<TEntity, TRelatedEntity> referenceReferenceBuilder)
                : base(referenceReferenceBuilder)
            {
            }

            protected override GenericTestReferenceReferenceBuilder<TEntity, TRelatedEntity> Wrap(ReferenceReferenceBuilder<TEntity, TRelatedEntity> referenceReferenceBuilder)
                => new GenericTypeTestReferenceReferenceBuilder<TEntity, TRelatedEntity>(referenceReferenceBuilder);

            public override TestReferenceReferenceBuilder<TEntity, TRelatedEntity> ForeignKey<TDependentEntity>(Expression<Func<TDependentEntity, object>> foreignKeyExpression)
                => Wrap(ReferenceReferenceBuilder.ForeignKey(typeof(TDependentEntity), foreignKeyExpression.GetPropertyAccessList().Select(p => p.Name).ToArray()));

            public override TestReferenceReferenceBuilder<TEntity, TRelatedEntity> PrincipalKey<TPrincipalEntity>(Expression<Func<TPrincipalEntity, object>> keyExpression)
                => Wrap(ReferenceReferenceBuilder.PrincipalKey(typeof(TPrincipalEntity), keyExpression.GetPropertyAccessList().Select(p => p.Name).ToArray()));
        }
    }
}
