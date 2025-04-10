namespace LibrarySystem.Web
{
    using CloudinaryDotNet;
    using LibrarySystem.Data;
    using LibrarySystem.Data.Repository;
    using LibrarySystem.Models;
    using LibrarySystem.Services;
    using LibrarySystem.Services.IService;
    using LibrarySystem.Utility;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Threading.Tasks;

    class Program
    {
        static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            });

            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            builder.Services.AddScoped(typeof(IService<>), typeof(Service<>));
            builder.Services.AddScoped<ISectionService, SectionService>();
            builder.Services.AddScoped<IAuthorService, AuthorService>();
            builder.Services.AddScoped<ISearchService, SearchService>();

            builder.Services.AddScoped<CloudinaryService>();

            var cloudinarySettings = builder.Configuration.GetSection("Cloudinary").Get<CloudinarySettings>();
            var account = new Account(cloudinarySettings.CloudName, cloudinarySettings.ApiKey, cloudinarySettings.ApiSecret);
            var cloudinary = new Cloudinary(account);
            builder.Services.AddSingleton(cloudinary);

            builder.Services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/AccessDenied";
            });

            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            


            builder.Services.AddRazorPages();

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
            app.UseSession();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                await CreateRoles(services);
                await CreateAdmin(services);
                await CreateLibrarian(services);
                await CreateReader(services);
                await CreateAuthors(services);
                await CreateImages(services);
                await CreateSections(services);
                await CreateTitles(services);
                await CreateTitlesAuthors(services);
                await CreateLibraryUnits(services);
            }

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            await app.RunAsync();
        }

        private static async Task CreateRoles(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            string[] roleNames = { SD.AdminRole, SD.LibrarianRole, SD.UserRole };

            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }

        private static async Task CreateAdmin(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userService = serviceProvider.GetRequiredService<IService<User>>();
            var admins = await userManager.GetUsersInRoleAsync(SD.AdminRole);

            if (admins.Any())
                return;

            var adminEmail = "admin@email.com";
            var identityUser = new IdentityUser { UserName = adminEmail, Email = adminEmail };
            var result = await userManager.CreateAsync(identityUser, "AdminPassword123!");

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(identityUser, SD.AdminRole);

                var user = new User
                {
                    FirstName = "Стоян",
                    MiddleName = "Пенков",
                    LastName = "Зланков",
                    IdentityUserId = identityUser.Id
                };

                await userService.AddAsync(user);
            }
        }

        private static async Task CreateLibrarian(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userService = serviceProvider.GetRequiredService<IService<User>>();
            var librarians = await userManager.GetUsersInRoleAsync(SD.LibrarianRole);

            if (librarians.Any())
                return;

            var librarianEmail = "librarian@email.com";
            var identityUser = new IdentityUser { UserName = librarianEmail, Email = librarianEmail };
            var result = await userManager.CreateAsync(identityUser, "LibrarianPassword123!");

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(identityUser, SD.LibrarianRole);

                var user = new User
                {
                    FirstName = "Елица",
                    MiddleName = "Пенчева",
                    LastName = "Николова",
                    IdentityUserId = identityUser.Id
                };

                await userService.AddAsync(user);
            }
        }

        private static async Task CreateReader(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userService = serviceProvider.GetRequiredService<IService<User>>();
            var readers = await userManager.GetUsersInRoleAsync(SD.UserRole);

            if (readers.Any())
                return;

            var readerEmail = "reader@email.com";
            var identityUser = new IdentityUser { UserName = readerEmail, Email = readerEmail };
            var result = await userManager.CreateAsync(identityUser, "ReaderPassword123!");

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(identityUser, SD.UserRole);

                var user = new User
                {
                    FirstName = "Йосиф",
                    MiddleName = "Тенев",
                    LastName = "Илиев",
                    IdentityUserId = identityUser.Id
                };

                await userService.AddAsync(user);
            }
        }

        private static async Task CreateAuthors(IServiceProvider serviceProvider)
        {
            var authorService = serviceProvider.GetRequiredService<IAuthorService>();
            var authors = await authorService.GetAllAsync();

            if (authors.Count() != 0)
                return;

            Author author1 = new Author() {FullName = "Иван Вазов" };
            Author author2 = new Author() {FullName = "Джани Родари" };
            Author author3 = new Author() {FullName = "Марк Твен" };
            Author author4 = new Author() {FullName = "Йордан Йовков" };
            Author author5 = new Author() {FullName = "Астрид Линдгрен" };

            await authorService.AddAsync(author1);
            await authorService.AddAsync(author2);
            await authorService.AddAsync(author3);
            await authorService.AddAsync(author4);
            await authorService.AddAsync(author5);
        }

        private static async Task CreateImages(IServiceProvider serviceProvider)
        {
            var imageService = serviceProvider.GetRequiredService<IService<Image>>();
            var images = await imageService.GetAllAsync();

            if (images.Count() != 0)
                return;

            Image image1 = new Image() {DestinationLink = "https://res.cloudinary.com/dxkehe0om/image/upload/v1744217337/p18j32fkeoto3flxneal.jpg" };
            Image image2 = new Image() {DestinationLink = "https://res.cloudinary.com/dxkehe0om/image/upload/v1744218190/muxfb7u5utibam6yvek4.jpg" };
            Image image3 = new Image() {DestinationLink = "https://res.cloudinary.com/dxkehe0om/image/upload/v1744218312/cdulu19jwkgcd70nonf3.jpg" };
            Image image4 = new Image() {DestinationLink = "https://res.cloudinary.com/dxkehe0om/image/upload/v1744218413/h86gcaofa7mdzze9tnv8.jpg" };
            Image image5 = new Image() {DestinationLink = "https://res.cloudinary.com/dxkehe0om/image/upload/v1744219100/xevbqc3oq9ysqzt2unwd.jpg" };

            await imageService.AddAsync(image1);
            await imageService.AddAsync(image2);
            await imageService.AddAsync(image3);
            await imageService.AddAsync(image4);
            await imageService.AddAsync(image5);
        }

        private static async Task CreateSections(IServiceProvider serviceProvider)
        {
            var sectionService = serviceProvider.GetService<ISectionService>();
            var sections = await sectionService.GetAllAsync();
            if (sections.Count() != 0)
                return;

            Section section1 = new Section()
            {
                Name = "Българска литература",
                Description = "Литература от български автори"
            };

            Section section2 = new Section()
            {
                Name = "Чуждестранна литература",
                Description = "Литература от чуждестранни автори"
            };

            await sectionService.AddAsync(section1);
            await sectionService.AddAsync(section2);
        }

        private static async Task CreateTitles(IServiceProvider serviceProvider)
        {
            var titleService = serviceProvider.GetService<IService<Title>>();
            var titles = await titleService.GetAllAsync();
            if (titles.Count() != 0)
                return;

            Title title1 = new Title()
            {
                Name = "Под игото",
                Description = "Романът пресъздва живота на един поробен народ в момента на осъзнаване ценността на свободата – път, по който не всички ще поемат, път, който ще открои героите - останалите в историята, но и обикновените хора, чиито имена остават неизвестни за поколенията. Изпълнен с величието на бунта и трагизма на погрома, с жизнелюбие и хумор, романът очертава лицето на една неповторима епоха от нашето минало.",
                SectionId = 1
            };

            Title title2 = new Title()
            {
                Name = "Приказки по телефона",
                Description = "Това са кратки, забавни приказки, които мъж на име Бианки от град Варезе разказвал на своето момиченце. Господин Бианки бил търговски пътник и шест дни в седмицата продавал лекарства, кръстосвайки цяла Италия на изток, на запад, на юг, на север и по средата. В неделя се връщал у дома, а в понеделник сутрин отново тръгвал на път. Но преди да тръгне, неговото момиченце му казвало, че иска всяка вечер по една приказка. Затова, където и да се намирал, точно в девет часа господин Бианки искал да го свържат с Варезе и разказвал на своето момиченце приказки по телефона.",
                SectionId = 2
            };

            Title title3 = new Title()
            {
                Name = "Приключенията на Том Сойер",
                Description = "Малкият палавник Том Сойер, който тайничко си похапва от сладкото на леля си, постоянно престъпва забраната й да се къпе в реката, не внимава в час или се забърква в най-невероятни щуротии, мигом печели сърцата на читателите. Сигурно защото в него те откриват самите себе си. И вече 120 години Том Сойер шества из целия свят. И никой не се изненадва, че той бодро прекрачва в XXI век.\r\nПоследвайте го и вие! Очакват ви много забавления и смях.",
                SectionId = 2
            };

            Title title4 = new Title()
            {
                Name = "Жетварят",
                Description = "Склонен съм да смятам тази своя работа за най-хубава споделя за \"Жетварят\" Йордан Йовков пред проф. Спиридон Казанджиев. Изразява и удовлетворението си от изясняването на основната идея в повестта: \"отношението на селянина към земята и труда като извор на всички духовни качества и прояви – на първо място религиозността\", като „неделима спътница на работата\".\r\nЙордан Йовков пише \"Жетварят\" в Добрич и Варна през 1918–1919 г. Повестта е издадена за първи път през 1920 г. от софийското книгоиздателство \"Образование\". При работата си върху нея вероятно използва свои непубликувани разкази с битова тематика, създадени в Добруджа преди войните. Към \"Жетварят\" се връща отново, когато се решава да го преиздаде през 1930 г. Преработката е доста мъчителна: \"Трябваше да се запази общата постройка, да се помиря с някои недостатъци, органически за произведението, които не можех да отстраня; трябваше да намеря точно ония места, които трябваше да се запълнят с нови моменти; новото трябваше да се даде така, че да не личи, та който чете книгата сега, да мисли, че е била такава и в първата ? редакция. Едно пресмятане и съобразяване, което напомняше твърде много работата на архитекта.\"\r\nРелигиозните мотиви в \"Жетварят\" предопределят и съдбата на повестта след 1944 г. – елегантно я заобикалят.\r\nСъвременният прочит на \"Жетварят\" ни припомня: \"Залови се за мъжка работа и не се бой... А хора и приятели сме – ще се приглеждаме, ще си помагаме!\"",
                SectionId = 1
            };

            Title title5 = new Title()
            {
                Name = "Пипи Дългото чорапче",
                Description = "Скъпи приятели,\r\nПри вас идва едно шведско момиче, което се казва Пипи Дългото чорапче. Тя е доста чудата, но аз се надявам, че въпреки това вие ще я обикнете. Пипи живее съвсем сама в една стара къща, наречена Вила Вилекула. Тя няма нито майка, нито татко, но това ни най-малко не я тревожи, защото си има кон и маймунка, а в съседната къща живеят две нейни другарчета - Томи и Аника. Пипи е най-силното момиче в света...\r\nАстрид Линдгрен",
                SectionId = 2
            };

            await titleService.AddAsync(title1);
            await titleService.AddAsync(title2);
            await titleService.AddAsync(title3);
            await titleService.AddAsync(title4);
            await titleService.AddAsync(title5);
        }

        private static async Task CreateTitlesAuthors(IServiceProvider serviceProvider)
        {
            var titleAuthorService = serviceProvider.GetService<IService<TitleAuthor>>();
            var titlesAuthors = await titleAuthorService.GetAllAsync();
            if (titlesAuthors.Count() != 0) return;

            TitleAuthor titleAuthor1 = new TitleAuthor() { TitleId = 1, AuthorId = 1};
            TitleAuthor titleAuthor2 = new TitleAuthor() { TitleId = 2, AuthorId = 2};
            TitleAuthor titleAuthor3 = new TitleAuthor() { TitleId = 3, AuthorId = 3};
            TitleAuthor titleAuthor4 = new TitleAuthor() { TitleId = 4, AuthorId = 4};
            TitleAuthor titleAuthor5 = new TitleAuthor() { TitleId = 5, AuthorId = 5};

            await titleAuthorService.AddAsync(titleAuthor1);
            await titleAuthorService.AddAsync(titleAuthor2);
            await titleAuthorService.AddAsync(titleAuthor3);
            await titleAuthorService.AddAsync(titleAuthor4);
            await titleAuthorService.AddAsync(titleAuthor5);
        }

        private static async Task CreateLibraryUnits(IServiceProvider serviceProvider)
        {
            var libraryUnitService = serviceProvider.GetService<IService<LibraryUnit>>();
            var libraryUnits = await libraryUnitService.GetAllAsync();

            if (libraryUnits.Count() != 0)
                return;

            LibraryUnit unit1 = new LibraryUnit() {
                InventoryNumber = "f100012567",
                Condition = "Добро",
                Medium = "Хартиен",
                IsScrapped = false,
                TitleId = 1,
                Isbn = "9783161484100",
                TypeLibraryUnit = "Книга",
                Year = 2002,
                PublishingHouse = "Хермес",
                ImageId = 1,
                IsAvailable = true,
                IsSavedByUser = false,
                SavedByReaderId = null};

            LibraryUnit unit2 = new LibraryUnit()
            {
                InventoryNumber = "f100018573",
                Condition = "Добро",
                Medium = "Хартиен",
                IsScrapped = false,
                TitleId = 2,
                Isbn = null,
                TypeLibraryUnit = "Книга",
                Year = 2023,
                PublishingHouse = "Пан",
                ImageId = 2,
                IsAvailable = true,
                IsSavedByUser = false,
                SavedByReaderId = null};

            LibraryUnit unit3 = new LibraryUnit()
            {
                InventoryNumber = "f12246937",
                Condition = "Добро",
                Medium = "Хартиен",
                IsScrapped = false,
                TitleId = 3,
                Isbn = "9780545010221",
                TypeLibraryUnit = "Книга",
                Year = 1997,
                PublishingHouse = "Пан",
                ImageId = 3,
                IsAvailable = true,
                IsSavedByUser = false,
                SavedByReaderId = null};

            LibraryUnit unit4 = new LibraryUnit()
            {
                InventoryNumber = "f1339838763",
                Condition = "Добро",
                Medium = "Хартиен",
                IsScrapped = false,
                TitleId = 4,
                Isbn = null,
                TypeLibraryUnit = "Книга",
                Year = 2016,
                PublishingHouse = "Унискорп",
                ImageId = 4,
                IsAvailable = true,
                IsSavedByUser = false,
                SavedByReaderId = null};

            LibraryUnit unit5 = new LibraryUnit()
            {
                InventoryNumber = "f100019779",
                Condition = "Лошо",
                Medium = "Хартиен",
                IsScrapped = false,
                TitleId = 5,
                Isbn = "9781402894626",
                TypeLibraryUnit = "Книга",
                Year = 1968,
                PublishingHouse = null,
                ImageId = 5,
                IsAvailable = true,
                IsSavedByUser = false,
                SavedByReaderId = null};

            await libraryUnitService.AddAsync(unit1);
            await libraryUnitService.AddAsync(unit2);
            await libraryUnitService.AddAsync(unit3);
            await libraryUnitService.AddAsync(unit4);
            await libraryUnitService.AddAsync(unit5);
        }
    }
}