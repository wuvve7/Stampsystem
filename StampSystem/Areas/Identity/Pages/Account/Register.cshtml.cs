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
    .Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.Name }).ToList();

        SectionList = _context.Sections
            .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.Name }).ToList();

        ReturnUrl = returnUrl?? Url.Content("~/");
        ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

        Input = new InputModel();
    }

    // When the form is posted (POST)
    public async Task<IActionResult> OnPostAsync( string returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");
        ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

        // تحقق حسب الدور
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

        if (!ModelState.IsValid)
        {
            // أعد ملء القوائم dropdown قبل عرض الصفحة
            RoleList = _roleManager.Roles.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Name
            }).ToList();

            AdministrationList = _context.Administrations.Select(a => new SelectListItem
            {
                Value = a.Id.ToString(),
                Text = a.Name
            }).ToList();

            SectionList = _context.Sections.Select(s => new SelectListItem
            {
                Value = s.Id.ToString(),
                Text = s.Name
            }).ToList();

            return Page();
        }

        if (ModelState.IsValid)
        {
            
            var user = new ApplicationUser
            {
                UserName = Input.Email,
                Email = Input.Email,
                FullName = Input.FullName,
                NationalID = Input.NationalID,
                EmployeeId = Input.EmployeeId,
               // Role = Input.Role,
                PhoneNumber = Input.PhoneNumber,
                Status = "pending", // Default status
            };
            // ======= هنا تبدأ التعديلات =======
            if (Input.Role == "DivisionDirector")
            {
                if (!string.IsNullOrWhiteSpace(Input.AdministrationName))
                {
                    // تحقق إذا الإدارة موجودة مسبقًا
                    var existingAdmin = await _context.Administrations
                        .FirstOrDefaultAsync(a => a.Name == Input.AdministrationName);

                    if (existingAdmin == null)
                    {
                        // إنشاء إدارة جديدة وحفظها
                        var newAdmin = new Administration { Name = Input.AdministrationName };
                        _context.Administrations.Add(newAdmin);
                        await _context.SaveChangesAsync();

                        // ربط المستخدم بالإدارة الجديدة
                        user.AdministrationId = newAdmin.Id;
                    }
                    else
                    {
                        // لو الإدارة موجودة، اربط المستخدم بها
                        user.AdministrationId = existingAdmin.Id;
                    }
                }
            }
            else if (Input.Role == "SectionManager")
            {
                // ربط المستخدم بالإدارة المختارة (من dropdown)
                user.AdministrationId = Input.AdministrationId;

                // إضافة اسم القسم (يمكنك أيضاً حفظه في جدول أقسام حسب تصميمك)
                user.SectionName = Input.SectionName;
            }
            else if (Input.Role == "UnitManager")
            {
                user.AdministrationId = Input.AdministrationId;
                user.SectionId = Input.SectionId;
                user.UnitName = Input.UnitName;
            }

            var result = await _userManager.CreateAsync(user, Input.Password);

            if (result.Succeeded)
            {
                _logger.LogInformation("User created a new account with password.");

                if (!string.IsNullOrEmpty(Input.Role))
                {
                    await _userManager.AddToRoleAsync(user, Input.Role);
                }

                var userId = await _userManager.GetUserIdAsync(user);
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ConfirmEmail",
                    pageHandler: null,
                    values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                    protocol: Request.Scheme);

                await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                if (_userManager.Options.SignIn.RequireConfirmedAccount)
                {
                    return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                }
                else
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        RoleList = _roleManager.Roles.Select(x => new SelectListItem
        {
            Text = x.Name,
            Value = x.Name
        }).ToList();

        AdministrationList = _context.Administrations.Select(a => new SelectListItem
        {
            Value = a.Id.ToString(),
            Text = a.Name
        }).ToList();

        SectionList = _context.Sections.Select(s => new SelectListItem
        {
            Value = s.Id.ToString(),
            Text = s.Name
        }).ToList();

        return Page();
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
