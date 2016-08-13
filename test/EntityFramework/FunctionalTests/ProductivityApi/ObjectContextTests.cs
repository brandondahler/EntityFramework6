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
                        false,
                        "SELECT Id, RemappedColumn AS MappedColumn, UnmappedColumn FROM MappedEntities;")
                    .First();

                Assert.Equal(mappedEntity.Id, mappedEntityFromSqlQuery.Id);
                Assert.Equal(mappedEntity.MappedColumn, mappedEntityFromSqlQuery.MappedColumn);
                Assert.Equal(mappedEntity.UnmappedColumn, mappedEntityFromSqlQuery.UnmappedColumn);

                Assert.Throws<EntityCommandExecutionException>(() =>
                    objectContext.ExecuteStoreQuery<MappedEntity>(
                            false,
                            "SELECT Id, RemappedColumn, UnmappedColumn FROM MappedEntities;")
                        .First());
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
                        true,
                        "SELECT Id, RemappedColumn, UnmappedColumn FROM MappedEntities;")
                    .First();

                Assert.Equal(mappedEntity.Id, mappedEntityFromSqlQuery.Id);
                Assert.Equal(mappedEntity.MappedColumn, mappedEntityFromSqlQuery.MappedColumn);
                Assert.Equal(mappedEntity.UnmappedColumn, mappedEntityFromSqlQuery.UnmappedColumn);

                Assert.Throws<EntityCommandExecutionException>(() =>
                    objectContext.ExecuteStoreQuery<MappedEntity>(
                            true,
                            "SELECT Id, RemappedColumn AS MappedColumn, UnmappedColumn FROM MappedEntities;")
                        .First());
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
                        false,
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
                                    false,
                                    "SELECT Id, RemappedColumn, UnmappedColumn FROM MappedEntities;")
                                .Result
                                .First())
                        .InnerExceptions));
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
                        true,
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
                                    true,
                                    "SELECT Id, RemappedColumn AS MappedColumn, UnmappedColumn FROM MappedEntities;")
                                .Result
                                .First())
                        .InnerExceptions));
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
                                false)
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
                                    false)
                                .First());
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
                                true)
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
                                    true)
                                .First());
                    }
                }

                if (dbConnectionOpened)
                    dbConnection.Close();
            }
        }
        
        #endregion
    }
}
