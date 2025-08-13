using Azure.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StampSystem.Data;
using StampSystem.Models;
using StampSystem.Utility;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;

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

    public List<SelectListItem> AdministrationList { get; private set; } = new List<SelectListItem>();
    public List<SelectListItem> SectionList { get; private set; } = new List<SelectListItem>();
    public List<SelectListItem> RoleList { get; set; } = new List<SelectListItem>();

    [BindProperty]
    public InputModel Input { get; set; }

    public string ReturnUrl { get; set; }
    public IList<AuthenticationScheme> ExternalLogins { get; set; }

    public class InputModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "البريد الإلكتروني")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "يجب أن تكون {0} بين {2} و {1} حروف.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "كلمة المرور")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "تأكيد كلمة المرور")]
        [Compare("Password", ErrorMessage = "كلمة المرور وتأكيدها غير متطابقين.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "الدور مطلوب")]
        [Display(Name = "الدور")]
        public string Role { get; set; }

        [Required]
        [Display(Name = "الاسم الكامل")]
        public string FullName { get; set; }

        [Required]
        [Display(Name = "الرقم الوطني")]
        public string NationalID { get; set; }

        [Required]
        [Display(Name = "رقم الموظف")]
        public int EmployeeId { get; set; }

        [Display(Name = "الإدارة")]
        public int? AdministrationId { get; set; } // اختيار من القائمة

        [Display(Name = "اسم الإدارة")]
        public string? AdministrationName { get; set; } // نص حر (لـ DivisionDirector)

        [Display(Name = "القسم")]
        public int? SectionId { get; set; } // اختيار من القائمة

        [Display(Name = "اسم القسم")]
        public string? SectionName { get; set; } // نص حر (لـ SectionManager)

        [Display(Name = "اسم الوحدة")]
        public string? UnitName { get; set; } // نص حر (لـ UnitManager)

        [Required]
        [Display(Name = "رقم الهاتف")]
        public string PhoneNumber { get; set; }

        public string Status { get; set; } = "Pending";
    }

    public async Task OnGetAsync(string returnUrl = null, int? administrationId = null)
    {
        if (!await _roleManager.RoleExistsAsync(CD.Role_HR))
        {
            await _roleManager.CreateAsync(new IdentityRole(CD.Role_HR));
            await _roleManager.CreateAsync(new IdentityRole(CD.Role_DivisionDirector));
            await _roleManager.CreateAsync(new IdentityRole(CD.Role_SectionManager));
            await _roleManager.CreateAsync(new IdentityRole(CD.Role_UnitManager));
        }

        RoleList = _roleManager.Roles.Select(r => new SelectListItem
        {
            Text = r.Name,
            Value = r.Name
        }).ToList();

        AdministrationList = await _context.Administrations
            .Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.AdministrationName })
            .ToListAsync();

        if (administrationId.HasValue)
        {
            SectionList = await _context.Sections
                .Where(s => s.AdministrationId == administrationId.Value)
                .Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.SectionName
                }).ToListAsync();

            // إذا تريد تملأ الـ Input.AdministrationId بقيمة الـ administrationId اللي وصلتك:
            Input.AdministrationId = administrationId;
        }
        else
        {
            SectionList = new List<SelectListItem>();
        }

        ReturnUrl = returnUrl ?? Url.Content("~/");
        ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

        if (Input == null)
            Input = new InputModel();
    }



    public async Task<IActionResult> OnPostAsync(string returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");
        ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

        // تحقق من صحة الإدخالات بناء على الدور
        if (string.IsNullOrEmpty(Input.Role))
        {
            ModelState.AddModelError("Input.Role", "الرجاء اختيار الدور.");
        }
        else
        {
            switch (Input.Role)
            {
                case CD.Role_DivisionDirector:
                    if (string.IsNullOrWhiteSpace(Input.AdministrationName))
                        ModelState.AddModelError("Input.AdministrationName", "اسم الإدارة مطلوب لمدير الإدارة.");
                    break;

                case CD.Role_SectionManager:
                    if (!Input.AdministrationId.HasValue)
                        ModelState.AddModelError("Input.AdministrationId", "اختر الإدارة أولاً.");
                    if (string.IsNullOrWhiteSpace(Input.SectionName))
                        ModelState.AddModelError("Input.SectionName", "اسم القسم مطلوب لرئيس القسم.");
                    break;

                case CD.Role_UnitManager:
                    if (!Input.AdministrationId.HasValue)
                        ModelState.AddModelError("Input.AdministrationId", "اختر الإدارة أولاً.");
                    if (!Input.SectionId.HasValue)
                        ModelState.AddModelError("Input.SectionId", "اختر القسم أولاً.");
                    if (string.IsNullOrWhiteSpace(Input.UnitName))
                        ModelState.AddModelError("Input.UnitName", "اسم الوحدة مطلوب لرئيس الوحدة.");
                    break;
            }
        }

        if (!ModelState.IsValid)
        {
            await LoadDropDownListsAsync(Input.AdministrationId);
            return Page();
        }

        // إنشاء أو جلب الإدارة/القسم/الوحدة حسب الدور
        if (Input.Role == CD.Role_DivisionDirector)
        {
            var admin = await _context.Administrations
                .FirstOrDefaultAsync(a => a.AdministrationName == Input.AdministrationName);

            if (admin == null)
            {
                admin = new Administration { AdministrationName = Input.AdministrationName, ManagerName = Input.FullName };
                _context.Administrations.Add(admin);
                await _context.SaveChangesAsync();
            }
            Input.AdministrationId = admin.Id;
        }
        else if (Input.Role == CD.Role_SectionManager)
        {
            var section = await _context.Sections
                .FirstOrDefaultAsync(s => s.SectionName == Input.SectionName && s.AdministrationId == Input.AdministrationId);

            if (section == null)
            {
                section = new Section
                {
                    SectionName = Input.SectionName,
                    AdministrationId = Input.AdministrationId.Value,
                    HeadName = Input.FullName
                };
                _context.Sections.Add(section);
                await _context.SaveChangesAsync();
            }
            Input.SectionId = section.Id;
        }
        else if (Input.Role == CD.Role_UnitManager)
        {
            var unit = await _context.Units
                .FirstOrDefaultAsync(u => u.UnitName == Input.UnitName && u.SectionId == Input.SectionId);

            if (unit == null)
            {
                unit = new Unit
                {
                    UnitName = Input.UnitName,
                    SectionId = Input.SectionId.Value,
                    HeadName = Input.FullName
                };
                _context.Units.Add(unit);
                await _context.SaveChangesAsync();
            }
        }

        // إنشاء المستخدم
        var user = new ApplicationUser
        {
            UserName = Input.Email,
            Email = Input.Email,
            FullName = Input.FullName,
            NationalID = Input.NationalID,
            EmployeeId = Input.EmployeeId,
            PhoneNumber = Input.PhoneNumber,
            AdministrationId = Input.AdministrationId,
            SectionId = Input.SectionId,
            UnitId = Input.Role == CD.Role_UnitManager ? _context.Units.FirstOrDefault(u => u.UnitName == Input.UnitName && u.SectionId == Input.SectionId)?.Id : null,
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
                await LoadDropDownListsAsync(Input.AdministrationId);
                return Page();
            }

            return RedirectToAction("RegistrationPending", "RegistrationRequests");
        }

        foreach (var error in result.Errors)
            ModelState.AddModelError("", error.Description);

        await LoadDropDownListsAsync(Input.AdministrationId);
        return Page();
    }

    private async Task LoadDropDownListsAsync(int? administrationId = null)
    {
        RoleList = _roleManager.Roles.Select(r => new SelectListItem
        {
            Text = r.Name,
            Value = r.Name
        }).ToList();

        AdministrationList = await _context.Administrations.Select(a => new SelectListItem
        {
            Value = a.Id.ToString(),
            Text = a.AdministrationName
        }).ToListAsync();

        SectionList = administrationId.HasValue
            ? await _context.Sections
                .Where(s => s.AdministrationId == administrationId.Value)
                .Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.SectionName
                }).ToListAsync()
            : new List<SelectListItem>();
    }

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
