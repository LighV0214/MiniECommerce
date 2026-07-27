# Create the first admin user

This project does not seed an admin user during app startup. Create the first admin explicitly with SQL so production behavior stays predictable.

1. Restore packages if needed.

   ```powershell
   dotnet restore MiniECommerce.sln
   ```

2. Generate a local SQL file. The script prompts for the admin password and writes a BCrypt hash, not the plain password.

   ```powershell
   .\scripts\admin\New-AdminSql.ps1 -Email admin@mini.com
   ```

3. Run the SQL against the target database.

   ```powershell
   psql "Host=localhost;Port=5432;Database=mini_ecommerce_dev;Username=postgres" -f scripts\admin\create-admin.local.sql
   ```

`create-admin.local.sql` is ignored by Git because it contains a password hash.
