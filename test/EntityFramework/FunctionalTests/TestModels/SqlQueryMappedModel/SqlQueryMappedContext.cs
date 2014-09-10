// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

namespace System.Data.Entity.TestModels.SqlQueryMappedModel
{
    public class SqlQueryMappedContext : DbContext
    {
        public SqlQueryMappedContext()
        {
            Database.SetInitializer(new SqlQueryMappedInitializer());
        }

        public DbSet<MappedEntity> MappedEntities { get; set; }
    }
}
