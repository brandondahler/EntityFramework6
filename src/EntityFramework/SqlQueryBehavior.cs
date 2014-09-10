// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

namespace System.Data.Entity
{
    /// <summary>
    /// Controls the column mapping behavior when executing a raw SQL query.
    /// </summary>
    public enum SqlQueryMappingBehavior
    {
        /// <summary>
        /// Only the member name is used.
        /// </summary>
        MemberNameOnly,

        /// <summary>
        /// The member name is used if valid; otherwise the column alias is used.
        /// </summary>
        ColumnAliasFallback,

        /// <summary>
        /// Only the column alias is used if the member has one specified.  If none is specified, the member name is used.
        /// </summary>
        ColumnAliasOnly,

        /// <summary>
        /// The column alias is used if specified and valid.  If it is invalid or none is specified, the member name is used.
        /// </summary>
        MemberNameFallback
    }
}
