using System.Threading.Tasks;
using Google.Apis.Admin.Directory.directory_v1;
using Google.Apis.Admin.Directory.directory_v1.Data;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;

namespace GSuiteDirectoryApiExample
{
    class Program
    {
        static void Main(string[] args)
        {
        }

        public async Task CreateOrUpdateUser(string appName, string auth, string admin, User user)
        {
            var foundUser = await InitDirectoryService(appName, auth, admin).Users.Get(user.PrimaryEmail).ExecuteAsync();
            if (foundUser == null)
                await CreateUser(appName, auth, admin, user);
            else
                await UpdateUser(appName, auth, user.PrimaryEmail, admin, user);
        }

        public async Task<User> CreateUser(string appName, string auth, string admin, User user) =>
            await InitDirectoryService(appName, auth, admin).Users.Insert(user).ExecuteAsync();

        public async Task UpdateUser(string appName, string auth, string admin, string userKey, User user) =>
           await InitDirectoryService(appName, auth, admin).Users.Update(user, userKey).ExecuteAsync();

        public async Task Delete(string appName, string auth, string admin, string userKey) =>
             await InitDirectoryService(appName, auth, admin).Users.Delete(userKey).ExecuteAsync();

        private DirectoryService InitDirectoryService(string appName, string auth, string admin)
        {
            //You can also import from file
            var credentials = GoogleCredential.FromJson(auth)
                //Just adding one scope to manage users
                .CreateScoped(new string[] { DirectoryService.ScopeConstants.AdminDirectoryUser })
                //Impersonating an admin user
                .CreateWithUser(admin)
                .UnderlyingCredential as ServiceAccountCredential;

            return new DirectoryService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credentials,
                ApplicationName = appName
            });
        }
    }
}
