// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

namespace ProductivityApiTests
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Entity;
    using System.Data.Entity.Core;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.TestModels.SqlQueryMappedModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;

    public class ObjectContextTests : FunctionalTestBase
    {
        
        #region Sql queries for configured entities

        #region Non-async

        [Fact]
        public void ObjectContext_ExecuteStoreQuery_DefaultBehavior()
        {
            using (var context = new SqlQueryMappedContext())
            {
                var objectContext = ((IObjectContextAdapter)context).ObjectContext;
                var mappedEntity = context.MappedEntities.First();

                var mappedEntityFromSqlQuery = objectContext.ExecuteStoreQuery<MappedEntity>(
                        "SELECT Id, RemappedColumn AS MappedColumn, UnmappedColumn FROM MappedEntities;")
                    .First();

                Assert.Equal(mappedEntity.Id, mappedEntityFromSqlQuery.Id);
                Assert.Equal(mappedEntity.MappedColumn, mappedEntityFromSqlQuery.MappedColumn);
                Assert.Equal(mappedEntity.UnmappedColumn, mappedEntityFromSqlQuery.UnmappedColumn);

                Assert.Throws<EntityCommandExecutionException>(() =>
                    objectContext.ExecuteStoreQuery<MappedEntity>(
                        "SELECT Id, RemappedColumn, UnmappedColumn FROM MappedEntities;")
                    .First());
            }
        }

        [Fact]
        public void ObjectContext_ExecuteStoreQuery_MemberNameOnly()
        {
            using (var context = new SqlQueryMappedContext())
            {
                var objectContext = ((IObjectContextAdapter)context).ObjectContext;
                var mappedEntity = context.MappedEntities.First();

                var mappedEntityFromSqlQuery = objectContext.ExecuteStoreQuery<MappedEntity>(
                        SqlQueryMappingBehavior.MemberNameOnly,
                        "SELECT Id, RemappedColumn AS MappedColumn, UnmappedColumn FROM MappedEntities;")
                    .First();

                Assert.Equal(mappedEntity.Id, mappedEntityFromSqlQuery.Id);
                Assert.Equal(mappedEntity.MappedColumn, mappedEntityFromSqlQuery.MappedColumn);
                Assert.Equal(mappedEntity.UnmappedColumn, mappedEntityFromSqlQuery.UnmappedColumn);

                Assert.Throws<EntityCommandExecutionException>(() =>
                    objectContext.ExecuteStoreQuery<MappedEntity>(
                            SqlQueryMappingBehavior.MemberNameOnly,
                            "SELECT Id, RemappedColumn, UnmappedColumn FROM MappedEntities;")
                        .First());
            }
        }

        [Fact]
        public void ObjectContext_ExecuteStoreQuery_ColumnAliasFallback()
        {
            using (var context = new SqlQueryMappedContext())
            {
                var objectContext = ((IObjectContextAdapter)context).ObjectContext;
                var mappedEntity = context.MappedEntities.First();
                var sqls = new[] {
                    "SELECT Id, RemappedColumn AS MappedColumn, UnmappedColumn FROM MappedEntities;",
                    "SELECT Id, RemappedColumn, UnmappedColumn FROM MappedEntities;"
                };

                foreach (var sql in sqls)
                {
                    var mappedEntityFromSqlQuery = objectContext.ExecuteStoreQuery<MappedEntity>(
                            SqlQueryMappingBehavior.ColumnAliasFallback,
                            sql)
                        .First();

                    Assert.Equal(mappedEntity.Id, mappedEntityFromSqlQuery.Id);
                    Assert.Equal(mappedEntity.MappedColumn, mappedEntityFromSqlQuery.MappedColumn);
                    Assert.Equal(mappedEntity.UnmappedColumn, mappedEntityFromSqlQuery.UnmappedColumn);
                }
            }
        }


        [Fact]
        public void ObjectContext_ExecuteStoreQuery_ColumnAliasOnly()
        {
            using (var context = new SqlQueryMappedContext())
            {
                var objectContext = ((IObjectContextAdapter)context).ObjectContext;
                var mappedEntity = context.MappedEntities.First();

                var mappedEntityFromSqlQuery = objectContext.ExecuteStoreQuery<MappedEntity>(
                        SqlQueryMappingBehavior.ColumnAliasOnly,
                        "SELECT Id, RemappedColumn, UnmappedColumn FROM MappedEntities;")
                    .First();

                Assert.Equal(mappedEntity.Id, mappedEntityFromSqlQuery.Id);
                Assert.Equal(mappedEntity.MappedColumn, mappedEntityFromSqlQuery.MappedColumn);
                Assert.Equal(mappedEntity.UnmappedColumn, mappedEntityFromSqlQuery.UnmappedColumn);

                Assert.Throws<EntityCommandExecutionException>(() =>
                    objectContext.ExecuteStoreQuery<MappedEntity>(
                            SqlQueryMappingBehavior.ColumnAliasOnly,
                            "SELECT Id, RemappedColumn AS MappedColumn, UnmappedColumn FROM MappedEntities;")
                        .First());
            }
        }

        [Fact]
        public void ObjectContext_ExecuteStoreQuery_MemberNameFallback()
        {
            using (var context = new SqlQueryMappedContext())
            {
                var objectContext = ((IObjectContextAdapter)context).ObjectContext;
                var mappedEntity = context.MappedEntities.First();
                var sqls = new[] {
                    "SELECT Id, RemappedColumn, UnmappedColumn FROM MappedEntities;",
                    "SELECT Id, RemappedColumn AS MappedColumn, UnmappedColumn FROM MappedEntities;"
                };

                foreach (var sql in sqls)
                {
                    var mappedEntityFromSqlQuery = objectContext.ExecuteStoreQuery<MappedEntity>(
                            SqlQueryMappingBehavior.MemberNameFallback,
                            sql)
                        .First();

                    Assert.Equal(mappedEntity.Id, mappedEntityFromSqlQuery.Id);
                    Assert.Equal(mappedEntity.MappedColumn, mappedEntityFromSqlQuery.MappedColumn);
                    Assert.Equal(mappedEntity.UnmappedColumn, mappedEntityFromSqlQuery.UnmappedColumn);
                }
            }
        }

        #endregion

        #region Async

#if !NET40

        [Fact]
        public void ObjectContext_ExecuteStoreQueryAsync_DefaultBehavior()
        {
            using (var context = new SqlQueryMappedContext())
            {
                var objectContext = ((IObjectContextAdapter)context).ObjectContext;
                var mappedEntity = context.MappedEntities.First();

                var mappedEntityFromSqlQuery = objectContext.ExecuteStoreQueryAsync<MappedEntity>(
                        "SELECT Id, RemappedColumn AS MappedColumn, UnmappedColumn FROM MappedEntities;")
                    .Result
                    .First();

                Assert.Equal(mappedEntity.Id, mappedEntityFromSqlQuery.Id);
                Assert.Equal(mappedEntity.MappedColumn, mappedEntityFromSqlQuery.MappedColumn);
                Assert.Equal(mappedEntity.UnmappedColumn, mappedEntityFromSqlQuery.UnmappedColumn);

                Assert.IsType<EntityCommandExecutionException>(
                    Assert.Single(
                        Assert.Throws<AggregateException>(() =>
                            objectContext.ExecuteStoreQueryAsync<MappedEntity>(
                                "SELECT Id, RemappedColumn, UnmappedColumn FROM MappedEntities;")
                            .Result
                            .First())
                        .InnerExceptions));
            }
        }

        [Fact]
        public void ObjectContext_ExecuteStoreQueryAsync_MemberNameOnly()
        {
            using (var context = new SqlQueryMappedContext())
            {
                var objectContext = ((IObjectContextAdapter)context).ObjectContext;
                var mappedEntity = context.MappedEntities.First();

                var mappedEntityFromSqlQuery = objectContext.ExecuteStoreQueryAsync<MappedEntity>(
                        SqlQueryMappingBehavior.MemberNameOnly,
                        "SELECT Id, RemappedColumn AS MappedColumn, UnmappedColumn FROM MappedEntities;")
                    .Result
                    .First();

                Assert.Equal(mappedEntity.Id, mappedEntityFromSqlQuery.Id);
                Assert.Equal(mappedEntity.MappedColumn, mappedEntityFromSqlQuery.MappedColumn);
                Assert.Equal(mappedEntity.UnmappedColumn, mappedEntityFromSqlQuery.UnmappedColumn);

                Assert.IsType<EntityCommandExecutionException>(
                    Assert.Single(
                        Assert.Throws<AggregateException>(() =>
                            objectContext.ExecuteStoreQueryAsync<MappedEntity>(
                                    SqlQueryMappingBehavior.MemberNameOnly,
                                    "SELECT Id, RemappedColumn, UnmappedColumn FROM MappedEntities;")
                                .Result
                                .First())
                        .InnerExceptions));
            }
        }

        [Fact]
        public void ObjectContext_ExecuteStoreQueryAsync_ColumnAliasFallback()
        {
            using (var context = new SqlQueryMappedContext())
            {
                var objectContext = ((IObjectContextAdapter)context).ObjectContext;
                var mappedEntity = context.MappedEntities.First();
                var sqls = new[] {
                    "SELECT Id, RemappedColumn AS MappedColumn, UnmappedColumn FROM MappedEntities;",
                    "SELECT Id, RemappedColumn, UnmappedColumn FROM MappedEntities;"
                };

                foreach (var sql in sqls)
                {
                    var mappedEntityFromSqlQuery = objectContext.ExecuteStoreQueryAsync<MappedEntity>(
                            SqlQueryMappingBehavior.ColumnAliasFallback,
                            sql)
                        .Result
                        .First();

                    Assert.Equal(mappedEntity.Id, mappedEntityFromSqlQuery.Id);
                    Assert.Equal(mappedEntity.MappedColumn, mappedEntityFromSqlQuery.MappedColumn);
                    Assert.Equal(mappedEntity.UnmappedColumn, mappedEntityFromSqlQuery.UnmappedColumn);
                }
            }
        }


        [Fact]
        public void ObjectContext_ExecuteStoreQueryAsync_ColumnAliasOnly()
        {
            using (var context = new SqlQueryMappedContext())
            {
                var objectContext = ((IObjectContextAdapter)context).ObjectContext;
                var mappedEntity = context.MappedEntities.First();

                var mappedEntityFromSqlQuery = objectContext.ExecuteStoreQueryAsync<MappedEntity>(
                        SqlQueryMappingBehavior.ColumnAliasOnly,
                        "SELECT Id, RemappedColumn, UnmappedColumn FROM MappedEntities;")
                    .Result
                    .First();

                Assert.Equal(mappedEntity.Id, mappedEntityFromSqlQuery.Id);
                Assert.Equal(mappedEntity.MappedColumn, mappedEntityFromSqlQuery.MappedColumn);
                Assert.Equal(mappedEntity.UnmappedColumn, mappedEntityFromSqlQuery.UnmappedColumn);

                Assert.IsType<EntityCommandExecutionException>(
                    Assert.Single(
                        Assert.Throws<AggregateException>(() =>
                            objectContext.ExecuteStoreQueryAsync<MappedEntity>(
                                    SqlQueryMappingBehavior.ColumnAliasOnly,
                                    "SELECT Id, RemappedColumn AS MappedColumn, UnmappedColumn FROM MappedEntities;")
                                .Result
                                .First())
                        .InnerExceptions));
            }
        }

        [Fact]
        public void ObjectContext_ExecuteStoreQueryAsync_MemberNameFallback()
        {
            using (var context = new SqlQueryMappedContext())
            {
                var objectContext = ((IObjectContextAdapter)context).ObjectContext;
                var mappedEntity = context.MappedEntities.First();
                var sqls = new[] {
                    "SELECT Id, RemappedColumn, UnmappedColumn FROM MappedEntities;",
                    "SELECT Id, RemappedColumn AS MappedColumn, UnmappedColumn FROM MappedEntities;"
                };

                foreach (var sql in sqls)
                {
                    var mappedEntityFromSqlQuery = objectContext.ExecuteStoreQueryAsync<MappedEntity>(
                            SqlQueryMappingBehavior.MemberNameFallback,
                            sql)
                        .Result
                        .First();

                    Assert.Equal(mappedEntity.Id, mappedEntityFromSqlQuery.Id);
                    Assert.Equal(mappedEntity.MappedColumn, mappedEntityFromSqlQuery.MappedColumn);
                    Assert.Equal(mappedEntity.UnmappedColumn, mappedEntityFromSqlQuery.UnmappedColumn);
                }
            }
        }

#endif

        #endregion

        #endregion

        #region Translate DbDataReader for configured entites

        [Fact]
        public void ObjectContext_Translate_DefaultBehavior()
        {
            using (var context = new SqlQueryMappedContext())
            {
                var objectContext = ((IObjectContextAdapter)context).ObjectContext;
                var dbConnection = context.Database.Connection;
                var dbConnectionOpened = false;
                var mappedEntity = context.MappedEntities.First();

                
                if (dbConnection.State != ConnectionState.Open)
                {
                    dbConnection.Open();
                    dbConnectionOpened = true;
                }

                using (var dbCommand = dbConnection.CreateCommand())
                {
                    dbCommand.CommandText = "SELECT Id, RemappedColumn AS MappedColumn, UnmappedColumn FROM MappedEntities;";

                    using (var dbDataReader = dbCommand.ExecuteReader())
                    {
                        var mappedEntityFromSqlQuery = objectContext.Translate<MappedEntity>(
                                dbDataReader)
                            .First();

                        Assert.Equal(mappedEntity.Id, mappedEntityFromSqlQuery.Id);
                        Assert.Equal(mappedEntity.MappedColumn, mappedEntityFromSqlQuery.MappedColumn);
                        Assert.Equal(mappedEntity.UnmappedColumn, mappedEntityFromSqlQuery.UnmappedColumn);
                    }
                }

                using (var dbCommand = dbConnection.CreateCommand())
                {
                    dbCommand.CommandText = "SELECT Id, RemappedColumn, UnmappedColumn FROM MappedEntities;";

                    using (var dbDataReader = dbCommand.ExecuteReader())
                    {
                        Assert.Throws<EntityCommandExecutionException>(() =>
                            objectContext.Translate<MappedEntity>(
                                dbDataReader)
                            .First());
                    }
                }

                if (dbConnectionOpened)
                    dbConnection.Close();

            }
        }

        [Fact]
        public void ObjectContext_Translate_MemberNameOnly()
        {
            using (var context = new SqlQueryMappedContext())
            {
                var objectContext = ((IObjectContextAdapter)context).ObjectContext;
                var dbConnection = context.Database.Connection;
                var dbConnectionOpened = false;
                var mappedEntity = context.MappedEntities.First();


                if (dbConnection.State != ConnectionState.Open)
                {
                    dbConnection.Open();
                    dbConnectionOpened = true;
                }

                using (var dbCommand = dbConnection.CreateCommand())
                {
                    dbCommand.CommandText = "SELECT Id, RemappedColumn AS MappedColumn, UnmappedColumn FROM MappedEntities;";

                    using (var dbDataReader = dbCommand.ExecuteReader())
                    {
                        var mappedEntityFromSqlQuery = objectContext.Translate<MappedEntity>(
                                dbDataReader,
                                SqlQueryMappingBehavior.MemberNameOnly)
                            .First();

                        Assert.Equal(mappedEntity.Id, mappedEntityFromSqlQuery.Id);
                        Assert.Equal(mappedEntity.MappedColumn, mappedEntityFromSqlQuery.MappedColumn);
                        Assert.Equal(mappedEntity.UnmappedColumn, mappedEntityFromSqlQuery.UnmappedColumn);
                    }
                }

                using (var dbCommand = dbConnection.CreateCommand())
                {
                    dbCommand.CommandText = "SELECT Id, RemappedColumn, UnmappedColumn FROM MappedEntities;";

                    using (var dbDataReader = dbCommand.ExecuteReader())
                    {
                        Assert.Throws<EntityCommandExecutionException>(() =>
                            objectContext.Translate<MappedEntity>(
                                    dbDataReader,
                                    SqlQueryMappingBehavior.MemberNameOnly)
                                .First());
                    }
                }

                if (dbConnectionOpened)
                    dbConnection.Close();
            }
        }

        [Fact]
        public void ObjectContext_Translate_ColumnAliasFallback()
        {
            using (var context = new SqlQueryMappedContext())
            {
                var objectContext = ((IObjectContextAdapter)context).ObjectContext;
                var dbConnection = context.Database.Connection;
                var dbConnectionOpened = false;
                var mappedEntity = context.MappedEntities.First();
                var sqls = new[] {
                    "SELECT Id, RemappedColumn AS MappedColumn, UnmappedColumn FROM MappedEntities;",
                    "SELECT Id, RemappedColumn, UnmappedColumn FROM MappedEntities;"
                };


                if (dbConnection.State != ConnectionState.Open)
                {
                    dbConnection.Open();
                    dbConnectionOpened = true;
                }

                foreach (var sql in sqls)
                {
                    using (var dbCommand = dbConnection.CreateCommand())
                    {
                        dbCommand.CommandText = sql;

                        using (var dbDataReader = dbCommand.ExecuteReader())
                        {
                            var mappedEntityFromSqlQuery = objectContext.Translate<MappedEntity>(
                                    dbDataReader,
                                    SqlQueryMappingBehavior.ColumnAliasFallback)
                                .First();

                            Assert.Equal(mappedEntity.Id, mappedEntityFromSqlQuery.Id);
                            Assert.Equal(mappedEntity.MappedColumn, mappedEntityFromSqlQuery.MappedColumn);
                            Assert.Equal(mappedEntity.UnmappedColumn, mappedEntityFromSqlQuery.UnmappedColumn);
                        }
                    }
                }

                if (dbConnectionOpened)
                    dbConnection.Close();
            }
        }


        [Fact]
        public void ObjectContext_Translate_ColumnAliasOnly()
        {
            using (var context = new SqlQueryMappedContext())
            {
                var objectContext = ((IObjectContextAdapter)context).ObjectContext;
                var dbConnection = context.Database.Connection;
                var dbConnectionOpened = false;
                var mappedEntity = context.MappedEntities.First();


                if (dbConnection.State != ConnectionState.Open)
                {
                    dbConnection.Open();
                    dbConnectionOpened = true;
                }

                using (var dbCommand = dbConnection.CreateCommand())
                {
                    dbCommand.CommandText = "SELECT Id, RemappedColumn, UnmappedColumn FROM MappedEntities;";

                    using (var dbDataReader = dbCommand.ExecuteReader())
                    {
                        var mappedEntityFromSqlQuery = objectContext.Translate<MappedEntity>(
                                dbDataReader,
                                SqlQueryMappingBehavior.ColumnAliasOnly)
                            .First();

                        Assert.Equal(mappedEntity.Id, mappedEntityFromSqlQuery.Id);
                        Assert.Equal(mappedEntity.MappedColumn, mappedEntityFromSqlQuery.MappedColumn);
                        Assert.Equal(mappedEntity.UnmappedColumn, mappedEntityFromSqlQuery.UnmappedColumn);
                    }
                }
                
                using (var dbCommand = dbConnection.CreateCommand())
                {
                    dbCommand.CommandText = "SELECT Id, RemappedColumn AS MappedColumn, UnmappedColumn FROM MappedEntities;";

                    using (var dbDataReader = dbCommand.ExecuteReader())
                    {
                        Assert.Throws<EntityCommandExecutionException>(() =>
                            objectContext.Translate<MappedEntity>(
                                    dbDataReader,
                                    SqlQueryMappingBehavior.ColumnAliasOnly)
                                .First());
                    }
                }

                if (dbConnectionOpened)
                    dbConnection.Close();
            }
        }

        [Fact]
        public void ObjectContext_Translate_MemberNameFallback()
        {
            using (var context = new SqlQueryMappedContext())
            {
                var objectContext = ((IObjectContextAdapter)context).ObjectContext;
                var dbConnection = context.Database.Connection;
                var dbConnectionOpened = false;
                var mappedEntity = context.MappedEntities.First();
                var sqls = new[] {
                    "SELECT Id, RemappedColumn, UnmappedColumn FROM MappedEntities;",
                    "SELECT Id, RemappedColumn AS MappedColumn, UnmappedColumn FROM MappedEntities;"
                };


                if (dbConnection.State != ConnectionState.Open)
                {
                    dbConnection.Open();
                    dbConnectionOpened = true;
                }

                foreach (var sql in sqls)
                {
                    using (var dbCommand = dbConnection.CreateCommand())
                    {
                        dbCommand.CommandText = sql;

                        using (var dbDataReader = dbCommand.ExecuteReader())
                        {
                            var mappedEntityFromSqlQuery = objectContext.Translate<MappedEntity>(
                                    dbDataReader, SqlQueryMappingBehavior.MemberNameFallback)
                                .First();

                            Assert.Equal(mappedEntity.Id, mappedEntityFromSqlQuery.Id);
                            Assert.Equal(mappedEntity.MappedColumn, mappedEntityFromSqlQuery.MappedColumn);
                            Assert.Equal(mappedEntity.UnmappedColumn, mappedEntityFromSqlQuery.UnmappedColumn);
                        }
                    }
                }

                if (dbConnectionOpened)
                    dbConnection.Close();
            }
        }

        #endregion
    }
}
