// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using Microsoft.Data.Entity.Relational.Metadata;
using Microsoft.Data.Entity.SqlServer.Metadata;
using Microsoft.Data.Entity.Tests;
using Xunit;

namespace Microsoft.Data.Entity.SqlServer.Tests.Metadata.ModelConventions
{
    public class SqlServerIdentityStrategyConventionTest
    {
        [Fact]
        public void Annotations_are_added_when_conventional_model_builder_is_used()
        {
            var model = SqlServerTestHelpers.Instance.CreateConventionBuilder().Model;

            Assert.Equal(1, model.Annotations.Count());

            Assert.Equal(SqlServerAnnotationNames.Prefix + SqlServerAnnotationNames.ValueGeneration, model.Annotations.Single().Name);
            Assert.Equal(SqlServerIdentityStrategy.IdentityColumn.ToString(), model.Annotations.Single().Value);
        }

        [Fact]
        public void Annotations_are_added_when_conventional_model_builder_is_used_with_sequences()
        {
            var model = SqlServerTestHelpers.Instance.CreateConventionBuilder()
                .UseSqlServerSequenceHiLo()
                .Model;

            Assert.Equal(3, model.Annotations.Count());

            Assert.Equal(SqlServerAnnotationNames.Prefix + SqlServerAnnotationNames.DefaultSequenceName, model.Annotations.ElementAt(0).Name);
            Assert.Equal(Sequence.DefaultName, model.Annotations.ElementAt(0).Value);

            Assert.Equal(
                SqlServerAnnotationNames.Prefix +
                RelationalAnnotationNames.Sequence +
                "." +
                Sequence.DefaultName,
                model.Annotations.ElementAt(1).Name);
            Assert.Equal(new Sequence(Sequence.DefaultName).Serialize(), model.Annotations.ElementAt(1).Value);

            Assert.Equal(SqlServerAnnotationNames.Prefix + SqlServerAnnotationNames.ValueGeneration, model.Annotations.ElementAt(2).Name);
            Assert.Equal(SqlServerIdentityStrategy.SequenceHiLo.ToString(), model.Annotations.ElementAt(2).Value);
        }
    }
}
