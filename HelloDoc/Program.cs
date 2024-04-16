using DataAccess.ServiceRepository;
using DataAccess.ServiceRepository.IServiceRepository;
using HelloDoc.DataContext;
using HelloDoc.Views.Shared;
using Services.Contracts;
using Services.Implementation;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<HelloDocDbContext>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<IRequestRepository , RequestRepository>();
builder.Services.AddTransient<IAdminCredential, AdminCredential>();
builder.Services.AddScoped<IRequestDataRepository , RequestDataRepository>();
builder.Services.AddScoped<IViewCaseRepository,ViewCaseRepository >();
builder.Services.AddScoped<IBlockCaseRepo, BlockCaseRepo>();
builder.Services.AddScoped<IAddOrUpdateRequestStatusLog, AddOrUpdateRequestStatusLog>();
builder.Services.AddScoped<IAddOrUpdateRequestNotes, AddOrUpdateRequestNotes>();
builder.Services.AddScoped<IVendorRepository, VendorRepository>();
builder.Services.AddScoped<IunitOfWork, unitOfWork>();
builder.Services.AddScoped<IRecordRepository, RecordRepository>();
builder.Services.AddScoped<IAdd, Add>();
builder.Services.AddScoped<IUpdateData, UpdateData>();



builder.Services.AddScoped<IAuthorizatoinRepository, AuthorizationRepository>();
builder.Services.AddScoped<IJwtRepository, JwtRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
//pattern: "{controller=home}/{action=index}/{id?}");
pattern: "{controller=Admin}/{action=Admindashboard}/{id?}");


app.Run();
