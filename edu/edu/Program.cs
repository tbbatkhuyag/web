using edu.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// ================= DB =================
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string not found");

builder.Services.AddDbContext<edu_portal_dbContext>(options =>
    options.UseSqlServer(connectionString));

// ================= CORE =================
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie();

builder.Services.AddSession();

var app = builder.Build();

// ================= PIPELINE =================
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

// ================= GRAPESJS DIR =================
string editorDir = Path.Combine(
    builder.Environment.ContentRootPath,
    "wwwroot",
    "kingster"
);

// static serve only (SAFE)
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(editorDir),
    RequestPath = ""
});

// ================= HOME =================
app.MapGet("/", () => Results.Redirect("/Нүүр.html"));

// ================= EDITOR OPEN =================
app.MapGet("/editor", () =>
{
    var file = Path.Combine(editorDir, "grapesjs.html");
    return Results.File(file, "text/html");
});

// ================= EDITOR API =================
app.MapGet("/api/editor/files", () =>
{
    if (!Directory.Exists(editorDir))
        return Results.NotFound();

    var systemFiles = new HashSet<string> { "grapesjs.html", "manager.html", "404.html", "tree.json" };
    var filesOnDisk = Directory.GetFiles(editorDir, "*.html")
        .Select(Path.GetFileName)
        .Where(f => !systemFiles.Contains(f))
        .ToHashSet();

    var treeFile = Path.Combine(editorDir, "tree.json");
    if (File.Exists(treeFile))
    {
        var json = File.ReadAllText(treeFile);
        var jArray = System.Text.Json.Nodes.JsonNode.Parse(json) as System.Text.Json.Nodes.JsonArray;
        
        var inTree = new HashSet<string>();
        void ExtractIds(System.Text.Json.Nodes.JsonArray arr) {
            if (arr == null) return;
            foreach (var node in arr) {
                if (node?["id"] != null) inTree.Add(node["id"].ToString());
                if (node?["children"] is System.Text.Json.Nodes.JsonArray children) ExtractIds(children);
            }
        }
        ExtractIds(jArray);

        var missingFiles = filesOnDisk.Where(f => !inTree.Contains(f)).ToList();
        foreach (var m in missingFiles) {
            var newNode = new System.Text.Json.Nodes.JsonObject();
            newNode["id"] = m;
            newNode["text"] = m;
            jArray.Add(newNode);
        }

        return Results.Content(jArray.ToJsonString(), "application/json");
    }

    var files = filesOnDisk.Select(f => new { id = f, text = f }).ToList();
    return Results.Ok(files);
});

