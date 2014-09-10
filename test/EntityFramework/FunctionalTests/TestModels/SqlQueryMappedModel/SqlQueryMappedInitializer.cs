// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

namespace System.Data.Entity.TestModels.SqlQueryMappedModel
{
    public class SqlQueryMappedInitializer : DropCreateDatabaseIfModelChanges<SqlQueryMappedContext>
    {
        protected override void Seed(SqlQueryMappedContext context)
        {
            new SqlQueryMappedSeeder().Seed(context);
        }
    }
}
