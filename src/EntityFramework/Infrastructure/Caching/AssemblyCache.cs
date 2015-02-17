// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

namespace System.Data.Entity.Infrastructure.Caching
{
    using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Security;

    /// <summary>
    /// Stores data persistently, purging the cache when the assembly that it is tied to has been modified or recompiled.
    /// 
    /// Specifically allows multiple types of key values to be stored within the same cache file.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    internal abstract class AssemblyCache<TKey, TValue>
    {
        /// <summary>
        /// Gets or sets a value indicating whether IO exceptions should be ignored.
        /// </summary>
        /// <value>
        /// <c>true</c> if IO exceptions should be ignored; otherwise, <c>false</c>.
        /// </value>
        protected bool SuppressIOExceptions { get; set; }


        private readonly string _cacheFilePath;
        private readonly string _valueKey;

        private readonly Guid _cacheAssemblyModuleVersionId;

        private static readonly Guid _entityFrameworkModuleVersionId = Assembly.GetExecutingAssembly().ManifestModule.ModuleVersionId;

        private const string CacheAssemblyModuleVersionIdKey = "CacheAssembly.ModuleVersionId";
        private const string EntityFrameworkModuleVersionIdKey = "EntityFramework.ModuleVersionId";
        private const string ValueKeyRoot = "EntityFramework.Values.";


        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyCache{TKey, TValue}"/> class.
        /// </summary>
        /// <param name="cacheAssembly">The assembly to tie the cache to.</param>
        /// <param name="cacheExtension">The extension to append to the assembly name.</param>
        /// <param name="valueKeySuffix">The suffix to use when storing the cached data.</param>
        protected AssemblyCache(Assembly cacheAssembly, string cacheExtension, string valueKeySuffix)
        {
            _cacheFilePath = cacheAssembly.Location + cacheExtension;
            _valueKey = ValueKeyRoot + valueKeySuffix;

            _cacheAssemblyModuleVersionId = cacheAssembly.ManifestModule.ModuleVersionId;
        }

        /// <summary>
        /// Adds the value to the cache.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        protected void AddValue(TKey key, TValue value)
        {
            var resources = 
                TryExecuteIO(() => {
                    using (var resourceReader = OpenResourceReader())
                    {
                        if (resourceReader != null)
                        {
                            return resourceReader.Cast<DictionaryEntry>()
                                .ToDictionary(de => (string)de.Key, de => de.Value);
                        }

                        return null;
                    }
                }) ?? new Dictionary<string, object>();



            IDictionary<TKey, TValue> valueDictionary = null;

            if (resources.ContainsKey(_valueKey))
            {
                valueDictionary = resources[_valueKey] as IDictionary<TKey, TValue>;
            }
            
            if (valueDictionary == null)
            {
                valueDictionary = new Dictionary<TKey, TValue>();
            }

            
            valueDictionary[key] = value;
            resources[_valueKey] = valueDictionary;


            TryExecuteIO(() => {
                using (var resourceWriter = new ResourceWriter(_cacheFilePath))
                {
                    resourceWriter.AddResource(EntityFrameworkModuleVersionIdKey, _entityFrameworkModuleVersionId);
                    resourceWriter.AddResource(CacheAssemblyModuleVersionIdKey, _cacheAssemblyModuleVersionId);


                    var otherResources = resources
                        .Where(kvp => kvp.Key != EntityFrameworkModuleVersionIdKey && kvp.Key != CacheAssemblyModuleVersionIdKey);

                    foreach (var resource in otherResources)
                    {
                        resourceWriter.AddResource(resource.Key, resource.Value);
                    }
                }
            });
        }

        /// <summary>
        /// Reads all of the values that have been stored.
        /// </summary>
        /// <returns>The cached key value pairs.</returns>
        protected IDictionary<TKey, TValue> ReadAllValues()
        {
            return TryExecuteIO(() => {
                using (var resourceReader = OpenResourceReader())
                {
                    if (resourceReader != null)
                    {
                        return (IDictionary<TKey, TValue>)resourceReader.Cast<DictionaryEntry>()
                            .FirstOrDefault(de => ((string)de.Key) == _valueKey)
                            .Value;
                    }

                    return null;
                }
            }) ?? new Dictionary<TKey, TValue>();
        }


        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Supresses exception in favor of invalidating the cache.")]
        [SuppressMessage("Microsoft.Reliability", "CA2000: Dispose objects before losing scope")]
        private ResourceReader OpenResourceReader()
        {
            if (File.Exists(_cacheFilePath))
            {
                ResourceReader resourceReader = null;

                try
                {
                    resourceReader = new ResourceReader(_cacheFilePath);
                } 
                catch (Exception)
                {
                    // Suppress resource file reading errors in favor of invalidating the cache.
                }

                // Outside try...catch to keep from suppressing valid exceptions.
                if (resourceReader != null)
                {
                    if (ResourceFileIsValid(resourceReader))
                    {
                        return resourceReader;
                    }

                    resourceReader.Dispose();
                }


                File.Delete(_cacheFilePath);
            }

            return null;
        }

        // <summary>
        // Determines if the resource file matches the current Entity Framework assembly identifier and the cache's assembly identifier.
        // </summary>
        // <param name="resourceReader">The resource reader.</param>
        // <returns><c>true</c> if the resource file is still valid according to the purging rules; otherwise <c>false</c>.</returns>
        private bool ResourceFileIsValid(ResourceReader resourceReader)
        {
            var storedEntityFrameworkModuleVersionId = (Guid?) resourceReader.Cast<DictionaryEntry>()
                    .FirstOrDefault(de => ((string) de.Key) == EntityFrameworkModuleVersionIdKey)
                    .Value;

            if (storedEntityFrameworkModuleVersionId != _entityFrameworkModuleVersionId)
                return false;


            var storedCacheAssemblyModuleVersionId = (Guid?)resourceReader.Cast<DictionaryEntry>()
                    .FirstOrDefault(de => ((string) de.Key) == CacheAssemblyModuleVersionIdKey)
                    .Value;

            return storedCacheAssemblyModuleVersionId == _cacheAssemblyModuleVersionId;
        }


        private bool TryExecuteIO(Action action)
        {
            return TryExecuteIO<bool>(() =>{
                action();

                return true;
            });
        }

        private T TryExecuteIO<T>(Func<T> action)
        {
            try
            {
                return action();
            }
            catch (SecurityException)
            {
                // Serialization isn't allowed due to security level
                if (!SuppressIOExceptions)
                {
                    throw;
                }

                return default(T);
            }
            catch (IOException)
            {
                if (!SuppressIOExceptions)
                {
                    throw;
                }

                return default(T);
            }
        }
    }
}
