using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Modulith.SharedKernel.Infrastructure;

public static class MigrationHelper
{
    public static void EnsureExtensionsExist(this MigrationBuilder migrationBuilder)
    {
        // Ensure pgvector extension exists
        migrationBuilder.Sql("CREATE EXTENSION IF NOT EXISTS vector;", suppressTransaction: true);
        
        // Ensure Apache AGE extension exists
        migrationBuilder.Sql("CREATE EXTENSION IF NOT EXISTS age;", suppressTransaction: true);
    }

    public static void DropExtensions(this MigrationBuilder migrationBuilder)
    {
        // Drop extensions if needed
        migrationBuilder.Sql("DROP EXTENSION IF EXISTS vector;", suppressTransaction: true);
        migrationBuilder.Sql("DROP EXTENSION IF EXISTS age;", suppressTransaction: true);
    }
} 