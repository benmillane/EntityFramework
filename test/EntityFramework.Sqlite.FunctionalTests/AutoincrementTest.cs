﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using Microsoft.Data.Entity.FunctionalTests;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Sqlite;
using Microsoft.Framework.DependencyInjection;
using Xunit;

namespace Microsoft.Data.Entity.Sqlite.FunctionalTests
{
    public class AutoincrementTest : IDisposable
    {
        private readonly DbContextOptions _options;
        private readonly IServiceProvider _provider;
        private readonly SqliteTestStore _testStore;

        [Fact]
        public void Autoincrement_prevents_reusing_rowid()
        {
            using (var context = CreateContext())
            {
                context.Database.EnsureCreated();
                context.People.Add(new Person { Name = "Bruce" });
                context.SaveChanges();

                var hero = context.People.First(p => p.Id == 1);

                context.People.Remove(hero);
                context.SaveChanges();
                context.People.Add(new Person { Name = "Batman" });
                context.SaveChanges();
                var gone = context.People.FirstOrDefault(p => p.Id == 1);
                var begins = context.People.FirstOrDefault(p => p.Id == 2);

                Assert.Null(gone);
                Assert.NotNull(begins);
            }
        }

        [Fact]
        public void Autoincrement_not_on_text()
        {
            using (var context = new JokerContext(_provider, _options))
            {
                var ex = Assert.Throws<SqliteException>(() => context.Database.EnsureCreated());
                Assert.Contains("AUTOINCREMENT is only allowed on an INTEGER PRIMARY KEY", ex.Message);
            }
        }

        public AutoincrementTest()
        {
            _testStore = SqliteTestStore.CreateScratch();

            var builder = new DbContextOptionsBuilder();
            builder.UseSqlite(_testStore.Connection);
            _options = builder.Options;
            _provider = new ServiceCollection()
                .AddEntityFramework()
                .AddSqlite()
                .AddDbContext<BatContext>()
                .ServiceCollection()
                .BuildServiceProvider();
        }

        private BatContext CreateContext() => new BatContext(_provider, _options);

        public void Dispose()
        {
            _testStore?.Dispose();
        }
    }

    public class JokerContext : DbContext
    {
        public JokerContext(IServiceProvider serviceProvider, DbContextOptions options)
            : base(serviceProvider, options)
        {
        }

        public DbSet<Person> People { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Person>(b =>
                {
                    b.ToSqliteTable("People2");
                    b.Key(t => t.Name);
                    b.Property(t => t.Name).StoreGeneratedPattern(StoreGeneratedPattern.Identity);
                });
        }
    }

    public class BatContext : DbContext
    {
        public BatContext(IServiceProvider serviceProvider, DbContextOptions options)
            : base(serviceProvider, options)
        {
        }

        public DbSet<Person> People { get; set; }
    }

    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
