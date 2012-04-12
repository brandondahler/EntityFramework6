namespace System.Data.Entity.Migrations.Design
{
    using System.Collections.Generic;
    using System.Data.Entity.Core.Metadata.Edm;
    using System.Data.Entity.Migrations.Model;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Linq;

    /// <summary>
    ///     Base class for providers that generate code for code-based migrations.
    /// </summary>
    [ContractClass(typeof(MigrationCodeGeneratorContracts))]
    public abstract class MigrationCodeGenerator
    {
        /// <summary>
        ///     Generates the code that should be added to the users project.
        /// </summary>
        /// <param name = "migrationId">Unique identifier of the migration.</param>
        /// <param name = "operations">Operations to be performed by the migration.</param>
        /// <param name = "sourceModel">Source model to be stored in the migration metadata.</param>
        /// <param name = "targetModel">Target model to be stored in the migration metadata.</param>
        /// <param name = "namespace">Namespace that code should be generated in.</param>
        /// <param name = "className">Name of the class that should be generated.</param>
        /// <returns>The generated code.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "namespace")]
        public abstract ScaffoldedMigration Generate(
            string migrationId,
            IEnumerable<MigrationOperation> operations,
            string sourceModel,
            string targetModel,
            string @namespace,
            string className);

        /// <summary>
        /// Gets the namespaces that must be output as "using" or "Imports" directives to handle
        /// the code generated by the given operations.
        /// </summary>
        /// <param name="operations">The operations for which code is going to be generated.</param>
        /// <returns>An ordered list of namespace names.</returns>
        protected virtual IEnumerable<string> GetNamespaces(IEnumerable<MigrationOperation> operations)
        {
            var namespaces = GetDefaultNamespaces();

            if (operations.OfType<AddColumnOperation>().Any(
                o => o.Column.Type == PrimitiveTypeKind.Geography || o.Column.Type == PrimitiveTypeKind.Geometry))
            {
                namespaces = namespaces.Concat(new[] { "System.Data.Entity.Core.Spatial" });
            }

            return namespaces.Distinct().OrderBy(n => n);
        }

        /// <summary>
        /// Gets the default namespaces that must be output as "using" or "Imports" directives for
        /// any code generated.
        /// </summary>
        /// <param name = "designer">A value indicating if this class is being generated for a code-behind file.</param>
        /// <returns>An ordered list of namespace names.</returns>
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        protected virtual IEnumerable<string> GetDefaultNamespaces(bool designer = false)
        {
            var namespaces
                = new List<string>
                      {
                          "System.Data.Entity.Migrations"
                      };

            if (designer)
            {
                namespaces.Add("System.Data.Entity.Migrations.Infrastructure");
            }
            else
            {
                namespaces.Insert(0, "System");
            }

            return namespaces;
        }

        #region Contracts

        [ContractClassFor(typeof(MigrationCodeGenerator))]
        internal abstract class MigrationCodeGeneratorContracts : MigrationCodeGenerator
        {
            public override ScaffoldedMigration Generate(
                string migrationId,
                IEnumerable<MigrationOperation> operations,
                string sourceModel,
                string targetModel,
                string @namespace,
                string className)
            {
                Contract.Requires(!string.IsNullOrWhiteSpace(migrationId));
                Contract.Requires(operations != null);
                Contract.Requires(!string.IsNullOrWhiteSpace(targetModel));
                Contract.Requires(!string.IsNullOrWhiteSpace(className));

                return default(ScaffoldedMigration);
            }
        }

        #endregion
    }
}
