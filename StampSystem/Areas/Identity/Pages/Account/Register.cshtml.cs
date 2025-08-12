using Azure.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using StampSystem.Data;
using StampSystem.Models;
using StampSystem.Utility;
using System.ComponentModel.DataAnnotations;
using System.Security.Policy;
using System.Text;
using System.Text.Encodings.Web;
//using static System.Net.Mime.MediaTypeNames;

public class RegisterModel : PageModel
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserStore<ApplicationUser> _userStore;
    private readonly IUserEmailStore<ApplicationUser> _emailStore;
    private readonly ILogger<RegisterModel> _logger;
    private readonly IEmailSender _emailSender;
    private readonly ApplicationDbContext _context;
    
    public RegisterModel(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IUserStore<ApplicationUser> userStore,
        SignInManager<ApplicationUser> signInManager,
        ILogger<RegisterModel> logger,
        IEmailSender emailSender,
        ApplicationDbContext context)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _userStore = userStore;
        _emailStore = GetEmailStore();
        _signInManager = signInManager;
        _logger = logger;
        _emailSender = emailSender;
        _context = context;
    }

    public List<SelectListItem>? AdministrationList { get; private set; } = new List<SelectListItem>();
    public List<SelectListItem>? SectionList { get; private set; } = new List<SelectListItem>();
    public List<SelectListItem>? RoleList { get; set; } = new List<SelectListItem>();


    [BindProperty]
    public InputModel Input { get; set; }
    
    public string ReturnUrl { get; set; }
    public IList<AuthenticationScheme> ExternalLogins { get; set; }

    public class InputModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Role List")]
        public string? Role { get; set; }
        

        public string FullName { get; set; }
        [Required]
        [Display(Name = "National ID")]
        public string NationalID { get; set; }
        [Required]
        public int EmployeeId { get; set; }
        [Display(Name = "Administration")]
        public int? AdministrationId { get; set; } // Dropdown
        public string? AdministrationName { get; set; } // Textbox

        public int? SectionId { get; set; } // Dropdown
        public string? SectionName { get; set; } // Textbox

        public string? UnitName { get; set; } // Always textbox
        
        [Required]
        public string Status { get; set; } = "pending"; // Default status

        [Required]
        public string PhoneNumber { get; set; }
    }

    // When the page is requested (GET)
    public async Task OnGetAsync( string returnUrl = null)
    {
        if (!await _roleManager.RoleExistsAsync(CD.Role_HR))
        {
            await _roleManager.CreateAsync(new IdentityRole(CD.Role_HR));
            await _roleManager.CreateAsync(new IdentityRole(CD.Role_DivisionDirector));
            await _roleManager.CreateAsync(new IdentityRole(CD.Role_SectionManager));
            await _roleManager.CreateAsync(new IdentityRole(CD.Role_UnitManager));
        }


        
            RoleList = _roleManager.Roles.Select(x =>new SelectListItem
            {
                Text = x.Name,
                Value = x.Name
            }).ToList();
        

        AdministrationList = _context.Administrations
    .Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.AdministrationName }).ToList();

        SectionList = _context.Sections
            .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.SectionName }).ToList();

        ReturnUrl = returnUrl?? Url.Content("~/");
        ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

        Input = new InputModel();
    }

    // When the form is posted (POST)
    public async Task<IActionResult> OnPostAsync(string returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");
        ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

        // التحقق من صحة الإدخالات حسب الدور
        if (Input.Role == "DivisionDirector" && string.IsNullOrWhiteSpace(Input.AdministrationName))
        {
            ModelState.AddModelError("Input.AdministrationName", "اسم الإدارة مطلوب لمدير الإدارة.");
        }
        else if (Input.Role == "SectionManager")
        {
            if (!Input.AdministrationId.HasValue)
            {
                ModelState.AddModelError("Input.AdministrationId", "اختر الإدارة أولاً.");
            }
            if (string.IsNullOrWhiteSpace(Input.SectionName))
            {
                ModelState.AddModelError("Input.SectionName", "اسم القسم مطلوب لرئيس القسم.");
            }
        }
        else if (Input.Role == "UnitManager")
        {
            if (!Input.AdministrationId.HasValue)
            {
                ModelState.AddModelError("Input.AdministrationId", "اختر الإدارة أولاً.");
            }
            if (!Input.SectionId.HasValue)
            {
                ModelState.AddModelError("Input.SectionId", "اختر القسم أولاً.");
            }
            if (string.IsNullOrWhiteSpace(Input.UnitName))
            {
                ModelState.AddModelError("Input.UnitName", "اسم الوحدة مطلوب لرئيس الوحدة.");
            }
        }

        if (string.IsNullOrEmpty(Input.Role))
        {
            ModelState.AddModelError("Input.Role", "Please select a role.");
        }

        if (!ModelState.IsValid)
        {
            // إعادة تحميل القوائم
            RoleList = _roleManager.Roles.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Name
            }).ToList();

            AdministrationList = _context.Administrations.Select(a => new SelectListItem
            {
                Value = a.Id.ToString(),
                Text = a.AdministrationName
            }).ToList();

            SectionList = _context.Sections.Select(s => new SelectListItem
            {
                Value = s.Id.ToString(),
                Text = s.SectionName
            }).ToList();

            return Page();
        }

        // هنا نطبق إضافة/تحديث الإدارات والأقسام والوحدات حسب الدور
        if (Input.Role == "DivisionDirector" && !string.IsNullOrWhiteSpace(Input.AdministrationName))
        {
            var existingAdmin = await _context.Administrations
                .FirstOrDefaultAsync(a => a.AdministrationName == Input.AdministrationName);

            if (existingAdmin == null)
            {
                var newAdmin = new Administration { AdministrationName = Input.AdministrationName };
                _context.Administrations.Add(newAdmin);
                await _context.SaveChangesAsync();
                Input.AdministrationId = newAdmin.Id;
            }
            else
            {
                Input.AdministrationId = existingAdmin.Id;
            }
        }
        else if (Input.Role == "SectionManager")
        {
            // ممكن تضيف هنا لو تبي إنشاء قسم جديد بنفس الطريقة لو ما موجود
            // لكن حسب الكود الحالي تفترض أنه تم اختيار قسم موجود
        }
        else if (Input.Role == "UnitManager")
        {
            // نفس الشيء للوحدة، ممكن تضيف إنشاء جديد هنا لو تحب
        }

        // إنشاء كائن المستخدم مع القيم المحدثة
        var user = new ApplicationUser
        {
            UserName = Input.Email,
            Email = Input.Email,
            FullName = Input.FullName,
            NationalID = Input.NationalID,
            PhoneNumber = Input.PhoneNumber,
            EmployeeId = Input.EmployeeId,
            AdministrationId = Input.AdministrationId,
            SectionId = Input.SectionId,
            UnitName = Input.UnitName,
            Role = Input.Role,
            Status = "Pending"
        };

        var result = await _userManager.CreateAsync(user, Input.Password);

        if (result.Succeeded)
        {
            var roleResult = await _userManager.AddToRoleAsync(user, Input.Role);
            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);
                ModelState.AddModelError("", "فشل في تعيين الدور للمستخدم.");
                // إعادة تحميل القوائم قبل إعادة الصفحة
                RoleList = _roleManager.Roles.Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Name
                }).ToList();

                AdministrationList = _context.Administrations.Select(a => new SelectListItem
                {
                    Value = a.Id.ToString(),
                    Text = a.AdministrationName
                }).ToList();

                SectionList = _context.Sections.Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.SectionName
                }).ToList();

                return Page();
            }

            return RedirectToAction("RegistrationPending", "RegistrationRequests");
        }
        else
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            // إعادة تحميل القوائم قبل إعادة الصفحة
            RoleList = _roleManager.Roles.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Name
            }).ToList();

            AdministrationList = _context.Administrations.Select(a => new SelectListItem
            {
                Value = a.Id.ToString(),
                Text = a.AdministrationName
            }).ToList();

            SectionList = _context.Sections.Select(s => new SelectListItem
            {
                Value = s.Id.ToString(),
                Text = s.SectionName
            }).ToList();

            return Page();
        }
    }


    /* إنشاء كائن الطلب الجديد
    var registrationRequest = new RegistrationRequest
        {
            //Name = Input.Email,
            Email = Input.Email,
            FullName = Input.FullName,
            NationalID = Input.NationalID,
            EmployeeId = Input.EmployeeId,
            Role = Input.Role ?? "",
            PhoneNumber = Input.PhoneNumber,
            Status = "pending",  // في انتظار الموافقة
        };

        // ربط الإدارات والأقسام حسب الدور
        if (Input.Role == "DivisionDirector")
        {
            if (!string.IsNullOrWhiteSpace(Input.AdministrationName))
            {
                var existingAdmin = await _context.Administrations
                    .FirstOrDefaultAsync(a => a.AdministrationName == Input.AdministrationName);

                if (existingAdmin == null)
                {
                    var newAdmin = new Administration { AdministrationName = Input.AdministrationName };
                    _context.Administrations.Add(newAdmin);
                    await _context.SaveChangesAsync();
                    registrationRequest.AdministrationId = newAdmin.Id;
                }
                else
                {
                    registrationRequest.AdministrationId = existingAdmin.Id;
                }
            }
        }
        else if (Input.Role == "SectionManager")
        {
            registrationRequest.AdministrationId = Input.AdministrationId;
            registrationRequest.SectionName = Input.SectionName;
        }
        else if (Input.Role == "UnitManager")
        {
            registrationRequest.AdministrationId = Input.AdministrationId;
            registrationRequest.SectionId = Input.SectionId;
            registrationRequest.UnitName = Input.UnitName;
        }

        // إضافة الطلب إلى قاعدة البيانات
        _context.RegistrationRequests.Add(registrationRequest);
        await _context.SaveChangesAsync();


        // إرسال رسالة تأكيد (اختياري)
        // await _emailSender.SendEmailAsync(...);

        // إعادة توجيه المستخدم إلى صفحة تأكيد التسجيل مع انتظار الموافقة
        return RedirectToAction("RegistrationPending", "RegistrationRequests");

    }
    */


    private IdentityUser CreateUser()
    {
        try
        {
            return Activator.CreateInstance<ApplicationUser>();
        }
        catch
        {
            throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor.");
        }
    }

    private IUserEmailStore<ApplicationUser> GetEmailStore()
    {
        if (!_userManager.SupportsUserEmail)
        {
            throw new NotSupportedException("The default UI requires a user store with email support.");
        }
        return (IUserEmailStore<ApplicationUser>)_userStore;
    }
}
