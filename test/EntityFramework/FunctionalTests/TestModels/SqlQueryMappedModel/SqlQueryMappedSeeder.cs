// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

namespace System.Data.Entity.TestModels.SqlQueryMappedModel
{
    public class SqlQueryMappedSeeder
    {
        public void Seed(SqlQueryMappedContext context)
        {
            context.MappedEntities.Add(
                new MappedEntity()
                {
                    MappedColumn = "MappedColumn",
                    UnmappedColumn = "UnmappedColumn"
                });

            context.SaveChanges();
        }
    }
}
