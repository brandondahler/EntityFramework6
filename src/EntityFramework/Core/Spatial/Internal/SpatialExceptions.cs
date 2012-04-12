﻿namespace System.Data.Entity.Core.Spatial.Internal
{
    using System.Data.Entity.Resources;

    internal static class SpatialExceptions
    {
        internal static ArgumentNullException ArgumentNull(string argumentName)
        {
            // TODO_SPATIAL: Remove this dependency on System.Data.Entity when moving to System.Data.Spatial
            return EntityUtil.ArgumentNull(argumentName);
        }

        internal static Exception ProviderValueNotCompatibleWithSpatialServices()
        {
            // TODO_SPATIAL: Remove this dependency on System.Data.Entity when moving to the System.Data.Spatial assembly
            return EntityUtil.Argument(Strings.Spatial_ProviderValueNotCompatibleWithSpatialServices, "providerValue");
        }

        /// <summary>
        /// Thrown whenever DbGeograpy/DbGeometry.WellKnownValue is set after regular construction (not deserialization instantiation).
        /// </summary>
        /// <returns><see cref="InvalidOperationException"/></returns>
        internal static InvalidOperationException WellKnownValueSerializationPropertyNotDirectlySettable()
        {
            // TODO_SPATIAL: Remove this dependency on System.Data.Entity when moving to the System.Data.Spatial assembly
            return EntityUtil.InvalidOperation(Strings.Spatial_WellKnownValueSerializationPropertyNotDirectlySettable);
        }

        #region Geography-specific exceptions

        internal static Exception GeographyValueNotCompatibleWithSpatialServices(string argumentName)
        {
            // TODO_SPATIAL: Remove this dependency when moving to the System.Data.Spatial assembly
            return EntityUtil.Argument(Strings.Spatial_GeographyValueNotCompatibleWithSpatialServices, argumentName);
        }

        internal static Exception WellKnownGeographyValueNotValid(string argumentName)
        {
            // TODO_SPATIAL: Remove this dependency on System.Data.Entity when moving to System.Data.Spatial
            return EntityUtil.Argument(Strings.Spatial_WellKnownGeographyValueNotValid, argumentName);
        }

        internal static Exception CouldNotCreateWellKnownGeographyValueNoSrid(string argumentName)
        {
            // TODO_SPATIAL: Remove this dependency when moving to the System.Data.Spatial assembly
            return EntityUtil.Argument(Strings.SqlSpatialservices_CouldNotCreateWellKnownGeographyValueNoSrid, argumentName);
        }

        internal static Exception CouldNotCreateWellKnownGeographyValueNoWkbOrWkt(string argumentName)
        {
            // TODO_SPATIAL: Remove this dependency when moving to the System.Data.Spatial assembly
            return EntityUtil.Argument(Strings.SqlSpatialservices_CouldNotCreateWellKnownGeographyValueNoWkbOrWkt, argumentName);
        }

        #endregion

        #region Geometry-specific exceptions

        internal static Exception GeometryValueNotCompatibleWithSpatialServices(string argumentName)
        {
            // TODO_SPATIAL: Remove this dependency when moving to the System.Data.Spatial assembly
            return EntityUtil.Argument(Strings.Spatial_GeometryValueNotCompatibleWithSpatialServices, argumentName);
        }

        internal static Exception WellKnownGeometryValueNotValid(string argumentName)
        {
            // TODO_SPATIAL: Remove this dependency on System.Data.Entity when moving to System.Data.Spatial
            throw EntityUtil.Argument(Strings.Spatial_WellKnownGeometryValueNotValid, argumentName);
        }

        internal static Exception CouldNotCreateWellKnownGeometryValueNoSrid(String argumentName)
        {
            // TODO_SPATIAL: Remove this dependency when moving to the System.Data.Spatial assembly
            return EntityUtil.Argument(Strings.SqlSpatialservices_CouldNotCreateWellKnownGeometryValueNoSrid, argumentName);
        }

        internal static Exception CouldNotCreateWellKnownGeometryValueNoWkbOrWkt(String argumentName)
        {
            // TODO_SPATIAL: Remove this dependency when moving to the System.Data.Spatial assembly
            return EntityUtil.Argument(Strings.SqlSpatialservices_CouldNotCreateWellKnownGeometryValueNoWkbOrWkt, argumentName);
        }

        #endregion

        #region SqlSpatialServices-specific Exceptions

        internal static Exception SqlSpatialServices_ProviderValueNotSqlType(Type requiredType)
        {
            return EntityUtil.Argument(
                Strings.SqlSpatialServices_ProviderValueNotSqlType(requiredType.AssemblyQualifiedName), "providerValue");
        }

        #endregion
    }
}
