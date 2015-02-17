﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

namespace System.Data.Entity.Infrastructure
{
    using System.Data.Entity.Internal;
    using System.Data.Entity.ModelConfiguration.Edm;
    using System.Data.Entity.Utilities;
    using System.Xml;
    using System.Xml.Linq;

    /// <summary>
    /// Utility class for reading a metadata model from .edmx.
    /// </summary>
    public static class EdmxReader
    {
        /// <summary>
        /// Reads a metadata model from .edmx.
        /// </summary>
        /// <param name="reader">XML reader for the .edmx</param>
        /// <param name="modelBuilder">The model builder to use to rebuild the metadata model.</param>
        /// <returns>The loaded metadata model.</returns>
        public static DbCompiledModel Read(XmlReader reader, DbModelBuilder modelBuilder)
        {
            Check.NotNull(reader, "reader");

            var document = XDocument.Load(reader);

            DbProviderInfo providerInfo;
            var mappingItemCollection = document.GetStorageMappingItemCollection(out providerInfo);

            return new DbCompiledModel(
                CodeFirstCachedMetadataWorkspace.Create(mappingItemCollection, providerInfo),
                modelBuilder);
        }
    }
}
