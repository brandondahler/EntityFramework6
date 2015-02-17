// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

namespace System.Data.Entity.Infrastructure.Caching
{
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Data.Entity.Core.Metadata.Edm;
    using System.Data.Entity.Internal;
    using System.Data.Entity.Migrations;
    using System.Data.Entity.ModelConfiguration.Edm;
    using System.Data.Entity.Utilities;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Resources;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Linq;

    /// <summary>
    /// A persistent cache that stores Entity Framework models inside an <see cref="AssemblyCache"/>.
    /// </summary>
    internal class AssemblyDbModelCache
        : AssemblyCache<IDbModelCacheKey, string>,
            IDbModelCache
    {
        private AssemblyDbModelCache(Assembly cacheAssembly)
            : base(cacheAssembly, ".entitycache", "DbCompiledModels")
        {
            // Suppress exceptions since it is not required for proper operation.
            SuppressIOExceptions = true;
        }

        /// <summary>
        /// Gets the stored models.
        /// </summary>
        /// <param name="modelBuilder">The model builder to help reconstruct the compiled model.</param>
        /// <returns>
        /// Set of key and compiled model pairs.
        /// </returns>
        public IDictionary<IDbModelCacheKey, DbCompiledModel> GetStoredModels(DbModelBuilder modelBuilder)
        {
            return ReadAllValues()
                .ToDictionary(kvp => kvp.Key, kvp => BuildCompiledModel(kvp.Value, modelBuilder));
        }

        /// <summary>
        /// Stores the given model.
        /// </summary>
        /// <param name="key">The key to store the model under.</param>
        /// <param name="dbModel">The model to store.</param>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public void StoreModel(IDbModelCacheKey key, DbModel dbModel)
        {
            var stringWriter = new StringWriter(CultureInfo.InvariantCulture);

            using (var xmlWriter = XmlWriter.Create(stringWriter))
            {
                EdmxWriter.WriteEdmx(dbModel, xmlWriter);

                AddValue(key, stringWriter.ToString());
            }
        }

        /// <summary>
        /// Creates a cache tied to the given assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>An AssemblyDbModelCache instance.</returns>
        public static AssemblyDbModelCache Create(Assembly assembly)
        {
            return new AssemblyDbModelCache(assembly);
        }


        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        private static DbCompiledModel BuildCompiledModel(string modelXml, DbModelBuilder modelBuilder)
        {
            using (var xmlReader = XmlReader.Create(new StringReader(modelXml)))
            {
                return EdmxReader.Read(xmlReader, modelBuilder);
            }
        }
    }
}
