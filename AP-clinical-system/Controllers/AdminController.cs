using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AP_clinical_system.Models.sql_Context;
using System.Reflection;
using System.Text.Json;

namespace AP_clinical_system.Controllers
{
    public class AdminController : Controller
    {
        private readonly AP_Context _context;
        private const string AdminPassword = "8687";
        private const string SessionKey = "AdminAuthenticated";

        public AdminController(AP_Context context)
        {
            _context = context;
        }

        // ─── Helpers ───────────────────────────────────────────────

        private bool IsAuthenticated()
        {
            return HttpContext.Session.GetString(SessionKey) == "true";
        }

        /// Returns the mapping: entity display name → DbSet property name
        private Dictionary<string, string> GetEntityMap()
        {
            var props = typeof(AP_Context)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.PropertyType.IsGenericType &&
                            p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>));

            var map = new Dictionary<string, string>();
            foreach (var p in props)
            {
                // Key = entity type name (e.g. "appointment"), Value = DbSet property name
                var entityType = p.PropertyType.GetGenericArguments()[0];
                map[entityType.Name] = p.Name;
            }
            return map;
        }

        private (IQueryable queryable, Type entityType)? GetDbSet(string entityName)
        {
            var prop = typeof(AP_Context)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.PropertyType.IsGenericType &&
                            p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
                .FirstOrDefault(p => p.PropertyType.GetGenericArguments()[0].Name == entityName);

            if (prop == null) return null;

            var dbSet = prop.GetValue(_context);
            var entityType = prop.PropertyType.GetGenericArguments()[0];

            // Cast to IQueryable
            var queryable = (IQueryable)dbSet!;
            return (queryable, entityType);
        }

        // ─── Pages ─────────────────────────────────────────────────

        [Route("admin")]
        public IActionResult Index()
        {
            if (IsAuthenticated())
            {
                var entities = GetEntityMap();
                var first = entities.Keys.FirstOrDefault();
                if (first != null)
                    return Redirect($"/admin/{first}");
            }
            return View("Login");
        }

        [HttpPost]
        [Route("admin/login")]
        public IActionResult Login(string password)
        {
            if (password == AdminPassword)
            {
                HttpContext.Session.SetString(SessionKey, "true");
                return Redirect("/admin");
            }
            ViewBag.Error = "Invalid password";
            return View("Login");
        }

        [Route("admin/logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove(SessionKey);
            return Redirect("/admin");
        }

        [Route("admin/{entityName}")]
        public IActionResult List(string entityName)
        {
            if (!IsAuthenticated()) return Redirect("/admin");

            var result = GetDbSet(entityName);
            if (result == null) return NotFound();

            ViewBag.EntityName = entityName;
            ViewBag.Entities = GetEntityMap();
            return View("List");
        }

        [Route("admin/{entityName}/new")]
        public IActionResult FormNew(string entityName)
        {
            if (!IsAuthenticated()) return Redirect("/admin");

            var result = GetDbSet(entityName);
            if (result == null) return NotFound();

            ViewBag.EntityName = entityName;
            ViewBag.RecordId = "";
            ViewBag.Entities = GetEntityMap();
            return View("Form");
        }

        [Route("admin/{entityName}/{id:guid}")]
        public IActionResult FormEdit(string entityName, Guid id)
        {
            if (!IsAuthenticated()) return Redirect("/admin");

            var result = GetDbSet(entityName);
            if (result == null) return NotFound();

            ViewBag.EntityName = entityName;
            ViewBag.RecordId = id.ToString();
            ViewBag.Entities = GetEntityMap();
            return View("Form");
        }

        // ─── API Endpoints ────────────────────────────────────────

        [HttpGet]
        [Route("admin/api/{entityName}/schema")]
        public IActionResult GetSchema(string entityName)
        {
            if (!IsAuthenticated()) return Unauthorized();

            var result = GetDbSet(entityName);
            if (result == null) return NotFound();

            var props = result.Value.entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var schema = props.Select(p => new
            {
                name = p.Name,
                type = GetFriendlyTypeName(p.PropertyType),
                isNullable = Nullable.GetUnderlyingType(p.PropertyType) != null ||
                             !p.PropertyType.IsValueType
            }).ToList();

            return Json(schema);
        }

        [HttpGet]
        [Route("admin/api/{entityName}/list")]
        public async Task<IActionResult> GetList(string entityName)
        {
            if (!IsAuthenticated()) return Unauthorized();

            var result = GetDbSet(entityName);
            if (result == null) return NotFound();

            var entityType = result.Value.entityType;
            var queryable = result.Value.queryable;

            // Use EF Core's generic method to list all
            var listMethod = typeof(EntityFrameworkQueryableExtensions)
                .GetMethod(nameof(EntityFrameworkQueryableExtensions.ToListAsync),
                    new[] { typeof(IQueryable<>).MakeGenericType(Type.MakeGenericMethodParameter(0)), typeof(CancellationToken) });

            // Fallback: materialize via reflection
            var data = new List<Dictionary<string, object?>>();
            var props = entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            // Use a generic approach
            var toListAsyncMethod = typeof(EntityFrameworkQueryableExtensions)
                .GetMethods()
                .First(m => m.Name == "ToListAsync" && m.GetParameters().Length == 2)
                .MakeGenericMethod(entityType);

            var task = (Task)toListAsyncMethod.Invoke(null, new object[] { queryable, CancellationToken.None })!;
            await task;
            var resultProperty = task.GetType().GetProperty("Result")!;
            var items = (System.Collections.IEnumerable)resultProperty.GetValue(task)!;

            foreach (var item in items)
            {
                var dict = new Dictionary<string, object?>();
                foreach (var prop in props)
                {
                    var val = prop.GetValue(item);
                    dict[prop.Name] = val;
                }
                data.Add(dict);
            }

            return Json(data);
        }

        [HttpGet]
        [Route("admin/api/{entityName}/{id:guid}")]
        public async Task<IActionResult> GetRecord(string entityName, Guid id)
        {
            if (!IsAuthenticated()) return Unauthorized();

            var result = GetDbSet(entityName);
            if (result == null) return NotFound();

            var entityType = result.Value.entityType;
            var entity = await _context.FindAsync(entityType, id);
            if (entity == null) return NotFound();

            var props = entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var dict = new Dictionary<string, object?>();
            foreach (var prop in props)
            {
                dict[prop.Name] = prop.GetValue(entity);
            }
            return Json(dict);
        }

        [HttpPost]
        [Route("admin/api/{entityName}/save")]
        public async Task<IActionResult> Save(string entityName, [FromBody] JsonElement body)
        {
            if (!IsAuthenticated()) return Unauthorized();

            var result = GetDbSet(entityName);
            if (result == null) return NotFound();

            var entityType = result.Value.entityType;
            var props = entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            // Determine if this is create or update
            Guid? existingId = null;
            if (body.TryGetProperty("id", out var idProp) && idProp.ValueKind == JsonValueKind.String)
            {
                if (Guid.TryParse(idProp.GetString(), out var parsedId) && parsedId != Guid.Empty)
                {
                    existingId = parsedId;
                }
            }

            object entity;
            if (existingId.HasValue)
            {
                entity = await _context.FindAsync(entityType, existingId.Value);
                if (entity == null) return NotFound();
            }
            else
            {
                entity = Activator.CreateInstance(entityType)!;
                // Set default id
                var idPropertyInfo = entityType.GetProperty("id");
                if (idPropertyInfo != null)
                    idPropertyInfo.SetValue(entity, Guid.NewGuid());

                // Set createdon
                var createdOnProp = entityType.GetProperty("createdon");
                if (createdOnProp != null)
                    createdOnProp.SetValue(entity, DateTime.UtcNow);
            }

            // Set modifiedon
            var modifiedOnProp = entityType.GetProperty("modifiedon");
            if (modifiedOnProp != null)
                modifiedOnProp.SetValue(entity, DateTime.UtcNow);

            // Map fields from body
            foreach (var prop in props)
            {
                if (prop.Name == "id" && existingId.HasValue) continue; // Don't overwrite PK on update
                if (prop.Name == "createdon" && existingId.HasValue) continue; // Don't overwrite createdon on update
                if (prop.Name == "modifiedon") continue; // Already set above

                if (body.TryGetProperty(prop.Name, out var jsonVal))
                {
                    try
                    {
                        var value = ConvertJsonElement(jsonVal, prop.PropertyType);
                        prop.SetValue(entity, value);
                    }
                    catch { /* Skip fields that fail conversion */ }
                }
            }

            if (!existingId.HasValue)
            {
                _context.Add(entity);
            }
            else
            {
                _context.Update(entity);
            }

            await _context.SaveChangesAsync();

            // Return the saved entity's id
            var savedId = entityType.GetProperty("id")?.GetValue(entity);
            return Json(new { success = true, id = savedId });
        }

        [HttpPost]
        [Route("admin/api/{entityName}/delete")]
        public async Task<IActionResult> Delete(string entityName, [FromBody] JsonElement body)
        {
            if (!IsAuthenticated()) return Unauthorized();

            var result = GetDbSet(entityName);
            if (result == null) return NotFound();

            var entityType = result.Value.entityType;

            if (!body.TryGetProperty("ids", out var idsArray)) return BadRequest();

            var deleted = 0;
            foreach (var idEl in idsArray.EnumerateArray())
            {
                if (Guid.TryParse(idEl.GetString(), out var id))
                {
                    var entity = await _context.FindAsync(entityType, id);
                    if (entity != null)
                    {
                        _context.Remove(entity);
                        deleted++;
                    }
                }
            }

            await _context.SaveChangesAsync();
            return Json(new { success = true, deleted });
        }

        [HttpPost]
        [Route("admin/api/{entityName}/set-inactive")]
        public async Task<IActionResult> SetInactive(string entityName, [FromBody] JsonElement body)
        {
            if (!IsAuthenticated()) return Unauthorized();

            var result = GetDbSet(entityName);
            if (result == null) return NotFound();

            var entityType = result.Value.entityType;
            var inactiveProp = entityType.GetProperty("inactive");
            if (inactiveProp == null) return BadRequest("Entity does not have an 'inactive' field");

            if (!body.TryGetProperty("ids", out var idsArray)) return BadRequest();
            if (!body.TryGetProperty("value", out var valueEl)) return BadRequest();

            bool inactiveValue = valueEl.GetBoolean();
            var updated = 0;

            foreach (var idEl in idsArray.EnumerateArray())
            {
                if (Guid.TryParse(idEl.GetString(), out var id))
                {
                    var entity = await _context.FindAsync(entityType, id);
                    if (entity != null)
                    {
                        inactiveProp.SetValue(entity, (bool?)inactiveValue);
                        _context.Update(entity);
                        updated++;
                    }
                }
            }

            await _context.SaveChangesAsync();
            return Json(new { success = true, updated });
        }

        // ─── Type Conversion Helpers ──────────────────────────────

        private string GetFriendlyTypeName(Type type)
        {
            var underlying = Nullable.GetUnderlyingType(type);
            var t = underlying ?? type;

            if (t == typeof(Guid)) return "guid";
            if (t == typeof(string)) return "string";
            if (t == typeof(int)) return "int";
            if (t == typeof(long)) return "long";
            if (t == typeof(bool)) return "bool";
            if (t == typeof(DateTime)) return "datetime";
            if (t == typeof(DateOnly)) return "date";
            if (t == typeof(TimeOnly)) return "time";
            if (t == typeof(decimal)) return "decimal";
            if (t == typeof(double)) return "double";
            if (t == typeof(float)) return "float";
            return t.Name.ToLower();
        }

        private object? ConvertJsonElement(JsonElement el, Type targetType)
        {
            var underlying = Nullable.GetUnderlyingType(targetType);
            var t = underlying ?? targetType;

            if (el.ValueKind == JsonValueKind.Null || el.ValueKind == JsonValueKind.Undefined)
            {
                return underlying != null || !targetType.IsValueType ? null : Activator.CreateInstance(targetType);
            }

            var str = el.ToString();

            if (string.IsNullOrEmpty(str) && (underlying != null || !targetType.IsValueType))
                return null;

            if (t == typeof(Guid)) return Guid.Parse(str);
            if (t == typeof(string)) return str;
            if (t == typeof(int)) return int.Parse(str);
            if (t == typeof(long)) return long.Parse(str);
            if (t == typeof(bool))
            {
                if (el.ValueKind == JsonValueKind.True) return true;
                if (el.ValueKind == JsonValueKind.False) return false;
                return bool.Parse(str);
            }
            if (t == typeof(DateTime)) return DateTime.Parse(str);
            if (t == typeof(DateOnly)) return DateOnly.Parse(str);
            if (t == typeof(TimeOnly)) return TimeOnly.Parse(str);
            if (t == typeof(decimal)) return decimal.Parse(str);
            if (t == typeof(double)) return double.Parse(str);
            if (t == typeof(float)) return float.Parse(str);

            return str;
        }
    }
}
