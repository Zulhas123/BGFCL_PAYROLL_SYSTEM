using Microsoft.EntityFrameworkCore;
using Entities;
using Microsoft.Data;
using System.Diagnostics.Contracts;
using Contracts;
using Repositories;
using Microsoft.AspNetCore.Builder;
using BgfclApp.ViewModels;
using System.Globalization;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using BgfclApp.Service;

var builder = WebApplication.CreateBuilder(args);


//Jwt configuration starts here
var jwtIssuer = builder.Configuration.GetSection("Jwt:Issuer").Get<string>();
var jwtKey = builder.Configuration.GetSection("Jwt:Key").Get<string>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
 .AddJwtBearer(options =>
 {
     options.TokenValidationParameters = new TokenValidationParameters
     {
         ValidateIssuer = true,
         ValidateAudience = true,
         ValidateLifetime = true,
         ValidateIssuerSigningKey = true,
         ValidIssuer = jwtIssuer,
         ValidAudience = jwtIssuer,
         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
     };
 });
//Jwt configuration ends here


builder.Services.AddControllersWithViews();

// custom services.
builder.Services.AddSingleton<BgfclContext>();
builder.Services.AddSingleton<ResponseViewModel>();
builder.Services.AddScoped<ICategoryContract, CategoryRepository>();
builder.Services.AddScoped<IDepartmentContract, DepartmentRepository>();
builder.Services.AddScoped<IDesignationContract, DesignationRepository>();
builder.Services.AddScoped<IBankTagContract, BankTagRepository>();
builder.Services.AddScoped<IBankTypeContract, BankTypeRepository>();
builder.Services.AddScoped<IBankContract, BankRepository>();
builder.Services.AddScoped<ILocationContract, LocationRepository>();
builder.Services.AddScoped<IEmployeeTypeContract, EmployeeTypeRepository>();
builder.Services.AddScoped<IGradeContract, GradeRepository>();
builder.Services.AddScoped<IBranchContract, BranchRepository>();
builder.Services.AddScoped<IGenderContract, GenderRepository>();
builder.Services.AddScoped<IReligionContract, ReligionRepository>();
builder.Services.AddScoped<IMaritalContract, MaritalRepository>();
builder.Services.AddScoped<IEmployeeContract, EmployeeRepository>();
builder.Services.AddScoped<IActiveStatusContract, ActiveStatusRepository>();
builder.Services.AddScoped<ISalarySettingContract, SalarySettingRepository>();
builder.Services.AddScoped<IAdvanceTaxContract, AdvanceTaxRepository>();
builder.Services.AddScoped<ILoanContract, LoanRepository>();
builder.Services.AddScoped<IBonusContract, BonusRepository>();
builder.Services.AddScoped<ISalaryReportOfficerContract, SalaryReportOfficerRepository>();
builder.Services.AddScoped<IBonusSheetContract, BonusSheetRepository>();
builder.Services.AddScoped<IAmenitiesReportContract, AmenitiesReportRepository>();
builder.Services.AddScoped<IMonthlyIncomeTexContract, MonthlyIncomeTexRepository>();
builder.Services.AddScoped<IYearlyIncomeTaxContract, YearlyIncomeTaxRepository>();
builder.Services.AddScoped<IPFSheetReportContract, PFSheetReportRepository>();
builder.Services.AddScoped<IAmenitiesContract, AmenitiesRepository>();
builder.Services.AddScoped<IReportContract, ReportRepository>();
builder.Services.AddScoped<IUserContract, UserRepository>();
builder.Services.AddScoped<IRolesContract, RolesRepository>();
builder.Services.AddScoped<ISchoolContract, SchoolRepository>();
builder.Services.AddScoped<IDataImportContract, DataImportRepository>();
builder.Services.AddScoped<IPFGratuityContract, PFGratuityRepository>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure global culture settings
var defaultCulture = "en-US";
var supportedCultures = new[] { new CultureInfo(defaultCulture) };

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture(defaultCulture);
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

var cultureInfo = new System.Globalization.CultureInfo("en-US");
System.Globalization.CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseStatusCodePages();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Apply culture settings
app.UseRequestLocalization(app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Login}/{id?}");
    //pattern: "{controller=Dashboard}/{action=Index}/{id?}"); 

app.Run();
