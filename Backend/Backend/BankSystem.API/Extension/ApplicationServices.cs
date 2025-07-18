using BankSystem.Data.Entities;
using BankSystem.Data.Repositories;
using BankSystem.Repository.Repositories;
using BankSystem.Repository.RepositoryInterfaces;
using BankSystem.Service.Helper;
using BankSystem.Service.Services.AccountService;
using BankSystem.Service.Services.ComplainService;
using BankSystem.Service.Services.FileScanService;
using BankSystem.Service.Services.Security;
using BankSystem.Service.Services.SubscriptionService;
using BankSystem.Service.Services.TransactionService;
using BankSystem.Service.Services.UserService;
using Microsoft.AspNetCore.Identity;
using BankSystem.Service.Helper.SMSService;
using BankSystem.Service.Helper.SMSServices;
using BankSystem.Service.Helper.IOTPService;
using BankSystem.Service.Services;
using BankSystem.Repository.Repositories.BankSystem.Data.Repositories;
using BankSystem.Service.Services.ReportService;
using BankSystem.Service.Services.FileHashService;
using BankSystem.Service.Services.LoanService;

namespace BankSystem.API.Extension
{
    public static class ApplicationServices
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {

            services.AddScoped<IRepository<Account>, AccountRepository>();
            services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IRepository<Account>, AccountRepository>();
            services.AddScoped<IRepository<Transaction>, TransactionRepository>();
            services.AddScoped<IRepository<Subscription>, SubscriptionRepository>();

            // Specific Repositories
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<ITransactionRepository, TransactionRepository>();
            services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
            services.AddScoped<IUserRepository, UserRepository>();

            services.AddScoped<FilesRepository>();
            services.AddScoped<IFilesRepository, FilesRepository>();
            services.AddScoped<ILoginService, LoginService>();
            services.AddScoped<ISMSService, SMSService>();
            services.AddScoped<IOTPService, OTPService>();
            services.AddScoped<ILoanRepository, LoanRepository>();



            // Services
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<ITransactionService, TransactionService>();
            services.AddScoped<IUserService, UserService>();

            services.AddScoped<IFileHashService, FileHashService>();
            services.AddScoped<FileScanService>();
            services.AddScoped<IFileScanService, FileScanService>();
            services.AddScoped<IComplainRepository, ComplainRepository>();
            services.AddScoped<IComplainService, ComplainService>();
            services.AddScoped<IChequeService, ChequeService>();
            services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
            services.AddTransient<ISMSService, SMSService>();
            services.AddScoped<IMessageRepository, MessageRepository>();
            services.AddScoped<IReportService , ReportService>();
            services.AddScoped<ILoanService, LoanService>();





            services.AddScoped<VirusTotalService>(provider =>
                new VirusTotalService(provider.GetRequiredService<IConfiguration>()["VirusTotal:ApiKey"]));

            // Utility Services
            services.AddScoped<EmailService>();
            services.AddScoped<OTPService>();
            services.AddScoped<InterestService>();
            services.AddScoped<TaxService>();
            services.AddSingleton<SharedDb>();


            // Password Hasher
            services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
        }
    }
}
