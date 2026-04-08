using edu.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

        var builder = WebApplication.CreateBuilder(args);

        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        builder.Services.AddDbContext<edu_portal_dbContext>(options =>
            options.UseSqlServer(connectionString));
        // Add services to the container.
        builder.Services.AddControllersWithViews()
                        // Maintain property names during serialization. See:
                        // https://github.com/aspnet/Announcements/issues/194
                        .AddNewtonsoftJson(options => options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver());
        // Add Kendo UI services to the services container"
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        }
        ).AddCookie();

        // Enables the in-memory cache to store session data (can be replaced by Redis or SQL server)
        builder.Services.AddDistributedMemoryCache();

        // Configure session options
        builder.Services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(30); // Set the session timeout (30 minutes by default)
            options.Cookie.HttpOnly = true;  // Protect session data from client-side access (security best practice)
            options.Cookie.IsEssential = true;  // Ensure session cookies are always sent, even without user consent (GDPR)
        });

        builder.Services.AddControllersWithViews();
        builder.Services.AddKendo();
        builder.Services.AddHttpClient();
        
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy => 
                policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
        });

        var app = builder.Build();

        app.UseCors("AllowAll");

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                //var context = services.GetRequiredService<NewsCat>();

            }
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseSession();
        app.MapControllerRoute(name: "lesson",
                        pattern: "lesson/{*index}",
                        defaults: new { controller = "lessons", action = "index" });
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        string editorDir = Path.Combine(builder.Environment.ContentRootPath, "wwwroot", "im", "max-themes.net", "demos", "kingster", "kingster");

        app.MapGet("/api/editor/files", () =>
        {
            if (!System.IO.Directory.Exists(editorDir)) return Results.NotFound();
            
            var systemFiles = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "manager.html", "grapesjs.html", "inject.ps1", "pages-tree.json" };
            
            // Recursive filter to remove system files from tree nodes
            System.Text.Json.Nodes.JsonArray FilterNodes(System.Text.Json.Nodes.JsonArray arr) {
                var result = new System.Text.Json.Nodes.JsonArray();
                foreach (var item in arr) {
                    var obj = item?.AsObject();
                    if (obj == null) continue;
                    var id = obj["id"]?.GetValue<string>() ?? "";
                    if (systemFiles.Contains(id)) continue;
                    if (obj["children"] is System.Text.Json.Nodes.JsonArray children) {
                        obj["children"] = FilterNodes(children);
                    }
                    result.Add(obj);
                }
                return result;
            }

            var treeFilePath = Path.Combine(editorDir, "pages-tree.json");
            if (System.IO.File.Exists(treeFilePath))
            {
                try {
                    var raw = System.IO.File.ReadAllText(treeFilePath);
                    var parsed = System.Text.Json.Nodes.JsonNode.Parse(raw)?.AsArray();
                    if (parsed != null) {
                        var filtered = FilterNodes(parsed);
                        return Results.Content(filtered.ToJsonString(), "application/json");
                    }
                } catch { /* fall through to rebuild */ }
            }

            var files = System.IO.Directory.GetFiles(editorDir, "*.html")
                                 .Where(f => !systemFiles.Contains(System.IO.Path.GetFileName(f)))
                                 .Select(f => new { id = System.IO.Path.GetFileName(f), text = System.IO.Path.GetFileName(f), children = new object[] {} })
                                 .ToList();
            var json = System.Text.Json.JsonSerializer.Serialize(files);
            System.IO.File.WriteAllText(treeFilePath, json);
            return Results.Content(json, "application/json");
        });


        app.MapPost("/api/editor/tree", async (HttpContext context) =>
        {
            using var reader = new StreamReader(context.Request.Body);
            var json = await reader.ReadToEndAsync();
            var treeFilePath = Path.Combine(editorDir, "pages-tree.json");
            await System.IO.File.WriteAllTextAsync(treeFilePath, json);
            return Results.Ok(new { message = "Tree saved" });
        });

        app.MapPost("/api/editor/files/rename", async (HttpContext context) =>
        {
            using var reader = new StreamReader(context.Request.Body);
            var body = await reader.ReadToEndAsync();
            var req = System.Text.Json.JsonSerializer.Deserialize<RenameReq>(body, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            
            if (req == null || string.IsNullOrEmpty(req.oldName) || string.IsNullOrEmpty(req.newName) || req.newName.Contains(".."))
                return Results.BadRequest(new { message = "Invalid input" });
                
            var oldPath = Path.Combine(editorDir, req.oldName);
            var newPath = Path.Combine(editorDir, req.newName);
            
            if (System.IO.File.Exists(oldPath)) 
            {
                System.IO.File.Move(oldPath, newPath);
            }
            return Results.Ok(new { message = "Renamed successfully" });
        });

        app.MapPost("/api/editor/files/duplicate", async (HttpContext context) =>
        {
            using var reader = new StreamReader(context.Request.Body);
            var body = await reader.ReadToEndAsync();
            var req = System.Text.Json.JsonSerializer.Deserialize<FileReq>(body, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (req == null || string.IsNullOrEmpty(req.file) || req.file.Contains("..")) return Results.BadRequest();
            
            var oldPath = Path.Combine(editorDir, req.file);
            var newBase = System.IO.Path.GetFileNameWithoutExtension(req.file) + "-copy";
            var ext = System.IO.Path.GetExtension(req.file);
            var newFileName = newBase + ext;
            var newPath = Path.Combine(editorDir, newFileName);
            
            int count = 1;
            while (System.IO.File.Exists(newPath))
            {
                newFileName = newBase + count + ext;
                newPath = Path.Combine(editorDir, newFileName);
                count++;
            }
            
            if (System.IO.File.Exists(oldPath)) System.IO.File.Copy(oldPath, newPath);
            return Results.Ok(new { newFile = newFileName, text = newFileName });
        });

        app.MapDelete("/api/editor/files", (HttpContext context) =>
        {
            var file = context.Request.Query["file"].FirstOrDefault();
            if (string.IsNullOrEmpty(file) || file.Contains("..")) return Results.BadRequest();
            var path = Path.Combine(editorDir, file);
            if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
            return Results.Ok(new { message = "Deleted" });
        });

        app.MapPost("/api/editor/save", async (HttpContext context) =>
        {
            var targetFile = context.Request.Query["file"].FirstOrDefault();
            if (string.IsNullOrEmpty(targetFile) || targetFile.Contains("..") || targetFile.Contains("/") || targetFile.Contains("\\"))
            {
                return Results.BadRequest(new { message = "Invalid file name" });
            }

            using var reader = new StreamReader(context.Request.Body);
            var html = await reader.ReadToEndAsync();
            var path = Path.Combine(editorDir, targetFile);
            await System.IO.File.WriteAllTextAsync(path, html);
            return Results.Ok(new { message = "Saved successfully to " + targetFile });
        });

        app.Run();

public class RenameReq { public string oldName { get; set; } = ""; public string newName { get; set; } = ""; }
public class FileReq { public string file { get; set; } = ""; }