app.MapPost("/api/editor/tree", async (HttpContext ctx) =>
{
    using var reader = new StreamReader(ctx.Request.Body);
    var json = await reader.ReadToEndAsync();
    await File.WriteAllTextAsync(Path.Combine(editorDir, "tree.json"), json);
    
    try {
        var doc = System.Text.Json.JsonDocument.Parse(json);
        var root = doc.RootElement;
        
        string FormatLabel(string name) {
            if(name.ToLower() == "index.html" || name.ToLower() == "нүүр.html") return "Home";
            name = System.Text.RegularExpressions.Regex.Replace(name, @"\.html$", "");
            name = name.Replace("-", " ");
            var textInfo = new System.Globalization.CultureInfo("en-US",false).TextInfo;
            return textInfo.ToTitleCase(name);
        }
        
        string BuildDesktopMenu(System.Text.Json.JsonElement nodes) {
            var html = "";
            foreach(var n in nodes.EnumerateArray()) {
                if (n.GetProperty("id").GetString() == "grapesjs.html" || n.GetProperty("id").GetString() == "manager.html") continue;
                var hasChildren = n.TryGetProperty("children", out var children) && children.GetArrayLength() > 0;
                var id = n.GetProperty("id").GetString();
                var text = n.GetProperty("text").GetString();
                
                var liClass = $"menu-item {(hasChildren ? "menu-item-has-children" : "")} kingster-normal-menu";
                var aClass = hasChildren ? "sf-with-ul-pre" : "";
                html += $"<li class=\"{liClass}\"><a href=\"{id}\" class=\"{aClass}\">{FormatLabel(text)}</a>";
                if (hasChildren) {
                    html += "<ul class=\"sub-menu\">" + BuildDesktopMenu(children) + "</ul>";
                }
                html += "</li>";
            }
            return html;
        }
        
        string BuildMobileMenu(System.Text.Json.JsonElement nodes) {
            var html = "";
            foreach(var n in nodes.EnumerateArray()) {
                if (n.GetProperty("id").GetString() == "grapesjs.html" || n.GetProperty("id").GetString() == "manager.html") continue;
                var hasChildren = n.TryGetProperty("children", out var children) && children.GetArrayLength() > 0;
                var id = n.GetProperty("id").GetString();
                var text = n.GetProperty("text").GetString();
                
                var liClass = $"menu-item {(hasChildren ? "menu-item-has-children" : "")}";
                html += $"<li class=\"{liClass}\"><a href=\"{id}\">{FormatLabel(text)}</a>";
                if (hasChildren) {
                    html += "<ul class=\"sub-menu\">" + BuildMobileMenu(children) + "</ul>";
                }
                html += "</li>";
            }
            return html;
        }
        
        var desktopMenu = BuildDesktopMenu(root);
        var mobileMenu = BuildMobileMenu(root);
        
        var allFiles = Directory.GetFiles(editorDir, "*.html");
        foreach (var f in allFiles) {
            var fname = Path.GetFileName(f).ToLower();
            if (fname == "manager.html" || fname == "grapesjs.html") continue;
            
            var hDoc = new HtmlAgilityPack.HtmlDocument();
            hDoc.Load(f);
            var dkNode = hDoc.DocumentNode.SelectSingleNode("//ul[@id='menu-main-navigation-1']");
            var mbNode = hDoc.DocumentNode.SelectSingleNode("//ul[@id='menu-main-navigation']");
            
            bool changed = false;
            if (dkNode != null) { dkNode.InnerHtml = desktopMenu; changed = true; }
            if (mbNode != null) { mbNode.InnerHtml = mobileMenu; changed = true; }
            
            if (changed) hDoc.Save(f, System.Text.Encoding.UTF8);
        }
    } catch { /* IGNORE IF JSON PARSE ERROR */ }

    return Results.Ok(new { message = "saved" });
});

app.MapPost("/api/editor/files/rename", async (HttpContext ctx) =>
{
    using var reader = new StreamReader(ctx.Request.Body);
    var json = await reader.ReadToEndAsync();
    var req = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(json);
    if (req == null || !req.ContainsKey("oldName") || !req.ContainsKey("newName")) return Results.BadRequest();
    var oldName = req["oldName"];
    var newName = req["newName"];
    if (oldName.Contains("/") || newName.Contains("/")) return Results.BadRequest();
    
    var oldPath = Path.Combine(editorDir, oldName);
    var newPath = Path.Combine(editorDir, newName);
    if (!File.Exists(oldPath) || File.Exists(newPath)) return Results.BadRequest();
    File.Move(oldPath, newPath);
    return Results.Ok();
});

app.MapPost("/api/editor/files/duplicate", async (HttpContext ctx) =>
{
    using var reader = new StreamReader(ctx.Request.Body);
    var json = await reader.ReadToEndAsync();
    var req = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(json);
    if (req == null || !req.ContainsKey("file")) return Results.BadRequest();
    var file = req["file"];
    if (file.Contains("/")) return Results.BadRequest();
    
    var oldPath = Path.Combine(editorDir, file);
    if (!File.Exists(oldPath)) return Results.BadRequest();
    var baseName = Path.GetFileNameWithoutExtension(file);
    var newName = baseName + "-copy.html";
    var counter = 1;
    while (File.Exists(Path.Combine(editorDir, newName))) {
        newName = $"{baseName}-copy{counter}.html";
        counter++;
    }
    File.Copy(oldPath, Path.Combine(editorDir, newName));
    return Results.Ok(new { newId = newName });
});

app.MapDelete("/api/editor/files", (HttpContext ctx) =>
{
    var file = ctx.Request.Query["file"].ToString();
    if (string.IsNullOrWhiteSpace(file) || file.Contains("/")) return Results.BadRequest();
    var path = Path.Combine(editorDir, file);
    if (File.Exists(path)) File.Delete(path);
    return Results.Ok();
});


