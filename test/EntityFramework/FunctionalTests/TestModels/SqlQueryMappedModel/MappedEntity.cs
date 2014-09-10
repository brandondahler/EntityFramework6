// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

namespace System.Data.Entity.TestModels.SqlQueryMappedModel
{
    using System.ComponentModel.DataAnnotations.Schema;

    public class MappedEntity
    {
        public int Id { get; set; }

        [Column("RemappedColumn")]
        public string MappedColumn { get; set; }

        public string UnmappedColumn { get; set; }
    }
}
