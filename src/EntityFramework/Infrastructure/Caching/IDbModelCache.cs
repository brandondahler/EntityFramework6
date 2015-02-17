// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

namespace System.Data.Entity.Infrastructure.Caching
{
    using System.Collections.Generic;
    using System.Data.Entity.Internal;

    /// <summary>
    /// A persistent cache to store Entity Framework models.
    /// </summary>
    public interface IDbModelCache
    {
        /// <summary>
        /// Gets the stored models.
        /// </summary>
        /// <param name="modelBuilder">The model builder to help reconstruct the compiled model.</param>
        /// <returns>Set of key and compiled model pairs.</returns>
        IDictionary<IDbModelCacheKey, DbCompiledModel> GetStoredModels(DbModelBuilder modelBuilder);

        /// <summary>
        /// Stores the given model.
        /// </summary>
        /// <param name="key">The key to store the model under.</param>
        /// <param name="dbModel">The model to store.</param>
        void StoreModel(IDbModelCacheKey key, DbModel dbModel);
    }
}