app.MapPost("/api/editor/sync-layout", async (HttpContext ctx) =>
{
    var req = await ctx.Request.ReadFromJsonAsync<SyncLayoutReq>();
    if (req == null) return Results.BadRequest();

    var files = Directory.GetFiles(editorDir, "*.html");
    foreach (var file in files)
    {
        var fname = Path.GetFileName(file);
        if (fname.ToLower() == "нүүр.html" || fname.ToLower() == "manager.html" || fname.ToLower() == "grapesjs.html") continue;

        var doc = new HtmlAgilityPack.HtmlDocument();
        doc.Load(file);

        if (!string.IsNullOrEmpty(req.mobileHeader))
        {
            var node = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'kingster-mobile-header-wrap')]");
            if (node != null) node.ParentNode.ReplaceChild(HtmlAgilityPack.HtmlNode.CreateNode(req.mobileHeader), node);
        }
        if (!string.IsNullOrEmpty(req.topBar))
        {
            var node = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'kingster-top-bar')]");
            if (node != null) node.ParentNode.ReplaceChild(HtmlAgilityPack.HtmlNode.CreateNode(req.topBar), node);
        }
        if (!string.IsNullOrEmpty(req.header))
        {
            var node = doc.DocumentNode.SelectSingleNode("//header");
            if (node != null) node.ParentNode.ReplaceChild(HtmlAgilityPack.HtmlNode.CreateNode(req.header), node);
        }
        if (!string.IsNullOrEmpty(req.footer))
        {
            var node = doc.DocumentNode.SelectSingleNode("//footer");
            if (node != null) node.ParentNode.ReplaceChild(HtmlAgilityPack.HtmlNode.CreateNode(req.footer), node);
        }

        doc.Save(file, System.Text.Encoding.UTF8);
    }
    return Results.Ok(new { message = "layout synced" });
});

app.MapPost("/api/editor/save", async (HttpContext ctx) =>
{
    var file = ctx.Request.Query["file"].ToString();
    if (string.IsNullOrWhiteSpace(file) || file.Contains("/") || file.Contains("\\"))
        return Results.BadRequest();
    using var reader = new StreamReader(ctx.Request.Body);
    var html = await reader.ReadToEndAsync();
    
    var filePath = Path.Combine(editorDir, file);
    await File.WriteAllTextAsync(filePath, html);

    if (file.ToLower() == "нүүр.html")
    {
        var mainDoc = new HtmlAgilityPack.HtmlDocument();
        mainDoc.Load(filePath);
        
        var mHeader = mainDoc.DocumentNode.SelectSingleNode("//div[contains(@class, 'kingster-mobile-header-wrap')]")?.OuterHtml;
        var topBar = mainDoc.DocumentNode.SelectSingleNode("//div[contains(@class, 'kingster-top-bar')]")?.OuterHtml;
        var header = mainDoc.DocumentNode.SelectSingleNode("//header")?.OuterHtml;
        var footer = mainDoc.DocumentNode.SelectSingleNode("//footer")?.OuterHtml;

        var allFiles = Directory.GetFiles(editorDir, "*.html");
        foreach (var f in allFiles)
        {
            var fname = Path.GetFileName(f).ToLower();
            if (fname == "нүүр.html" || fname == "manager.html" || fname == "grapesjs.html") continue;

            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.Load(f);
            bool changed = false;

            if (!string.IsNullOrEmpty(mHeader)) {
                var node = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'kingster-mobile-header-wrap')]");
                if (node != null) { node.ParentNode.ReplaceChild(HtmlAgilityPack.HtmlNode.CreateNode(mHeader), node); changed = true; }
            }
            if (!string.IsNullOrEmpty(topBar)) {
                var node = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'kingster-top-bar')]");
                if (node != null) { node.ParentNode.ReplaceChild(HtmlAgilityPack.HtmlNode.CreateNode(topBar), node); changed = true; }
            }
            if (!string.IsNullOrEmpty(header)) {
                var node = doc.DocumentNode.SelectSingleNode("//header");
                if (node != null) { node.ParentNode.ReplaceChild(HtmlAgilityPack.HtmlNode.CreateNode(header), node); changed = true; }
            }
            if (!string.IsNullOrEmpty(footer)) {
                var node = doc.DocumentNode.SelectSingleNode("//footer");
                if (node != null) { node.ParentNode.ReplaceChild(HtmlAgilityPack.HtmlNode.CreateNode(footer), node); changed = true; }
            }

            if (changed) doc.Save(f, System.Text.Encoding.UTF8);
        }
    }

    return Results.Ok(new { message = "saved" });
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

class SyncLayoutReq
{
    public string mobileHeader { get; set; }
    public string topBar { get; set; }
    public string header { get; set; }
    public string footer { get; set; }
}
