﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Metadata.Internal;
using Microsoft.Data.Entity.Metadata.ModelConventions;
using Microsoft.Data.Entity.Relational.Metadata.ModelConventions;
using Xunit;

namespace Microsoft.Data.Entity.Relational.Metadata
{
    public class RelationalEntityTypeAttributeConventionTest
    {
        [Table("MyTable", Schema = "MySchema")]
        public class A
        {
            public int Id { get; set; }

            public string Name { get; set; }
        }

        [Fact]
        public void TableAttribute_sets_column_name_order_and_type_with_conventional_builder()
        {
            var modelBuilder = new ModelBuilder(new TestConventionalSetBuilder().AddConventions(new CoreConventionSetBuilder().CreateConventionSet()));

            var entityBuilder = modelBuilder.Entity<A>();

            Assert.Equal("MyTable", entityBuilder.Metadata.Relational().Table);
            Assert.Equal("MySchema", entityBuilder.Metadata.Relational().Schema);
        }


        [Fact]
        public void TableAttribute_overrides_configuration_from_convention_source()
        {
            var entityBuilder = CreateInternalEntityTypeBuilder<A>();

            entityBuilder.Annotation(RelationalAnnotationNames.Prefix + RelationalAnnotationNames.TableName, "ConventionalName", ConfigurationSource.Convention);
            entityBuilder.Annotation(RelationalAnnotationNames.Prefix + RelationalAnnotationNames.Schema, "ConventionalSchema", ConfigurationSource.Convention);

            new RelationalTableAttributeConvention().Apply(entityBuilder);

            Assert.Equal("MyTable", entityBuilder.Metadata.Relational().Table);
            Assert.Equal("MySchema", entityBuilder.Metadata.Relational().Schema);
        }

        [Fact]
        public void TableAttribute_does_not_override_configuration_from_explicit_source()
        {
            var entityBuilder = CreateInternalEntityTypeBuilder<A>();

            entityBuilder.Annotation(RelationalAnnotationNames.Prefix + RelationalAnnotationNames.TableName, "ExplicitName", ConfigurationSource.Explicit);
            entityBuilder.Annotation(RelationalAnnotationNames.Prefix + RelationalAnnotationNames.Schema, "ExplicitName", ConfigurationSource.Explicit);

            new RelationalTableAttributeConvention().Apply(entityBuilder);

            Assert.Equal("ExplicitName", entityBuilder.Metadata.Relational().Table);
            Assert.Equal("ExplicitName", entityBuilder.Metadata.Relational().Schema);
        }

        private InternalEntityTypeBuilder CreateInternalEntityTypeBuilder<T>()
        {
            var conventionSet = new ConventionSet();
            conventionSet.EntityTypeAddedConventions.Add(new PropertyDiscoveryConvention());

            var modelBuilder = new InternalModelBuilder(new Model(), conventionSet);

            return modelBuilder.Entity(typeof(T), ConfigurationSource.Explicit);
        }


        private class TestConventionalSetBuilder : RelationalConventionSetBuilder
        {
        }
    }
}
